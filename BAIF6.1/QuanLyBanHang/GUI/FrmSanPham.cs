using QuanLyBanHang.BUS;
using QuanLyBanHang.DTO;

namespace QuanLyBanHang.GUI
{
    
    public class FrmSanPham : Form
    {
        private readonly SanPhamBUS bus = new();
        private readonly TextBox txtMa, txtTen, txtTim;
        private readonly NumericUpDown numGia, numTon;
        private readonly DataGridView dgv;

        public FrmSanPham()
        {
            UI.ChuanBiForm(this, "Quản lý Sản phẩm", 900, 600);
            UI.TieuDe(this, "QUẢN LÝ SẢN PHẨM / HÀNG HÓA", 20, 12);

            UI.Nhan(this, "Mã SP / Mã vạch:", 20, 65);  txtMa = UI.O(this, 160, 65);
            UI.Nhan(this, "Tên sản phẩm:", 20, 105);   txtTen = UI.O(this, 160, 105, 300);
            UI.Nhan(this, "Giá bán (đ):", 500, 65);    numGia = UI.So(this, 620, 65, 160, 1_000_000_000);
            UI.Nhan(this, "Số lượng tồn:", 500, 105);  numTon = UI.So(this, 620, 105, 160, 1_000_000);

            int y = 150;
            UI.Nut(this, "Thêm", 20, y).Click += (s, e) => ThucHien(() => bus.Them(DocForm()), "Đã thêm sản phẩm.");
            UI.Nut(this, "Sửa", 130, y, 100, Color.FromArgb(52, 101, 164)).Click += (s, e) => ThucHien(() => bus.Sua(DocForm()), "Đã cập nhật sản phẩm.");
            UI.Nut(this, "Xóa", 240, y, 100, Color.FromArgb(192, 57, 43)).Click += (s, e) =>
            {
                if (UI.XacNhan($"Xóa sản phẩm {txtMa.Text}?"))
                    ThucHien(() => bus.Xoa(txtMa.Text), "Đã xóa sản phẩm.");
            };
            UI.Nut(this, "Làm mới", 350, y, 100, Color.Gray).Click += (s, e) => { XoaForm(); TaiDuLieu(); };

            UI.Nhan(this, "Tìm (tên / mã vạch):", 470, y + 5);
            txtTim = UI.O(this, 620, y + 4, 160);
            UI.Nut(this, "Tìm", 790, y, 90).Click += (s, e) => TimKiem();
            txtTim.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { TimKiem(); e.SuppressKeyPress = true; } };

            dgv = UI.Luoi(this, 20, 200, 860, 380);
            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                var r = dgv.Rows[e.RowIndex];
                txtMa.Text = r.Cells["MaSP"].Value.ToString();
                txtTen.Text = r.Cells["TenSP"].Value.ToString();
                numGia.Value = Convert.ToDecimal(r.Cells["GiaBan"].Value);
                numTon.Value = Convert.ToInt32(r.Cells["SoLuongTon"].Value);
                txtMa.ReadOnly = true; 
            };

            Load += (s, e) => TaiDuLieu();
        }

        private SanPhamDTO DocForm() => new()
        {
            MaSP = txtMa.Text.Trim(),
            TenSP = txtTen.Text.Trim(),
            GiaBan = numGia.Value,
            SoLuongTon = (int)numTon.Value
        };

        private void XoaForm()
        {
            txtMa.ReadOnly = false;
            txtMa.Clear(); txtTen.Clear(); txtTim.Clear();
            numGia.Value = 0; numTon.Value = 0;
            txtMa.Focus();
        }

        private void ThucHien(Func<bool> hanhDong, string thongBao)
        {
            try
            {
                if (hanhDong()) { UI.BaoThanhCong(thongBao); XoaForm(); TaiDuLieu(); }
                else MessageBox.Show("Không tìm thấy bản ghi để cập nhật.");
            }
            catch (Exception ex) { UI.BaoLoi(ex); }
        }

        private void TaiDuLieu() => HienThi(bus.LayTatCa());

        private void TimKiem()
        {
            try { HienThi(bus.TimKiem(txtTim.Text)); }
            catch (Exception ex) { UI.BaoLoi(ex); }
        }

        private void HienThi(System.Data.DataTable dt)
        {
            dgv.DataSource = dt;
            UI.DatTieuDeCot(dgv, ("MaSP", "Mã SP / Mã vạch"), ("TenSP", "Tên sản phẩm"),
                ("GiaBan", "Giá bán"), ("SoLuongTon", "Tồn kho"));
        }
    }
}
