using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEStore.Entities;
using MyEStore.Models;
using System.Linq;

namespace MyEStore.Controllers
{
    public class ProductsController : Controller
    {
        private readonly MyeStoreContext _ctx;

        public ProductsController(MyeStoreContext ctx)
        {
            _ctx = ctx;
        }

        // Cập nhật: Hỗ trợ VỪA tìm kiếm VỪA lọc theo Danh mục (maloai)
        public IActionResult Index(string searchString, int? maloai)
        {
            ViewData["CurrentFilter"] = searchString;

            var query = _ctx.HangHoas.AsQueryable();

            // 1. Lọc theo từ khóa tìm kiếm (nếu có)
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(hh => hh.TenHh.Contains(searchString));
            }

            // 2. Lọc theo Mã loại / Danh mục (nếu có)
            if (maloai.HasValue)
            {
                query = query.Where(hh => hh.MaLoai == maloai.Value);
            }

            var data = query.Select(hh => new HangHoaVM
            {
                MaHh = hh.MaHh,
                TenHh = hh.TenHh,
                DonGia = hh.DonGia,
                Hinh = hh.Hinh
            }).ToList();

            return View(data);
        }

        public IActionResult Details(string slug, int id)
        {
            var product = _ctx.HangHoas
                .Where(p => p.MaHh == id)
                .Select(p => new HangHoaVM
                {
                    MaHh = p.MaHh,
                    TenHh = p.TenHh,
                    DonGia = p.DonGia,
                    Hinh = p.Hinh,
                    MoTa = p.MoTa
                })
                .SingleOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            // Tạo slug chuẩn từ tên sản phẩm
            var correctSlug = product.TenHh.ToSlug();

            // Nếu slug trong URL không khớp thì redirect về URL đúng
            if (slug != correctSlug)
            {
                return RedirectToRoute("product", new { slug = correctSlug, id = product.MaHh });
            }

            return View(product);
        }

        public IActionResult Create()
        {
            var quyenJson = HttpContext.Session.GetString("Quyen");
            if (quyenJson != null)
            {
                var quyenList = System.Text.Json.JsonSerializer.Deserialize<List<PhanQuyen>>(quyenJson);
                var quyenTrang = quyenList.FirstOrDefault(q => q.MaTrang == 1);

                if (quyenTrang == null || !quyenTrang.Them)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
            }

            return View();
        }

        // Action xử lý khi bấm chọn danh mục từ Menu (Dạng: /Products/Category/1001)
        public IActionResult Category(int id)
        {
            var dsSanPham = _ctx.HangHoas
                                .Where(hh => hh.MaLoai == id)
                                .Select(hh => new HangHoaVM
                                {
                                    MaHh = hh.MaHh,
                                    TenHh = hh.TenHh,
                                    DonGia = hh.DonGia,
                                    Hinh = hh.Hinh
                                })
                                .ToList();

            return View("Index", dsSanPham);
        }
    }
}