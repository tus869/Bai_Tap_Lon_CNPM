using Microsoft.AspNetCore.Mvc;
using QLDASV.Data;
using QLDASV.Models;
using System.Linq;

namespace QLDASV.Controllers
{
    // Chỉ có 1 class LoginController duy nhất ở đây
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Login hoặc /Login/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Login/Index hoặc /Login/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string tenDangNhap, string matKhau)
        {
            var user = _context.NguoiDungs
                .FirstOrDefault(u => u.TenDangNhap == tenDangNhap && u.MatKhau == matKhau);

            if (user != null)
            {
                // Lưu Session (Phải có app.UseSession() trong Program.cs mới chạy được dòng này)
                HttpContext.Session.SetString("UserRole", user.VaiTro);

                // Kiểm tra chính xác từng chữ cái (Phân biệt hoa thường)
                if (user.VaiTro == "Admin")
                    return RedirectToAction("Index", "Admin");

                if (user.VaiTro == "Lecturer")
                    return RedirectToAction("Index", "Lecturer");

                if (user.VaiTro == "Student")
                    return RedirectToAction("Index", "Student");

                // Nếu nó không vào 3 cái trên, nó sẽ hiện thông báo này:
                return Content($"Đăng nhập thành công nhưng không tìm thấy vai trò: '{user.VaiTro}'. Kiểm tra lại chữ hoa/thường!");
            }

            ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác!";
            return View();
        }

        // Hàm Logout (Tùy chọn thêm vào cho đủ bộ)
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }


    }
}