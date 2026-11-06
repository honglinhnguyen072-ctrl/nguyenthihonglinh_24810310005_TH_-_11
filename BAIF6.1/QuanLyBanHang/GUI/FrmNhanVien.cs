using QuanLyBanHang.BUS;
using QuanLyBanHang.DTO;

namespace QuanLyBanHang.GUI
{
    public class FrmNhanVien : Form
    {
        private readonly NhanVienBUS bus = new();
        private readonly TextBox txtMa, txtTen, txtSdt, txtTim;
        private readonly ComboBox cboChucVu;
        private readonly DataGridView dgv;

        public FrmNhanVien()
        {
            UI.ChuanBiForm(this, "Quản lý Nhân viên", 900, 600);
            UI.TieuDe(this, "QUẢN LÝ NHÂN VIÊN", 20, 12);

            UI.Nhan(this, "Mã NV:", 20, 65);           txtMa = UI.O(this, 160, 65, 100); txtMa.ReadOnly = true;
            UI.Nhan(this, "Tên nhân viên:", 20, 105);  txtTen = UI.O(this, 160, 105, 300);
            UI.Nhan(this, "Số điện thoại:", 500, 65);  txtSdt = UI.O(this, 620, 65, 160);
            UI.Nhan(this, "Chức vụ:", 500, 105);
            cboChucVu = new ComboBox { Left = 620, Top = 105, Width = 160 };
            cboChucVu.Items.AddRange(new object[] { "Thu ngân", "Quản lý", "Kho", "Bảo vệ" });
            Controls.Add(cboChucVu);

            int y = 150;
            UI.Nut(this, "Thêm", 20, y).Click += (s, e) => ThucHien(() => bus.Them(DocForm()), "Đã thêm nhân viên.");
            UI.Nut(this, "Sửa", 130, y, 100, Color.FromArgb(52, 101, 164)).Click += (s, e) =>
            {
                if (txtMa.Text == "") { MessageBox.Show("Chọn nhân viên trên lưới trước."); return; }
                ThucHien(() => bus.Sua(DocForm()), "Đã cập nhật nhân viên.");
            };
            UI.Nut(this, "Xóa", 240, y, 100, Color.FromArgb(192, 57, 43)).Click += (s, e) =>
            {
                if (txtMa.Text == "") { MessageBox.Show("Chọn nhân viên trên lưới trước."); return; }
                if (UI.XacNhan($"Xóa nhân viên {txtTen.Text}?"))
                    ThucHien(() => bus.Xoa(int.Parse(txtMa.Text)), "Đã xóa nhân viên.");
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
                txtMa.Text = r.Cells["MaNV"].Value.ToString();
                txtTen.Text = r.Cells["TenNV"].Value.ToString();
                txtSdt.Text = r.Cells["SoDienThoai"].Value?.ToString();
                cboChucVu.Text = r.Cells["ChucVu"].Value?.ToString();
            };
            Load += (s, e) => TaiDuLieu();
        }

        private NhanVienDTO DocForm() => new()
        {
            MaNV = int.TryParse(txtMa.Text, out int ma) ? ma : 0,
            TenNV = txtTen.Text.Trim(),
            SoDienThoai = txtSdt.Text.Trim(),
            ChucVu = cboChucVu.Text.Trim()
        };

        private void XoaForm() { txtMa.Clear(); txtTen.Clear(); txtSdt.Clear(); txtTim.Clear(); cboChucVu.Text = ""; }

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
                UI.DatTieuDeCot(dgv, ("MaNV", "Mã NV"), ("TenNV", "Tên nhân viên"),
                    ("SoDienThoai", "Số điện thoại"), ("ChucVu", "Chức vụ"));
            }
            catch (Exception ex) { UI.BaoLoi(ex); }
        }
    }
}
