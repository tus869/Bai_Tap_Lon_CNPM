using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CNPM.Models
{
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        public string? Email { get; set; }
        public string? MatKhau { get; set; }
        public string? Role { get; set; }
        public int? MaHocSinh { get; set; } // Khóa ngoại liên kết

        [Required]
        [StringLength(50)]
        public string TenDangNhap { get; set; }

        [Required]
        [StringLength(255)]
        public string MatKhauHash { get; set; }

        [Required]
        [StringLength(20)]
        public string VaiTro { get; set; }

        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime? LanDangNhapCuoi { get; set; }

        // Navigation properties
        public virtual GiaoVien GiaoVien { get; set; }
        public virtual HocSinh HocSinh { get; set; }
        public virtual ICollection<TinTuc> TinTucs { get; set; }
    }
}