# Bài 6.1 – Hệ thống Quản lý Bán hàng & Siêu thị mini (Retail Sales System)

## MỤC 1: THÔNG TIN SINH VIÊN

- **Họ và tên sinh viên:** NGUYỄN THỊ HỒNG LINH
- **Mã sinh viên:** 24810310005
- **Lớp / Khóa:** D19QTANM1 – Lập trình .NET

## Công nghệ & yêu cầu đã thực hiện

| Yêu cầu đề bài | Cách làm trong project |
|---|---|
| CSDL `SalesDB` 5 bảng có khóa ngoại | `Database/ScriptDB.sql` – NhanVien, KhachHang, SanPham, HoaDon, ChiTietHoaDon (CHECK, UNIQUE, DEFAULT, PK kép `(MaHD, MaSP)`) + dữ liệu mẫu |
| Kiến trúc 3-Tier | `DTO/` – `DAL/` (ADO.NET) – `BUS/` (kiểm tra nghiệp vụ) – `GUI/` (WinForms) |
| Chống SQL Injection 100% | Mọi câu lệnh dùng `SqlParameter` (khai báo kiểu `SqlDbType`), không nối chuỗi SQL |
| CRUD Sản phẩm / Khách hàng / Nhân viên | `FrmSanPham`, `FrmKhachHang`, `FrmNhanVien` |
| Tìm sản phẩm theo tên hoặc mã vạch với `%` | `SanPhamDAL.TimKiem`: `LIKE @tuKhoa`, ký tự `%` nằm trong **giá trị** tham số |
| Lập hóa đơn bắt buộc dùng `SqlTransaction` | `HoaDonDAL.LapHoaDon`: INSERT HoaDon → INSERT ChiTietHoaDon → trừ `SoLuongTon` → cộng `DiemTichLuy` → `Commit()`; thiếu hàng → `trans.Rollback()` |
| Tích điểm `DiemTichLuy += TongTien / 100000` | `KhachHangBUS.TinhDiem` |
| Doanh thu ngày / tháng bằng `ExecuteScalar` | `ThongKeDAL.DoanhThuNgay`, `DoanhThuThang` |
| Top 5 sản phẩm bán chạy bằng `SqlDataReader` | `ThongKeDAL.Top5BanChay` |

Trừ tồn kho dùng câu lệnh có điều kiện để không bao giờ bán âm kho:

```sql
UPDATE SanPham SET SoLuongTon = SoLuongTon - @sl
WHERE MaSP = @sp AND SoLuongTon >= @sl   -- 0 dòng bị ảnh hưởng => Rollback
```

## MỤC 2: HÌNH ẢNH KẾT QUẢ CÁC MÀN HÌNH CHỨC NĂNG

1. **Màn hình Chính / Tổng quan hệ thống** – doanh thu hôm nay, doanh thu tháng, số mặt hàng, số hóa đơn.
<img width="361" height="238" alt="Screenshot 2026-11-06 201316" src="https://github.com/user-attachments/assets/092572e6-87d7-49cb-a894-15830d974534" />

  

2. **Màn hình Quản lý Danh mục (CRUD – Thêm/Sửa/Xóa)** – thêm sản phẩm "Trà xanh C2 455ml" rồi sửa giá/tồn kho.

  <img width="425" height="299" alt="Screenshot 2026-11-06 201328" src="https://github.com/user-attachments/assets/a9d79791-4159-4761-85d4-915620c903fd" />


3. **Màn hình Thực thi Nghiệp vụ chính (Lập Hóa đơn / Bán hàng)** – quét mã vạch thêm vào giỏ, chọn khách hàng, thanh toán: SqlTransaction COMMIT (hóa đơn số 4, cộng +2 điểm).

  <img width="465" height="304" alt="Screenshot 2026-11-06 201338" src="https://github.com/user-attachments/assets/aace7af0-a169-45f5-8385-cfa1152669a5" />


4. **Màn hình Báo cáo – Thống kê** – doanh thu ngày/tháng (ExecuteScalar), Top 5 bán chạy (SqlDataReader), danh sách hóa đơn.

<img width="428" height="319" alt="Screenshot 2026-11-06 201404" src="https://github.com/user-attachments/assets/4a18cc6c-7ba4-4f0c-a7cf-c31c2556f1b9" />


5. **Minh họa An toàn Dữ liệu (Transaction Rollback)** – bán 10 "Nước giặt OMO" trong khi tồn kho chỉ còn 5 → `trans.Rollback()`, cả hóa đơn lẫn dòng "Sữa tươi Vinamilk" cùng giỏ đều không bị ghi.

  <img width="463" height="286" alt="Screenshot 2026-11-06 201414" src="https://github.com/user-attachments/assets/88cb2da0-b55e-4570-80e1-c703621c6800" />


   **Phòng chống SQL Injection** – nhập `' OR 1=1 --` vào ô tìm kiếm: chuỗi được truyền như dữ liệu qua `SqlParameter`, không trả về bản ghi nào.

  <img width="418" height="290" alt="Screenshot 2026-11-06 201423" src="https://github.com/user-attachments/assets/5cff432f-622b-4d57-9db9-0086c6519e5e" />


## Cấu trúc thư mục

```
├── Database/
│   └── ScriptDB.sql          # Tạo CSDL SalesDB + dữ liệu mẫu
├── QuanLyBanHang/
│   ├── DTO/                  # SanPhamDTO, KhachHangDTO, NhanVienDTO, HoaDonDTO, ChiTietHoaDonDTO, TopSanPhamDTO
│   ├── DAL/                  # DBConnection, SanPhamDAL, KhachHangDAL, NhanVienDAL, HoaDonDAL (SqlTransaction), ThongKeDAL
│   ├── BUS/                  # SanPhamBUS, KhachHangBUS, NhanVienBUS, HoaDonBUS, ThongKeBUS
│   ├── GUI/                  # FrmMain, FrmSanPham, FrmKhachHang, FrmNhanVien, FrmBanHang, FrmThongKe, UI
│   └── App.config / Program.cs
├── images/                   # Ảnh chụp màn hình
├── README.md
└── QuanLyBanHang.sln
```

## Cách chạy

1. Cần Visual Studio 2022/2026 với .NET 10 và **SQL Server LocalDB** (cài kèm workload ".NET desktop development").
2. Mở `QuanLyBanHang.sln`, nhấn **F5**. NuGet tự tải `Microsoft.Data.SqlClient`.
3. Lần chạy đầu, chương trình tự chạy `Database/ScriptDB.sql` để tạo CSDL `SalesDB` và dữ liệu mẫu (hoặc chạy script thủ công trong SSMS).
4. Dùng SQL Server Express thay vì LocalDB: sửa `Data Source` trong `App.config` (ví dụ `.\SQLEXPRESS`).

Mã vạch mẫu để thử chức năng quét: `8934563138165` (Mì Hảo Hảo), `8934822101336` (Gạo ST25), `8934868106241` (OMO – tồn 5, dùng để thử Rollback).
# Bài 6.2 – Hệ thống Quản lý Nhân sự & Tính lương (HRM & Payroll System)

## MỤC 1: THÔNG TIN SINH VIÊN

- **Họ và tên sinh viên:** NGUYỄN THỊ HỒNG LINH
- **Mã sinh viên:** 24810310005
- **Lớp / Khóa:** D19QTANM1 – Lập trình .NET

## Công nghệ & yêu cầu đã thực hiện

| Yêu cầu đề bài | Cách làm trong project |
|---|---|
| CSDL `HRMDB` 4 bảng | `Database/ScriptDB.sql` – PhongBan, NhanVien, BangChamCong, BangLuong + dữ liệu mẫu (tháng 10/2026 đã chốt) |
| Kiến trúc 3-Tier | `DTO/` – `DAL/` – `BUS/` – `GUI/` |
| CRUD hồ sơ, chuyển phòng ban | `FrmNhanVien` (Thêm / Sửa / Xóa / nút **Chuyển** phòng ban) |
| Tìm theo tên, phòng ban hoặc CCCD (parameterized) | `NhanVienDAL.TimKiem`: `(HoTen LIKE @tk OR CCCD LIKE @tk) AND (@pb = 0 OR MaPB = @pb)` |
| Chấm công **ngắt kết nối** | `ChamCongDAL`: `SqlDataAdapter.Fill` vào `DataSet`/`DataTable`; nhân viên chưa có dòng chấm công được thêm sẵn (RowState = Added); nhập ngày công, giờ OT trực tiếp trên `DataGridView` |
| "Lưu chấm công" bằng `SqlCommandBuilder` + `adapter.Update()` | `ChamCongDAL.LuuChamCong` – đồng bộ toàn bộ dòng Added/Modified trong 1 lệnh |
| Công thức lương | `LuongBUS.TinhThucLinh`: `ThucLinh = (LuongCoBan × HeSoLuong / 22) × SoNgayCong + SoGioOT × 50.000` |
| "Chốt lương tháng" bằng `SqlTransaction` | `LuongDAL.ChotLuong`: kiểm tra chưa chốt → INSERT BangLuong cho toàn bộ nhân viên → `UPDATE BangChamCong SET DaChot = 1` (khóa chấm công) → `Commit()`; lỗi → `Rollback()` |
| Báo cáo | `FrmBangLuong`: tổng quỹ lương (`ExecuteScalar`), thống kê theo phòng ban (`SqlDataReader`) |

**Ghi chú thiết kế CSDL:**
- Đề bài ghi `MaLuong` là khóa chính Identity và đồng thời `(MaNV, ThangNam)` là khóa chính. Một bảng chỉ có 1 khóa chính nên em để `MaLuong` là PRIMARY KEY và `(MaNV, ThangNam)` là **UNIQUE**: mỗi nhân viên chỉ có 1 bảng lương mỗi tháng.
- Thêm cột `DaChot BIT` vào `BangChamCong` để "khóa dữ liệu chấm công" sau khi chốt lương. Cùng lý do, `(MaNV, ThangNam)` của BangChamCong cũng là UNIQUE.

## MỤC 2: HÌNH ẢNH KẾT QUẢ CÁC MÀN HÌNH CHỨC NĂNG

1. **Màn hình Chính / Tổng quan hệ thống**
<img width="361" height="233" alt="Screenshot 2026-11-06 200745" src="https://github.com/user-attachments/assets/73ee8756-cc2e-4641-8518-bc27716d0d26" />

 
2. **Màn hình Quản lý Danh mục (CRUD – Thêm/Sửa/Xóa)** – thêm nhân viên "Ngô Thanh Sơn", chuyển "Lê Quốc Bảo" sang Phòng Kinh doanh, lọc theo phòng ban.

  <img width="461" height="310" alt="Screenshot 2026-11-06 200759" src="https://github.com/user-attachments/assets/2618f865-96c9-4169-aad7-61d1a99f5232" />


3. **Màn hình Thực thi Nghiệp vụ chính (Chấm công)** – DataTable nạp bằng SqlDataAdapter, nhập ngày công/giờ OT trên lưới, tính lương xem trước.
<img width="464" height="326" alt="Screenshot 2026-11-06 200811" src="https://github.com/user-attachments/assets/b8d03aba-ab37-4e2f-a204-c2bfb69f4063" />


   Bấm **Lưu chấm công**: SqlCommandBuilder + `adapter.Update()` đồng bộ 9 dòng trong 1 thao tác.

 <img width="463" height="325" alt="Screenshot 2026-11-06 200822" src="https://github.com/user-attachments/assets/e50422e5-0263-4675-9c44-b318a3713e92" />

4. **Màn hình Báo cáo – Thống kê** – bảng lương tháng 11/2026 sau khi chốt, tổng quỹ lương, thống kê theo phòng ban.

  <img width="463" height="323" alt="Screenshot 2026-11-06 200834" src="https://github.com/user-attachments/assets/c878f229-ae23-458a-bd66-5412905ae605" />


5. **Minh họa An toàn Dữ liệu (Transaction Rollback)** – bấm "Chốt lương tháng" lần thứ hai cho tháng 11/2026 đã chốt → giao dịch ROLLBACK, không ghi trùng bảng lương; lưới chấm công đã bị khóa, nút Lưu bị vô hiệu hóa.

  <img width="463" height="326" alt="Screenshot 2026-11-06 200846" src="https://github.com/user-attachments/assets/fc0e96f7-dc3e-4153-a47a-bba2d0fd5231" />

## Cấu trúc thư mục

```
├── Database/
│   └── ScriptDB.sql          # Tạo CSDL HRMDB + dữ liệu mẫu
├── QuanLyNhanSu/
│   ├── DTO/                  # PhongBanDTO, NhanVienDTO, ChamCongDTO, BangLuongDTO, ThongKePhongBanDTO
│   ├── DAL/                  # DBConnection, PhongBanDAL, NhanVienDAL, ChamCongDAL (Disconnected), LuongDAL (SqlTransaction)
│   ├── BUS/                  # NhanVienBUS, ChamCongBUS, LuongBUS
│   ├── GUI/                  # FrmMain, FrmNhanVien, FrmChamCong, FrmBangLuong, UI
│   └── App.config / Program.cs
├── images/                   # Ảnh chụp màn hình
├── README.md
└── QuanLyNhanSu.sln
```

## Cách chạy

1. Cần Visual Studio 2022/2026 với .NET 10 và **SQL Server LocalDB** (cài kèm workload ".NET desktop development").
2. Mở `QuanLyNhanSu.sln`, nhấn **F5**. NuGet tự tải `Microsoft.Data.SqlClient`.
3. Lần chạy đầu, chương trình tự chạy `Database/ScriptDB.sql` để tạo CSDL `HRMDB` (hoặc chạy script thủ công trong SSMS).
4. Dùng SQL Server Express thay vì LocalDB: sửa `Data Source` trong `App.config`.

Quy trình thử: **Chấm công & Chốt lương** → chọn tháng → nhập ngày công/giờ OT → **Lưu chấm công** → **Tính lương (xem trước)** → **Chốt lương tháng** → xem ở **Bảng lương – Báo cáo**.
