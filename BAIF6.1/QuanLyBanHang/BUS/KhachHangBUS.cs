using System.Data;
using System.Text.RegularExpressions;
using QuanLyBanHang.DAL;
using QuanLyBanHang.DTO;

namespace QuanLyBanHang.BUS
{
    public class KhachHangBUS
    {
        private readonly KhachHangDAL dal = new();

        public DataTable LayTatCa(string tuKhoa = "") => dal.LayTatCa(tuKhoa?.Trim() ?? "");
        public bool Them(KhachHangDTO kh) { KiemTra(kh); return dal.Them(kh); }
        public bool Sua(KhachHangDTO kh) { KiemTra(kh); return dal.Sua(kh); }
        public bool Xoa(int ma) => dal.Xoa(ma);

        public static int TinhDiem(decimal tongTien) => (int)(tongTien / 100000);

        private static void KiemTra(KhachHangDTO kh)
        {
            if (string.IsNullOrWhiteSpace(kh.TenKH)) throw new ArgumentException("Tên khách hàng không được trống.");
            if (!string.IsNullOrWhiteSpace(kh.DienThoai) && !Regex.IsMatch(kh.DienThoai, @"^0\d{9,10}$"))
                throw new ArgumentException("Số điện thoại phải bắt đầu bằng 0 và có 10–11 chữ số.");
            if (kh.DiemTichLuy < 0) throw new ArgumentException("Điểm tích lũy không được âm.");
        }
    }
}
