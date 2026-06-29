using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CNPM.Data;
using CNPM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CNPM.Controllers
{
    public class AccountController : Controller
    {
        private readonly SchoolContext _context;

        public AccountController(SchoolContext context)
        {
            _context = context;
        }

        // ==============================
        // 1. ĐĂNG NHẬP
        // ==============================
        [HttpGet]
        public IActionResult Login()
        {
            // Nếu đã đăng nhập rồi thì chuyển hướng về trang chủ
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Kiểm tra tài khoản trong Database
            var user = await _context.TaiKhoans
                .FirstOrDefaultAsync(u => u.TenDangNhap == username && u.TrangThai == true);

            if (user != null)
            {
                // Đối chiếu mật khẩu (Nếu bạn dùng Hash thì cần code Hash ở đây, hiện tại đang so sánh chuỗi trực tiếp)
                if (user.MatKhauHash == password)
                {
                    // Tạo danh sách Claims (Thông tin lưu trong Cookie)
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.TenDangNhap),
                        new Claim(ClaimTypes.Email, user.Email ?? ""),
                        new Claim(ClaimTypes.Role, user.VaiTro) // Dùng user.VaiTro hoặc user.Role tùy model của bạn
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Thực hiện đăng nhập (Lưu Cookie)
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác, hoặc tài khoản chưa được duyệt!";
            return View();
        }

        // ==============================
        // 2. ĐĂNG XUẤT
        // ==============================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // ==============================
        // 3. ĐĂNG KÝ TUYỂN SINH (Dành cho Học sinh mới)
        // ==============================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("HoTen,NgaySinh,GioiTinh,DiaChi,SoDienThoai,Email,HoTenPhuHuynh,SdtPhuHuynh")] HocSinh hocSinh)
        {
            // Loại bỏ bẫy lỗi cho các trường không nhập ở form đăng ký
            ModelState.Remove("LopHoc");
            ModelState.Remove("TaiKhoan");
            ModelState.Remove("BangDiems");
            ModelState.Remove("ThamGiaCLBs");

            if (ModelState.IsValid)
            {
                // Mặc định gán trạng thái là "Chờ duyệt"
                hocSinh.TrangThai = "Chờ duyệt";

                _context.Add(hocSinh);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký hồ sơ thành công! Vui lòng chờ nhà trường duyệt để được cấp tài khoản.";
                return RedirectToAction(nameof(Login));
            }

            return View(hocSinh);
        }

        // ==============================
        // 4. TRANG BÁO LỖI QUYỀN TRUY CẬP
        // ==============================
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}