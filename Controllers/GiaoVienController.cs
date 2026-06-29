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
    // Yêu cầu quyền Admin mới được truy cập vào controller này 
    [Authorize(Roles = "Admin")]
    public class GiaoVienController : Controller
    {
        private readonly SchoolContext _context;

        // Tiêm dependency (Dependency Injection) DbContext để tương tác với cơ sở dữ liệu
        public GiaoVienController(SchoolContext context)
        {
            _context = context;
        }

        // DANH SÁCH GIÁO VIÊN (Dành cho Admin)
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách giáo viên, dùng Include để lấy kèm dữ liệu Lớp chủ nhiệm (Eager Loading)
            var danhSachGV = await _context.GiaoViens
                .Include(g => g.LopHocChuNhiems)
                .ToListAsync();
            return View(danhSachGV);
        }

  
        // AllowAnonymous cho phép truy cập mà không cần đăng nhập
        [AllowAnonymous]
        public async Task<IActionResult> GocGiaoVien()
        {
            // Chỉ hiển thị các giáo viên đang còn công tác (TrangThaiCongTac == true)
            var danhSachGV = await _context.GiaoViens.Where(g => g.TrangThaiCongTac == true).ToListAsync();
            return View(danhSachGV);
        }

        public IActionResult Create() => View();


        // THÊM MỚI GIÁO VIÊN (Kèm tạo tự động tài khoản)
 
        [HttpPost]
        [ValidateAntiForgeryToken] // Chống tấn công CSRF (Giả mạo yêu cầu liên trang)
        public async Task<IActionResult> Create([Bind("HoTen,SoDienThoai,Email,ToChuyenMon,ChuyenMon")] GiaoVien giaoVien)
        {
            giaoVien.ChuyenMon = Request.Form["ChuyenMon"].ToString();

            // Loại bỏ các trường không nhập từ Form ra khỏi ModelState để vượt qua bước kiểm tra IsValid
            ModelState.Remove("TaiKhoan");
            ModelState.Remove("MaTaiKhoan");
            ModelState.Remove("TrangThaiCongTac");
            ModelState.Remove("CauLacBos");
            ModelState.Remove("PhanCongs");
            ModelState.Remove("LopHocChuNhiems");

            if (ModelState.IsValid)
            {
                // Kiểm tra xem email (dùng làm tên đăng nhập) đã tồn tại trong bảng Tài Khoản chưa
                var accountExists = await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == giaoVien.Email);
                if (accountExists)
                {
                    ViewBag.CustomError = "Email này đã được cấp tài khoản!";
                    return View(giaoVien); // Trả về form và báo lỗi
                }

                // TỰ ĐỘNG TẠO TÀI KHOẢN CHO GIÁO VIÊN MỚI
                var newAccount = new TaiKhoan
                {
                    Email = giaoVien.Email,
                    TenDangNhap = giaoVien.Email, // Dùng Email làm tên đăng nhập
                    MatKhau = giaoVien.SoDienThoai, // Mặc định dùng Số điện thoại làm mật khẩu
                    MatKhauHash = giaoVien.SoDienThoai,
                    // Phân quyền tự động: Nếu thuộc Ban Giám Hiệu thì cấp quyền Admin, ngược lại là Teacher
                    Role = (giaoVien.ToChuyenMon == "Ban Giám Hiệu") ? "Admin" : "Teacher",
                    VaiTro = (giaoVien.ToChuyenMon == "Ban Giám Hiệu") ? "Admin" : "Teacher",
                    TrangThai = true,
                    NgayTao = DateTime.Now
                };

                // Lưu tài khoản vào DB trước để sinh ra MaTaiKhoan
                _context.TaiKhoans.Add(newAccount);
                await _context.SaveChangesAsync();

                // Gán mã tài khoản vừa tạo cho Giáo Viên
                giaoVien.MaTaiKhoan = newAccount.MaTaiKhoan;

                // NGẮT TRACKING OBJECT TÀI KHOẢN
                // Vì newAccount đang được EF Core theo dõi, nếu để giaoVien.TaiKhoan = newAccount,
                // EF Core có thể sẽ cố gắng thêm bảng Tài Khoản lần nữa gây lỗi trùng lặp khóa chính.
                giaoVien.TaiKhoan = null;

                // Lưu thông tin Giáo viên
                _context.GiaoViens.Add(giaoVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }//_context.GiaoViens: Là khu vực kệ sách chứa hồ sơ giáo viên.
            return View(giaoVien);
        }

        // ==========================================
        // SỬA THÔNG TIN GIÁO VIÊN
        // ==========================================
        [HttpGet] //lấy dữ liệu từ database ra để hiển thị lên form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var gv = await _context.GiaoViens.FindAsync(id); // Tìm giáo viên theo ID
            if (gv == null) return NotFound();

            return View(gv);
        }

        [HttpPost]// Nhận dữ liệu từ form gửi lên để cập nhật vào database
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaGV,MaTaiKhoan,HoTen,SoDienThoai,Email,ToChuyenMon,ChuyenMon,TrangThaiCongTac")] GiaoVien giaoVien)
        {
            if (id != giaoVien.MaGV) return NotFound();

            // Lấy dữ liệu trong bộ nhớ)
            // Việc này giúp ta lấy được các thông tin cũ (như MaTaiKhoan) mà form Edit không gửi lên
            var gvGoc = await _context.GiaoViens.AsNoTracking().FirstOrDefaultAsync(g => g.MaGV == id);
            if (gvGoc == null) return NotFound();

            // Gán lại mã tài khoản cũ để không bị mất liên kết với bảng TaiKhoan
            giaoVien.MaTaiKhoan = gvGoc.MaTaiKhoan;

            // Tự động lấy lại Email gốc nếu form không gửi lên (tránh lỗi null)
            if (string.IsNullOrEmpty(giaoVien.Email))
            {
                giaoVien.Email = gvGoc.Email;
            }

            // Xóa validation các trường không liên đới trong form sửa
            ModelState.Remove("TaiKhoan");
            ModelState.Remove("CauLacBos");
            ModelState.Remove("PhanCongs");
            ModelState.Remove("LopHocChuNhiems");

            if (ModelState.IsValid)
            {
                try
                {
                    // Ngắt Tracking điều hướng để tránh lỗi đụng độ bộ nhớ của Entity Framework
                    giaoVien.TaiKhoan = null;

                    // Cập nhật thông tin vào Database
                    _context.Update(giaoVien);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Lỗi lưu vào CSDL. Hãy kiểm tra lại các trường dữ liệu!");
                }
            }
            return View(giaoVien);
        }

        // XÓA GIÁO VIÊN
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound(); // 1. Kiểm tra ID có hợp lệ không
            var gv = await _context.GiaoViens.FirstOrDefaultAsync(m => m.MaGV == id);// 2. Tìm giáo viên trong DB
            if (gv == null) return NotFound();// 3. Kiểm tra xem giáo viên có tồn tại không
            return View(gv); // 4. Trả về giao diện (View) để người dùng xác nhận xóa
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Tìm và xóa giáo viên theo ID
            var gv = await _context.GiaoViens.FindAsync(id);
            if (gv != null) _context.GiaoViens.Remove(gv);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Hàm kiểm tra xem giáo viên có tồn tại không (thường dùng phụ trợ)
        private bool GiaoVienExists(int id) => _context.GiaoViens.Any(e => e.MaGV == id);
    }
}