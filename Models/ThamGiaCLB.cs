using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class ThamGiaCLB
    {
        public int MaHS { get; set; }
        [ForeignKey("MaHS")]
        public virtual HocSinh HocSinh { get; set; }

        public int MaCLB { get; set; }
        [ForeignKey("MaCLB")]
        public virtual CauLacBo CauLacBo { get; set; }

        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string VaiTro { get; set; } = "Thành viên";

        // Thêm cột này để Admin/Giáo viên duyệt
        [StringLength(50)]
        public string TrangThai { get; set; } = "Chờ duyệt";
    }
}