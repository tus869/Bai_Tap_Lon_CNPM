using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDASV.Models
{
    public class DoAn
    {
        [Key]
        public int MaDoAn { get; set; }

        // Các thuộc tính cơ bản
        public string? TenDoAn { get; set; } // Tùy chọn, có thể lấy từ DeTai
        public string? MoTa { get; set; }
        public string? LinkSourceCode { get; set; }
        public string? LinkBaoCao { get; set; }

        // Điểm số (Cho phép null nếu chưa chấm)
        public double? DiemHuongDan { get; set; }
        public double? DiemHoiDong { get; set; }
        public double? DiemTong { get; set; }

        public string TrangThai { get; set; } = "Chờ duyệt"; // Chờ duyệt, Đang thực hiện, Đã hoàn thành
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        // --- KHÓA NGOẠI ---

        // 1. Sinh viên (Bắt buộc)
        public int MaSinhVien { get; set; }
        [ForeignKey("MaSinhVien")]
        public virtual NguoiDung? SinhVien { get; set; }

        // 2. Giảng viên hướng dẫn (QUAN TRỌNG: Thêm dấu ? để cho phép null)
        public int? MaGiangVienHD { get; set; }
        [ForeignKey("MaGiangVienHD")]
        public virtual NguoiDung? GiangVienHD { get; set; }

        // 3. Đề tài (Bắt buộc)
        public int MaDeTai { get; set; }
        [ForeignKey("MaDeTai")]
        public virtual DeTai? DeTai { get; set; }


    }
}