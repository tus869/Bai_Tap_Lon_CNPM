using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CNPM.Data;
using CNPM.Models;

namespace CNPM.Controllers
{
    // BẮT BUỘC có dòng ": Controller" này thì hệ thống mới hiểu ViewBag và View()
    public class GioiThieuController : Controller
    {
        // Khai báo kết nối Database (_context)
        private readonly SchoolContext _context;

        // Hàm khởi tạo (Constructor) để nhận kết nối Database
        public GioiThieuController(SchoolContext context)
        {
            _context = context;
        }

        // Action Index (Hàm xử lý cho trang Giới thiệu)
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            // 1. Đếm tổng quan từ các bảng có sẵn
            ViewBag.TongSoGiaoVien = await _context.GiaoViens.CountAsync();
            ViewBag.TongSoHocSinh = await _context.HocSinhs.CountAsync();
            ViewBag.TongSoLop = await _context.LopHocs.CountAsync();

            // 2. Đếm số lượng học sinh (Dùng Contains để chống lỗi khoảng trắng và chữ hoa/thường)
            // Khối 10
            ViewBag.K10_Nam = await _context.HocSinhs.CountAsync(h => h.LopHoc != null && h.LopHoc.TenLop.Contains("10") && h.GioiTinh != null && h.GioiTinh.Contains("Nam"));
            ViewBag.K10_Nu = await _context.HocSinhs.CountAsync(h => h.LopHoc != null && h.LopHoc.TenLop.Contains("10") && h.GioiTinh != null && (h.GioiTinh.Contains("Nữ") || h.GioiTinh.Contains("Nu")));
            ViewBag.SoLop10 = await _context.LopHocs.CountAsync(l => l.TenLop.Contains("10"));

            // Khối 11
            ViewBag.K11_Nam = await _context.HocSinhs.CountAsync(h => h.LopHoc != null && h.LopHoc.TenLop.Contains("11") && h.GioiTinh != null && h.GioiTinh.Contains("Nam"));
            ViewBag.K11_Nu = await _context.HocSinhs.CountAsync(h => h.LopHoc != null && h.LopHoc.TenLop.Contains("11") && h.GioiTinh != null && (h.GioiTinh.Contains("Nữ") || h.GioiTinh.Contains("Nu")));
            ViewBag.SoLop11 = await _context.LopHocs.CountAsync(l => l.TenLop.Contains("11"));

            // Khối 12
            ViewBag.K12_Nam = await _context.HocSinhs.CountAsync(h => h.LopHoc != null && h.LopHoc.TenLop.Contains("12") && h.GioiTinh != null && h.GioiTinh.Contains("Nam"));
            ViewBag.K12_Nu = await _context.HocSinhs.CountAsync(h => h.LopHoc != null && h.LopHoc.TenLop.Contains("12") && h.GioiTinh != null && (h.GioiTinh.Contains("Nữ") || h.GioiTinh.Contains("Nu")));
            ViewBag.SoLop12 = await _context.LopHocs.CountAsync(l => l.TenLop.Contains("12"));

            // 3. Lấy dữ liệu từ các bảng Giới thiệu mới tạo
            ViewBag.ThongTinTruong = await _context.ThongTinNhaTruongs.FirstOrDefaultAsync();

            ViewBag.HinhAnhs = await _context.HinhAnhGioiThieus
                                             .Where(h => h.TrangThai == true)
                                             .OrderBy(h => h.ThuTu)
                                             .ToListAsync();

            ViewBag.PhongChucNangs = await _context.PhongChucNangs.ToListAsync();
            ViewBag.ThongKeNhanSus = await _context.ThongKeNhanSus.ToListAsync();

            return View();
        }
    }
}