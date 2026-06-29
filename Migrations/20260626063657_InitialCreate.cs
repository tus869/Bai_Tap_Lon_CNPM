using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CNPM.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonHocs",
                columns: table => new
                {
                    MaMon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenMon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoTiet = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonHocs", x => x.MaMon);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaHocSinh = table.Column<int>(type: "int", nullable: true),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhauHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LanDangNhapCuoi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "GiaoViens",
                columns: table => new
                {
                    MaGV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ToChuyenMon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChuyenMon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThaiCongTac = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaoViens", x => x.MaGV);
                    table.ForeignKey(
                        name: "FK_GiaoViens_TaiKhoans_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTaiKhoan");
                });

            migrationBuilder.CreateTable(
                name: "TinTucs",
                columns: table => new
                {
                    MaTinTuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HinhAnhURL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgayDang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaNguoiDang = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinTucs", x => x.MaTinTuc);
                    table.ForeignKey(
                        name: "FK_TinTucs_TaiKhoans_MaNguoiDang",
                        column: x => x.MaNguoiDang,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTaiKhoan");
                });

            migrationBuilder.CreateTable(
                name: "CauLacBos",
                columns: table => new
                {
                    MaCLB = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCLB = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MaGVPhuTrach = table.Column<int>(type: "int", nullable: true),
                    LichSinhHoat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ThoiGian = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauLacBos", x => x.MaCLB);
                    table.ForeignKey(
                        name: "FK_CauLacBos_GiaoViens_MaGVPhuTrach",
                        column: x => x.MaGVPhuTrach,
                        principalTable: "GiaoViens",
                        principalColumn: "MaGV");
                });

            migrationBuilder.CreateTable(
                name: "LopHocs",
                columns: table => new
                {
                    MaLop = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLop = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaGV = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LopHocs", x => x.MaLop);
                    table.ForeignKey(
                        name: "FK_LopHocs_GiaoViens_MaGV",
                        column: x => x.MaGV,
                        principalTable: "GiaoViens",
                        principalColumn: "MaGV");
                });

            migrationBuilder.CreateTable(
                name: "HocSinhs",
                columns: table => new
                {
                    MaHS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaLop = table.Column<int>(type: "int", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    HoTenPhuHuynh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SdtPhuHuynh = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    TrangThaiHoc = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HocSinhs", x => x.MaHS);
                    table.ForeignKey(
                        name: "FK_HocSinhs_LopHocs_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHocs",
                        principalColumn: "MaLop");
                    table.ForeignKey(
                        name: "FK_HocSinhs_TaiKhoans_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTaiKhoan");
                });

            migrationBuilder.CreateTable(
                name: "HoSoTuyenSinhs",
                columns: table => new
                {
                    MaHoSo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiemToan = table.Column<double>(type: "float", nullable: true),
                    DiemVan = table.Column<double>(type: "float", nullable: true),
                    DiemAnh = table.Column<double>(type: "float", nullable: true),
                    TruongCu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaLop = table.Column<int>(type: "int", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HoTenPhuHuynh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SdtPhuHuynh = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoTuyenSinhs", x => x.MaHoSo);
                    table.ForeignKey(
                        name: "FK_HoSoTuyenSinhs_LopHocs_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHocs",
                        principalColumn: "MaLop");
                });

            migrationBuilder.CreateTable(
                name: "PhanCongs",
                columns: table => new
                {
                    MaPhanCong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaGV = table.Column<int>(type: "int", nullable: false),
                    MaLop = table.Column<int>(type: "int", nullable: false),
                    MaMon = table.Column<int>(type: "int", nullable: false),
                    HocKy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanCongs", x => x.MaPhanCong);
                    table.ForeignKey(
                        name: "FK_PhanCongs_GiaoViens_MaGV",
                        column: x => x.MaGV,
                        principalTable: "GiaoViens",
                        principalColumn: "MaGV",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhanCongs_LopHocs_MaLop",
                        column: x => x.MaLop,
                        principalTable: "LopHocs",
                        principalColumn: "MaLop",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhanCongs_MonHocs_MaMon",
                        column: x => x.MaMon,
                        principalTable: "MonHocs",
                        principalColumn: "MaMon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BangDiems",
                columns: table => new
                {
                    MaDiem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHS = table.Column<int>(type: "int", nullable: false),
                    MaMon = table.Column<int>(type: "int", nullable: false),
                    HocKy = table.Column<int>(type: "int", nullable: false),
                    DiemMieng = table.Column<double>(type: "float", nullable: true),
                    Diem15P = table.Column<double>(type: "float", nullable: true),
                    Diem45P = table.Column<double>(type: "float", nullable: true),
                    DiemThi = table.Column<double>(type: "float", nullable: true),
                    DiemTrungBinh = table.Column<double>(type: "float", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangDiems", x => x.MaDiem);
                    table.ForeignKey(
                        name: "FK_BangDiems_HocSinhs_MaHS",
                        column: x => x.MaHS,
                        principalTable: "HocSinhs",
                        principalColumn: "MaHS",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BangDiems_MonHocs_MaMon",
                        column: x => x.MaMon,
                        principalTable: "MonHocs",
                        principalColumn: "MaMon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThamGiaCLBs",
                columns: table => new
                {
                    MaHS = table.Column<int>(type: "int", nullable: false),
                    MaCLB = table.Column<int>(type: "int", nullable: false),
                    NgayDangKy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThamGiaCLBs", x => new { x.MaHS, x.MaCLB });
                    table.ForeignKey(
                        name: "FK_ThamGiaCLBs_CauLacBos_MaCLB",
                        column: x => x.MaCLB,
                        principalTable: "CauLacBos",
                        principalColumn: "MaCLB",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThamGiaCLBs_HocSinhs_MaHS",
                        column: x => x.MaHS,
                        principalTable: "HocSinhs",
                        principalColumn: "MaHS",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BangDiems_MaHS",
                table: "BangDiems",
                column: "MaHS");

            migrationBuilder.CreateIndex(
                name: "IX_BangDiems_MaMon",
                table: "BangDiems",
                column: "MaMon");

            migrationBuilder.CreateIndex(
                name: "IX_CauLacBos_MaGVPhuTrach",
                table: "CauLacBos",
                column: "MaGVPhuTrach");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoViens_MaTaiKhoan",
                table: "GiaoViens",
                column: "MaTaiKhoan",
                unique: true,
                filter: "[MaTaiKhoan] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HocSinhs_MaLop",
                table: "HocSinhs",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_HocSinhs_MaTaiKhoan",
                table: "HocSinhs",
                column: "MaTaiKhoan",
                unique: true,
                filter: "[MaTaiKhoan] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoTuyenSinhs_MaLop",
                table: "HoSoTuyenSinhs",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_LopHocs_MaGV",
                table: "LopHocs",
                column: "MaGV");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongs_MaGV",
                table: "PhanCongs",
                column: "MaGV");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongs_MaLop",
                table: "PhanCongs",
                column: "MaLop");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongs_MaMon",
                table: "PhanCongs",
                column: "MaMon");

            migrationBuilder.CreateIndex(
                name: "IX_ThamGiaCLBs_MaCLB",
                table: "ThamGiaCLBs",
                column: "MaCLB");

            migrationBuilder.CreateIndex(
                name: "IX_TinTucs_MaNguoiDang",
                table: "TinTucs",
                column: "MaNguoiDang");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BangDiems");

            migrationBuilder.DropTable(
                name: "HoSoTuyenSinhs");

            migrationBuilder.DropTable(
                name: "PhanCongs");

            migrationBuilder.DropTable(
                name: "ThamGiaCLBs");

            migrationBuilder.DropTable(
                name: "TinTucs");

            migrationBuilder.DropTable(
                name: "MonHocs");

            migrationBuilder.DropTable(
                name: "CauLacBos");

            migrationBuilder.DropTable(
                name: "HocSinhs");

            migrationBuilder.DropTable(
                name: "LopHocs");

            migrationBuilder.DropTable(
                name: "GiaoViens");

            migrationBuilder.DropTable(
                name: "TaiKhoans");
        }
    }
}
