using QuanLyBanHang.BUS;
using QuanLyBanHang.DTO;

namespace QuanLyBanHang.GUI
{
   
    public class FrmKhachHang : Form
    {
        private readonly KhachHangBUS bus = new();
        private readonly TextBox txtMa, txtTen, txtSdt, txtTim;
        private readonly NumericUpDown numDiem;
        private readonly DataGridView dgv;

        public FrmKhachHang()
        {
            UI.ChuanBiForm(this, "Quản lý Khách hàng", 900, 600);
            UI.TieuDe(this, "QUẢN LÝ KHÁCH HÀNG", 20, 12);

            UI.Nhan(this, "Mã KH:", 20, 65);          txtMa = UI.O(this, 160, 65, 100); txtMa.ReadOnly = true;
            UI.Nhan(this, "Tên khách hàng:", 20, 105); txtTen = UI.O(this, 160, 105, 300);
            UI.Nhan(this, "Điện thoại:", 500, 65);     txtSdt = UI.O(this, 620, 65, 160);
            UI.Nhan(this, "Điểm tích lũy:", 500, 105); numDiem = UI.So(this, 620, 105, 160, 1_000_000);

            int y = 150;
            UI.Nut(this, "Thêm", 20, y).Click += (s, e) => ThucHien(() => bus.Them(DocForm()), "Đã thêm khách hàng.");
            UI.Nut(this, "Sửa", 130, y, 100, Color.FromArgb(52, 101, 164)).Click += (s, e) =>
            {
                if (txtMa.Text == "") { MessageBox.Show("Chọn khách hàng trên lưới trước."); return; }
                ThucHien(() => bus.Sua(DocForm()), "Đã cập nhật khách hàng.");
            };
            UI.Nut(this, "Xóa", 240, y, 100, Color.FromArgb(192, 57, 43)).Click += (s, e) =>
            {
                if (txtMa.Text == "") { MessageBox.Show("Chọn khách hàng trên lưới trước."); return; }
                if (UI.XacNhan($"Xóa khách hàng {txtTen.Text}?"))
                    ThucHien(() => bus.Xoa(int.Parse(txtMa.Text)), "Đã xóa khách hàng.");
            };
            UI.Nut(this, "Làm mới", 350, y, 100, Color.Gray).Click += (s, e) => { XoaForm(); TaiDuLieu(); };

            UI.Nhan(this, "Tìm (tên / SĐT):", 470, y + 5);
            txtTim = UI.O(this, 620, y + 4, 160);
            UI.Nut(this, "Tìm", 790, y, 90).Click += (s, e) => TaiDuLieu();

            dgv = UI.Luoi(this, 20, 200, 860, 380);
            dgv.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                var r = dgv.Rows[e.RowIndex];
                txtMa.Text = r.Cells["MaKH"].Value.ToString();
                txtTen.Text = r.Cells["TenKH"].Value.ToString();
                txtSdt.Text = r.Cells["DienThoai"].Value?.ToString();
                numDiem.Value = Convert.ToInt32(r.Cells["DiemTichLuy"].Value);
            };
            Load += (s, e) => TaiDuLieu();
        }

        private KhachHangDTO DocForm() => new()
        {
            MaKH = int.TryParse(txtMa.Text, out int ma) ? ma : 0,
            TenKH = txtTen.Text.Trim(),
            DienThoai = txtSdt.Text.Trim(),
            DiemTichLuy = (int)numDiem.Value
        };

        private void XoaForm() { txtMa.Clear(); txtTen.Clear(); txtSdt.Clear(); txtTim.Clear(); numDiem.Value = 0; }

        private void ThucHien(Func<bool> hanhDong, string thongBao)
        {
            try
            {
                if (hanhDong()) { UI.BaoThanhCong(thongBao); XoaForm(); TaiDuLieu(); }
            }
            catch (Exception ex) { UI.BaoLoi(ex); }
        }

        private void TaiDuLieu()
        {
            try
            {
                dgv.DataSource = bus.LayTatCa(txtTim.Text);
                UI.DatTieuDeCot(dgv, ("MaKH", "Mã KH"), ("TenKH", "Tên khách hàng"),
                    ("DienThoai", "Điện thoại"), ("DiemTichLuy", "Điểm tích lũy"));
            }
            catch (Exception ex) { UI.BaoLoi(ex); }
        }
    }
}
