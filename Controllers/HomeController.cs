using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDASV.Data;
using QLDASV.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

namespace QLDASV.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. TRANG CHỦ (Nơi bạn sẽ dán code giao diện THPT Mường Ảng vào)
        public IActionResult Index()
        {
            // Trả về thẳng giao diện Index.cshtml
            return View();
        }

        // 2. TÌM KIẾM (Đã được làm gọn)
        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            ViewBag.Keyword = keyword;
            if (string.IsNullOrEmpty(keyword)) return RedirectToAction("Index");

            var query = _context.DoAns
                .Include(d => d.DeTai)
                .Include(d => d.SinhVien)
                .Include(d => d.GiangVienHD)
                .AsQueryable();

            keyword = keyword.Trim();
            query = query.Where(d =>
                (d.DeTai != null && d.DeTai.TenDeTai.Contains(keyword)) ||
                (d.SinhVien != null && d.SinhVien.HoTen.Contains(keyword)));

            var results = await query.ToListAsync();
            return View("SearchResults", results);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}