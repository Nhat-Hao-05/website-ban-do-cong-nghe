using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEStore.Entities;
using System.IO;

namespace MyEStore.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        private readonly MyeStoreContext _ctx;
        public AdminController(MyeStoreContext ctx) => _ctx = ctx;

        public IActionResult Index() => View();

        #region 
        public IActionResult ManagerUsers()
        {
            var danhSach = _ctx.KhachHangs.OrderByDescending(k => k.MaKh).ToList();
            return View(danhSach);
        }

        [HttpPost] 
        public IActionResult ToggleUserStatus(string id)
        {
            var kh = _ctx.KhachHangs.Find(id);
            if (kh != null)
            {
                kh.HieuLuc = !kh.HieuLuc;
                _ctx.SaveChanges();
            }

            return RedirectToAction(nameof(ManagerUsers));
        }

        #endregion

        #region 
        public IActionResult ManagerProducts()
        {
            var products = _ctx.HangHoas.Include(h => h.MaLoaiNavigation).ToList();
            return View(products);
        }
        [HttpGet]
        public IActionResult CreateProducts()
        {
            ViewBag.Loai = _ctx.Loais.ToList();
            try
            {
                ViewBag.NhaCungCap = _ctx.NhaCungCaps.ToList();
            }
            catch
            {
                ViewBag.NhaCungCap = null;
            }
            return View();
        }

        // ----------------------------------------------------
        // 2. HÀM XỬ LÝ LƯU SẢN PHẨM (POST)
        // 🎯 LỖI 405 XẢY RA DO THIẾU THẺ [HttpPost] Ở ĐÂY NÈ!
        // ----------------------------------------------------
        [HttpPost]
        public IActionResult CreateProducts(HangHoa model, IFormFile? Hinh)
        {
            // Bỏ qua kiểm tra các thuộc tính liên kết
            ModelState.Remove(nameof(model.MaLoaiNavigation));
            ModelState.Remove(nameof(model.MaNccNavigation));
            ModelState.Remove(nameof(model.Hinh));

            // Gán các trường bắt buộc của CSDL
            model.NgaySx = DateTime.Now;
            model.SoLanXem = 0;

            if (!string.IsNullOrEmpty(model.TenHh))
            {
                model.TenAlias = model.TenHh.ToLower().Trim().Replace(" ", "-");
            }

            if (string.IsNullOrEmpty(model.MoTaDonVi))
            {
                model.MoTaDonVi = "Chiếc";
            }

            // Xử lý lưu ảnh
            if (Hinh != null && Hinh.Length > 0)
            {
                var fileName = Path.GetFileName(Hinh.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Hinh/HangHoa", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Hinh.CopyTo(stream);
                }
                model.Hinh = fileName;
            }
            else
            {
                model.Hinh = "default.jpg";
            }

            // Lưu vào CSDL
            _ctx.HangHoas.Add(model);
            _ctx.SaveChanges();

            // Lưu xong tự chuyển về trang danh sách sản phẩm
            return RedirectToAction(nameof(ManagerProducts));
        }
        #endregion
        public IActionResult DeleteProduct(int id)
        {
            var hh = _ctx.HangHoas.SingleOrDefault(p => p.MaHh == id);
            if (hh != null)
            {
                try
                {
                    _ctx.HangHoas.Remove(hh);
                    _ctx.SaveChanges();
                   
                    TempData["Message"] = "Đã xóa sản phẩm thành công!";
                }
                catch (Exception ex)
                {
                   
                    TempData["Error"] = "Không thể xóa sản phẩm này vì đã có trong đơn hàng!";
                }
            }

           
            return RedirectToAction("ManagerProducts");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var hh = _ctx.HangHoas.SingleOrDefault(p => p.MaHh == id);
            if (hh == null) return NotFound();

           
            ViewBag.Loai = _ctx.Loais.ToList();
            return View(hh);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HangHoa model, IFormFile? HinhUpload)
        {
            if (id != model.MaHh) return NotFound();

            try
            {
                var hh = _ctx.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hh == null) return NotFound();

               
                hh.TenHh = model.TenHh;
                hh.DonGia = model.DonGia;
                hh.MaLoai = model.MaLoai;
                hh.MoTa = model.MoTa;

                
                if (HinhUpload != null)
                {
                    
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(HinhUpload.FileName);
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "HangHoa", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await HinhUpload.CopyToAsync(stream);
                    }
                    hh.Hinh = fileName; 
                }

                _ctx.Update(hh);
                await _ctx.SaveChangesAsync();
                TempData["Message"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("ManagerProducts"); 
            }
            catch
            {
                ViewBag.Loai = _ctx.Loais.ToList();
                return View(model);
            }
        }
    }
}