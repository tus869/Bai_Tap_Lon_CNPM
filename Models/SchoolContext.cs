using Microsoft.EntityFrameworkCore;
using CNPM.Models;

namespace CNPM.Data
{
    public class SchoolContext : DbContext
    {
        public SchoolContext(DbContextOptions<SchoolContext> options) : base(options) { }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<GiaoVien> GiaoViens { get; set; }
        public DbSet<LopHoc> LopHocs { get; set; }
        public DbSet<HocSinh> HocSinhs { get; set; }
        public DbSet<MonHoc> MonHocs { get; set; }
        public DbSet<PhanCong> PhanCongs { get; set; }
        public DbSet<BangDiem> BangDiems { get; set; }
        public DbSet<CauLacBo> CauLacBos { get; set; }
        public DbSet<ThamGiaCLB> ThamGiaCLBs { get; set; }
        public DbSet<TinTuc> TinTucs { get; set; }
        public DbSet<HoSoTuyenSinh> HoSoTuyenSinhs { get; set; }
        public DbSet<ThongTinNhaTruong> ThongTinNhaTruongs { get; set; }
        public DbSet<PhongChucNang> PhongChucNangs { get; set; }
        public DbSet<ThongKeNhanSu> ThongKeNhanSus { get; set; }
        public DbSet<HinhAnhGioiThieu> HinhAnhGioiThieus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình khóa chính phức hợp cho bảng trung gian ThamGiaCLB
            modelBuilder.Entity<ThamGiaCLB>()
                .HasKey(t => new { t.MaHS, t.MaCLB });

            //[THÊM MỚI] Gieo hạt dữ liệu(Data Seeding)
            // Tạo tài khoản Admin mặc định
            modelBuilder.Entity<TaiKhoan>().HasData(
            new TaiKhoan
            {
                MaTaiKhoan = 1,
                TenDangNhap = "admin",
                Email = "admin@thptmuongang.edu.vn",
                MatKhau = "1",
                MatKhauHash = "123456",
                VaiTro = "Admin",
                Role = "Admin",
                TrangThai = true,
                NgayTao = System.DateTime.Now
            }
            );
        }
    }
}