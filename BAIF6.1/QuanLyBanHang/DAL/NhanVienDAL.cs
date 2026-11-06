using System.Data;
using Microsoft.Data.SqlClient;
using QuanLyBanHang.DTO;

namespace QuanLyBanHang.DAL
{
    public class NhanVienDAL
    {
        public DataTable LayTatCa(string tuKhoa = "")
        {
            using var conn = DBConnection.GetConnection();
            using var cmd = new SqlCommand(
                "SELECT MaNV, TenNV, SoDienThoai, ChucVu FROM NhanVien " +
                "WHERE TenNV LIKE @tk OR SoDienThoai LIKE @tk ORDER BY MaNV", conn);
            cmd.Parameters.Add("@tk", SqlDbType.NVarChar, 110).Value = "%" + tuKhoa + "%";
            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public bool Them(NhanVienDTO nv)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "INSERT INTO NhanVien (TenNV, SoDienThoai, ChucVu) VALUES (@ten, @sdt, @cv)", conn);
            ThemThamSo(cmd, nv);
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Sua(NhanVienDTO nv)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(
                "UPDATE NhanVien SET TenNV = @ten, SoDienThoai = @sdt, ChucVu = @cv WHERE MaNV = @ma", conn);
            ThemThamSo(cmd, nv);
            cmd.Parameters.Add("@ma", SqlDbType.Int).Value = nv.MaNV;
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool Xoa(int maNV)
        {
            using var conn = DBConnection.GetConnection();
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM NhanVien WHERE MaNV = @ma", conn);
            cmd.Parameters.Add("@ma", SqlDbType.Int).Value = maNV;
            return cmd.ExecuteNonQuery() > 0;
        }

        private static void ThemThamSo(SqlCommand cmd, NhanVienDTO nv)
        {
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 100).Value = nv.TenNV;
            cmd.Parameters.Add("@sdt", SqlDbType.VarChar, 15).Value =
                string.IsNullOrWhiteSpace(nv.SoDienThoai) ? DBNull.Value : nv.SoDienThoai;
            cmd.Parameters.Add("@cv", SqlDbType.NVarChar, 50).Value = (object)nv.ChucVu ?? DBNull.Value;
        }
    }
}
