using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CNPM.Data; // Cần có thư viện này để gọi SchoolContext
using System.Threading.Tasks;
using System.Linq;

namespace CNPM.Controllers
{
    // BẮT BUỘC: Chỉ có tài khoản Giáo viên mới được vào trang này
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        // Khai báo _context để kết nối Cơ sở dữ liệu
        private readonly SchoolContext _context;

        public TeacherController(SchoolContext context)
        {
            _context = context;
        }

        // Giao diện chính (Dashboard) của Giáo viên
        public IActionResult Index()
        {
            return View();
        }

        // Chức năng: Xem lớp chủ nhiệm của Giáo viên
        public async Task<IActionResult> LopCuaToi()
        {
            // 1. Lấy Tên đăng nhập (Email/SĐT) của người đang dùng hệ thống
            var username = User.Identity.Name;
            var taiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.TenDangNhap == username);

            if (taiKhoan == null) return NotFound("Lỗi xác thực tài khoản.");

            // 2. Tra cứu sang bảng Giáo viên để lấy Mã Giáo Viên (MaGV)
            var giaoVien = await _context.GiaoViens.FirstOrDefaultAsync(g => g.MaTaiKhoan == taiKhoan.MaTaiKhoan);

            if (giaoVien == null) return NotFound("Tài khoản này chưa được liên kết với hồ sơ Giáo viên.");

            // 3. Tìm tất cả các lớp học có MaGV trùng với giáo viên này
            var lopHocs = await _context.LopHocs
                .Include(l => l.HocSinhs) // Gộp bảng Học sinh để đếm được sĩ số
                .Where(l => l.MaGV == giaoVien.MaGV)
                .ToListAsync();

            ViewBag.TenGiaoVien = giaoVien.HoTen;
            return View(lopHocs);
        }
    }
}