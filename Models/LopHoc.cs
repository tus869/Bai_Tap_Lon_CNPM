using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class LopHoc
    {
        [Key]
        public int MaLop { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên lớp")]
        [StringLength(50)]
        public string TenLop { get; set; }

        // BỔ SUNG: Khóa ngoại liên kết với Giáo Viên để làm Giáo viên Chủ nhiệm
        public int? MaGV { get; set; }

        [ForeignKey("MaGV")]
        public virtual GiaoVien GiaoVien { get; set; }

        // BỔ SUNG: Danh sách học sinh thuộc lớp này
        public virtual ICollection<HocSinh> HocSinhs { get; set; }
    }
}