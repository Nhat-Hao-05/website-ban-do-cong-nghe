namespace MyEStore.Models
{
    public class CartItem
    {
        public int MaHh {  get; set; }
        public required string TenHh { get; set; }
        public double DonGia {  get; set; }
        public required string Hinh {  get; set; }
        public int SoLuong { get; set; }
        public double ThanhTien => SoLuong * DonGia;

        public required string MoTa { get; set; }
    }
}
