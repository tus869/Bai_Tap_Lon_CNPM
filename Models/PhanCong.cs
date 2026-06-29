using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class PhanCong
    {
        [Key]
        public int MaPhanCong { get; set; }

        public int MaGV { get; set; }
        [ForeignKey("MaGV")]
        public virtual GiaoVien GiaoVien { get; set; }

        public int MaLop { get; set; }
        [ForeignKey("MaLop")]
        public virtual LopHoc LopHoc { get; set; }

        public int MaMon { get; set; }
        [ForeignKey("MaMon")]
        public virtual MonHoc MonHoc { get; set; }

        public int HocKy { get; set; }
    }
}