using MyEStore.Entities;

namespace MyEStore.Models.ViewModels
{
    public class ProductsVM
    {
        public HangHoa HangHoa { get; set; }
        public List<Loai> DanhSachLoai { get; set; }
    }
}
