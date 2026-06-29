using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class BangDiem
    {
        [Key]
        public int MaDiem { get; set; }

        [Required]
        public int MaHS { get; set; }
        [ForeignKey("MaHS")]
        public virtual HocSinh HocSinh { get; set; }

        [Required]
        public int MaMon { get; set; }
        [ForeignKey("MaMon")]
        public virtual MonHoc MonHoc { get; set; }

        [Required]
        public int HocKy { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
        public double? DiemMieng { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
        public double? Diem15P { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
        public double? Diem45P { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
        public double? DiemThi { get; set; }

        // Điểm trung bình được hệ thống tự tính
        public double? DiemTrungBinh { get; set; }

        // Trạng thái cho quy trình Maker - Checker
        public string TrangThai { get; set; } = "Chờ duyệt";
    }
}