using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class GiaoVien
    {
        [Key]
        public int MaGV { get; set; }

        public int? MaTaiKhoan { get; set; }

        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan TaiKhoan { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        [StringLength(15)]
        public string SoDienThoai { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string ToChuyenMon { get; set; }

        // === ĐÂY LÀ DÒNG BẠN CẦN THÊM VÀO ===
        [StringLength(100)]
        public string? ChuyenMon { get; set; }
        // =====================================

        public bool TrangThaiCongTac { get; set; } = true;

        // Navigation properties
        public virtual ICollection<LopHoc> LopHocChuNhiems { get; set; }
        public virtual ICollection<PhanCong> PhanCongs { get; set; }
        public virtual ICollection<CauLacBo> CauLacBos { get; set; }
    }
}