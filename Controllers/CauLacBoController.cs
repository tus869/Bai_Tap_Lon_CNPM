using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CNPM.Data;
using CNPM.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace CNPM.Controllers
{
    [Authorize]
    public class CauLacBoController : Controller
    {
        private readonly SchoolContext _context;

        public CauLacBoController(SchoolContext context)
        {
            _context = context;
        }

        /* -------------------------------------------------------------------------- */
        /* 1. KHU VỰC CÔNG KHAI (PUBLIC)                                              */
        /* -------------------------------------------------------------------------- */
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var danhSachClb = await _context.CauLacBos
                .Include(c => c.GiaoVienPhuTrach)
                .Where(c => c.TrangThai == "Đã duyệt" || c.TrangThai == null)
                .ToListAsync();
            return View(danhSachClb);
        }

        // --- ĐÂY LÀ HÀM BỊ THIẾU GÂY LỖI KHI XEM THÀNH VIÊN ---
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cauLacBo = await _context.CauLacBos
                .Include(c => c.GiaoVienPhuTrach)
                .Include(c => c.ThamGiaCLBs)      // Bắt buộc phải có để load danh sách
                    .ThenInclude(t => t.HocSinh)  // Bắt buộc phải có để lấy tên Học sinh
                .FirstOrDefaultAsync(m => m.MaCLB == id);

            if (cauLacBo == null) return NotFound();

            return View(cauLacBo);
        }

        /* -------------------------------------------------------------------------- */
        /* 2. KHU VỰC DÀNH CHO GIÁO VIÊN                                              */
        /* -------------------------------------------------------------------------- */
        [Authorize(Roles = "Teacher")]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create([Bind("TenCLB,LichSinhHoat,ThoiGian,MoTa")] CauLacBo cauLacBo)
        {
            ModelState.Remove("GiaoVienPhuTrach");
            ModelState.Remove("ThamGiaCLBs");
            ModelState.Remove("TrangThai");

            if (ModelState.IsValid)
            {
                var maTaiKhoan = int.Parse(User.FindFirst("MaTaiKhoan")?.Value ?? "0");
                var giaoVien = await _context.GiaoViens.FirstOrDefaultAsync(g => g.MaTaiKhoan == maTaiKhoan);

                if (giaoVien != null)
                {
                    cauLacBo.MaGVPhuTrach = giaoVien.MaGV;
                    cauLacBo.TrangThai = "Chờ duyệt";
                    _context.Add(cauLacBo);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Đã gửi yêu cầu thành công!";
                    return RedirectToAction(nameof(DanhSachCuaToi));
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewBag.Error = "Lỗi: " + string.Join(", ", errors);
            return View(cauLacBo);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DanhSachCuaToi()
        {
            var maTaiKhoan = int.Parse(User.FindFirst("MaTaiKhoan")?.Value ?? "0");
            var giaoVien = await _context.GiaoViens.FirstOrDefaultAsync(g => g.MaTaiKhoan == maTaiKhoan);
            var clbCuaToi = await _context.CauLacBos
                .Where(c => c.MaGVPhuTrach == giaoVien.MaGV)
                .ToListAsync();
            return View(clbCuaToi);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> QuanLyThanhVien(int id)
        {
            // Đảm bảo nạp đầy đủ ThamGiaCLBs và Học sinh bên trong
            var clb = await _context.CauLacBos
                .Include(c => c.ThamGiaCLBs)
                    .ThenInclude(t => t.HocSinh)
                        .ThenInclude(h => h.LopHoc) // Nạp thêm Lớp để hiện tên lớp
                .FirstOrDefaultAsync(c => c.MaCLB == id);

            if (clb == null) return NotFound();

            return View(clb);
        }

        // --- ĐÃ SỬA LỖI KHÓA CHÍNH PHỨC HỢP (CẦN CẢ MaHS VÀ MaCLB) ---
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DuyetThanhVien(int maHS, int maCLB)
        {
            var maTaiKhoanGV = int.Parse(User.FindFirst("MaTaiKhoan")?.Value ?? "0");
            var giaoVien = await _context.GiaoViens.FirstOrDefaultAsync(g => g.MaTaiKhoan == maTaiKhoanGV);

            // Kiểm tra CLB này có phải do giáo viên này phụ trách không
            var clb = await _context.CauLacBos.FindAsync(maCLB);
            if (clb != null && clb.MaGVPhuTrach == giaoVien.MaGV)
            {
                var tv = await _context.ThamGiaCLBs.FindAsync(maHS, maCLB);
                if (tv != null)
                {
                    tv.TrangThai = "Đã duyệt";
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Đã duyệt thành công!";
                }
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền duyệt CLB này!";
            }
            return RedirectToAction("QuanLyThanhVien", new { id = maCLB });
        }

        // --- ĐÃ SỬA LỖI KHÓA CHÍNH PHỨC HỢP ---
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> XoaThanhVien(int maHS, int maCLB)
        {
            var tv = await _context.ThamGiaCLBs.FindAsync(maHS, maCLB);
            if (tv != null)
            {
                _context.ThamGiaCLBs.Remove(tv);
                await _context.SaveChangesAsync();
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        /* -------------------------------------------------------------------------- */
        /* 3. KHU VỰC DÀNH CHO ADMIN                                                  */
        /* -------------------------------------------------------------------------- */
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DuyetCLB()
        {
            var dsChoDuyet = await _context.CauLacBos
                .Include(c => c.GiaoVienPhuTrach)
                .Where(c => c.TrangThai == "Chờ duyệt")
                .ToListAsync();
            return View(dsChoDuyet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> XuLyDuyet(int id, string action)
        {
            var clb = await _context.CauLacBos.FindAsync(id);
            if (clb == null) return NotFound();

            clb.TrangThai = (action == "Approve") ? "Đã duyệt" : "Từ chối";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DuyetCLB));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DanhSachQuanLy()
        {
            var dsCLB = await _context.CauLacBos.Include(c => c.GiaoVienPhuTrach).ToListAsync();
            return View(dsCLB);
        }
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKyCLB(int maCLB)
        {
            // 1. Lấy thông tin học sinh từ tài khoản đang đăng nhập
            var maTaiKhoan = int.Parse(User.FindFirst("MaTaiKhoan")?.Value ?? "0");
            var hocSinh = await _context.HocSinhs.FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);

            if (hocSinh == null)
            {
                TempData["Error"] = "Bạn cần là học sinh mới có thể đăng ký.";
                return RedirectToAction(nameof(Index));
            }

            // 2. Kiểm tra xem đã đăng ký chưa (tránh trùng lặp)
            var daDangKy = await _context.ThamGiaCLBs.AnyAsync(t => t.MaCLB == maCLB && t.MaHS == hocSinh.MaHS);
            if (daDangKy)
            {
                TempData["Error"] = "Bạn đã gửi yêu cầu đăng ký câu lạc bộ này rồi!";
                return RedirectToAction(nameof(Index));
            }

            // 3. Tạo bản ghi đăng ký mới
            var thamGia = new ThamGiaCLB
            {
                MaCLB = maCLB,
                MaHS = hocSinh.MaHS,
                NgayDangKy = DateTime.Now,
                TrangThai = "Chờ duyệt", // Mặc định là chờ giáo viên duyệt
                VaiTro = "Thành viên"
            };

            _context.ThamGiaCLBs.Add(thamGia);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đã gửi yêu cầu đăng ký thành công! Vui lòng chờ giáo viên duyệt.";
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin, Teacher")] // Admin và Teacher đều vào được
        public async Task<IActionResult> DanhSachThanhVienToanTruong()
        {
            var dsCLB = await _context.CauLacBos
                .Include(c => c.GiaoVienPhuTrach)
                .Include(c => c.ThamGiaCLBs)
                    .ThenInclude(t => t.HocSinh)
                .ToListAsync();

            return View(dsCLB);
        }
        // Hàm xử lý cập nhật chức vụ/vai trò
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CapNhatVaiTro(int maHS, int maCLB, string vaiTro)
        {
            var maTaiKhoanGV = int.Parse(User.FindFirst("MaTaiKhoan")?.Value ?? "0");
            var giaoVien = await _context.GiaoViens.FirstOrDefaultAsync(g => g.MaTaiKhoan == maTaiKhoanGV);

            // Kiểm tra bảo mật: Xem CLB này có đúng do giáo viên đang đăng nhập quản lý không
            var clb = await _context.CauLacBos.FindAsync(maCLB);
            if (clb != null && clb.MaGVPhuTrach == giaoVien.MaGV)
            {
                var tv = await _context.ThamGiaCLBs.FindAsync(maHS, maCLB);
                if (tv != null)
                {
                    tv.VaiTro = vaiTro;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Đã cập nhật vai trò thành công!";
                }
            }
            else
            {
                TempData["Error"] = "Bạn không có quyền thực hiện thao tác này!";
            }

            return RedirectToAction("QuanLyThanhVien", new { id = maCLB });
        }
    }
}