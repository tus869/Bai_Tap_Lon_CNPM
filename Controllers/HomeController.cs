using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CNPM.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CNPM.Controllers
{
    public class HomeController : Controller
    {
        private readonly SchoolContext _context;

        public HomeController(SchoolContext context)
        {
            _context = context;
        }

        // Trang chủ hiển thị 3 tin tức mới nhất
        public async Task<IActionResult> Index()
        {
            var news = await _context.TinTucs
                .OrderByDescending(t => t.NgayDang)
                .Take(3)
                .ToListAsync();
            return View(news);
        }

        public IActionResult GioiThieu()
        {
            return View();
        }

        public IActionResult Privacy() => View();
    }
}