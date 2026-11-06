using QuanLyBanHang.DAL;
using QuanLyBanHang.DTO;

namespace QuanLyBanHang.BUS
{
    public class ThongKeBUS
    {
        private readonly ThongKeDAL dal = new();
        public decimal DoanhThuNgay(DateTime ngay) => dal.DoanhThuNgay(ngay);
        public decimal DoanhThuThang(DateTime ngay) => dal.DoanhThuThang(ngay.Month, ngay.Year);
        public List<TopSanPhamDTO> Top5BanChay() => dal.Top5BanChay();
    }
}
