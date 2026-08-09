USE [MyeStore];
GO

ALTER TABLE [dbo].[HangHoa] ALTER COLUMN [Hinh] NVARCHAR(500);
-- 1. Xóa dữ liệu ở bảng ChiTietHD trước để gỡ ràng buộc khóa ngoại
DELETE FROM [dbo].[ChiTietHD];
GO

-- 2. Xóa sạch dữ liệu cũ trong bảng HangHoa
DELETE FROM [dbo].[HangHoa];
GO

-- 3. Reset lại mã tự tăng MaHH về 0 (sản phẩm đầu tiên sẽ bắt đầu từ mã 1)
DBCC CHECKIDENT ('[dbo].[HangHoa]', RESEED, 0);
GO

-- 5. CHÈN BỘ 40 SẢN PHẨM CÔNG NGHỆ MỚI
INSERT INTO [dbo].[HangHoa] 
    ([TenHH], [TenAlias], [MaLoai], [MoTaDonVi], [DonGia], [Hinh], [NgaySX], [GiamGia], [SoLanXem], [MoTa], [MaNCC]) 
VALUES 

-----------------------------------------------------------------------------------------------------------------------
-- NHÓM 1: ĐIỆN THOẠI (10 SẢN PHẨM - MaLoai = 1000)
-----------------------------------------------------------------------------------------------------------------------
(N'iPhone 15 Pro Max 256GB', N'iphone-15-pro-max-256gb', 1000, N'Chiếc', 29990000, N'https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=600', GETDATE(), 0.05, 120, N'Khung Titanium, Chip A17 Pro, Camera Zoom 5x', N'AP'),
(N'Samsung Galaxy S24 Ultra 5G', N'samsung-galaxy-s24-ultra', 1000, N'Chiếc', 27990000, N'https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=600', GETDATE(), 0.08, 95, N'Tích hợp Galaxy AI, Bút S-Pen, Camera 200MP', N'SS'),
(N'Xiaomi 14 Ultra 512GB', N'xiaomi-14-ultra', 1000, N'Chiếc', 24990000, N'https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?w=600', GETDATE(), 0.0, 45, N'Ống kính Leica Summilux, Snapdragon 8 Gen 3', N'XM'),
(N'OPPO Find X7 Ultra', N'oppo-find-x7-ultra', 1000, N'Chiếc', 22990000, N'https://images.unsplash.com/photo-1598327105666-5b89351aff97?w=600', GETDATE(), 0.05, 60, N'Hệ thống 2 camera Periscope Hasselblad đỉnh cao', N'OP'),
(N'iPhone 14 Pro 128GB', N'iphone-14-pro-128gb', 1000, N'Chiếc', 22490000, N'https://images.unsplash.com/photo-1678685888221-cda773a3dcdb?w=600', GETDATE(), 0.1, 180, N'Màn hình Dynamic Island, Chip A16 Bionic', N'AP'),
(N'Samsung Galaxy Z Fold5 512GB', N'samsung-galaxy-z-fold5', 1000, N'Chiếc', 34990000, N'https://images.unsplash.com/photo-1580910051074-3eb694886505?w=600', GETDATE(), 0.12, 110, N'Màn hình gập độc đáo, bản lề Flex không khe hở', N'SS'),
(N'Google Pixel 8 Pro', N'google-pixel-8-pro', 1000, N'Chiếc', 19990000, N'https://images.unsplash.com/photo-1592899677977-9c10ca588bbd?w=600', GETDATE(), 0.0, 75, N'Thuật toán chụp ảnh AI đỉnh cao từ Google', N'GG'),
(N'Vivo X100 Pro 5G', N'vivo-x100-pro', 1000, N'Chiếc', 20990000, N'https://images.unsplash.com/photo-1565849904461-04a58ad377e0?w=600', GETDATE(), 0.05, 40, N'Ống kính ZEISS APO, Chip Dimensity 9300', N'VV'),
(N'OnePlus 12 512GB', N'oneplus-12', 1000, N'Chiếc', 18490000, N'https://images.unsplash.com/photo-1574944985070-8f3ebc6b79d2?w=600', GETDATE(), 0.0, 30, N'Màn hình 2K 120Hz, Sạc siêu nhanh 100W', N'1P'),
(N'Realme GT 5 Pro', N'realme-gt-5-pro', 1000, N'Chiếc', 14990000, N'https://images.unsplash.com/photo-1546054454-aa26e2b734c7?w=600', GETDATE(), 0.0, 25, N'Cấu hình Flagship giá rẻ, Tản nhiệt siêu rộng', N'RM'),

-----------------------------------------------------------------------------------------------------------------------
-- NHÓM 2: LAPTOP (10 SẢN PHẨM - MaLoai = 1001)
-----------------------------------------------------------------------------------------------------------------------
(N'MacBook Pro 14 M3 Max 36GB', N'macbook-pro-14-m3-max', 1001, N'Chiếc', 79990000, N'https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=600', GETDATE(), 0.03, 210, N'Chip M3 Max quái vật, Màn hình Liquid Retina XDR', N'AP'),
(N'Dell XPS 13 Plus 9320', N'dell-xps-13-plus-9320', 1001, N'Chiếc', 41990000, N'https://images.unsplash.com/photo-1593642632823-8f785ba67e45?w=600', GETDATE(), 0.05, 130, N'Thiết kế tương lai, Phím cảm ứng lực, Màn 4K OLED', N'DE'),
(N'ASUS ROG Zephyrus G14 OLED', N'asus-rog-zephyrus-g14', 1001, N'Chiếc', 38990000, N'https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=600', GETDATE(), 0.08, 150, N'Màn hình ROG OLED 120Hz, Card RTX 4060', N'AS'),
(N'Lenovo ThinkPad X1 Carbon Gen 11', N'thinkpad-x1-carbon-gen-11', 1001, N'Chiếc', 45990000, N'https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?w=600', GETDATE(), 0.05, 85, N'Vỏ sợi Carbon siêu bền, Bàn phím gõ êm nhất thế giới', N'LN'),
(N'HP Spectre x360 14 2-in-1', N'hp-spectre-x360-14', 1001, N'Chiếc', 36990000, N'https://images.unsplash.com/photo-1544731612-de7f96afe55f?w=600', GETDATE(), 0.0, 65, N'Xoay gập 360 độ, Màn hình cảm ứng OLED kèm Bút', N'HP'),
(N'Acer Predator Helios 16 Gaming', N'acer-predator-helios-16', 1001, N'Chiếc', 42990000, N'https://images.unsplash.com/photo-1525547719571-a2d4ac8945e2?w=600', GETDATE(), 0.1, 140, N'Card RTX 4070, Màn hình 240Hz siêu mượt', N'AC'),
(N'MSI Raider GE78 HX Gaming', N'msi-raider-ge78-hx', 1001, N'Chiếc', 64990000, N'https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=600', GETDATE(), 0.05, 90, N'Đèn LED Matrix RGB dải ma trận, Core i9 Gen 13', N'MS'),
(N'MacBook Air 13 inch M2 256GB', N'macbook-air-13-m2', 1001, N'Chiếc', 24990000, N'https://images.unsplash.com/photo-1611186871348-b1ce696e52c9?w=600', GETDATE(), 0.05, 300, N'Mỏng nhẹ 1.24kg, Sạc MagSafe 3, Pin 18 tiếng', N'AP'),
(N'ASUS Zenbook 14 OLED Ultra 7', N'asus-zenbook-14-oled', 1001, N'Chiếc', 28990000, N'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?w=600', GETDATE(), 0.0, 105, N'Màn hình OLED 3K, Chip Intel Core Ultra tích hợp AI', N'AS'),
(N'LG Gram 17 2023 Ultra Light', N'lg-gram-17-2023', 1001, N'Chiếc', 33990000, N'https://images.unsplash.com/photo-1531297484001-80022131f5a1?w=600', GETDATE(), 0.1, 70, N'Màn hình 17 inch siêu to nhưng nặng chưa tới 1.35kg', N'LG'),

-----------------------------------------------------------------------------------------------------------------------
-- NHÓM 3: ĐỒNG HỒ THÔNG MINH (10 SẢN PHẨM - MaLoai = 1002)
-----------------------------------------------------------------------------------------------------------------------
(N'Apple Watch Ultra 2 GPS + Cellular', N'apple-watch-ultra-2', 1002, N'Chiếc', 21990000, N'https://images.unsplash.com/photo-1510017803434-a899398421b3?w=600', GETDATE(), 0.0, 160, N'Vỏ Titanium 49mm, Chống nước 100m, Pin 36 giờ', N'AP'),
(N'Samsung Galaxy Watch6 Classic 47mm', N'galaxy-watch6-classic', 1002, N'Chiếc', 7990000, N'https://images.unsplash.com/photo-1523275335684-37898b6baf30?w=600', GETDATE(), 0.15, 180, N'Vòng xoay Bezel xoay cơ học, Đo nhịp tim & Điện tâm đồ', N'SS'),
(N'Garmin Fenix 7 Pro Solar Titanium', N'garmin-fenix-7-pro', 1002, N'Chiếc', 23990000, N'https://images.unsplash.com/photo-1508685096489-7aacd43bd3b1?w=600', GETDATE(), 0.0, 90, N'Sạc năng lượng mặt trời, Bản đồ GPS đa băng tần', N'GM'),
(N'Apple Watch Series 9 GPS 41mm', N'apple-watch-series-9', 1002, N'Chiếc', 9890000, N'https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?w=600', GETDATE(), 0.1, 250, N'Cử chỉ chạm hai lần Double Tap, Chip S9 SIP', N'AP'),
(N'Xiaomi Watch S3 eSIM', N'xiaomi-watch-s3', 1002, N'Chiếc', 3690000, N'https://images.unsplash.com/photo-1579586337278-3befd40fd17a?w=600', GETDATE(), 0.05, 80, N'Viền mặt đồng hồ tháo rời thay đổi linh hoạt', N'XM'),
(N'Huawei Watch GT 4 46mm Dây Da', N'huawei-watch-gt-4', 1002, N'Chiếc', 5490000, N'https://images.unsplash.com/photo-1544117519-31a4b719223d?w=600', GETDATE(), 0.1, 110, N'Thiết kế mặt bát giác độc đáo, Pin tới 14 ngày', N'HW'),
(N'Garmin Forerunner 965 AMOLED', N'garmin-forerunner-965', 1002, N'Chiếc', 16490000, N'https://images.unsplash.com/photo-1557180295-76eee20ae8aa?w=600', GETDATE(), 0.0, 65, N'Đồng hồ chuyên chạy bộ vặn năng, Màn hình AMOLED', N'GM'),
(N'Amazfit GTR 4 Smartwatch', N'amazfit-gtr-4', 1002, N'Chiếc', 4190000, N'https://images.unsplash.com/photo-1522335789203-aabd1fc54bc9?w=600', GETDATE(), 0.05, 50, N'Định vị GPS 6 vệ tinh, Hơn 150 chế độ thể thao', N'AZ'),
(N'TicWatch Pro 5 Wear OS', N'ticwatch-pro-5', 1002, N'Chiếc', 7290000, N'https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=600', GETDATE(), 0.0, 40, N'Màn hình 2 lớp siêu tiết kiệm pin, Chip Snapdragon W5+', N'TW'),
(N'Fitbit Sense 2 Health Watch', N'fitbit-sense-2', 1002, N'Chiếc', 6290000, N'https://images.unsplash.com/photo-1575311373937-040b8e1fd5b6?w=600', GETDATE(), 0.1, 35, N'Theo dõi căng thẳng Stress cEDA cả ngày', N'FB'),

-----------------------------------------------------------------------------------------------------------------------
-- NHÓM 4: MÁY ẢNH (10 SẢN PHẨM - MaLoai = 1003)
-----------------------------------------------------------------------------------------------------------------------
(N'Sony Alpha A7 Mark IV (Body)', N'sony-alpha-a7-mark-iv', 1003, N'Bộ', 53990000, N'https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=600', GETDATE(), 0.0, 220, N'Full-Frame 33MP, Quay video 4K 60p, Lấy nét AI', N'SN'),
(N'Canon EOS R6 Mark II (Body)', N'canon-eos-r6-mark-ii', 1003, N'Bộ', 58990000, N'https://images.unsplash.com/photo-1607604276583-eef5d076aa5f?w=600', GETDATE(), 0.05, 140, N'Chụp liên tiếp 40fps, Quay 4K 60p không crop', N'CN'),
(N'Fujifilm X-T5 (Body) Black', N'fujifilm-x-t5', 1003, N'Bộ', 41990000, N'https://images.unsplash.com/photo-1502920917128-1aa500764cbd?w=600', GETDATE(), 0.0, 190, N'Thiết kế Hoài cổ, Cảm biến APS-C 40.2 MP, Giả lập màu phim', N'FJ'),
(N'Nikon Z6 II kèm Kit 24-70mm', N'nikon-z6-ii-kit', 1003, N'Bộ', 46990000, N'https://images.unsplash.com/photo-1512790182412-b19e6d611397?w=600', GETDATE(), 0.08, 95, N'2 Cảm biến xử lý EXPEED 6, Khả năng chụp đêm siêu tốt', N'NK'),
(N'Sony Alpha A6700 (Body)', N'sony-a6700', 1003, N'Bộ', 32990000, N'https://images.unsplash.com/photo-1581591524425-c7e0978865ef?w=600', GETDATE(), 0.0, 110, N'Cảm biến APS-C 26MP, Chip AI nhận diện vật thể thông minh', N'SN'),
(N'Canon EOS R10 kèm Lens Kit 18-45mm', N'canon-eos-r10-kit', 1003, N'Bộ', 21990000, N'https://images.unsplash.com/photo-1564466809058-bf81182fe920?w=600', GETDATE(), 0.05, 130, N'Máy ảnh Mirrorless nhỏ gọn giá rẻ cho người mới bắt đầu', N'CN'),
(N'Fujifilm X100VI Compact Camera', N'fujifilm-x100vi', 1003, N'Chiếc', 47990000, N'https://images.unsplash.com/photo-1526170375885-4d8ecf77b99f?w=600', GETDATE(), 0.0, 310, N'Máy ảnh du lịch cao cấp, Cảm biến 40MP, Chống rung 6 trục', N'FJ'),
(N'Panasonic Lumix S5 II (Body)', N'panasonic-lumix-s5-ii', 1003, N'Bộ', 43990000, N'https://images.unsplash.com/photo-1500634245200-e5245c7e73ef?w=600', GETDATE(), 0.05, 60, N'Lấy nét theo pha PDAF mới, Chống rung vô địch cho vlogger', N'PS'),
(N'Leica Q3 Compact Full-Frame', N'leica-q3', 1003, N'Chiếc', 169990000, N'https://images.unsplash.com/photo-1510127034890-ba27508e9f1c?w=600', GETDATE(), 0.0, 85, N'Đỉnh cao xa xỉ, Ống kính Summilux 28mm f/1.7 ASPH, 60MP', N'LC'),
(N'Olympus OM-1 Mark II (Body)', N'olympus-om-1-mark-ii', 1003, N'Bộ', 51990000, N'https://images.unsplash.com/photo-1616423640778-28d1b53229bd?w=600', GETDATE(), 0.0, 40, N'Khả năng chống thời tiết siêu việt, Chụp siêu tốc 120fps', N'OP');
GO

-- 4. Thêm các Nhà cung cấp còn thiếu vào bảng NhaCungCap (Bổ sung giá trị cho cột Logo)
INSERT INTO [dbo].[NhaCungCap] ([MaNCC], [TenCongTy], [Logo], [NguoiLienLac], [Email], [DienThoai], [DiaChi], [MoTa])
SELECT 
    v.MaNCC, 
    v.TenCongTy, 
    N'logo.png', 
    N'Đại diện ' + v.TenCongTy, 
    LOWER(v.MaNCC) + N'@gmail.com', 
    N'0900000000', 
    N'Việt Nam', 
    N'Nhà cung cấp ' + v.TenCongTy
FROM (VALUES 
    (N'AP', N'Apple'), (N'SS', N'Samsung'), (N'XM', N'Xiaomi'), (N'OP', N'OPPO'),
    (N'GG', N'Google'), (N'VV', N'Vivo'), (N'1P', N'OnePlus'), (N'RM', N'Realme'),
    (N'DE', N'Dell'), (N'AS', N'ASUS'), (N'LN', N'Lenovo'), (N'HP', N'HP'),
    (N'AC', N'Acer'), (N'MS', N'MSI'), (N'LG', N'LG'), (N'GM', N'Garmin'),
    (N'HW', N'Huawei'), (N'AZ', N'Amazfit'), (N'TW', N'TicWatch'), (N'FB', N'Fitbit'),
    (N'SN', N'Sony'), (N'CN', N'Canon'), (N'FJ', N'Fujifilm'), (N'NK', N'Nikon'),
    (N'PS', N'Panasonic'), (N'LC', N'Leica')
) AS v(MaNCC, TenCongTy)
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[NhaCungCap] n WHERE n.MaNCC = v.MaNCC);
GO


ALTER TABLE HoaDon ADD PaypalOrderID NVARCHAR(100) NULL;

-- Bước 1: Xóa các chi tiết hóa đơn liên quan trước (tránh lỗi khóa ngoại)
DELETE FROM [MyeStore].[dbo].[ChiTietHD]
WHERE MaHD BETWEEN 10248 AND 11116;

-- Bước 2: Xóa các hóa đơn trong khoảng từ 10248 tới 11116
DELETE FROM [MyeStore].[dbo].[HoaDon]
WHERE MaHD BETWEEN 10248 AND 11116;


UPDATE Loai SET TenLoai = N'Điện thoại thông minh' WHERE MaLoai = 1000;
UPDATE Loai SET TenLoai = N'Laptop & Máy tính' WHERE MaLoai = 1001;
UPDATE Loai SET TenLoai = N'Đồng hồ thông minh' WHERE MaLoai = 1002;

IF EXISTS (SELECT 1 FROM Loai WHERE MaLoai = 1003)
BEGIN
    UPDATE Loai SET TenLoai = N'Máy ảnh' WHERE MaLoai = 1003;
END
ELSE
BEGIN
    INSERT INTO Loai (MaLoai, TenLoai) VALUES (1003, N'Máy ảnh');
END

UPDATE HangHoa 
SET MaLoai = 1000 
WHERE TenHh LIKE N'%iPhone%' 
   OR TenHh LIKE N'%Galaxy S%' 
   OR TenHh LIKE N'%Galaxy Z%' 
   OR TenHh LIKE N'%Xiaomi%' 
   OR TenHh LIKE N'%OPPO%' 
   OR TenHh LIKE N'%Pixel%' 
   OR TenHh LIKE N'%Vivo%' 
   OR TenHh LIKE N'%OnePlus%' 
   OR TenHh LIKE N'%Realme%';

-- 2. Laptop -> 1001
UPDATE HangHoa 
SET MaLoai = 1001 
WHERE TenHh LIKE N'%MacBook%' 
   OR TenHh LIKE N'%Dell%' 
   OR TenHh LIKE N'%ASUS%' 
   OR TenHh LIKE N'%ThinkPad%' 
   OR TenHh LIKE N'%HP%' 
   OR TenHh LIKE N'%Predator%' 
   OR TenHh LIKE N'%Raider%' 
   OR TenHh LIKE N'%Gram%';

-- 3. Đồng hồ -> 1002
UPDATE HangHoa 
SET MaLoai = 1002 
WHERE TenHh LIKE N'%Watch%' 
   OR TenHh LIKE N'%Garmin%' 
   OR TenHh LIKE N'%Amazfit%' 
   OR TenHh LIKE N'%TicWatch%' 
   OR TenHh LIKE N'%Fitbit%' 
   OR TenHh LIKE N'%Huawei%';

-- 4. Máy ảnh -> 1003
UPDATE HangHoa 
SET MaLoai = 1003 
WHERE TenHh LIKE N'%Sony%' 
   OR TenHh LIKE N'%Canon%' 
   OR TenHh LIKE N'%Fujifilm%' 
   OR TenHh LIKE N'%Nikon%' 
   OR TenHh LIKE N'%Panasonic%' 
   OR TenHh LIKE N'%Leica%' 
   OR TenHh LIKE N'%Olympus%';