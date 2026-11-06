using System.ComponentModel;
using System.Data;
using QuanLyBanHang.BUS;
using QuanLyBanHang.DAL;
using QuanLyBanHang.DTO;

namespace QuanLyBanHang.GUI
{
    
    public class FrmBanHang : Form
    {
        private readonly SanPhamBUS spBus = new();
        private readonly HoaDonBUS hdBus = new();
        private readonly BindingList<ChiTietHoaDonDTO> gioHang = new();

        private readonly ComboBox cboNV, cboKH, cboSP;
        private readonly TextBox txtMaVach;
        private readonly NumericUpDown numSL;
        private readonly Label lblGia, lblTon, lblTong, lblDiem;
        private readonly DataGridView dgvGio, dgvHoaDon;

        public FrmBanHang()
        {
            UI.ChuanBiForm(this, "Bán hàng – Lập hóa đơn", 1100, 680);
            UI.TieuDe(this, "BÁN HÀNG / LẬP HÓA ĐƠN", 20, 12);

         
            UI.Nhan(this, "Nhân viên:", 20, 60);   cboNV = UI.Chon(this, 120, 60, 230);
            UI.Nhan(this, "Khách hàng:", 380, 60); cboKH = UI.Chon(this, 480, 60, 260);
            cboKH.SelectedIndexChanged += (s, e) => CapNhatTong();

        
            var grp = new GroupBox { Text = "Thêm sản phẩm vào giỏ", Left = 20, Top = 100, Width = 720, Height = 120, Font = UI.FontDam };
            Controls.Add(grp);
            UI.Nhan(grp, "Quét mã vạch:", 12, 30); txtMaVach = UI.O(grp, 125, 30, 180);
            txtMaVach.Font = UI.FontChu;
            txtMaVach.KeyDown += (s, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                e.SuppressKeyPress = true;
                QuetMaVach();
            };
            UI.Nhan(grp, "hoặc chọn:", 320, 30);
            cboSP = UI.Chon(grp, 410, 30, 295); cboSP.Font = UI.FontChu;
            cboSP.SelectedIndexChanged += (s, e) => HienThongTinSP();

            UI.Nhan(grp, "Số lượng:", 12, 72); numSL = UI.So(grp, 125, 72, 80, 100000); numSL.Minimum = 1; numSL.Value = 1;
            numSL.Font = UI.FontChu;
            lblGia = UI.Nhan(grp, "Giá: –", 225, 72);
            lblTon = UI.Nhan(grp, "Tồn: –", 390, 72);
            UI.Nut(grp, "+ Thêm vào giỏ", 530, 68, 175).Click += (s, e) => ThemVaoGio();

        
            UI.Nhan(this, "GIỎ HÀNG", 20, 230, true);
            dgvGio = UI.Luoi(this, 20, 255, 720, 270);
            dgvGio.AutoGenerateColumns = false;
            ThemCot(dgvGio, "MaSP", "Mã SP", 25);
            ThemCot(dgvGio, "TenSP", "Tên sản phẩm", 40);
            ThemCot(dgvGio, "SoLuong", "SL", 10);
            ThemCot(dgvGio, "DonGia", "Đơn giá", 15, "N0");
            ThemCot(dgvGio, "ThanhTien", "Thành tiền", 18, "N0");
            dgvGio.DataSource = gioHang;

            UI.Nut(this, "Xóa dòng", 20, 535, 110, Color.FromArgb(192, 57, 43)).Click += (s, e) =>
            {
                if (dgvGio.CurrentRow?.DataBoundItem is ChiTietHoaDonDTO ct) { gioHang.Remove(ct); CapNhatTong(); }
            };
            UI.Nut(this, "Xóa giỏ", 140, 535, 110, Color.Gray).Click += (s, e) => { gioHang.Clear(); CapNhatTong(); };

            lblTong = new Label { Left = 330, Top = 532, AutoSize = true, Font = new Font("Segoe UI", 16f, FontStyle.Bold), ForeColor = Color.FromArgb(192, 57, 43) };
            lblDiem = new Label { Left = 330, Top = 568, AutoSize = true, ForeColor = UI.MauChinh, Font = UI.FontDam };
            Controls.AddRange(new Control[] { lblTong, lblDiem });

            var btnTT = UI.Nut(this, "💳  THANH TOÁN", 20, 600, 720, Color.FromArgb(230, 126, 34));
            btnTT.Height = 55; btnTT.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
            btnTT.Click += (s, e) => ThanhToan();

         
            UI.Nhan(this, "HÓA ĐƠN ĐÃ LẬP", 760, 60, true);
            dgvHoaDon = UI.Luoi(this, 760, 85, 320, 570);

            Load += (s, e) => { TaiDanhMuc(); CapNhatTong(); };
        }

        private static void ThemCot(DataGridView g, string prop, string tieuDe, int fill, string format = null)
        {
            var c = new DataGridViewTextBoxColumn { DataPropertyName = prop, Name = prop, HeaderText = tieuDe, FillWeight = fill };
            if (format != null) { c.DefaultCellStyle.Format = format; c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; }
            g.Columns.Add(c);
        }

        private void TaiDanhMuc()
        {
            try
            {
                cboNV.DataSource = new NhanVienBUS().LayTatCa();
                cboNV.DisplayMember = "TenNV"; cboNV.ValueMember = "MaNV";

                DataTable kh = new KhachHangBUS().LayTatCa();
                DataRow le = kh.NewRow();
                le["MaKH"] = 0; le["TenKH"] = "— Khách lẻ —"; le["DiemTichLuy"] = 0;
                kh.Rows.InsertAt(le, 0);
                kh.Columns.Add("HienThi", typeof(string), "TenKH + IIF(DienThoai IS NULL, '', ' (' + DienThoai + ')')");
                cboKH.DataSource = kh;
                cboKH.DisplayMember = "HienThi"; cboKH.ValueMember = "MaKH";

                TaiSanPham();
                TaiHoaDon();
            }
            catch (Exception ex) { UI.BaoLoi(ex); }
        }

        private void TaiSanPham()
        {
            DataTable sp = spBus.LayTatCa();
            sp.Columns.Add("HienThi", typeof(string), "TenSP");
            cboSP.DataSource = sp;
            cboSP.DisplayMember = "HienThi"; cboSP.ValueMember = "MaSP";
        }

        private void TaiHoaDon()
        {
            dgvHoaDon.DataSource = hdBus.LayDanhSach();
            UI.DatTieuDeCot(dgvHoaDon, ("MaHD", "Số HĐ"), ("NgayLap", "Ngày lập"), ("TenNV", "NV"),
                ("KhachHang", "Khách"), ("TongTien", "Tổng tiền"));
            if (dgvHoaDon.Columns.Contains("TenNV")) dgvHoaDon.Columns["TenNV"].Visible = false;
        }

        private void HienThongTinSP()
        {
            if (cboSP.SelectedItem is DataRowView r)
            {
                lblGia.Text = "Giá: " + UI.Tien((decimal)r["GiaBan"]);
                lblTon.Text = "Tồn: " + r["SoLuongTon"];
            }
        }

        private void QuetMaVach()
        {
            try
            {
                var sp = spBus.LayTheoMa(txtMaVach.Text);
                if (sp == null) { MessageBox.Show("Không tìm thấy sản phẩm có mã vạch " + txtMaVach.Text); return; }
                cboSP.SelectedValue = sp.MaSP;
                ThemVaoGio();
                txtMaVach.Clear();
            }
            catch (Exception ex) { UI.BaoLoi(ex); }
        }

        private void ThemVaoGio()
        {
            if (cboSP.SelectedItem is not DataRowView r) return;
            string ma = r["MaSP"].ToString();
            int sl = (int)numSL.Value;

            // Cùng sản phẩm thì cộng dồn số lượng (ChiTietHoaDon có khóa chính MaHD + MaSP)
            var daCo = gioHang.FirstOrDefault(x => x.MaSP == ma);
            if (daCo != null) { daCo.SoLuong += sl; gioHang.ResetBindings(); }
            else gioHang.Add(new ChiTietHoaDonDTO { MaSP = ma, TenSP = r["TenSP"].ToString(), SoLuong = sl, DonGia = (decimal)r["GiaBan"] });

            numSL.Value = 1;
            CapNhatTong();
        }

        private int? MaKHDangChon()
        {
            int ma = cboKH.SelectedValue is int v ? v : 0;
            return ma > 0 ? ma : null;
        }

        private void CapNhatTong()
        {
            if (lblTong == null) return;
            decimal tong = HoaDonBUS.TinhTongTien(gioHang);
            lblTong.Text = "Tổng tiền: " + UI.Tien(tong);
            lblDiem.Text = MaKHDangChon().HasValue
                ? $"Điểm tích lũy được cộng: +{KhachHangBUS.TinhDiem(tong)} (100.000đ = 1 điểm)"
                : "Khách lẻ – không tích điểm";
        }

        private void ThanhToan()
        {
            try
            {
                int maNV = cboNV.SelectedValue is int nv ? nv : 0;
                int maHD = hdBus.ThanhToan(maNV, MaKHDangChon(), gioHang.ToList());
                UI.BaoThanhCong($"Thanh toán thành công!\nSố hóa đơn: {maHD}\nTổng tiền: {lblTong.Text.Replace("Tổng tiền: ", "")}\n\n" +
                                "Đã COMMIT: ghi HoaDon, ChiTietHoaDon, trừ tồn kho và cộng điểm tích lũy.");
                gioHang.Clear();
                object nvCu = cboNV.SelectedValue, khCu = cboKH.SelectedValue;
                TaiDanhMuc();
                cboNV.SelectedValue = nvCu;
                cboKH.SelectedValue = khCu;
                CapNhatTong();
            }
            catch (GiaoDichException ex)
            {
                MessageBox.Show(ex.Message, "Transaction Rollback", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex) { UI.BaoLoi(ex); }
        }
    }
}
