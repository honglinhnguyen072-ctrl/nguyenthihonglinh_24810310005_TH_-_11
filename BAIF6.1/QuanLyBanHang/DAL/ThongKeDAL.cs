using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyBanHang.DTO;

namespace QuanLyBanHang.DAL
{
    public class ThongKeDAL
    {
        public decimal DoanhThuNgay(DateTime ngay)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT ISNULL(SUM(TongTien), 0) FROM HoaDon WHERE CAST(NgayLap AS DATE) = @ngay", conn);
            cmd.Parameters.Add("@ngay", SqlDbType.Date).Value = ngay.Date;
            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        public decimal DoanhThuThang(int thang, int nam)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT ISNULL(SUM(TongTien), 0) FROM HoaDon WHERE MONTH(NgayLap) = @thang AND YEAR(NgayLap) = @nam", conn);
            cmd.Parameters.Add("@thang", SqlDbType.Int).Value = thang;
            cmd.Parameters.Add("@nam", SqlDbType.Int).Value = nam;
            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        public List<TopSanPhamDTO> Top5BanChay()
        {
            var ds = new List<TopSanPhamDTO>();
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "SELECT TOP 5 sp.MaSP, sp.TenSP, SUM(ct.SoLuong) AS TongSL, SUM(ct.SoLuong * ct.DonGia) AS DoanhThu " +
                "FROM ChiTietHoaDon ct JOIN SanPham sp ON ct.MaSP = sp.MaSP " +
                "GROUP BY sp.MaSP, sp.TenSP ORDER BY TongSL DESC", conn);
            using SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                ds.Add(new TopSanPhamDTO
                {
                    MaSP = rd.GetString(0),
                    TenSP = rd.GetString(1),
                    TongSoLuong = rd.GetInt32(2),
                    DoanhThu = rd.GetDecimal(3)
                });
            }
            return ds;
        }
    }
}
