namespace MyEStore.Models
{
    public class HangHoaVM
    {
        public int MaHh { get; set; }
        public string? TenHh { get; set; } = string.Empty;
        public double DonGia { get; set; }
        public string? Hinh { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
    }
}
