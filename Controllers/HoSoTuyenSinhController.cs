using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CNPM.Data;
using CNPM.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace CNPM.Controllers
{
    // Yêu cầu quyền Admin (Ban Giám Hiệu) cho toàn bộ Controller này.
    // Những hàm nào có [AllowAnonymous] mới cho phép người ngoài truy cập.
    [Authorize(Roles = "Admin")]
    public class HoSoTuyenSinhController : Controller
    {
        private readonly SchoolContext _context;

        // Khởi tạo Controller và tiêm (inject) Database Context để thao tác với DB
        public HoSoTuyenSinhController(SchoolContext context)
        {
            _context = context;
        }

        /* =======================================================
           1. HỌC SINH: MÀN HÌNH ĐĂNG KÝ TRỰC TUYẾN NGOÀI TRANG CHỦ
           ======================================================= */

        // Cho phép người dùng chưa đăng nhập (học sinh) truy cập vào form đăng ký
        [AllowAnonymous]
        public IActionResult DangKy()
        {
            return View();
        }

        // 2. XỬ LÝ LƯU HỒ SƠ ĐĂNG KÝ (POST: /HoSoTuyenSinh/DangKy)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        // ĐÃ BỔ SUNG "Email" VÀO DANH SÁCH BIND BÊN DƯỚI:
        public async Task<IActionResult> DangKy([Bind("HoTen,NgaySinh,GioiTinh,SoDienThoai,Email,DiaChi,HoTenPhuHuynh,SdtPhuHuynh,DiemToan,DiemVan,DiemAnh,TruongCu")] HoSoTuyenSinh hoSo)
        {
            // Bỏ qua validate các trường dữ liệu hệ thống sẽ tự sinh
            ModelState.Remove("LopHoc");
            ModelState.Remove("MaLop");
            ModelState.Remove("TrangThai");
            ModelState.Remove("NgayDangKy");
            // ĐÃ XÓA DÒNG: ModelState.Remove("Email"); để hệ thống bắt buộc kiểm tra Email

            bool daTonTai = await _context.HoSoTuyenSinhs.AnyAsync(h => h.SoDienThoai == hoSo.SoDienThoai);
            if (daTonTai)
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng.");
                ViewBag.Error = "Số điện thoại này đã được sử dụng để đăng ký hồ sơ. Vui lòng kiểm tra lại hoặc sử dụng số điện thoại khác!";
                return View(hoSo);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    hoSo.TrangThai = "Chờ duyệt";
                    hoSo.NgayDangKy = DateTime.Now;

                    // Vẫn giữ cơ chế dự phòng: Nếu vì lý do nào đó Email vẫn trống, tự động tạo email
                    if (string.IsNullOrEmpty(hoSo.Email))
                    {
                        hoSo.Email = hoSo.SoDienThoai + "@thptmuongang.edu.vn";
                    }

                    _context.Add(hoSo);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Đăng ký tuyển sinh trực tuyến thành công! Hồ sơ của bạn đã được gửi đến Ban Giám Hiệu chờ xét duyệt.";
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    string loiChiTiet = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    ViewBag.Error = "Lỗi hệ thống khi lưu hồ sơ: " + loiChiTiet;
                    return View(hoSo);
                }
            }

            var errorList = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewBag.Error = "Dữ liệu không hợp lệ: " + string.Join(" | ", errorList);

            return View(hoSo);
        }

        /* =======================================================
           2. BAN GIÁM HIỆU: MÀN HÌNH XEM DANH SÁCH HỒ SƠ CHỜ DUYỆT
           ======================================================= */

        // Lưu ý: Hàm này không có [AllowAnonymous] nên chỉ có BGH mới vào được
        public async Task<IActionResult> ThongBaoDuyet()
        {
            // Lấy ra danh sách các hồ sơ đang có trạng thái "Chờ duyệt"
            var danhSachCho = await _context.HoSoTuyenSinhs
                .Where(h => h.TrangThai == "Chờ duyệt")
                .ToListAsync();

            // Lấy danh sách các lớp học gửi sang View để BGH có thể chọn lớp xếp cho học sinh
            ViewBag.DanhSachLop = await _context.LopHocs.ToListAsync();

            return View(danhSachCho);
        }

        /* =======================================================
           3. BAN GIÁM HIỆU: DUYỆT HỒ SƠ VÀ XẾP LỚP
           ======================================================= */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChapNhanVaoLop(int id, int maLop)
        {
            // Kiểm tra xem BGH đã chọn lớp cụ thể chưa
            if (maLop <= 0)
            {
                TempData["Error"] = "Vui lòng chọn một lớp học cụ thể để xếp lớp cho học sinh!";
                return RedirectToAction(nameof(ThongBaoDuyet));
            }

            // Dùng Transaction để đảm bảo tính toàn vẹn dữ liệu. 
            // Phải thành công cả 3 bước (Tạo TK, Thêm Học Sinh, Đổi trạng thái) thì mới lưu vào DB.
            // Nếu 1 bước lỗi, toàn bộ quá trình sẽ bị hủy bỏ (Rollback) để tránh rác dữ liệu.
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Tìm hồ sơ tương ứng trong DB
                    var hoSo = await _context.HoSoTuyenSinhs.FindAsync(id);
                    if (hoSo == null)
                    {
                        TempData["Error"] = "Không tìm thấy hồ sơ cần duyệt.";
                        return RedirectToAction(nameof(ThongBaoDuyet));
                    }

                    // Tạo email mặc định nếu hồ sơ cũ chưa có
                    if (string.IsNullOrEmpty(hoSo.Email))
                    {
                        hoSo.Email = hoSo.SoDienThoai + "@thptmuongang.edu.vn";
                    }

                    // Bước 1: Tạo tài khoản để học sinh đăng nhập vào hệ thống
                    // Tên đăng nhập và Mật khẩu mặc định là Số điện thoại
                    // Bước 1: Tạo tài khoản (Tên đăng nhập = Email, Mật khẩu = SĐT)
                    var taiKhoanMoi = new TaiKhoan
                    {
                        Email = hoSo.Email,
                        TenDangNhap = hoSo.Email,       // ĐÃ SỬA: Dùng Email làm tên đăng nhập
                        MatKhau = hoSo.SoDienThoai,     // SĐT làm mật khẩu
                        MatKhauHash = hoSo.SoDienThoai,
                        Role = "Student",
                        VaiTro = "Student",
                        TrangThai = true,
                        NgayTao = DateTime.Now
                    };
                    _context.TaiKhoans.Add(taiKhoanMoi);
                    await _context.SaveChangesAsync();

                    // Bước 2: Đồng bộ (copy) dữ liệu từ Hồ Sơ sang bảng Học Sinh chính thức
                    var hocSinhChinhThuc = new HocSinh
                    {
                        HoTen = hoSo.HoTen,
                        NgaySinh = hoSo.NgaySinh,
                        GioiTinh = hoSo.GioiTinh,
                        SoDienThoai = hoSo.SoDienThoai,
                        Email = hoSo.Email,
                        DiaChi = hoSo.DiaChi,
                        HoTenPhuHuynh = hoSo.HoTenPhuHuynh,
                        SdtPhuHuynh = hoSo.SdtPhuHuynh,
                        MaLop = maLop, // Lớp do BGH vừa chọn trên View
                        MaTaiKhoan = taiKhoanMoi.MaTaiKhoan, // Liên kết với tài khoản vừa tạo ở Bước 1
                        TrangThaiHoc = true,
                        LopHoc = null,
                        TaiKhoan = null
                    };
                    _context.HocSinhs.Add(hocSinhChinhThuc);

                    // Bước 3: Cập nhật lại hồ sơ thành "Đã duyệt" để ẩn khỏi danh sách chờ
                    hoSo.TrangThai = "Đã duyệt";
                    hoSo.MaLop = maLop;
                    _context.HoSoTuyenSinhs.Update(hoSo);

                    // Lưu lại các thay đổi của bước 2 và 3
                    await _context.SaveChangesAsync();

                    // Xác nhận hoàn tất Transaction và lưu vĩnh viễn vào Database
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = $"Tuyệt vời! Học sinh [{hoSo.HoTen}] đã được xếp vào lớp thành công.";
                }
                catch (Exception ex)
                {
                    // Nếu có bất kỳ lỗi nào xảy ra trong khối try, hủy bỏ toàn bộ thay đổi
                    await transaction.RollbackAsync();
                    string loiChiTiet = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    TempData["Error"] = "Lỗi lưu dữ liệu SQL: " + loiChiTiet;
                }
            }

            return RedirectToAction(nameof(ThongBaoDuyet));
        }

        /* =======================================================
           4. BAN GIÁM HIỆU: XỬ LÝ TỪ CHỐI HỒ SƠ ĐĂNG KÝ
           ======================================================= */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TuChoiHoSo(int id)
        {
            try
            {
                // Tìm hồ sơ dựa vào id
                var hoSo = await _context.HoSoTuyenSinhs.FindAsync(id);
                if (hoSo == null)
                {
                    TempData["Error"] = "Không tìm thấy hồ sơ thí sinh.";
                    return RedirectToAction(nameof(ThongBaoDuyet));
                }

                // Chuyển trạng thái sang Từ chối. Hồ sơ vẫn lưu trong lịch sử nhưng không được xét duyệt tiếp.
                hoSo.TrangThai = "Từ chối";
                _context.HoSoTuyenSinhs.Update(hoSo);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Đã từ chối hồ sơ đăng ký của thí sinh: {hoSo.HoTen}.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xử lý từ chối hồ sơ: " + ex.Message;
            }

            return RedirectToAction(nameof(ThongBaoDuyet));
        }
    }
}