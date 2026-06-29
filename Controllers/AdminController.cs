using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CNPM.Controllers
{
    // BẮT BUỘC: Chỉ có tài khoản Admin (Ban Giám Hiệu) mới được vào
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // Giao diện chính (Dashboard) của Admin
        public IActionResult Index()
        {
            return View();
        }
    }
}