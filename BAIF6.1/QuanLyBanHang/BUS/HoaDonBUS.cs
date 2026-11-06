using System.Data;
using QuanLyBanHang.DAL;
using QuanLyBanHang.DTO;

namespace QuanLyBanHang.BUS
{
    public class HoaDonBUS
    {
        private readonly HoaDonDAL dal = new();

        public DataTable LayDanhSach() => dal.LayDanhSach();

        public static decimal TinhTongTien(IEnumerable<ChiTietHoaDonDTO> gio) => gio.Sum(x => x.ThanhTien);

        public int ThanhToan(int maNV, int? maKH, List<ChiTietHoaDonDTO> gioHang)
        {
            if (gioHang == null || gioHang.Count == 0) throw new ArgumentException("Giỏ hàng đang trống.");
            if (maNV <= 0) throw new ArgumentException("Chưa chọn nhân viên lập hóa đơn.");

            var hd = new HoaDonDTO
            {
                MaNV = maNV,
                MaKH = maKH,
                ChiTiet = gioHang,
                TongTien = TinhTongTien(gioHang)
            };
            int diem = maKH.HasValue ? KhachHangBUS.TinhDiem(hd.TongTien) : 0;
            return dal.LapHoaDon(hd, diem);
        }
    }
}
