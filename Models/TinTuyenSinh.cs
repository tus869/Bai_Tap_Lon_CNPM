using System;
using System.ComponentModel.DataAnnotations;

namespace QLDASV.Models
{
    public class TinTuyenSinh
    {
        [Key]
        public int Id { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public string HinhAnh { get; set; } // Đường dẫn ảnh
        public DateTime NgayDang { get; set; } = DateTime.Now;
        public bool HienThi { get; set; } = true;
    }
}