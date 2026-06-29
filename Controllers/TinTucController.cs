using Microsoft.AspNetCore.Mvc;

namespace CNPM.Controllers
{
    public class TinTucController : Controller
    {
        // Điều hướng đến trang danh sách Tin tức
        public IActionResult Index()
        {
            return View();
        }
    }
}