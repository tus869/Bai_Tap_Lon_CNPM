using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CNPM.Data;
using CNPM.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace CNPM.Controllers
{
    // Yêu cầu người dùng phải đăng nhập và có quyền "Admin" mới được truy cập vào Controller này
    [Authorize(Roles = "Admin")]
    public class MonHocController : Controller
    {
        // Khai báo biến _context (đại diện cho CSDL) ở chế độ chỉ đọc (readonly) để bảo vệ dữ liệu
        private readonly SchoolContext _context;

        // Constructor: Sử dụng cơ chế Dependency Injection (Tiêm phụ thuộc) để truyền SchoolContext vào
        public MonHocController(SchoolContext context)
        {
            _context = context;
        }

        // 1. CHỨC NĂNG XEM DANH SÁCH MÔN HỌC (READ)
        // Dùng bất đồng bộ (async/await) để tối ưu hiệu suất, không làm treo server khi chờ lấy dữ liệu từ DB
        public async Task<IActionResult> Index()
        {
            // Lấy toàn bộ danh sách môn học từ CSDL và truyền sang View để hiển thị
            return View(await _context.MonHocs.ToListAsync());
        }

        // 2. CHỨC NĂNG THÊM MỚI (CREATE) - Bước hiển thị Form
        // Phương thức GET: Chỉ hiển thị giao diện form nhập liệu (không tương tác DB)
        public IActionResult Create()
        {
            return View();
        }

        // 3. CHỨC NĂNG THÊM MỚI (CREATE) - Bước xử lý dữ liệu gửi lên
        [HttpPost] // Đánh dấu đây là phương thức nhận dữ liệu gửi lên từ form (POST request)
        [ValidateAntiForgeryToken] // Bảo mật: Chống tấn công giả mạo request liên trang (CSRF)
        // [Bind]: Chỉ cho phép nhận đúng 2 trường TenMon và SoTiet từ form, chống tấn công Overposting
        public async Task<IActionResult> Create([Bind("TenMon,SoTiet")] MonHoc monHoc)
        {
            // Kiểm tra xem dữ liệu nhập vào có hợp lệ không (ví dụ: có bỏ trống tên môn không?)
            if (ModelState.IsValid)
            {
                _context.Add(monHoc);             // Thêm đối tượng mới vào bộ nhớ tạm của context
                await _context.SaveChangesAsync();// Lưu thay đổi xuống cơ sở dữ liệu thực tế
                return RedirectToAction(nameof(Index)); // Thành công thì chuyển hướng về trang danh sách (Index)
            }
            // Nếu dữ liệu lỗi, trả lại đúng View Create kèm theo dữ liệu cũ để người dùng sửa
            return View(monHoc);
        }

        // 4. CHỨC NĂNG XÓA (DELETE) - Bước hiển thị trang xác nhận xóa
        // Phương thức GET: Nhận vào một ID (có thể null) để tìm môn học cần xóa
        public async Task<IActionResult> Delete(int? id)
        {
            // Nếu không truyền ID lên thì trả về lỗi 404 (Không tìm thấy)
            if (id == null) return NotFound();

            // Tìm môn học đầu tiên có Mã môn trùng với ID truyền vào
            var monHoc = await _context.MonHocs.FirstOrDefaultAsync(m => m.MaMon == id);

            // Nếu tìm không thấy môn học trong DB thì cũng báo lỗi 404
            if (monHoc == null) return NotFound();

            // Tìm thấy thì truyền đối tượng môn học sang View để hiển thị chi tiết cho admin xác nhận
            return View(monHoc);
        }

        // 5. CHỨC NĂNG XÓA (DELETE) - Bước xử lý xóa thực sự trong DB
        [HttpPost, ActionName("Delete")] // Đổi tên Action thành "Delete" cho đúng chuẩn routing dù tên hàm là DeleteConfirmed
        [ValidateAntiForgeryToken] // Vẫn phải có bảo mật chống CSRF
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Tìm môn học cần xóa dựa vào ID
            var monHoc = await _context.MonHocs.FindAsync(id);

            // Nếu tìm thấy, đánh dấu môn học này là "Cần xóa"
            if (monHoc != null) _context.MonHocs.Remove(monHoc);

            // Lưu lại thay đổi xuống DB (thực hiện lệnh DELETE trong SQL)
            await _context.SaveChangesAsync();

            // Xóa xong thì quay về trang danh sách
            return RedirectToAction(nameof(Index));
        }
    }
}