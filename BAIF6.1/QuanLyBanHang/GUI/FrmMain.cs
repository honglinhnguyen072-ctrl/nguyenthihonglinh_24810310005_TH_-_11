using QuanLyBanHang.BUS;

namespace QuanLyBanHang.GUI
{
   
    public class FrmMain : Form
    {
        private readonly Label lblDoanhThuNgay, lblDoanhThuThang, lblSoSP, lblSoHD;

        public FrmMain()
        {
            UI.ChuanBiForm(this, "Siêu thị mini – Hệ thống quản lý bán hàng", 760, 470);

            var banner = new Panel { Left = 0, Top = 0, Width = 760, Height = 80, BackColor = UI.MauChinh };
            Controls.Add(banner);
            banner.Controls.Add(new Label
            {
                Text = "🛒  SIÊU THỊ MINI – RETAIL SALES SYSTEM", AutoSize = true, Left = 24, Top = 14,
                Font = new Font("Segoe UI", 17f, FontStyle.Bold), ForeColor = Color.White
            });
            banner.Controls.Add(new Label
            {
                Text = "Kiến trúc 3-Tier (DTO – DAL – BUS – GUI) · ADO.NET · SqlTransaction", AutoSize = true,
                Left = 28, Top = 50, ForeColor = Color.FromArgb(220, 255, 250)
            });

         
            lblDoanhThuNgay = The("Doanh thu hôm nay", 24);
            lblDoanhThuThang = The("Doanh thu tháng này", 206);
            lblSoSP = The("Số mặt hàng", 388);
            lblSoHD = The("Tổng số hóa đơn", 570);

            int y = 230;
            NutChucNang("📦  Sản phẩm", 24, y, () => new FrmSanPham());
            NutChucNang("👥  Khách hàng", 270, y, () => new FrmKhachHang());
            NutChucNang("🧑‍💼  Nhân viên", 516, y, () => new FrmNhanVien());
            NutChucNang("🧾  Bán hàng / Lập hóa đơn", 24, y + 80, () => new FrmBanHang(), 466, Color.FromArgb(230, 126, 34));
            NutChucNang("📊  Báo cáo – Thống kê", 516, y + 80, () => new FrmThongKe(), 220, Color.FromArgb(52, 101, 164));

            Controls.Add(new Label
            {
                Text = "Sinh viên: Trần Nhật Đông – MSV 24810320177 – Lớp D19QTANM1",
                AutoSize = true, Left = 24, Top = 430, ForeColor = Color.Gray
            });

            Load += (s, e) => CapNhatTongQuan();
        }

        private Label The(string tieuDe, int x)
        {
            var p = new Panel { Left = x, Top = 100, Width = 166, Height = 100, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            p.Controls.Add(new Label { Text = tieuDe, Left = 10, Top = 12, AutoSize = true, ForeColor = Color.Gray });
            var v = new Label { Left = 10, Top = 45, AutoSize = true, Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = UI.MauChinh };
            p.Controls.Add(v);
            Controls.Add(p);
            return v;
        }

        private void NutChucNang(string text, int x, int y, Func<Form> taoForm, int w = 220, Color? mau = null)
        {
            var b = UI.Nut(this, text, x, y, w, mau);
            b.Height = 60;
            b.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            b.Click += (s, e) =>
            {
                using var f = taoForm();
                f.ShowDialog(this);
                CapNhatTongQuan();
            };
        }

        private void CapNhatTongQuan()
        {
            try
            {
                var tk = new ThongKeBUS();
                lblDoanhThuNgay.Text = UI.Tien(tk.DoanhThuNgay(DateTime.Today));
                lblDoanhThuThang.Text = UI.Tien(tk.DoanhThuThang(DateTime.Today));
                lblSoSP.Text = new SanPhamBUS().LayTatCa().Rows.Count.ToString();
                lblSoHD.Text = new HoaDonBUS().LayDanhSach().Rows.Count.ToString();
            }
            catch (Exception ex) { UI.BaoLoi(ex); }
        }
    }
}
