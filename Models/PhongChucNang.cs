using System.ComponentModel.DataAnnotations;

namespace CNPM.Models
{
    public class PhongChucNang
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên phòng không được để trống")]
        public string TenPhong { get; set; }

        public int TongSo { get; set; }
        public int SuDungTot { get; set; }

        // Mức độ hư hỏng
        public int HongNang { get; set; }
        public int SuaChuaNho { get; set; }
    }
}