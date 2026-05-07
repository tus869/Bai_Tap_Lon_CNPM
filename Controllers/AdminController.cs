using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDASV.Data;
using QLDASV.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLDASV.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Kiểm tra quyền Admin
        private bool IsAdmin() => HttpContext.Session.GetString("UserRole") == "Admin";

        // --- 1. THỐNG KÊ & DASHBOARD ---
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Login");

            // Thống kê số lượng
            ViewBag.TotalStudents = await _context.NguoiDungs.CountAsync(u => u.VaiTro == "Student");
            ViewBag.TotalLecturers = await _context.NguoiDungs.CountAsync(u => u.VaiTro == "Lecturer");
            ViewBag.TotalProjects = await _context.DoAns.CountAsync();

            // Tính điểm trung bình (DiemTong)
            var projectsWithGrades = await _context.DoAns.Where(d => d.DiemTong != null).ToListAsync();
            ViewBag.AvgScore = projectsWithGrades.Any() ? Math.Round(projectsWithGrades.Average(d => d.DiemTong.Value), 2) : 0;

            // Dữ liệu biểu đồ trạng thái đồ án
            var statusData = await _context.DoAns
                .GroupBy(d => d.TrangThai)
                .Select(g => new { Label = g.Key, Count = g.Count() }).ToListAsync();

            ViewBag.ChartStatusLabels = JsonSerializer.Serialize(statusData.Select(x => x.Label));
            ViewBag.ChartStatusData = JsonSerializer.Serialize(statusData.Select(x => x.Count));

            return View();
        }

        // --- 2. QUẢN LÝ SINH VIÊN & GIẢNG VIÊN ---
        public async Task<IActionResult> UserList(string vaiTro, string keyword)
        {
            if (!IsAdmin()) return Forbid();
            var users = _context.NguoiDungs.AsQueryable();
            if (!string.IsNullOrEmpty(vaiTro)) users = users.Where(u => u.VaiTro == vaiTro);
            if (!string.IsNullOrEmpty(keyword)) users = users.Where(u => u.HoTen.Contains(keyword) || u.MaSo.Contains(keyword));

            return View(await users.ToListAsync());
        }

        // (Các hàm Create/Edit/Delete User tương tự như mẫu bạn đã có nhưng dùng UserList để quay về)

        // --- 3. QUẢN LÝ ĐỀ TÀI (Topic) ---
        public async Task<IActionResult> TopicList()
        {
            return View(await _context.DeTais.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateTopic(DeTai deTai)
        {
            if (ModelState.IsValid)
            {
                _context.DeTais.Add(deTai);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TopicList));
            }
            return View(deTai);
        }

        // --- 4. PHÂN CÔNG & QUẢN LÝ ĐỒ ÁN ---
        public async Task<IActionResult> ProjectList(string trangThai)
        {
            if (!IsAdmin()) return Forbid();

            var projects = _context.DoAns
                .Include(d => d.SinhVien)
                .Include(d => d.GiangVienHD)
                .Include(d => d.DeTai)
                .AsQueryable();

            if (!string.IsNullOrEmpty(trangThai)) projects = projects.Where(d => d.TrangThai == trangThai);

            return View(await projects.ToListAsync());
        }

        public IActionResult AssignProject()
        {
            ViewBag.MaSinhVien = new SelectList(_context.NguoiDungs.Where(u => u.VaiTro == "Student"), "Id", "HoTen");
            ViewBag.MaGiangVienHD = new SelectList(_context.NguoiDungs.Where(u => u.VaiTro == "Lecturer"), "Id", "HoTen");
            ViewBag.MaDeTai = new SelectList(_context.DeTais, "MaDeTai", "TenDeTai");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignProject(DoAn doAn)
        {
            doAn.TrangThai = "Đang thực hiện";
            doAn.NgayDangKy = DateTime.Now;
            _context.DoAns.Add(doAn);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ProjectList));
        }

        // --- 5. NHẬP ĐIỂM ĐỒ ÁN ---
        public async Task<IActionResult> GradeProject(int id)
        {
            var doAn = await _context.DoAns.Include(d => d.SinhVien).FirstOrDefaultAsync(m => m.MaDoAn == id);
            return View(doAn);
        }

        [HttpPost]
        public async Task<IActionResult> GradeProject(int id, double diemHD, double diemHDong)
        {
            var doAn = await _context.DoAns.FindAsync(id);
            if (doAn != null)
            {
                doAn.DiemHuongDan = diemHD;
                doAn.DiemHoiDong = diemHDong;
                doAn.DiemTong = (diemHD + diemHDong) / 2; // Công thức tính điểm tổng
                doAn.TrangThai = "Đã hoàn thành";

                _context.Update(doAn);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ProjectList));
        }
    }
}