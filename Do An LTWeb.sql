-- =============================================
--  DATABASE: QL_BanSach
-- =============================================
use master
IF DB_ID('QL_BanSach') IS NOT NULL DROP DATABASE QL_BanSach

GO
IF DB_ID('QL_BanSach') IS NULL
    CREATE DATABASE QL_BanSach;
GO

USE QL_BanSach;
GO

-- =============================================
--  XÓA BẢNG CŨ (NẾU TỒN TẠI)
-- =============================================
IF OBJECT_ID('ChiTietTrangThai') IS NOT NULL DROP TABLE ChiTietTrangThai;
IF OBJECT_ID('ChiTietGH') IS NOT NULL DROP TABLE ChiTietGH;
IF OBJECT_ID('ChiTietDonDH') IS NOT NULL DROP TABLE ChiTietDonDH;
IF OBJECT_ID('Sach_TacGia') IS NOT NULL DROP TABLE Sach_TacGia;
IF OBJECT_ID('DonDatHang') IS NOT NULL DROP TABLE DonDatHang;
IF OBJECT_ID('DiaChiGiaoHang') IS NOT NULL DROP TABLE DiaChiGiaoHang;
IF OBJECT_ID('KhachHang') IS NOT NULL DROP TABLE KhachHang;
IF OBJECT_ID('Sach') IS NOT NULL DROP TABLE Sach;
IF OBJECT_ID('GioHang') IS NOT NULL DROP TABLE GioHang;
IF OBJECT_ID('XaPhuong') IS NOT NULL DROP TABLE XaPhuong;
IF OBJECT_ID('QuanHuyen') IS NOT NULL DROP TABLE QuanHuyen;
IF OBJECT_ID('ThanhPho') IS NOT NULL DROP TABLE ThanhPho;
IF OBJECT_ID('HinhThucThanhToan') IS NOT NULL DROP TABLE HinhThucThanhToan;
IF OBJECT_ID('TrangThai') IS NOT NULL DROP TABLE TrangThai;
IF OBJECT_ID('TacGia') IS NOT NULL DROP TABLE TacGia;
IF OBJECT_ID('NhaXuatBan') IS NOT NULL DROP TABLE NhaXuatBan;
IF OBJECT_ID('TheLoaiSach') IS NOT NULL DROP TABLE TheLoaiSach;
IF OBJECT_ID('NhanVien') IS NOT NULL DROP TABLE NhanVien;
GO

-- =============================================
--  TẠO BẢNG
-- =============================================
CREATE TABLE TheLoaiSach(
	MaTLS int PRIMARY KEY,
	TenTLS nvarchar(200)
);
SELECT * FROM TheLoaiSach;
CREATE TABLE NhaXuatBan(
	MaNXB int PRIMARY KEY,
	TenNXB nvarchar(200),
	DiaChiNXB nvarchar(200),
	Email nvarchar(200),
	SDT varchar(11)
);

CREATE TABLE TacGia(
	MaTG int PRIMARY KEY,
	HoTenTG nvarchar(200),
	ButDanh nvarchar(200),
	NgaySinh date,
	GioiTinh nvarchar(5)
);

CREATE TABLE NhanVien(
	MaNV int PRIMARY KEY,
	HoTenNV nvarchar(60),
	SDTNV varchar(11),
	DiaChiNV nvarchar(200),
	MatKhauNV nvarchar(200),
	GioiTinhNV nvarchar(5),
	Email nvarchar(200)
);

CREATE TABLE GioHang(
	MaGH int PRIMARY KEY,
	NgayUpdateCuoiGH date
);

CREATE TABLE TrangThai(
	MaTT int PRIMARY KEY,
	TenTT nvarchar(200)
);

CREATE TABLE HinhThucThanhToan(
	MaHTTT int PRIMARY KEY,
	TenHTTT nvarchar(50)
);

CREATE TABLE ThanhPho(
	MaTP int PRIMARY KEY,
	TenTP nvarchar(200)
);

CREATE TABLE QuanHuyen(
	MaQH int PRIMARY KEY,
	TenQH nvarchar(200),
	MaTP int FOREIGN KEY REFERENCES ThanhPho(MaTP)
);

CREATE TABLE XaPhuong(
	MaXP int PRIMARY KEY,
	TenXP nvarchar(200),
	ChiPhiGHXP float,
	MaQH int FOREIGN KEY REFERENCES QuanHuyen(MaQH)
);

CREATE TABLE Sach(
	MaSach int PRIMARY KEY,
	TenSach nvarchar(200),
	Gia float,
	AddNgonNgu nvarchar(20),
	SoTrang int,
	Mota nvarchar(500),
	MaTLS int FOREIGN KEY REFERENCES TheLoaiSach(MaTLS),
	MaNXB int FOREIGN KEY REFERENCES NhaXuatBan(MaNXB),
	Hinh varchar(200)
);

CREATE TABLE KhachHang(
	MaKH int PRIMARY KEY,
	HoTenKH nvarchar(200),
	email nvarchar(200),
	SDT varchar(11),
	DiaChiKH nvarchar(200),
	MatKhau nvarchar(200),
	MaGH int FOREIGN KEY REFERENCES GioHang(MaGH)
);

CREATE TABLE DiaChiGiaoHang(
	MaDCGH int PRIMARY KEY,
	HoTenNN nvarchar(200),
	SoNha nvarchar(200),
	GhiChu nvarchar(200),
	MaXP int FOREIGN KEY REFERENCES XaPhuong(MaXP),
	MaKH int FOREIGN KEY REFERENCES KhachHang(MaKH)
);

CREATE TABLE DonDatHang(
	MaDon int PRIMARY KEY,
	NgayDat date,
	TongTien float,
	PhiShip float,
	ThueVAT float,
	MaKH int FOREIGN KEY REFERENCES KhachHang(MaKH),
	MaHTTT int FOREIGN KEY REFERENCES HinhThucThanhToan(MaHTTT),
	MaDCGH int FOREIGN KEY REFERENCES DiaChiGiaoHang(MaDCGH)
);

CREATE TABLE Sach_TacGia(
	MaSach int,
	MaTG int,
	PRIMARY KEY(MaSach, MaTG),
	FOREIGN KEY(MaSach) REFERENCES Sach(MaSach),
	FOREIGN KEY(MaTG) REFERENCES TacGia(MaTG)
);

CREATE TABLE ChiTietDonDH(
	MaDon int,
	MaSach int,
	SoLuongCTDDH int,
	DonGiaCTDDH float,
	PRIMARY KEY(MaDon, MaSach),
	FOREIGN KEY(MaDon) REFERENCES DonDatHang(MaDon),
	FOREIGN KEY(MaSach) REFERENCES Sach(MaSach)
);

CREATE TABLE ChiTietGH(
	MaGH int,
	MaSach int,
	SoLuongSachCTGH int,
	PRIMARY KEY(MaGH, MaSach),
	FOREIGN KEY(MaGH) REFERENCES GioHang(MaGH),
	FOREIGN KEY(MaSach) REFERENCES Sach(MaSach)
);

CREATE TABLE ChiTietTrangThai(
	MaDon int,
	MaTT int,
	NgayCapNhatTT date,
	GhiChuTT nvarchar(510),
	PRIMARY KEY(MaDon, MaTT),
	FOREIGN KEY(MaDon) REFERENCES DonDatHang(MaDon),
	FOREIGN KEY(MaTT) REFERENCES TrangThai(MaTT)
);
GO

CREATE TABLE Sach_TheLoai(
    MaSach int,
    MaTLS int,
    PRIMARY KEY(MaSach, MaTLS),
    FOREIGN KEY(MaSach) REFERENCES Sach(MaSach),
    FOREIGN KEY(MaTLS) REFERENCES TheLoaiSach(MaTLS)
);


-- =============================================
--  COMMENT DELETE (KHI CẦN THÌ BỎ --)
-- =============================================
--DELETE FROM ChiTietTrangThai;
--DELETE FROM ChiTietGH;
--DELETE FROM ChiTietDonDH;
--DELETE FROM Sach_TacGia;
--DELETE FROM DonDatHang;
--DELETE FROM DiaChiGiaoHang;
--DELETE FROM KhachHang;
--DELETE FROM Sach;
--DELETE FROM GioHang;
--DELETE FROM XaPhuong;
--DELETE FROM HinhThucThanhToan;
--DELETE FROM TrangThai;
--DELETE FROM TacGia;
--DELETE FROM NhaXuatBan;
--DELETE FROM TheLoaiSach;
--DELETE FROM NhanVien;
GO

-- =============================================
--  DỮ LIỆU MẪU
-- =============================================

INSERT INTO TheLoaiSach VALUES
(1, N'Tiểu Thuyết'), (2, N'Kinh Tế'), (3, N'Khoa Học'),
(4, N'Truyện Trinh Thám'), (5, N'Truyện Cổ Tích');

INSERT INTO TheLoaiSach (MaTLS, TenTLS)
VALUES
(6, N'Tâm Lý Học'),
(7, N'Lịch Sử'),
(8, N'Công Nghệ Thông Tin'),
(9, N'Truyện Cổ Tích Việt Nam'),     -- con của 'Truyện Cổ Tích'
(10, N'Truyện Cổ Tích Thế Giới'),   -- con của 'Truyện Cổ Tích'
(11, N'Khoa Học Vũ Trụ');  

INSERT INTO NhaXuatBan VALUES
(101, N'NXB Trẻ', N'TP. HCM', N'info@nxbtre.com.vn', '02838421008'),
(102, N'NXB Lao Động', N'Hà Nội', N'ld@nxbld.vn', '02437345373'),
(103, N'NXB Văn Học', N'Đà Nẵng', N'vh@nxbvh.vn', '02363678901');

INSERT INTO TacGia (MaTG, HoTenTG) VALUES
(201, N'Nguyễn Nhật Ánh'),
(202, N'Dale Carnegie'),
(203, N'Arthur Conan Doyle'),
(204, N'Grimm Brothers'),
(205, N'Stephen Hawking'),
(206, N'Stephen Covey'),
(207, N'Thomas Friedman'),
(208, N'Antoine de Saint-Exupéry');

INSERT INTO TrangThai VALUES
(1001, N'Chờ xác nhận'), (1002, N'Đang giao'),
(1003, N'Hoàn tất'), (1004, N'Đã hủy');

INSERT INTO HinhThucThanhToan VALUES
(801, N'Thanh toán khi nhận hàng'), (802, N'Chuyển khoản ngân hàng');

INSERT INTO ThanhPho VALUES
(1, N'Hồ Chí Minh'), (2, N'Hà Nội'), (3, N'Đà Nẵng');

INSERT INTO QuanHuyen VALUES
(101, N'Quận 1', 1), (102, N'Quận 2', 1), (103, N'Quận 3', 1),
(104, N'Quận 4', 1), (105, N'Quận 5', 1),
(201, N'Quận Ba Đình', 2), (202, N'Quận Hoàn Kiếm', 2),
(301, N'Quận Hải Châu', 3), (302, N'Quận Thanh Khê', 3);

INSERT INTO XaPhuong VALUES
(601, N'Phường Bến Nghé', 15000, 101),
(602, N'Phường Bến Thành', 15000, 101),
(603, N'Phường Cô Giang', 15000, 101),
(604, N'Phường Nguyễn Thái Bình', 15000, 101),
(605, N'Phường An Phú', 20000, 102),
(606, N'Phường Thảo Điền', 20000, 102),
(607, N'Phường An Khánh', 20000, 102),
(608, N'Phường 1', 18000, 103),
(609, N'Phường 2', 18000, 103),
(610, N'Phường 3', 18000, 103),
(611, N'Phường 1', 17000, 104),
(612, N'Phường 2', 17000, 104),
(613, N'Phường 1', 16000, 105),
(614, N'Phường 2', 16000, 105),
(615, N'Phường Ngọc Hà', 25000, 201),
(616, N'Phường Điện Biên', 25000, 201),
(617, N'Phường Hàng Bạc', 25000, 202),
(618, N'Phường Hàng Bài', 25000, 202),
(619, N'Phường Thạch Thang', 22000, 301),
(620, N'Phường Hải Châu 1', 22000, 301),
(621, N'Phường Tân Chính', 22000, 302),
(622, N'Phường Thanh Khê Đông', 22000, 302);

INSERT INTO GioHang VALUES
(401, '2025-10-20'), (402, '2025-10-18');

INSERT INTO NhanVien VALUES
(301, N'Lê Văn C', '0911223344', N'Q1, TP.HCM', N'12345', N'Nam', N'lvanc@mail.com');

INSERT INTO Sach VALUES
(1001, N'Mắt Biếc', 120000, N'Việt', 250, N'Tiểu thuyết tình cảm', 1, 101, '1.jpg'),
(1002, N'Đắc Nhân Tâm', 150000, N'Việt', 300, N'Sách kỹ năng sống', 2, 102, '2.jpg'),
(1003, N'Vũ Trụ Trong Vỏ Hạt Dẻ', 200000, N'Anh', 180, N'Sách khoa học phổ thông', 3, 101, '3.jpg'),
(1004, N'Thám Tử Sherlock Holmes', 180000, N'Anh', 400, N'Truyện trinh thám nổi tiếng', 4, 103, '4.jpg'),
(1005, N'Cô Bé Lọ Lem', 90000, N'Việt', 120, N'Truyện cổ tích kinh điển', 5, 103, '5.jpg'),
(1006, N'Bảy Thói Quen Hiệu Quả', 210000, N'Việt', 350, N'Sách phát triển bản thân', 2, 102, '6.jpg'),
(1007, N'Trên Đường Băng', 130000, N'Việt', 200, N'Sách truyền cảm hứng', 1, 101, '7.jpg'),
(1008, N'Giấc Mơ Thiên Đường', 170000, N'Việt', 260, N'Tiểu thuyết hiện đại', 1, 101, '8.jpg'),
(1009, N'Thế Giới Phẳng', 250000, N'Việt', 400, N'Kinh tế toàn cầu', 2, 102, '9.jpg'),
(1010, N'Bí Ẩn Vũ Trụ', 300000, N'Anh', 320, N'Khám phá khoa học không gian', 3, 101, '10.jpg'),
(1011, N'Trại Hoa Vàng', 95000, N'Việt', 180, N'Truyện học trò nhẹ nhàng', 1, 101, '11.jpg'),
(1012, N'Hoàng Tử Bé', 110000, N'Pháp', 150, N'Truyện triết lý cho trẻ nhỏ', 5, 103, '12.jpg');

INSERT INTO KhachHang(MaKH, HoTenKH, email,SDT, DiaChiKH, MatKhau, MaGH) VALUES
(501, N'Trần Văn A', N'tva@mail.com', '0901112222', N'123 Nguyễn Trãi', N'pass123', 401),
(502, N'Lê Thị B', N'ltb@mail.com', '0903334444', N'456 Hai Bà Trưng', N'pass456', 402);


INSERT INTO DiaChiGiaoHang VALUES
(701, N'Trần Văn A', N'123A', N'Gọi trước khi giao', 601, 501),
(702, N'Lê Thị B', N'456B', N'Giao giờ hành chính', 602, 502);

INSERT INTO Sach_TacGia (MaSach, MaTG) VALUES
(1001, 201), -- Mắt Biếc - Nguyễn Nhật Ánh
(1002, 202), -- Đắc Nhân Tâm - Dale Carnegie
(1003, 205), -- Vũ Trụ Trong Vỏ Hạt Dẻ - Stephen Hawking
(1004, 203), -- Thám Tử Sherlock Holmes - Arthur Conan Doyle
(1005, 204), -- Cô Bé Lọ Lem - Grimm Brothers
(1006, 206), -- Bảy Thói Quen Hiệu Quả - Stephen Covey
(1007, 201), -- Trên Đường Băng - Nguyễn Nhật Ánh
(1008, 201), -- Giấc Mơ Thiên Đường - Nguyễn Nhật Ánh
(1009, 207), -- Thế Giới Phẳng - Thomas Friedman
(1010, 205), -- Bí Ẩn Vũ Trụ - Stephen Hawking
(1011, 201), -- Trại Hoa Vàng - Nguyễn Nhật Ánh
(1012, 208); -- Hoàng Tử Bé - Antoine de Saint-Exupéry

INSERT INTO Sach_TheLoai VALUES (1001, 1), (1001, 6);
INSERT INTO Sach_TheLoai VALUES (1002, 2), (1002, 6);
INSERT INTO Sach_TheLoai VALUES (1003, 3), (1003, 11);
INSERT INTO Sach_TheLoai VALUES (1004, 4);
INSERT INTO Sach_TheLoai VALUES (1005, 5), (1005, 10);
INSERT INTO Sach_TheLoai VALUES (1006, 2), (1006, 6);
INSERT INTO Sach_TheLoai VALUES (1007, 1), (1007, 6);
INSERT INTO Sach_TheLoai VALUES (1008, 1), (1008, 6);
INSERT INTO Sach_TheLoai VALUES (1009, 2);
INSERT INTO Sach_TheLoai VALUES (1010, 3), (1010, 11);
INSERT INTO Sach_TheLoai VALUES (1011, 1), (1011, 6);
INSERT INTO Sach_TheLoai VALUES (1012, 5), (1012, 6);

INSERT INTO DonDatHang VALUES
(901, '2025-10-21', 135.00, 15.00, 0.05, 501, 801, 701),
(902, '2025-10-22', 420.00, 30.00, 0.10, 502, 802, 702);

INSERT INTO ChiTietDonDH VALUES
(901, 1001, 1, 120.00),
(901, 1005, 1, 90.00),
(902, 1002, 2, 150.00),
(902, 1006, 1, 210.00);

INSERT INTO ChiTietGH VALUES
(401, 1004, 1),
(401, 1007, 2),
(402, 1003, 1),
(402, 1012, 1);

INSERT INTO ChiTietTrangThai VALUES
(901, 1001, '2025-10-21', N'Mới tạo'),
(901, 1002, '2025-10-22', N'Đang giao'),
(902, 1001, '2025-10-22', N'Đã xác nhận');
GO

-- =============================================
--  KIỂM TRA DỮ LIỆU
-- =============================================
SELECT * FROM TheLoaiSach;
SELECT * FROM NhaXuatBan;
SELECT * FROM TacGia;
SELECT * FROM Sach;
SELECT * FROM GioHang;
SELECT * FROM KhachHang;
SELECT * FROM XaPhuong;
SELECT * FROM DiaChiGiaoHang;
SELECT * FROM HinhThucThanhToan;
SELECT * FROM DonDatHang;
SELECT * FROM TrangThai;
SELECT * FROM ChiTietDonDH;
SELECT * FROM ChiTietGH;
SELECT * FROM ChiTietTrangThai;
SELECT * FROM NhanVien;



select*from KhachHang
select*from TheLoaiSach



UPDATE Sach SET 
    Gia = CASE MaSach
        WHEN 1001 THEN 120000
        WHEN 1002 THEN 150000
        WHEN 1003 THEN 200000
        WHEN 1004 THEN 180000
        WHEN 1005 THEN 90000
        WHEN 1006 THEN 210000
        WHEN 1007 THEN 130000
        WHEN 1008 THEN 170000
        WHEN 1009 THEN 250000
        WHEN 1010 THEN 300000
        WHEN 1011 THEN 95000
        WHEN 1012 THEN 110000
    END;


UPDATE XaPhuong SET 
    ChiPhiGHXP = CASE MaXP
        WHEN 601 THEN 15000
        WHEN 602 THEN 15000
        WHEN 603 THEN 15000
        WHEN 604 THEN 15000
        WHEN 605 THEN 20000
        WHEN 606 THEN 20000
        WHEN 607 THEN 20000
        WHEN 608 THEN 18000
        WHEN 609 THEN 18000
        WHEN 610 THEN 18000
        WHEN 611 THEN 17000
        WHEN 612 THEN 17000
        WHEN 613 THEN 16000
        WHEN 614 THEN 16000
        WHEN 615 THEN 25000
        WHEN 616 THEN 25000
        WHEN 617 THEN 25000
        WHEN 618 THEN 25000
        WHEN 619 THEN 22000
        WHEN 620 THEN 22000
        WHEN 621 THEN 22000
        WHEN 622 THEN 22000
    END;


UPDATE ChiTietDonDH SET 
    DonGiaCTDDH = CASE 
        WHEN MaDon = 901 AND MaSach = 1001 THEN 120000
        WHEN MaDon = 901 AND MaSach = 1005 THEN 90000
        WHEN MaDon = 902 AND MaSach = 1002 THEN 150000
        WHEN MaDon = 902 AND MaSach = 1006 THEN 210000
    END;


UPDATE DonDatHang SET 
    TongTien = CASE MaDon
        WHEN 901 THEN 210000  
        WHEN 902 THEN 510000  
    END,
    PhiShip = CASE MaDon
        WHEN 901 THEN 15000
        WHEN 902 THEN 30000
    END,
    ThueVAT = CASE MaDon
        WHEN 901 THEN 10500   
        WHEN 902 THEN 51000   
    END;

-- 5. Kiểm tra dữ liệu sau khi cập nhật
SELECT 'Sach' as Table_Name, MaSach, TenSach, Gia FROM Sach;
SELECT 'XaPhuong' as Table_Name, MaXP, TenXP, ChiPhiGHXP FROM XaPhuong;
SELECT 'ChiTietDonDH' as Table_Name, MaDon, MaSach, SoLuongCTDDH, DonGiaCTDDH FROM ChiTietDonDH;
SELECT 'DonDatHang' as Table_Name, MaDon, TongTien, PhiShip, ThueVAT FROM DonDatHang;

-- THÊM CỘT MaTT VÀO BẢNG DonDatHang
