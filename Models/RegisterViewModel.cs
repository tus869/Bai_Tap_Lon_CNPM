using System.ComponentModel.DataAnnotations;

namespace QLDASV.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Mã Sinh viên / Mã Giảng viên")]
        [Display(Name = "Tên đăng nhập (Mã số)")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Họ và Tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Mật khẩu")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp")]
        public string ConfirmPassword { get; set; }

        // Chọn vai trò: SinhVien hoặc GiangVien
        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        public string Role { get; set; }
    }
}