using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class HoSoTuyenSinh
    {
        [Key]
        public int MaHoSo { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string HoTen { get; set; }

        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }

        // Điểm xét tuyển
        public double? DiemToan { get; set; }
        public double? DiemVan { get; set; }
        public double? DiemAnh { get; set; }

        public string TruongCu { get; set; }
        public string TrangThai { get; set; }

        public DateTime? NgayDangKy { get; set; }


        public int? MaLop { get; set; }

        [ForeignKey("MaLop")]
        public virtual LopHoc LopHoc { get; set; }

        // BỔ SUNG THÊM DÒNG NÀY ĐỂ TRÁNH LỖI NOT FOUND DIACHI
        public string DiaChi { get; set; }

        public string HoTenPhuHuynh { get; set; }
        public string SdtPhuHuynh { get; set; }
    }
}