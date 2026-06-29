using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CNPM.Data;
using CNPM.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace CNPM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PhanCongController : Controller
    {
        private readonly SchoolContext _context;

        public PhanCongController(SchoolContext context)
        {
            _context = context;
        }

        // 1. Danh sách Phân công
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.PhanCongs
                .Include(p => p.GiaoVien)
                .Include(p => p.LopHoc)
                .Include(p => p.MonHoc);
            return View(await schoolContext.ToListAsync());
        }

        // 2. Phân công mới (GET)
        public IActionResult Create()
        {
            // Tải dữ liệu cho 3 Dropdown
            ViewData["MaGV"] = new SelectList(_context.GiaoViens, "MaGV", "HoTen");
            ViewData["MaLop"] = new SelectList(_context.LopHocs, "MaLop", "TenLop");
            ViewData["MaMon"] = new SelectList(_context.MonHocs, "MaMon", "TenMon");
            return View();
        }

        // 3. Xử lý lưu Phân công (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaGV,MaLop,MaMon,HocKy")] PhanCong phanCong)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phanCong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Nếu lỗi, nạp lại dữ liệu Dropdown để hiển thị lại form
            ViewData["MaGV"] = new SelectList(_context.GiaoViens, "MaGV", "HoTen", phanCong.MaGV);
            ViewData["MaLop"] = new SelectList(_context.LopHocs, "MaLop", "TenLop", phanCong.MaLop);
            ViewData["MaMon"] = new SelectList(_context.MonHocs, "MaMon", "TenMon", phanCong.MaMon);
            return View(phanCong);
        }

        // 4. Hủy phân công
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var phanCong = await _context.PhanCongs
                .Include(p => p.GiaoVien)
                .Include(p => p.LopHoc)
                .Include(p => p.MonHoc)
                .FirstOrDefaultAsync(m => m.MaPhanCong == id);

            if (phanCong == null) return NotFound();

            return View(phanCong);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var phanCong = await _context.PhanCongs.FindAsync(id);
            if (phanCong != null) _context.PhanCongs.Remove(phanCong);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}