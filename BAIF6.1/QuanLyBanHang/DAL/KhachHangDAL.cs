using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyBanHang.DTO;

namespace QuanLyBanHang.DAL
{
    public class KhachHangDAL
    {
        public DataTable LayTatCa(string tuKhoa = "")
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand(
                "SELECT MaKH, TenKH, DienThoai, DiemTichLuy FROM KhachHang " +
                "WHERE TenKH LIKE @tk OR DienThoai LIKE @tk ORDER BY MaKH", conn);
            cmd.Parameters.Add("@tk", SqlDbType.NVarChar, 110).Value = "%" + tuKhoa + "%";
            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public bool Them(KhachHangDTO kh)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO KhachHang (TenKH, DienThoai, DiemTichLuy) VALUES (@ten, @dt, @diem)", conn);
            ThemThamSo(cmd, kh);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Sua(KhachHangDTO kh)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE KhachHang SET TenKH = @ten, DienThoai = @dt, DiemTichLuy = @diem WHERE MaKH = @ma", conn);
            ThemThamSo(cmd, kh);
            cmd.Parameters.Add("@ma", SqlDbType.Int).Value = kh.MaKH;
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Xoa(int maKH)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM KhachHang WHERE MaKH = @ma", conn);
            cmd.Parameters.Add("@ma", SqlDbType.Int).Value = maKH;
            return cmd.ExecuteNonQuery() > 0;
        }

        private static void ThemThamSo(SqlCommand cmd, KhachHangDTO kh)
        {
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 100).Value = kh.TenKH;
            cmd.Parameters.Add("@dt", SqlDbType.VarChar, 15).Value =
                string.IsNullOrWhiteSpace(kh.DienThoai) ? DBNull.Value : kh.DienThoai;
            cmd.Parameters.Add("@diem", SqlDbType.Int).Value = kh.DiemTichLuy;
        }
    }
}
