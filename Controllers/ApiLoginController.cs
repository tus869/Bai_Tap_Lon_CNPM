using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDASV.Data;
using QLDASV.Models;
using System.Linq;

namespace QLDASV.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class ApiLoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiLoginController(ApplicationDbContext context)
        {
            _context = context;
        }
        public DbSet<DoAn> DoAns { get; set; }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Login model)
        {
            // 1. Kiểm tra dữ liệu đầu vào
            if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new { Success = false, Message = "Vui lòng nhập đầy đủ tài khoản và mật khẩu." });
            }

            // 2. Mật khẩu mặc định
            if (model.Password != "123456")
            {
                return Unauthorized(new { Success = false, Message = "Mật khẩu không đúng." });
            }

            // 3. Tìm trong bảng NguoiDung (Tìm theo TenDangNhap)
            // Lưu ý: Cấu trúc mới dùng TenDangNhap thay vì MaSinhVien/MaGiangVien
            var user = _context.NguoiDungs.FirstOrDefault(u => u.TenDangNhap == model.Username);

            if (user != null)
            {
                // Mapping dữ liệu từ bảng mới sang format JSON cũ để App không bị lỗi
                // VaiTro trong DB: "Student" -> Trả về Client: "SinhVien" (nếu cần khớp logic cũ)
                string roleResponse = user.VaiTro == "Student" ? "SinhVien" :
                                      (user.VaiTro == "Lecturer" ? "GiangVien" : "Admin");

                return Ok(new
                {
                    Success = true,
                    Role = roleResponse,
                    Message = "Đăng nhập thành công!",
                    Info = new
                    {
                        HoVaTen = user.HoTen,       // Cột mới là HoTen
                        Email = "",                 // Email property does not exist, return empty or remove as needed
                        MaSo = user.MaSo,          // Thay cho MaSinhVien
                                                    // ChuyenNganh = user.ChuyenNganh // Thay cho MaLop/Khoa
                    }
                });
            }

            return NotFound(new { Success = false, Message = "Tài khoản không tồn tại." });
        }
    }
}