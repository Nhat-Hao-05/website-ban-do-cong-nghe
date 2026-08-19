using System;
using System.Collections.Generic;

namespace MyEStore.Entities;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public string? MaKh { get; set; }

    // Thêm dấu ? vào các kiểu dữ liệu dưới đây để chấp nhận giá trị NULL từ SQL Server
    public DateTime? NgayDat { get; set; }

    public DateTime? NgayCan { get; set; }

    public DateTime? NgayGiao { get; set; }

    public string? HoTen { get; set; }

    public string? DiaChi { get; set; }

    public string? CachThanhToan { get; set; }

    public string? CachVanChuyen { get; set; }

    public double? PhiVanChuyen { get; set; }  // Thêm ?

    public int? MaTrangThai { get; set; }      // Thêm ?

    public string? MaNv { get; set; }

    public string? GhiChu { get; set; }

    public string? PaypalOrderID { get; set; }

    public virtual ICollection<ChiTietHd> ChiTietHds { get; set; } = new List<ChiTietHd>();

    public virtual KhachHang? MaKhNavigation { get; set; }

    public virtual NhanVien? MaNvNavigation { get; set; }

    public virtual TrangThai? MaTrangThaiNavigation { get; set; }
}