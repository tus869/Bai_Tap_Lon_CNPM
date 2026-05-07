using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDASV.Models
{
    // Đổi tên class DeTais thành DeTai (số ít) cho chuẩn EF Core
    public class DeTai
    {
        [Key]
        public int MaDeTai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đề tài")]
        public string TenDeTai { get; set; }

        public string? MoTa { get; set; }

        public string? ChuyenNganh { get; set; } // CNTT, KinhTe...

        public int SoLuongToiDa { get; set; } = 1; // Mặc định 1 sinh viên

        public DateTime? NgayTao { get; set; } = DateTime.Now;

        public bool TrangThai { get; set; } = true;

        // BỔ SUNG: Khóa ngoại liên kết với Giảng viên (để biết GV nào đã "nhận thầu" đề tài này)
        public int? MaGiangVien { get; set; }
        [ForeignKey("MaGiangVien")]
        public virtual NguoiDung? GiangVien { get; set; }
    }
}