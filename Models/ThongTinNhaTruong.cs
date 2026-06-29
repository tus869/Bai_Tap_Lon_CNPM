using System;
using System.ComponentModel.DataAnnotations;

namespace CNPM.Models
{
    public class ThongTinNhaTruong
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên Hiệu trưởng")]
        [StringLength(100)]
        public string HieuTruong { get; set; }

        [Required]
        public string DiaChi { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string DienThoai { get; set; }

        // Hồ sơ pháp lý
        public string QuyetDinhThanhLap { get; set; }
        public string MaTruong { get; set; }
        public string LoaiHinh { get; set; }        // VD: Công lập
        public string MoHinhDaoTao { get; set; }    // VD: THPT
        public string MucDoKiemDinh { get; set; }
        public int NamThanhLap { get; set; }

        // Cơ sở vật chất đất đai
        public string QuyenSuDungDat { get; set; }  // VD: Sở hữu Nhà nước
        public double TongDienTich { get; set; }    // Đơn vị: m2
        public string QuyMoCsvc { get; set; }       // VD: Kiên cố & Bán kiên cố

        // Bảng 2: Dữ liệu phòng học cơ bản (Gộp luôn vào đây cho tiện quản lý)
        public int PhongHocChuan { get; set; }      // 7x9 m2
        public int PhongHocLon { get; set; }        // > 63 m2
        public int PhongHocNho { get; set; }        // < 63 m2
    }
}