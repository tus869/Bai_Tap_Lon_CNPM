using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDASV.Data;
using QLDASV.Models;

namespace QLDASV.Controllers
{
    public class GiangVienController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // Helper: Lấy ID người dùng hiện tại
        private int GetCurrentUserId()
        {
            var idStr = HttpContext.Session.GetString("UserId");
            return int.TryParse(idStr, out int id) ? id : 0;
        }

        // 1. DASHBOARD GIẢNG VIÊN
        public async Task<IActionResult> Index()
        {
            var gvId = GetCurrentUserId();
            if (gvId == 0) return RedirectToAction("Index", "Login");

            // Lấy thông tin giảng viên
            var giangVien = await _context.NguoiDungs.FindAsync(gvId);
            ViewBag.GiangVien = giangVien;

            // --- THỐNG KÊ SỐ LIỆU ---

            // Sửa lỗi: Thay vì đếm DeTai (không có MaGiangVien), ta đếm tổng số DoAn giảng viên này hướng dẫn
            ViewBag.TongSoDoAn = await _context.DoAns.CountAsync(d => d.MaGiangVienHD == gvId);

            // Đếm số sinh viên đang hướng dẫn (Chưa hoàn thành và chưa hủy)
            ViewBag.DangHuongDan = await _context.DoAns.CountAsync(d => d.MaGiangVienHD == gvId
                                            && d.TrangThai != "Đã hoàn thành"
                                            && d.TrangThai != "Hủy");

            // Đếm số đồ án đã hoàn thành
            ViewBag.DaHoanThanh = await _context.DoAns.CountAsync(d => d.MaGiangVienHD == gvId
                                            && d.TrangThai == "Đã hoàn thành");

            // Lấy danh sách 5 đồ án mới nhất
            var recentProjects = await _context.DoAns
                .Include(d => d.SinhVien)
                .Include(d => d.DeTai)
                .Where(d => d.MaGiangVienHD == gvId)
                .OrderByDescending(d => d.NgayDangKy)
                .Take(5)
                .ToListAsync();

            return View(recentProjects);
        }

        // 2. DANH SÁCH SINH VIÊN ĐANG HƯỚNG DẪN
        public async Task<IActionResult> SinhVien(string search)
        {
            var gvId = GetCurrentUserId();
            var query = _context.DoAns
                .Include(d => d.SinhVien)
                .Include(d => d.DeTai)
                .Where(d => d.MaGiangVienHD == gvId);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d =>
                    (d.SinhVien != null && d.SinhVien.HoTen != null && d.SinhVien.HoTen.Contains(search)) ||
                    (d.DeTai != null && d.DeTai.TenDeTai != null && d.DeTai.TenDeTai.Contains(search))
                );
            }

            var result = await query.ToListAsync();
            return View(result);
        }

        // 3. CHI TIẾT ĐỒ ÁN & CHẤM ĐIỂM
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var gvId = GetCurrentUserId();

            var doAn = await _context.DoAns
                .Include(d => d.SinhVien)
                .Include(d => d.DeTai)
                .FirstOrDefaultAsync(m => m.MaDoAn == id);

            // Bảo mật: Chỉ xem được đồ án mình hướng dẫn
            if (doAn == null || doAn.MaGiangVienHD != gvId) return Forbid();

            // Lấy tài liệu sinh viên nộp
            ViewBag.TaiLieus = await _context.TaiLieus
                .Where(t => t.MaDoAn == doAn.MaDoAn)
                .OrderByDescending(t => t.NgayUpload)
                .ToListAsync();

            return View(doAn);
        }

        // 4. XỬ LÝ CHẤM ĐIỂM
        [HttpPost]
        public async Task<IActionResult> ChamDiem(int maDoAn, double diemHuongDan, double diemHoiDong)
        {
            var doAn = await _context.DoAns.FindAsync(maDoAn);
            if (doAn != null)
            {
                doAn.DiemHuongDan = diemHuongDan;
                doAn.DiemHoiDong = diemHoiDong;

                // Tính điểm tổng (Làm tròn 2 chữ số thập phân)
                doAn.DiemTong = Math.Round(((diemHuongDan + diemHoiDong) / 2), 2);

                // Tự động chuyển trạng thái nếu đạt (>= 4.0)
                if (doAn.DiemTong >= 4.0)
                {
                    doAn.TrangThai = "Đã hoàn thành";
                }

                _context.Update(doAn);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã cập nhật điểm số thành công!";
            }
            return RedirectToAction("Details", new { id = maDoAn });
        }

        // 5. QUẢN LÝ ĐỀ TÀI (Link sang trang chung)
        public IActionResult MyTopics()
        {
            return RedirectToAction("Index", "DeTai");
        }
        // 6. DANH SÁCH ĐỒ ÁN CHỜ GIẢNG VIÊN NHẬN (Sinh viên đã đăng ký nhưng chưa có GV)
        public async Task<IActionResult> DanhSachCho()
        {
            var gvId = GetCurrentUserId();
            if (gvId == 0) return RedirectToAction("Index", "Login");

            // Lấy các đồ án đang "Chờ duyệt" và chưa có Giảng viên HD
            var pendingProjects = await _context.DoAns
                .Include(d => d.SinhVien)
                .Include(d => d.DeTai)
                .Where(d => d.MaGiangVienHD == null && d.TrangThai == "Chờ duyệt")
                .OrderByDescending(d => d.NgayDangKy)
                .ToListAsync();

            return View(pendingProjects);
        }

        // 7. XỬ LÝ NHẬN HƯỚNG DẪN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NhanHuongDan(int maDoAn)
        {
            var gvId = GetCurrentUserId();
            if (gvId == 0) return RedirectToAction("Index", "Login");

            var doAn = await _context.DoAns.FindAsync(maDoAn);

            // Nếu đồ án hợp lệ và thực sự chưa có ai nhận
            if (doAn != null && doAn.MaGiangVienHD == null)
            {
                doAn.MaGiangVienHD = gvId; // Gán ID của giảng viên này vào
                doAn.TrangThai = "Đang thực hiện"; // Cập nhật trạng thái

                _context.Update(doAn);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đã nhận hướng dẫn thành công!";
            }
            else
            {
                TempData["Error"] = "Đồ án này đã có người nhận hoặc không tồn tại.";
            }

            return RedirectToAction(nameof(DanhSachCho));
        }
        // 1. XEM KHO ĐỀ TÀI CHUNG (Chưa có ai nhận)
        public async Task<IActionResult> KhoDeTai()
        {
            var kho = await _context.DeTais
                .Where(t => t.MaGiangVien == null && t.TrangThai == true)
                .OrderByDescending(t => t.NgayTao)
                .ToListAsync();
            return View(kho); // Bạn có thể tự tạo View cho hàm này tương tự trang Thư Viện
        }

        // 2. GIẢNG VIÊN BẤM NHẬN ĐỀ TÀI
        [HttpPost]
        public async Task<IActionResult> NhanDeTai(int maDeTai)
        {
            var gvId = GetCurrentUserId();
            var deTai = await _context.DeTais.FindAsync(maDeTai);

            if (deTai != null && deTai.MaGiangVien == null)
            {
                deTai.MaGiangVien = gvId; // Đánh dấu đề tài này là của Giảng viên này
                _context.Update(deTai);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã nhận đề tài thành công! Giờ sinh viên có thể thấy và đăng ký.";
            }
            return RedirectToAction(nameof(KhoDeTai));
        }

        // Hàm này dùng để HIỂN THỊ giao diện chấm điểm (GET)
        [HttpGet]
        public IActionResult ChamDiem(int id = 1)
        {
            // Chỉ đơn giản là gọi cái View (giao diện Mockup) mà chúng ta vừa làm lên
            return View();
        }
    }
}