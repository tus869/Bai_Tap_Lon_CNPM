using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDASV.Data;
using QLDASV.Models;

namespace QLDASV.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper: Lấy ID sinh viên từ Session
        private int? GetStudentId() => HttpContext.Session.GetInt32("UserId");
        private bool IsStudent()
        {
            var role = HttpContext.Session.GetString("UserRole");
            return role == "Student" || role == "SinhVien";
        }

        // --- MẶC ĐỊNH CHUYỂN HƯỚNG VỀ MY PROJECT ---
        public IActionResult Index()
        {
            return RedirectToAction(nameof(MyProject));
        }

        // --- 1. ĐỒ ÁN CỦA TÔI ---
        public async Task<IActionResult> MyProject()
        {
            if (!IsStudent()) return RedirectToAction("Index", "Login");

            int studentId = GetStudentId() ?? 0;

            var myProject = await _context.DoAns
                .Include(d => d.DeTai)
                .Include(d => d.GiangVienHD)
                .FirstOrDefaultAsync(d => d.MaSinhVien == studentId);

            return View(myProject); // Sẽ tự động tìm file Views/SinhVien/MyProject.cshtml
        }

        // --- 2. XEM DANH SÁCH ĐỀ TÀI ĐỂ ĐĂNG KÝ ---
        public async Task<IActionResult> RegisterTopic()
        {
            if (!IsStudent()) return RedirectToAction("Index", "Login");

            int studentId = GetStudentId() ?? 0;

            var hasProject = await _context.DoAns.AnyAsync(d => d.MaSinhVien == studentId);
            if (hasProject)
            {
                TempData["Error"] = "Bạn đã có đồ án, không thể đăng ký thêm!";
                return RedirectToAction(nameof(MyProject));
            }

            // Fix lỗi an toàn dữ liệu: Lọc cẩn thận các đề tài đã có người đăng ký
            var occupiedTopicIds = await _context.DoAns
                .Where(d => d.MaDeTai != null)
                .Select(d => (int)d.MaDeTai)
                .ToListAsync();

            var availableTopics = await _context.DeTais
                .Where(t => t.TrangThai == true && t.MaGiangVien != null && !occupiedTopicIds.Contains(t.MaDeTai))
                .ToListAsync();

            return View(availableTopics); // Sẽ tự động tìm Views/SinhVien/RegisterTopic.cshtml
        }

        // --- 3. XỬ LÝ ĐĂNG KÝ ĐỀ TÀI ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int maDeTai)
        {
            if (!IsStudent()) return Forbid();

            int studentId = GetStudentId() ?? 0;

            var hasProject = await _context.DoAns.AnyAsync(d => d.MaSinhVien == studentId);
            if (hasProject)
            {
                TempData["Error"] = "Bạn đã đăng ký đồ án rồi!";
                return RedirectToAction(nameof(MyProject));
            }

            var deTai = await _context.DeTais.FindAsync(maDeTai);
            if (deTai == null || deTai.MaGiangVien == null)
            {
                TempData["Error"] = "Đề tài không hợp lệ hoặc chưa có giảng viên phụ trách.";
                return RedirectToAction(nameof(RegisterTopic));
            }

            var newDoAn = new DoAn
            {
                MaSinhVien = studentId,
                MaDeTai = maDeTai,
                NgayDangKy = DateTime.Now,
                TrangThai = "Đang thực hiện",
                MaGiangVienHD = deTai.MaGiangVien, // Gán GV từ đề tài sang
                TenDoAn = deTai.TenDeTai
            };

            _context.DoAns.Add(newDoAn);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đăng ký đề tài thành công! Bạn đã được tự động ghép cặp với Giảng viên.";
            return RedirectToAction(nameof(MyProject));
        }
    }
}