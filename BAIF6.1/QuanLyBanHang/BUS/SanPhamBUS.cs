using System.Data;
using QuanLyBanHang.DAL;
using QuanLyBanHang.DTO;

namespace QuanLyBanHang.BUS
{
    public class SanPhamBUS
    {
        private readonly SanPhamDAL dal = new();

        public DataTable LayTatCa() => dal.LayTatCa();
        public DataTable TimKiem(string tuKhoa) => dal.TimKiem(tuKhoa?.Trim() ?? "");
        public SanPhamDTO LayTheoMa(string ma) => dal.LayTheoMa(ma?.Trim());

        public bool Them(SanPhamDTO sp) { KiemTra(sp); return dal.Them(sp); }
        public bool Sua(SanPhamDTO sp) { KiemTra(sp); return dal.Sua(sp); }

        public bool Xoa(string ma)
        {
            if (string.IsNullOrWhiteSpace(ma)) throw new ArgumentException("Chưa chọn sản phẩm cần xóa.");
            return dal.Xoa(ma);
        }

        private static void KiemTra(SanPhamDTO sp)
        {
            if (string.IsNullOrWhiteSpace(sp.MaSP)) throw new ArgumentException("Mã sản phẩm / mã vạch không được trống.");
            if (sp.MaSP.Length > 20) throw new ArgumentException("Mã sản phẩm tối đa 20 ký tự.");
            if (string.IsNullOrWhiteSpace(sp.TenSP)) throw new ArgumentException("Tên sản phẩm không được trống.");
            if (sp.GiaBan <= 0) throw new ArgumentException("Giá bán phải lớn hơn 0.");
            if (sp.SoLuongTon < 0) throw new ArgumentException("Số lượng tồn không được âm.");
        }
    }
}
