using QuanLyBanHang.BUS;

namespace QuanLyBanHang.GUI
{
    
    public class FrmThongKe : Form
    {
        private readonly ThongKeBUS bus = new();
        private readonly DateTimePicker dtp;
        private readonly Label lblNgay, lblThang;
        private readonly DataGridView dgvTop, dgvHD;

        public FrmThongKe()
        {
            UI.ChuanBiForm(this, "Báo cáo – Thống kê", 900, 640);
            UI.TieuDe(this, "BÁO CÁO – THỐNG KÊ DOANH THU", 20, 12);

            UI.Nhan(this, "Chọn ngày:", 20, 65);
            dtp = new DateTimePicker { Left = 110, Top = 65, Width = 150, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
            Controls.Add(dtp);
            UI.Nut(this, "Xem thống kê", 280, 60, 150).Click += (s, e) => TaiThongKe();
            dtp.ValueChanged += (s, e) => TaiThongKe();

            lblNgay = The("Doanh thu trong ngày (ExecuteScalar)", 20);
            lblThang = The("Doanh thu trong tháng (ExecuteScalar)", 460);

            UI.Nhan(this, "TOP 5 SẢN PHẨM BÁN CHẠY NHẤT (SqlDataReader)", 20, 220, true);
            dgvTop = UI.Luoi(this, 20, 245, 860, 175);

            UI.Nhan(this, "DANH SÁCH HÓA ĐƠN", 20, 430, true);
            dgvHD = UI.Luoi(this, 20, 455, 860, 170);

            Load += (s, e) => TaiThongKe();
        }

        private Label The(string tieuDe, int x)
        {
            var p = new Panel { Left = x, Top = 110, Width = 420, Height = 95, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            p.Controls.Add(new Label { Text = tieuDe, Left = 12, Top = 10, AutoSize = true, ForeColor = Color.Gray });
            var v = new Label { Left = 12, Top = 40, AutoSize = true, Font = new Font("Segoe UI", 18f, FontStyle.Bold), ForeColor = UI.MauChinh };
            p.Controls.Add(v);
            Controls.Add(p);
            return v;
        }

        private void TaiThongKe()
        {
            try
            {
                DateTime d = dtp.Value.Date;
                lblNgay.Text = UI.Tien(bus.DoanhThuNgay(d)) + $"   ({d:dd/MM/yyyy})";
                lblThang.Text = UI.Tien(bus.DoanhThuThang(d)) + $"   ({d:MM/yyyy})";

                var top = bus.Top5BanChay()
                    .Select((x, i) => new { Hang = i + 1, x.MaSP, x.TenSP, x.TongSoLuong, x.DoanhThu })
                    .ToList();
                dgvTop.DataSource = top;
                UI.DatTieuDeCot(dgvTop, ("Hang", "Hạng"), ("MaSP", "Mã SP"), ("TenSP", "Tên sản phẩm"),
                    ("TongSoLuong", "Số lượng bán"), ("DoanhThu", "Doanh thu"));
                if (dgvTop.Columns.Contains("Hang")) dgvTop.Columns["Hang"].FillWeight = 30;

                dgvHD.DataSource = new HoaDonBUS().LayDanhSach();
                UI.DatTieuDeCot(dgvHD, ("MaHD", "Số HĐ"), ("NgayLap", "Ngày lập"), ("TenNV", "Nhân viên"),
                    ("KhachHang", "Khách hàng"), ("TongTien", "Tổng tiền"));
            }
            catch (Exception ex) { UI.BaoLoi(ex); }
        }
    }
}
