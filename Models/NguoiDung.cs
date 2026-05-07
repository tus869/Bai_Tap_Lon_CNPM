using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDASV.Models
{
    [Table("NguoiDung")] // Đảm bảo khớp với tên bảng trong SQL
    public class NguoiDung
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã số không được để trống")]
        public string MaSo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string TenDangNhap { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string HoTen { get; set; } = string.Empty;

        public string? VaiTro { get; set; }

        public string? ChuyenNganh { get; set; }

        public string? Lop { get; set; }

        public string? Email { get; set; }

        public string? SoDienThoai { get; set; }
    }
}
