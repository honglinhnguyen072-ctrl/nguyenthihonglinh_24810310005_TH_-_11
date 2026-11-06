
IF DB_ID(N'SalesDB') IS NULL
    CREATE DATABASE SalesDB;
GO
USE SalesDB;
GO

CREATE TABLE NhanVien (
    MaNV        INT IDENTITY(1,1) PRIMARY KEY,
    TenNV       NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15) UNIQUE,
    ChucVu      NVARCHAR(50)
);

CREATE TABLE KhachHang (
    MaKH        INT IDENTITY(1,1) PRIMARY KEY,
    TenKH       NVARCHAR(100) NOT NULL,
    DienThoai   VARCHAR(15) UNIQUE,
    DiemTichLuy INT NOT NULL DEFAULT 0
);

CREATE TABLE SanPham (
    MaSP       VARCHAR(20) PRIMARY KEY,
    TenSP      NVARCHAR(150) NOT NULL,
    GiaBan     DECIMAL(18,2) NOT NULL CHECK (GiaBan > 0),
    SoLuongTon INT NOT NULL CHECK (SoLuongTon >= 0)
);

CREATE TABLE HoaDon (
    MaHD     INT IDENTITY(1,1) PRIMARY KEY,
    NgayLap  DATETIME NOT NULL DEFAULT GETDATE(),
    MaNV     INT NOT NULL FOREIGN KEY REFERENCES NhanVien(MaNV),
    MaKH     INT NULL FOREIGN KEY REFERENCES KhachHang(MaKH),
    TongTien DECIMAL(18,2) NOT NULL
);

CREATE TABLE ChiTietHoaDon (
    MaHD    INT NOT NULL FOREIGN KEY REFERENCES HoaDon(MaHD),
    MaSP    VARCHAR(20) NOT NULL FOREIGN KEY REFERENCES SanPham(MaSP),
    SoLuong INT NOT NULL CHECK (SoLuong > 0),
    DonGia  DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (MaHD, MaSP)
);
GO

INSERT INTO NhanVien (TenNV, SoDienThoai, ChucVu) VALUES
(N'Nguyễn Văn An',  '0901000001', N'Quản lý'),
(N'Trần Thị Bình',  '0901000002', N'Thu ngân'),
(N'Lê Minh Cường',  '0901000003', N'Thu ngân');

INSERT INTO KhachHang (TenKH, DienThoai, DiemTichLuy) VALUES
(N'Phạm Thu Hà',    '0912000001', 5),
(N'Hoàng Đức Minh', '0912000002', 0),
(N'Vũ Lan Anh',     '0912000003', 12);

INSERT INTO SanPham (MaSP, TenSP, GiaBan, SoLuongTon) VALUES
('8934563138165', N'Mì Hảo Hảo tôm chua cay',      4500,  200),
('8934588012228', N'Nước suối Aquafina 500ml',      6000,  150),
('8935049501503', N'Sữa tươi Vinamilk 1L',         32000,   60),
('8934673573214', N'Bánh Oreo 133g',               21000,   40),
('8936036020151', N'Dầu ăn Neptune 1L',            52000,   25),
('8934822101336', N'Gạo ST25 túi 5kg',            185000,   10),
('8935001800019', N'Coca-Cola lon 330ml',          10000,  120),
('8934868106241', N'Nước giặt OMO 2.7kg',         159000,    5);

INSERT INTO HoaDon (NgayLap, MaNV, MaKH, TongTien) VALUES
(DATEADD(DAY, -3, GETDATE()), 2, 1, 153000),
(DATEADD(DAY, -1, GETDATE()), 3, NULL, 63000),
(GETDATE(), 2, 3, 390000);

INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES
(1, '8934563138165', 10, 4500), (1, '8935049501503', 2, 32000), (1, '8935001800019', 2, 10000), (1, '8934588012228', 4, 6000),
(2, '8934673573214', 3, 21000),
(3, '8934822101336', 2, 185000), (3, '8935001800019', 2, 10000);
GO
