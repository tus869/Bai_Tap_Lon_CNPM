using System.ComponentModel.DataAnnotations;

namespace CNPM.Models
{
    public class HinhAnhGioiThieu
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UrlAnh { get; set; }       // Đường dẫn ảnh lưu trong /wwwroot/images/

        [StringLength(100)]
        public string TieuDe { get; set; }       // Dòng chữ hiển thị đè lên ảnh (VD: Căng tin nhà trường)

        public int ThuTu { get; set; }           // Sắp xếp ảnh nào hiện trước, ảnh nào hiện sau

        public bool TrangThai { get; set; } = true; // True = Đang hiển thị, False = Đã ẩn
    }
}