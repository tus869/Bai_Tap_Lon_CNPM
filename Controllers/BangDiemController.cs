using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CNPM.Data;
using CNPM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CNPM.Controllers
{
    [Authorize(Roles = "Admin, Teacher, Student")]
    public class BangDiemController : Controller
    {
        private readonly SchoolContext _context;

        public BangDiemController(SchoolContext context)
        {
            _context = context;
        }

        #region 1. CHỨC NĂNG CHUNG (XEM THÔNG TIN)

        [Authorize(Roles = "Admin, Teacher")]
        public async Task<IActionResult> ChiTietDiemHocSinh(int id)
        {
            // Lấy thông tin chi tiết của học sinh kèm theo lớp
            var hocSinh = await _context.HocSinhs.Include(h => h.LopHoc).FirstOrDefaultAsync(h => h.MaHS == id);
            if (hocSinh == null) return NotFound();

            // Chỉ hiển thị bảng điểm đã được duyệt
            var bangDiem = await _context.BangDiems
                .Include(b => b.MonHoc)
                .Where(b => b.MaHS == id && b.TrangThai == "Đã duyệt")
                .ToListAsync();

            ViewBag.HocSinhInfo = hocSinh;
            return View(bangDiem);
        }

        [Authorize(Roles = "Admin, Teacher")]
        public async Task<IActionResult> Index()
        {
            // Hiển thị danh sách toàn bộ học sinh để chọn xem điểm
            var danhSachHocSinh = await _context.HocSinhs.Include(h => h.LopHoc).ToListAsync();
            return View(danhSachHocSinh);
        }
        #endregion

        #region 2. CHỨC NĂNG GIÁO VIÊN (NHẬP & SỬA ĐIỂM)

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> NhapDiem()
        {
            // Lấy thông tin GV từ Claims để xác định lớp mình chủ nhiệm
            var maTaiKhoanClaim = User.FindFirst("MaTaiKhoan")?.Value;
            int maTaiKhoan = int.Parse(maTaiKhoanClaim ?? "0");
            var giaoVien = await _context.GiaoViens.FirstOrDefaultAsync(gv => gv.MaTaiKhoan == maTaiKhoan);

            // Chỉ lấy học sinh thuộc lớp của giáo viên này
            var hocSinhCuaLopMinh = await _context.HocSinhs
                .Where(hs => hs.LopHoc.MaGV == giaoVien.MaGV)
                .ToListAsync();

            ViewBag.DanhSachHocSinh = new SelectList(hocSinhCuaLopMinh, "MaHS", "HoTen");
            ViewBag.DanhSachMonHoc = new SelectList(await _context.MonHocs.ToListAsync(), "MaMon", "TenMon");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> NhapDiem([Bind("MaHS,MaMon,HocKy,DiemMieng,Diem15P,Diem45P,DiemThi")] BangDiem bangDiem)
        {
            // Loại bỏ các trường không cần thiết trong validate form
            ModelState.Remove("HocSinh"); ModelState.Remove("MonHoc");
            ModelState.Remove("TrangThai"); ModelState.Remove("DiemTrungBinh");

            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp: Một HS chỉ có 1 bảng điểm/môn/học kỳ
                bool daCoDiem = await _context.BangDiems.AnyAsync(b =>
                    b.MaHS == bangDiem.MaHS && b.MaMon == bangDiem.MaMon && b.HocKy == bangDiem.HocKy);

                if (daCoDiem)
                {
                    ModelState.AddModelError("", "Học sinh này đã có điểm môn này. Vui lòng chỉnh sửa tại mục 'Danh sách điểm'.");
                }
                else
                {
                    // Logic tính điểm trung bình (He số: Miệng 1, 15p 1, 45p 2, Thi 3)
                    // ... (đoạn tính toán của bạn)
                    bangDiem.TrangThai = "Chờ duyệt";
                    _context.Add(bangDiem);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Gửi yêu cầu duyệt điểm thành công!";
                    return RedirectToAction(nameof(NhapDiem));
                }
            }
            return View(bangDiem);
        }
        #endregion

        #region 3. CHỨC NĂNG ADMIN (QUẢN LÝ & DUYỆT ĐIỂM)

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DuyetDiem()
        {
            // Lọc danh sách điểm đang ở trạng thái 'Chờ duyệt'
            var danhSachChoDuyet = await _context.BangDiems
                .Include(b => b.HocSinh).ThenInclude(h => h.LopHoc)
                .Include(b => b.MonHoc)
                .Where(b => b.TrangThai == "Chờ duyệt")
                .ToListAsync();
            return View(danhSachChoDuyet);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> XuLyDuyet(int id, string action)
        {
            var bangDiem = await _context.BangDiems.FindAsync(id);
            if (bangDiem == null) return NotFound();

            // Cập nhật trạng thái dựa trên hành động của Admin
            bangDiem.TrangThai = (action == "Approve") ? "Đã duyệt" : "Từ chối";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DuyetDiem));
        }
        #endregion

        #region 4. CHỨC NĂNG HỌC SINH (TRA CỨU)

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> XemDiem()
        {
            // Lấy MaHS từ User.Claims để bảo mật, tránh việc học sinh tự ý sửa ID trên URL
            var maTaiKhoanClaim = User.FindFirst("MaTaiKhoan")?.Value;
            int maTaiKhoan = int.Parse(maTaiKhoanClaim ?? "0");

            var hocSinh = await _context.HocSinhs.FirstOrDefaultAsync(hs => hs.MaTaiKhoan == maTaiKhoan);

            // Chỉ hiển thị điểm đã được duyệt chính thức
            var bangDiemCaNhan = await _context.BangDiems
                .Include(b => b.MonHoc)
                .Where(b => b.MaHS == hocSinh.MaHS && b.TrangThai == "Đã duyệt")
                .OrderBy(b => b.HocKy)
                .ToListAsync();

            return View(bangDiemCaNhan);
        }
        #endregion
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> DanhSachDiem()
        {
            // Lấy danh sách điểm kèm thông tin Học sinh và Môn học
            var danhSachDiem = await _context.BangDiems
                .Include(b => b.HocSinh)
                .Include(b => b.MonHoc)
                .ToListAsync();

            return View(danhSachDiem);
        }
        // ĐỔI TÊN THÀNH EditDiem ĐỂ KHỚP VỚI ĐƯỜNG DẪN URL
        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> EditDiem(int? id)
        {
            if (id == null) return NotFound();

            var bangDiem = await _context.BangDiems
                .Include(b => b.HocSinh)
                .Include(b => b.MonHoc)
                .FirstOrDefaultAsync(m => m.MaDiem == id);

            if (bangDiem == null) return NotFound();

            // Để không bị lỗi "Select List" ở trang Edit, bạn cần load lại dữ liệu cho dropdown
            ViewBag.DanhSachHocSinh = new SelectList(await _context.HocSinhs.ToListAsync(), "MaHS", "HoTen", bangDiem.MaHS);
            ViewBag.DanhSachMonHoc = new SelectList(await _context.MonHocs.ToListAsync(), "MaMon", "TenMon", bangDiem.MaMon);

            return View(bangDiem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> EditDiem(int id, [Bind("MaDiem,MaHS,MaMon,HocKy,DiemMieng,Diem15P,Diem45P,DiemThi")] BangDiem bangDiem)
        {
            if (id != bangDiem.MaDiem) return NotFound();

            ModelState.Remove("HocSinh"); ModelState.Remove("MonHoc"); // Loại bỏ lỗi validate model

            if (ModelState.IsValid)
            {
                bangDiem.TrangThai = "Chờ duyệt"; // Mỗi khi sửa điểm, đưa về trạng thái chờ duyệt
                _context.Update(bangDiem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã cập nhật điểm thành công!";
                return RedirectToAction("DanhSachDiem");
            }
            return View(bangDiem);
        }
    }
}