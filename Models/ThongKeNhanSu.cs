using System.ComponentModel.DataAnnotations;

namespace CNPM.Models
{
    public class ThongKeNhanSu
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nhập loại nhân sự (VD: Giáo viên biên chế)")]
        public string PhanLoai { get; set; }

        public int TongSo { get; set; }
        public int DaiHoc { get; set; }
        public int SauDaiHoc { get; set; }
        public int DangBoiDuong { get; set; }

        public string GhiChu { get; set; }
    }
}