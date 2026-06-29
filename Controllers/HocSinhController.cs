using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CNPM.Data;
using CNPM.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace CNPM.Controllers
{
    [Authorize]
    public class HocSinhController : Controller
    {
        private readonly SchoolContext _context;

        public HocSinhController(SchoolContext context)
        {
            _context = context;
        }

        // 1. Danh sách học sinh
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Index(int? maLop)
        {
            if (maLop.HasValue)
            {
                var hocSinhs = await _context.HocSinhs
                                             .Where(h => h.MaLop == maLop)
                                             .ToListAsync();
                return View(hocSinhs);
            }

            var tatCaHocSinh = await _context.HocSinhs.ToListAsync();
            return View(tatCaHocSinh);
        }

        // 2. Thêm mới học sinh (GET)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["MaLop"] = new SelectList(_context.LopHocs, "MaLop", "TenLop");
            return View();
        }

        // 3. Xử lý thêm mới (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("HoTen,MaLop,NgaySinh,GioiTinh,DiaChi,SoDienThoai,HoTenPhuHuynh,SdtPhuHuynh")] HocSinh hocSinh)
        {
            ModelState.Remove("LopHoc");
            ModelState.Remove("TaiKhoan");
            ModelState.Remove("BangDiems");
            ModelState.Remove("ThamGiaCLBs");

            if (ModelState.IsValid)
            {
                try
                {
                    hocSinh.LopHoc = null;
                    hocSinh.TaiKhoan = null;

                    _context.Add(hocSinh);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thêm mới học sinh thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Lỗi khi lưu dữ liệu: " + (ex.InnerException?.Message ?? ex.Message);
                }
            }

            ViewData["MaLop"] = new SelectList(_context.LopHocs, "MaLop", "TenLop", hocSinh.MaLop);
            return View(hocSinh);
        }

        // 4. Chỉnh sửa hồ sơ (GET) - HIỂN THỊ FORM
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var hocSinh = await _context.HocSinhs.FindAsync(id);
            if (hocSinh == null) return NotFound();

            ViewData["MaLop"] = new SelectList(_context.LopHocs, "MaLop", "TenLop", hocSinh.MaLop);
            return View(hocSinh);
        }

        // 5. Xử lý cập nhật (POST) - LƯU VÀO DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("MaHS,MaTaiKhoan,MaLop,HoTen,NgaySinh,GioiTinh,DiaChi,SoDienThoai,HoTenPhuHuynh,SdtPhuHuynh,TrangThaiHoc")] HocSinh hocSinh)
        {
            if (id != hocSinh.MaHS) return NotFound();

            ModelState.Remove("LopHoc");
            ModelState.Remove("TaiKhoan");
            ModelState.Remove("BangDiems");
            ModelState.Remove("ThamGiaCLBs");

            if (ModelState.IsValid)
            {
                try
                {
                    // NGẮT LIÊN KẾT & XÓA BỘ NHỚ TẠM (TRACKING COLLISION FIX)
                    hocSinh.LopHoc = null;
                    hocSinh.TaiKhoan = null;
                    _context.ChangeTracker.Clear();

                    _context.Update(hocSinh);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật thông tin học sinh vào lớp thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HocSinhExists(hocSinh.MaHS)) return NotFound();
                    else throw;
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Lỗi khi lưu dữ liệu: " + (ex.InnerException?.Message ?? ex.Message);
                }
            }

            ViewData["MaLop"] = new SelectList(_context.LopHocs, "MaLop", "TenLop", hocSinh.MaLop);
            return View(hocSinh);
        }

        private bool HocSinhExists(int id)
        {
            return _context.HocSinhs.Any(e => e.MaHS == id);
        }

        // 6. Hàm hiển thị danh sách chờ duyệt
        public async Task<IActionResult> DanhSachChoDuyet()
        {
            var dsChoDuyet = await _context.HocSinhs.Where(h => h.TrangThai == "Chờ duyệt").ToListAsync();
            return View(dsChoDuyet);
        }

        // 7. Xử lý duyệt tuyển sinh
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DuyetTuyenSinh(int id, string action)
        {
            var hs = await _context.HocSinhs.FindAsync(id);
            if (hs == null) return NotFound();

            if (action == "Approve")
            {
                hs.TrangThai = "Đã duyệt";

                // Kiểm tra tài khoản trùng lặp dựa trên Tên đăng nhập (Mã SV / SĐT)
                var existingAccount = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.TenDangNhap == hs.Email);
                if (existingAccount == null)
                {
                    // Đề phòng dữ liệu Email bị trống
                    if (string.IsNullOrEmpty(hs.Email))
                    {
                        hs.Email = hs.SoDienThoai + "@thptmuongang.edu.vn";
                    }

                    var newAccount = new TaiKhoan
                    {
                        Email = hs.Email,
                        TenDangNhap = hs.Email,       // ĐÃ SỬA: Dùng Email làm tên đăng nhập
                        MatKhau = hs.SoDienThoai,     // SĐT làm mật khẩu
                        MatKhauHash = hs.SoDienThoai,
                        Role = "Student",
                        VaiTro = "Student",
                        TrangThai = true,
                        NgayTao = DateTime.Now,
                        MaHocSinh = hs.MaHS
                    };
                    _context.TaiKhoans.Add(newAccount);
                }
            }
            else
            {
                hs.TrangThai = "Từ chối";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DanhSachChoDuyet));
        }
        // ==========================================
        // HỌC SINH: XEM THÔNG TIN CÁ NHÂN
        // ==========================================
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ThongTinCaNhan()
        {
            var username = User.Identity.Name;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.TenDangNhap == username);
            if (taiKhoan == null)
            {
                return Content("Lỗi: Không tìm thấy tài khoản hệ thống của bạn.");
            }

            var hocSinh = await _context.HocSinhs
                .Include(h => h.LopHoc)
                .FirstOrDefaultAsync(h => h.MaTaiKhoan == taiKhoan.MaTaiKhoan);

            // ĐÃ FIX LỖI: Trả về thông báo text thay vì gọi View("Error") gây crash
            if (hocSinh == null)
            {
                return Content("Tài khoản của bạn hiện chưa được liên kết với hồ sơ học sinh nào. Vui lòng liên hệ Giáo viên chủ nhiệm.");
            }

            return View(hocSinh);
        }
    }
}