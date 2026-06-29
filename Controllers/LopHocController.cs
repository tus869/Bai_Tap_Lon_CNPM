using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CNPM.Data;
using CNPM.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace CNPM.Controllers
{
    // BẮT BUỘC: Chỉ Admin (Ban Giám Hiệu) mới được quản lý lớp
    [Authorize(Roles = "Admin")]
    public class LopHocController : Controller
    {
        private readonly SchoolContext _context;

        public LopHocController(SchoolContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH LỚP HỌC
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách lớp kèm theo thông tin Giáo viên chủ nhiệm
            var danhSachLop = await _context.LopHocs.Include(l => l.GiaoVien).ToListAsync();
            return View(danhSachLop);
        }

        // 2. FORM THÊM MỚI LỚP HỌC (GET)
        public IActionResult Create()
        {
            // Lấy danh sách toàn bộ giáo viên để BGH chọn làm Chủ nhiệm
            ViewBag.MaGV = new SelectList(_context.GiaoViens, "MaGV", "HoTen");
            return View();
        }

        // 3. XỬ LÝ LƯU LỚP HỌC (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenLop,MaGV")] LopHoc lopHoc)
        {
            // Loại bỏ kiểm tra các trường liên kết để tránh lỗi "Field is required"
            ModelState.Remove("GiaoVien");
            ModelState.Remove("HocSinhs");

            if (ModelState.IsValid)
            {
                _context.LopHocs.Add(lopHoc);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Đã tạo lớp {lopHoc.TenLop} và phân công Giáo viên chủ nhiệm thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaGV = new SelectList(_context.GiaoViens, "MaGV", "HoTen", lopHoc.MaGV);
            return View(lopHoc);
        }

        // 4. XEM CHI TIẾT LỚP HỌC (Gồm GV Chủ nhiệm và Danh sách Học sinh)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var lopHoc = await _context.LopHocs
                .Include(l => l.GiaoVien)    // Lấy thông tin GV Chủ nhiệm
                .Include(l => l.HocSinhs)    // Lấy toàn bộ Học sinh thuộc lớp này
                .FirstOrDefaultAsync(m => m.MaLop == id);

            if (lopHoc == null) return NotFound();

            return View(lopHoc);
        }
        [HttpPost]
        public async Task<IActionResult> SuaTenLop(int maLop, string tenLopMoi)
        {
            // Tìm lớp học trong Database dựa vào Mã lớp
            var lop = await _context.LopHocs.FindAsync(maLop);

            if (lop != null)
            {
                // Cập nhật tên mới
                lop.TenLop = tenLopMoi;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã cập nhật tên lớp thành công!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy lớp học này.";
            }

            // Load lại trang danh sách (Sửa "Index" thành tên View danh sách của bạn nếu khác)
            return RedirectToAction(nameof(Index));
        }
    }
}