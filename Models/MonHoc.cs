using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CNPM.Models
{
    public class MonHoc
    {
        [Key]
        public int MaMon { get; set; }

        [Required]
        [StringLength(100)]
        public string TenMon { get; set; }

        public int SoTiet { get; set; }

        // Navigation properties
        public virtual ICollection<PhanCong> PhanCongs { get; set; }
        public virtual ICollection<BangDiem> BangDiems { get; set; }
    }
}