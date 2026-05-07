using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDASV.Data;
using QLDASV.Models;

namespace QLDASV.Controllers
{
    public class TuyenSinhController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TuyenSinhController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper check Admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // 1. TRANG CHỦ TUYỂN SINH (Ai cũng xem được)
        public async Task<IActionResult> Index()
        {
            ViewBag.IsAdmin = IsAdmin();
            // Lấy danh sách tin, sắp xếp tin mới nhất lên đầu
            return View(await _context.TinTuyenSinhs.OrderByDescending(t => t.NgayDang).ToListAsync());
        }

        // 2. CHI TIẾT TIN
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tin = await _context.TinTuyenSinhs.FindAsync(id);
            if (tin == null) return NotFound();

            return View(tin);
        }

        // 3. TẠO TIN MỚI (Admin only)
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TinTuyenSinh tin)
        {
            if (!IsAdmin()) return Forbid();

            // Sửa lỗi ModelState: Bỏ qua validate Id vì nó tự tăng
            ModelState.Remove("Id");

            if (ModelState.IsValid)
            {
                tin.NgayDang = DateTime.Now;

                // SỬA LỖI: Dùng thuộc tính HinhAnh thay vì AnhBia
                if (string.IsNullOrEmpty(tin.HinhAnh))
                {
                    tin.HinhAnh = "https://via.placeholder.com/800x400?text=Tuyen+Sinh+DNU";
                }

                _context.Add(tin);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đăng tin thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(tin);
        }

        // 4. XÓA TIN (Admin only)
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return Forbid();

            var tin = await _context.TinTuyenSinhs.FindAsync(id);
            if (tin != null)
            {
                _context.TinTuyenSinhs.Remove(tin);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa tin tuyển sinh!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}