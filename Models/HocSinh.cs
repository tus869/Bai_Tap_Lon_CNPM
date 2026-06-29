using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CNPM.Models
{
    public class HocSinh
    {
        [Key]
        public int MaHS { get; set; }

        public int? MaTaiKhoan { get; set; }
        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan TaiKhoan { get; set; }

        [StringLength(100)]
        public string Email { get; set; } // THÊM DÒNG NÀY

        [StringLength(50)]
        public string TrangThai { get; set; } = "Chờ duyệt";

        public int? MaLop { get; set; }
        [ForeignKey("MaLop")]
        public virtual LopHoc LopHoc { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; }

        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        public string GioiTinh { get; set; }

        [StringLength(255)]
        public string DiaChi { get; set; }

        // Bổ sung SoDienThoai để nhận dữ liệu từ HoSoTuyenSinh
        [StringLength(15)]
        public string SoDienThoai { get; set; }

        [StringLength(100)]
        public string HoTenPhuHuynh { get; set; }

        [StringLength(15)]
        public string SdtPhuHuynh { get; set; }

        public bool TrangThaiHoc { get; set; } = true;

        // BẠN LƯU Ý: Phần khai báo TaiKhoan bị lặp lại lần 2 ở đây ĐÃ ĐƯỢC XÓA BỎ

        // Navigation properties
        public virtual ICollection<BangDiem> BangDiems { get; set; }
        public virtual ICollection<ThamGiaCLB> ThamGiaCLBs { get; set; }
    }
}