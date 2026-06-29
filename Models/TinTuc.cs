using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class TinTuc
    {
        [Key]
        public int MaTinTuc { get; set; }

        [Required]
        [StringLength(255)]
        public string TieuDe { get; set; }

        [Required]
        public string NoiDung { get; set; }

        [StringLength(255)]
        public string HinhAnhURL { get; set; }

        public DateTime NgayDang { get; set; } = DateTime.Now;

        public int? MaNguoiDang { get; set; }
        [ForeignKey("MaNguoiDang")]
        public virtual TaiKhoan TaiKhoanNguoiDang { get; set; }
    }
}