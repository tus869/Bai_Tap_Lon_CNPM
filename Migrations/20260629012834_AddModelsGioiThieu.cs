using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CNPM.Migrations
{
    /// <inheritdoc />
    public partial class AddModelsGioiThieu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MatKhau",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "HinhAnhGioiThieus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrlAnh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HinhAnhGioiThieus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhongChucNangs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TongSo = table.Column<int>(type: "int", nullable: false),
                    SuDungTot = table.Column<int>(type: "int", nullable: false),
                    HongNang = table.Column<int>(type: "int", nullable: false),
                    SuaChuaNho = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongChucNangs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThongKeNhanSus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhanLoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TongSo = table.Column<int>(type: "int", nullable: false),
                    DaiHoc = table.Column<int>(type: "int", nullable: false),
                    SauDaiHoc = table.Column<int>(type: "int", nullable: false),
                    DangBoiDuong = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongKeNhanSus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThongTinNhaTruongs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HieuTruong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuyetDinhThanhLap = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaTruong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoaiHinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoHinhDaoTao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MucDoKiemDinh = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NamThanhLap = table.Column<int>(type: "int", nullable: false),
                    QuyenSuDungDat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TongDienTich = table.Column<double>(type: "float", nullable: false),
                    QuyMoCsvc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhongHocChuan = table.Column<int>(type: "int", nullable: false),
                    PhongHocLon = table.Column<int>(type: "int", nullable: false),
                    PhongHocNho = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongTinNhaTruongs", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "MaTaiKhoan",
                keyValue: 1,
                columns: new[] { "MatKhau", "NgayTao" },
                values: new object[] { "1", new DateTime(2026, 6, 29, 8, 28, 33, 921, DateTimeKind.Local).AddTicks(2408) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HinhAnhGioiThieus");

            migrationBuilder.DropTable(
                name: "PhongChucNangs");

            migrationBuilder.DropTable(
                name: "ThongKeNhanSus");

            migrationBuilder.DropTable(
                name: "ThongTinNhaTruongs");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MatKhau",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "MaTaiKhoan",
                keyValue: 1,
                columns: new[] { "MatKhau", "NgayTao" },
                values: new object[] { "123456", new DateTime(2026, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }
    }
}
