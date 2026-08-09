using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEStore.Entities;
using MyEStore.Helpers;
using MyEStore.Models;
using MyEStore.Models.ViewModels;

namespace MyEStore.Controllers
{
    public class CartController : Controller
    {
        private readonly MyeStoreContext _ctx;
        public CartController(MyeStoreContext ctx) => _ctx = ctx;


        public List<CartItem> GioHang => HttpContext.Session.Get<List<CartItem>>("CART") ?? new List<CartItem>();

        public IActionResult Index() => View(GioHang);

        public IActionResult AddToCart(int id, int qty = 1)
        {
            var cart = GioHang;
            var item = cart.SingleOrDefault(p => p.MaHh == id);

            if (item != null)
            {
                item.SoLuong += qty;
            }
            else
            {
                var hangHoa = _ctx.HangHoas.SingleOrDefault(p => p.MaHh == id);
                if (hangHoa == null) return RedirectToAction("Index", "Home");

                cart.Add(new CartItem
                {
                    MaHh = id,
                    SoLuong = qty,
                    TenHh = hangHoa.TenHh,
                    Hinh = hangHoa.Hinh,
                    DonGia = hangHoa.DonGia,
                    MoTa = hangHoa.MoTa
                });
            }

            HttpContext.Session.Set("CART", cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateCart(int id, int qty)
        {
            var cart = GioHang;
            var item = cart.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                // Tránh số lượng âm hoặc bằng 0
                if (qty <= 0) cart.Remove(item);
                else item.SoLuong = qty;

                HttpContext.Session.Set("CART", cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult RemoveCart(int id)
        {
            var cart = GioHang;
            var item = cart.SingleOrDefault(p => p.MaHh == id);
            if (item != null)
            {
                cart.Remove(item);
                HttpContext.Session.Set("CART", cart);
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public IActionResult Checkout()
        {
            if (GioHang.Count == 0) return RedirectToAction("Index");
            return View(GioHang);
        }
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Checkout(string hoTen, string dienThoai, string diaChi, string ghiChu, string paymentMethod)
        {
            var cart = GioHang;
            if (cart.Count == 0) return RedirectToAction("Index");

            // Lấy Mã khách hàng chuẩn
            var maKh = User.FindFirst("MaKH")?.Value
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                    ?? User.Identity?.Name;

            var hoaDon = new HoaDon
            {
                MaKh = maKh,
                HoTen = hoTen,
                DiaChi = diaChi,
                CachThanhToan = paymentMethod ?? "COD",
                NgayDat = DateTime.Now,
                NgayCan = DateTime.Now.AddDays(3),
                MaTrangThai = 0,
                GhiChu = $"SĐT: {dienThoai} | Ghi chú: {ghiChu}"
            };

            using (var transaction = await _ctx.Database.BeginTransactionAsync())
            {
                try
                {
                    _ctx.Add(hoaDon);
                    await _ctx.SaveChangesAsync();

                    var chiTietHds = cart.Select(item => new ChiTietHd
                    {
                        MaHd = hoaDon.MaHd,
                        MaHh = item.MaHh,
                        DonGia = item.DonGia,
                        SoLuong = item.SoLuong,
                        GiamGia = 0
                    }).ToList();

                    _ctx.AddRange(chiTietHds);
                    await _ctx.SaveChangesAsync();

                    await transaction.CommitAsync();

                    // Xóa giỏ hàng sau khi lưu DB thành công
                    HttpContext.Session.Remove("CART");

                    // 🎯 ĐOẠN ĐIỀU HƯỚNG MỚI ĐỂ DEMO GIẢ LẬP THANH TOÁN:
                    if (paymentMethod == "COD")
                    {
                        // Nếu chọn COD -> Đến thẳng trang Success
                        return RedirectToAction("Success", new { id = hoaDon.MaHd });
                    }
                    else
                    {
                        // Nếu chọn QR / Ví / Thẻ -> Sang trang Chờ Quét Mã (Giả Lập)
                        return RedirectToAction("ChoThanhToan", new { id = hoaDon.MaHd });
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    TempData["Error"] = "Lỗi đặt hàng: " + errorMsg;

                    return RedirectToAction("Index");
                }
            }
        }
        public IActionResult Success(int id)
        {
            var hoadon = _ctx.HoaDons
                .Where(h => h.MaHd == id)
                .Select(h => new HoaDon
                {
                    MaHd = h.MaHd,
                    HoTen = h.HoTen,
                    DiaChi = h.DiaChi,
                    CachThanhToan = h.CachThanhToan,
                    NgayDat = h.NgayDat,
                    GhiChu = h.GhiChu,
                    ChiTietHds = h.ChiTietHds.Select(ct => new ChiTietHd
                    {
                        MaHd = ct.MaHd,
                        MaHh = ct.MaHh,
                        DonGia = ct.DonGia,
                        SoLuong = ct.SoLuong,
                        MaHhNavigation = ct.MaHhNavigation != null ? new HangHoa
                        {
                            TenHh = ct.MaHhNavigation.TenHh
                        } : null
                    }).ToList()
                })
                .FirstOrDefault();

            if (hoadon == null)
            {
                return NotFound();
            }

            return View(hoadon);
        }
        public IActionResult Fail(string orderId)
        {
            return View(new PaypalTransactionViewModel { OrderId = orderId, Status = "FAILED" });
        }

        [Authorize]
        public IActionResult DonHang()
        {
            // Lấy Mã khách hàng chuẩn
            var maKh = User.FindFirst("MaKH")?.Value
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                    ?? User.Identity?.Name;

            // Lấy danh sách hóa đơn của khách hàng đang đăng nhập
            var dsHoaDon = _ctx.HoaDons
                .Where(h => h.MaKh == maKh)
                .OrderByDescending(h => h.NgayDat)
                .Select(h => new HoaDon
                {
                    MaHd = h.MaHd,
                    NgayDat = h.NgayDat,
                    HoTen = h.HoTen,
                    CachThanhToan = h.CachThanhToan,
                    GhiChu = h.GhiChu,
                    MaTrangThai = h.MaTrangThai,
                    ChiTietHds = h.ChiTietHds.Select(ct => new ChiTietHd
                    {
                        MaHh = ct.MaHh,
                        SoLuong = ct.SoLuong,
                        DonGia = ct.DonGia,
                        MaHhNavigation = ct.MaHhNavigation != null ? new HangHoa
                        {
                            TenHh = ct.MaHhNavigation.TenHh
                        } : null
                    }).ToList()
                })
                .ToList();

            return View(dsHoaDon);
        }

        [HttpGet]
        public IActionResult TraCuu(int? maHd)
        {
            // Nếu chưa nhập mã đơn thì chỉ hiện ô tìm kiếm
            if (maHd == null) return View();

            // Tìm kiếm đơn hàng theo Mã HD trong Database
            var hoadon = _ctx.HoaDons
                .Where(h => h.MaHd == maHd.Value)
                .Select(h => new HoaDon
                {
                    MaHd = h.MaHd,
                    HoTen = h.HoTen,
                    DiaChi = h.DiaChi,
                    CachThanhToan = h.CachThanhToan,
                    NgayDat = h.NgayDat,
                    GhiChu = h.GhiChu,
                    MaTrangThai = h.MaTrangThai,
                    ChiTietHds = h.ChiTietHds.Select(ct => new ChiTietHd
                    {
                        MaHd = ct.MaHd,
                        MaHh = ct.MaHh,
                        DonGia = ct.DonGia,
                        SoLuong = ct.SoLuong,
                        MaHhNavigation = ct.MaHhNavigation != null ? new HangHoa
                        {
                            TenHh = ct.MaHhNavigation.TenHh
                        } : null
                    }).ToList()
                })
                .FirstOrDefault();

            if (hoadon == null)
            {
                ViewBag.Message = $"Không tìm thấy thông tin cho mã đơn hàng #{maHd}";
            }

            return View(hoadon);
        }

        public IActionResult ChoThanhToan(int id)
        {
            var hoadon = _ctx.HoaDons
                .Where(h => h.MaHd == id)
                .Select(h => new HoaDon
                {
                    MaHd = h.MaHd,
                    HoTen = h.HoTen,
                    DiaChi = h.DiaChi,
                    CachThanhToan = h.CachThanhToan,
                    NgayDat = h.NgayDat,
                    GhiChu = h.GhiChu,
                    ChiTietHds = h.ChiTietHds.Select(ct => new ChiTietHd
                    {
                        MaHd = ct.MaHd,
                        MaHh = ct.MaHh,
                        DonGia = ct.DonGia,
                        SoLuong = ct.SoLuong,
                        MaHhNavigation = ct.MaHhNavigation != null ? new HangHoa
                        {
                            TenHh = ct.MaHhNavigation.TenHh
                        } : null
                    }).ToList()
                })
                .FirstOrDefault();

            if (hoadon == null) return NotFound();

            return View(hoadon);
        }
    }
    }
