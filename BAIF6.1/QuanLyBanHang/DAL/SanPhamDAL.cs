using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyBanHang.DTO;

namespace QuanLyBanHang.DAL
{

    public class SanPhamDAL
    {
        public DataTable LayTatCa()
        {
            using var conn = DBConnection.GetConnection();
            using var da = new SqlDataAdapter("SELECT MaSP, TenSP, GiaBan, SoLuongTon FROM SanPham ORDER BY TenSP", conn);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

     
        public DataTable TimKiem(string tuKhoa)
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand(
                "SELECT MaSP, TenSP, GiaBan, SoLuongTon FROM SanPham " +
                "WHERE TenSP LIKE @tuKhoa OR MaSP LIKE @tuKhoa ORDER BY TenSP", conn);
            cmd.Parameters.Add("@tuKhoa", SqlDbType.NVarChar, 160).Value = "%" + tuKhoa + "%";
            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public SanPhamDTO LayTheoMa(string maSP)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("SELECT MaSP, TenSP, GiaBan, SoLuongTon FROM SanPham WHERE MaSP = @ma", conn);
            cmd.Parameters.Add("@ma", SqlDbType.VarChar, 20).Value = maSP;
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return new SanPhamDTO
            {
                MaSP = rd.GetString(0),
                TenSP = rd.GetString(1),
                GiaBan = rd.GetDecimal(2),
                SoLuongTon = rd.GetInt32(3)
            };
        }

        public bool Them(SanPhamDTO sp)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO SanPham (MaSP, TenSP, GiaBan, SoLuongTon) VALUES (@ma, @ten, @gia, @ton)", conn);
            ThemThamSo(cmd, sp);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Sua(SanPhamDTO sp)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE SanPham SET TenSP = @ten, GiaBan = @gia, SoLuongTon = @ton WHERE MaSP = @ma", conn);
            ThemThamSo(cmd, sp);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Xoa(string maSP)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM SanPham WHERE MaSP = @ma", conn);
            cmd.Parameters.Add("@ma", SqlDbType.VarChar, 20).Value = maSP;
            return cmd.ExecuteNonQuery() > 0;
        }

        private static void ThemThamSo(SqlCommand cmd, SanPhamDTO sp)
        {
            cmd.Parameters.Add("@ma", SqlDbType.VarChar, 20).Value = sp.MaSP;
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 150).Value = sp.TenSP;
            var gia = cmd.Parameters.Add("@gia", SqlDbType.Decimal);
            gia.Precision = 18; gia.Scale = 2; gia.Value = sp.GiaBan;
            cmd.Parameters.Add("@ton", SqlDbType.Int).Value = sp.SoLuongTon;
        }
    }
}
