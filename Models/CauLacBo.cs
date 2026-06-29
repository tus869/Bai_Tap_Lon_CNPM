using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class CauLacBo
    {
        [Key] // Đây là khóa chính duy nhất của bảng CauLacBo
        public int MaCLB { get; set; }

        [Required]
        [StringLength(150)]
        public string TenCLB { get; set; }

        public int? MaGVPhuTrach { get; set; }
        [ForeignKey("MaGVPhuTrach")]
        public virtual GiaoVien? GiaoVienPhuTrach { get; set; }

        [StringLength(100)]
        public string? LichSinhHoat { get; set; }

        [StringLength(50)]
        public string? ThoiGian { get; set; }

        public string? MoTa { get; set; }

        public string TrangThai { get; set; } = "Chờ duyệt";

        // Navigation property
        public virtual ICollection<ThamGiaCLB>? ThamGiaCLBs { get; set; }
    }
}