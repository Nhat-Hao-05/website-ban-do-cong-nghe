using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEStore.Entities;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace MyEStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyeStoreContext _ctx;

        public AccountController(MyeStoreContext ctx)
        {
            _ctx = ctx;
        }

        #region Đăng Nhập

        // 🔴 1. HÀM MỞ GIAO DIỆN ĐĂNG NHẬP (Cần hàm này để hết lỗi 405)
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        // 🟢 2. HÀM XỬ LÝ KHI BẤM NÚT ĐĂNG NHẬP
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string matKhau, string? ReturnUrl)
        {
            email = email?.Trim().ToLowerInvariant();
            matKhau = matKhau?.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ email và mật khẩu.";
                return View();
            }

            // --- 1. KIỂM TRA BẢNG NHÂN VIÊN / ADMIN ---
            var nv = await _ctx.NhanViens.AsNoTracking()
                        .FirstOrDefaultAsync(n => n.Email.ToLower() == email);

            if (nv != null)
            {
                // Mật khẩu bảng NhanVien đang lưu dạng chuỗi thường ("123")
                if (string.Equals(nv.MatKhau?.Trim(), matKhau, StringComparison.Ordinal))
                {
                    var nvClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, nv.HoTen ?? "Admin"),
                new Claim(ClaimTypes.Email, nv.Email),
                new Claim("MaNV", nv.MaNv),
                new Claim(ClaimTypes.Role, "Admin") // Phân quyền Admin để vào AdminController
            };

                    var nvIdentity = new ClaimsIdentity(nvClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var nvPrincipal = new ClaimsPrincipal(nvIdentity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, nvPrincipal);

                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }

                    // Đăng nhập Admin thành công ➔ Chuyển sang trang Quản lý sản phẩm
                    return RedirectToAction("ManagerProducts", "Admin");
                }
            }

            // --- 2. KIỂM TRA BẢNG KHÁCH HÀNG ---
            var kh = await _ctx.KhachHangs.AsNoTracking()
                        .SingleOrDefaultAsync(k => k.Email.ToLower() == email);

            if (kh != null)
            {
                if (kh.HieuLuc == false)
                {
                    ViewBag.Error = "Tài khoản của bạn đã bị khóa.";
                    return View();
                }

                string salt = kh.RandomKey?.Trim() ?? "";
                string hashedResult;

                using (var sha512 = SHA512.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(matKhau + salt);
                    byte[] hashBytes = sha512.ComputeHash(inputBytes);
                    hashedResult = BitConverter.ToString(hashBytes).Replace("-", "");
                }

                if (hashedResult.Length > 50)
                {
                    hashedResult = hashedResult.Substring(0, 50);
                }

                if (string.Equals(hashedResult, kh.MatKhau.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, kh.HoTen),
                new Claim(ClaimTypes.Email, kh.Email),
                new Claim("MaKH", kh.MaKh),
                new Claim(ClaimTypes.Role, kh.VaiTro == 2 ? "Admin" : "Customer")
            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }

                    if (kh.VaiTro == 2)
                    {
                        return RedirectToAction("ManagerProducts", "Admin");
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            // Nếu cả Nhân viên và Khách hàng đều không đúng
            ViewBag.Error = "Email hoặc mật khẩu không chính xác.";
            return View();
        }

        #endregion


        #region Đăng Ký
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string hoTen,
            string gioiTinh,
            DateTime? ngaySinh,
            string dienThoai,
            string email,
            string matKhau,
            string diaChi,
            IFormFile? anhDaiDien)
        {
            email = email?.Trim().ToLowerInvariant();
            matKhau = matKhau?.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ Email và Mật khẩu.";
                return View();
            }

            // 1. Kiểm tra Email đã tồn tại trong Database chưa
            var existingUser = await _ctx.KhachHangs.AsNoTracking()
                                .AnyAsync(k => k.Email.ToLower() == email);
            if (existingUser)
            {
                ViewBag.Error = "Email này đã được sử dụng. Vui lòng chọn Email khác!";
                return View();
            }

            // 2. Tạo RandomKey (Salt) 8 ký tự
            string randomKey = Guid.NewGuid().ToString("N").Substring(0, 8);

            // 3. Mã hóa Mật khẩu bằng SHA512 + RandomKey (Trùng khớp 100% logic hàm Login)
            string hashedResult;
            // Thay SHA512 bằng MD5 để ra chuỗi 32 ký tự
            using (var sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(matKhau + randomKey);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);
                hashedResult = BitConverter.ToString(hashBytes).Replace("-", "");
            }

            // 👉 Cắt lấy 50 ký tự đầu để vừa vặn với cột MatKhau trong DB
            if (hashedResult.Length > 50)
            {
                hashedResult = hashedResult.Substring(0, 50);
            }

            // 4. Xử lý Upload Ảnh đại diện (nếu người dùng có chọn tệp)
            string fileName = "default-avatar.png";
            if (anhDaiDien != null && anhDaiDien.Length > 0)
            {
                fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(anhDaiDien.FileName);
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", "KhachHang");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string filePath = Path.Combine(uploadFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await anhDaiDien.CopyToAsync(stream);
                }
            }

            // 5. Khởi tạo đối tượng KhachHang mới
            var kh = new KhachHang
            {
                MaKh = "KH" + DateTime.Now.ToString("yyyyMMddHHmmss"), // Tự sinh Mã KH không lo bị trùng
                HoTen = hoTen,
                GioiTinh = gioiTinh == "Nam",                        // Chuyển "Nam"/"Nữ" sang kiểu bool (1/0)
                NgaySinh = ngaySinh ?? DateTime.Now,
                DienThoai = dienThoai,
                Email = email,
                MatKhau = hashedResult,                              // Lưu chuỗi đã SHA512
                RandomKey = randomKey,                              // Lưu Salt để lúc Login dùng lại
                DiaChi = diaChi,
                Hinh = fileName,
                HieuLuc = true,                                     // Kích hoạt tài khoản ngay (1)
                VaiTro = 0                                          // 0 = Khách hàng thường
            };

            // 6. Lưu vào Database
            _ctx.KhachHangs.Add(kh);
            await _ctx.SaveChangesAsync();

            // 7. Chuyển hướng sang trang Đăng nhập sau khi tạo thành công
            return RedirectToAction("Login", "Account");
        }
        #endregion

        #region Đăng Xuất & Từ Chối
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion

        #region Trang Cá Nhân
        [Authorize] // Bắt buộc phải đăng nhập mới vào được trang này
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Lấy Email hoặc Mã KH từ Cookie đã lưu lúc đăng nhập
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            // Tìm thông tin khách hàng trong Database
            var khachHang = await _ctx.KhachHangs.FirstOrDefaultAsync(k => k.Email.ToLower() == email.ToLower());

            if (khachHang == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(khachHang);
        }
        #endregion

        // GET: /Account/Orders
        [Authorize]
        public IActionResult Orders()
        {
            // Mẹo: Sau này bạn lấy danh sách đơn hàng từ DB theo User tại đây
            return View();
        }
    }
}