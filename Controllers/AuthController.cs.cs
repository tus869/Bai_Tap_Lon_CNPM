using Microsoft.AspNetCore.Mvc;
using CNPM.Data;
using CNPM.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore; // Bắt buộc phải có để dùng AnyAsync()

namespace CNPM.Controllers
{
    public class AuthController : Controller
    {
        private readonly SchoolContext _context;

        // Tiêm (Inject) SchoolContext vào Controller
        public AuthController(SchoolContext context)
        {
            _context = context;
        }

        /* =========================================
           1. CHỨC NĂNG ĐĂNG NHẬP
        ========================================= */
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ Tài khoản và Mật khẩu.";
                return View();
            }

            var account = await _context.TaiKhoans
                .FirstOrDefaultAsync(a => a.TenDangNhap == username && a.MatKhauHash == password && a.TrangThai == true);

            if (account != null)
            {
                account.LanDangNhapCuoi = System.DateTime.Now;
                await _context.SaveChangesAsync();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, account.TenDangNhap),
                    new Claim(ClaimTypes.Role, account.VaiTro),
                    new Claim("MaTaiKhoan", account.MaTaiKhoan.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác!";
            return View();
        }

        /* =========================================
           2. CHỨC NĂNG ĐĂNG XUẤT
        ========================================= */
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        /* =========================================
           3. CHỨC NĂNG TỪ CHỐI QUYỀN TRUY CẬP
        ========================================= */
        public IActionResult AccessDenied()
        {
            return View();
        }

        /* =========================================
           4. CHỨC NĂNG ĐĂNG KÝ
        ========================================= */
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string password, string confirmPassword)
        {
            // 1. Kiểm tra xác nhận mật khẩu
            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            // 2. Kiểm tra tài khoản trùng lặp
            bool exists = await _context.TaiKhoans.AnyAsync(a => a.TenDangNhap == username);
            if (exists)
            {
                ViewBag.Error = "Tên tài khoản này đã được sử dụng!";
                return View();
            }

            // 3. Tạo tài khoản mới nếu dữ liệu hợp lệ
            if (ModelState.IsValid)
            {
                var newAccount = new TaiKhoan
                {
                    TenDangNhap = username,
                    MatKhauHash = password,
                    VaiTro = "Student",
                    TrangThai = true,
                    NgayTao = System.DateTime.Now
                };

                _context.TaiKhoans.Add(newAccount);
                await _context.SaveChangesAsync();

                // Đăng ký xong chuyển thẳng về trang Đăng nhập
                return RedirectToAction("Login");
            }

            return View();
        }
    }
}