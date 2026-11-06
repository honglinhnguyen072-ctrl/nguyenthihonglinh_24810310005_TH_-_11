using System.Data;
using System.Text.RegularExpressions;
using QuanLyBanHang.DAL;
using QuanLyBanHang.DTO;

namespace QuanLyBanHang.BUS
{
    public class NhanVienBUS
    {
        private readonly NhanVienDAL dal = new();

        public DataTable LayTatCa(string tuKhoa = "") => dal.LayTatCa(tuKhoa?.Trim() ?? "");
        public bool Them(NhanVienDTO nv) { KiemTra(nv); return dal.Them(nv); }
        public bool Sua(NhanVienDTO nv) { KiemTra(nv); return dal.Sua(nv); }
        public bool Xoa(int ma) => dal.Xoa(ma);

        private static void KiemTra(NhanVienDTO nv)
        {
            if (string.IsNullOrWhiteSpace(nv.TenNV)) throw new ArgumentException("Tên nhân viên không được trống.");
            if (!string.IsNullOrWhiteSpace(nv.SoDienThoai) && !Regex.IsMatch(nv.SoDienThoai, @"^0\d{9,10}$"))
                throw new ArgumentException("Số điện thoại phải bắt đầu bằng 0 và có 10–11 chữ số.");
        }
    }
}
