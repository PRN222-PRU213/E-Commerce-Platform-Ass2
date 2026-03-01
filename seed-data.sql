-- ============================================
-- E-Commerce Platform - Seed Data Script
-- ============================================
-- This script creates sample data for the local database
-- Run this script after running migrations

USE [ECommercePlatformDB]; -- Database name from appsettings.json
GO

SELECT * FROM EKycVerifications

SELECT * FROM users

-- Delete old data (if needed)
DELETE FROM reviews;
DELETE FROM order_items;
DELETE FROM payments;
DELETE FROM shipments;
DELETE FROM orders;
DELETE FROM CartItems;
DELETE FROM Carts;
DELETE FROM product_variants;
DELETE FROM products;
DELETE FROM shops;
DELETE FROM categories;
DELETE FROM users;
DELETE FROM roles;
DELETE FROM Refunds
-- GO

-- ============================================
-- 1. ROLES
-- ============================================
INSERT INTO roles (RoleId, Name, Description, CreatedAt)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'Admin', 'System administrator', GETDATE()),
    ('22222222-2222-2222-2222-222222222222', 'Customer', 'Customer', GETDATE()),
    ('33333333-3333-3333-3333-333333333333', 'Seller', 'Shop owner', GETDATE());
GO

-- ============================================
-- 2. USERS
-- ============================================
-- Password: "123456" (BCrypt hash)
-- You can change the password hash as needed
INSERT INTO users (Id, Name, PasswordHash, Email, RoleId, Status, CreatedAt)
VALUES
    -- Admin
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Admin User', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'admin@example.com', '11111111-1111-1111-1111-111111111111', 1, GETDATE()),
    
    -- Sellers
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Shop Owner 1', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'seller1@example.com', '33333333-3333-3333-3333-333333333333', 1, GETDATE()),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Shop Owner 2', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'seller2@example.com', '33333333-3333-3333-3333-333333333333', 1, GETDATE()),
    
    -- Customers
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'Customer 1', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'customer1@example.com', '22222222-2222-2222-2222-222222222222', 1, GETDATE()),
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Customer 2', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'customer2@example.com', '22222222-2222-2222-2222-222222222222', 1, GETDATE()),
    ('ffffffff-ffff-ffff-ffff-ffffffffffff', 'Customer 3', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'customer3@example.com', '22222222-2222-2222-2222-222222222222', 1, GETDATE());
GO
SELECT * From users
UPDATE users SET EmailVerified = 1 WHERE EmailVerified = 0;
UPDATE users
SET PasswordHash = '123456'
WHERE Email = 'admin@example.com';

Update users SET PasswordHash = '123456' WHERE Email = 'seller1@example.com';

-- ============================================
-- 3. CATEGORIES
-- ============================================
INSERT INTO categories (Id, Name, Status)
VALUES
    ('10000000-0000-0000-0000-000000000001', 'Quần', 'Active'),
    ('10000000-0000-0000-0000-000000000002', 'Áo', 'Active'),
    ('10000000-0000-0000-0000-000000000003', 'Quần áo bộ', 'Active'),
    ('10000000-0000-0000-0000-000000000004', 'Áo khoác', 'Active'),
    ('10000000-0000-0000-0000-000000000005', 'Giày', 'Active'),
    ('10000000-0000-0000-0000-000000000006', 'Dép', 'Active');
GO

-- ============================================
-- 4. SHOPS
-- ============================================
INSERT INTO shops (Id, UserId, ShopName, Description, Status, CreatedAt)
VALUES
    ('20000000-0000-0000-0000-000000000001', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Tech Store', 'Trusted electronics store', 'Active', GETDATE()),
    ('20000000-0000-0000-0000-000000000002', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 'Fashion Shop', 'Men and women fashion', 'Active', GETDATE());
GO

-- ============================================
-- 5. PRODUCTS
-- ============================================
UPDATE products SET Status = 'active' WHERE Status = 'Active';
SELECT * FROM products
INSERT INTO products (Id, ShopId, CategoryId, Name, Description, BasePrice, Status, AvgRating, ImageUrl, CreatedAt)
VALUES
    -- QUẦN (10 sản phẩm)
    ('30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001', 'Quần Jean Nam Slim Fit', 'Quần jean nam form slim fit, chất liệu denim cao cấp', 450000, 'Active', 4.5, 'https://images.unsplash.com/photo-1542272604-787c3835535d?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001', 'Quần Kaki Nam Công Sở', 'Quần kaki nam phong cách công sở, thoải mái', 380000, 'Active', 4.3, 'https://images.unsplash.com/photo-1473966968600-fa801b869a1a?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001', 'Quần Short Jean Nữ', 'Quần short jean nữ thời trang, năng động', 280000, 'Active', 4.2, 'https://images.unsplash.com/photo-1591195853828-11db59a44f6b?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001', 'Quần Jogger Nam', 'Quần jogger nam thoải mái, phù hợp thể thao', 320000, 'Active', 4.4, 'https://images.unsplash.com/photo-1552902865-b72c031ac5ea?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000005', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001', 'Quần Tây Nam Đen', 'Quần tây nam màu đen thanh lịch', 520000, 'Active', 4.6, 'https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000006', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001', 'Quần Legging Nữ', 'Quần legging nữ co giãn 4 chiều', 250000, 'Active', 4.3, 'https://images.unsplash.com/photo-1506629082955-511b1aa562c8?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000007', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001', 'Quần Culottes Nữ', 'Quần culottes nữ ống rộng thời trang', 350000, 'Active', 4.1, 'https://images.unsplash.com/photo-1509631179647-0177331693ae?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000008', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001', 'Quần Cargo Nam', 'Quần cargo nam nhiều túi tiện dụng', 420000, 'Active', 4.4, 'https://images.unsplash.com/photo-1517438476312-10d79c077509?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000009', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001', 'Quần Palazzo Nữ', 'Quần palazzo nữ ống suông thanh lịch', 380000, 'Active', 4.2, 'https://images.unsplash.com/photo-1551854838-212c50b4c184?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000010', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000001', 'Quần Thể Thao Unisex', 'Quần thể thao unisex thoáng mát', 290000, 'Active', 4.5, 'https://images.unsplash.com/photo-1556821840-3a63f95609a7?w=800', GETDATE()),
    
    -- ÁO (10 sản phẩm)
    ('30000000-0000-0000-0000-000000000011', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Áo Thun Nam Basic', 'Áo thun nam cổ tròn basic nhiều màu', 180000, 'Active', 4.4, 'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000012', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Áo Sơ Mi Nam Trắng', 'Áo sơ mi nam trắng công sở cao cấp', 350000, 'Active', 4.6, 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000013', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Áo Polo Nam', 'Áo polo nam thể thao lịch lãm', 280000, 'Active', 4.3, 'https://images.unsplash.com/photo-1625910513413-5fc42e712b4f?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000014', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Áo Croptop Nữ', 'Áo croptop nữ thời trang trẻ trung', 150000, 'Active', 4.2, 'https://images.unsplash.com/photo-1503342217505-b0a15ec3261c?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000015', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Áo Kiểu Nữ Công Sở', 'Áo kiểu nữ thanh lịch cho công sở', 320000, 'Active', 4.5, 'https://images.unsplash.com/photo-1564257631407-4deb1f99d992?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000016', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Áo Len Dệt Kim', 'Áo len dệt kim ấm áp mùa đông', 420000, 'Active', 4.4, 'https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000017', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Áo Tank Top Nữ', 'Áo tank top nữ năng động mùa hè', 130000, 'Active', 4.1, 'https://images.unsplash.com/photo-1503342394128-c104d54dba01?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000018', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Áo Hoodie Unisex', 'Áo hoodie unisex form rộng thoải mái', 380000, 'Active', 4.6, 'https://images.unsplash.com/photo-1556821840-3a63f95609a7?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000019', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Áo Thun Oversize', 'Áo thun oversize phong cách streetwear', 220000, 'Active', 4.3, 'https://images.unsplash.com/photo-1583744946564-b52ac1c389c8?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000020', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Áo Sơ Mi Kẻ Sọc', 'Áo sơ mi kẻ sọc nam nữ đều mặc được', 300000, 'Active', 4.2, 'https://images.unsplash.com/photo-1598033129183-c4f50c736f10?w=800', GETDATE()),
    
    -- QUẦN ÁO BỘ (10 sản phẩm)
    ('30000000-0000-0000-0000-000000000021', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000003', 'Bộ Đồ Ngủ Nữ Lụa', 'Bộ đồ ngủ nữ chất liệu lụa cao cấp', 450000, 'Active', 4.5, 'https://images.unsplash.com/photo-1631947430066-48c30d57b943?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000022', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000003', 'Bộ Vest Nam Công Sở', 'Bộ vest nam lịch lãm cho công sở', 1800000, 'Active', 4.7, 'https://images.unsplash.com/photo-1507679799987-c73779587ccf?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000023', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000003', 'Bộ Thể Thao Nam', 'Bộ thể thao nam thoải mái vận động', 520000, 'Active', 4.4, 'https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000024', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000003', 'Bộ Đồ Ở Nhà Nữ', 'Bộ đồ ở nhà nữ cotton mềm mại', 280000, 'Active', 4.3, 'https://images.unsplash.com/photo-1617137968427-85924c800a22?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000025', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000003', 'Bộ Jumpsuit Nữ', 'Bộ jumpsuit nữ thời trang dạo phố', 480000, 'Active', 4.2, 'https://images.unsplash.com/photo-1525507119028-ed4c629a60a3?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000026', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000003', 'Bộ Đồ Tập Yoga Nữ', 'Bộ đồ tập yoga nữ co giãn tốt', 550000, 'Active', 4.6, 'https://images.unsplash.com/photo-1518611012118-696072aa579a?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000027', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000003', 'Bộ Đầm Dự Tiệc', 'Bộ đầm dự tiệc sang trọng cho nữ', 850000, 'Active', 4.5, 'https://images.unsplash.com/photo-1566174053879-31528523f8ae?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000028', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000003', 'Bộ Đồ Thể Thao Nữ', 'Bộ đồ thể thao nữ năng động', 480000, 'Active', 4.4, 'https://images.unsplash.com/photo-1518459031867-a89b944bffe4?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000029', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000003', 'Bộ Pyjama Gia Đình', 'Bộ pyjama cho cả gia đình đồng phục', 650000, 'Active', 4.3, 'https://images.unsplash.com/photo-1571945153237-4929e783af4a?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000030', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000003', 'Bộ Đồ Công Sở Nữ', 'Bộ đồ công sở nữ thanh lịch', 720000, 'Active', 4.5, 'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=800', GETDATE()),
    
    -- ÁO KHOÁC (10 sản phẩm)
    ('30000000-0000-0000-0000-000000000031', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000004', 'Áo Khoác Jean Nam', 'Áo khoác jean nam phong cách bụi bặm', 550000, 'Active', 4.5, 'https://images.unsplash.com/photo-1576995853123-5a10305d93c0?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000032', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000004', 'Áo Khoác Da Nam', 'Áo khoác da nam cao cấp classic', 1200000, 'Active', 4.7, 'https://images.unsplash.com/photo-1551028719-00167b16eac5?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000033', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000004', 'Áo Khoác Bomber', 'Áo khoác bomber unisex thời thượng', 480000, 'Active', 4.4, 'https://images.unsplash.com/photo-1559551409-dadc959f76b8?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000034', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000004', 'Áo Khoác Gió Nhẹ', 'Áo khoác gió nhẹ chống nước', 350000, 'Active', 4.3, 'https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000035', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000004', 'Áo Khoác Blazer Nữ', 'Áo khoác blazer nữ công sở', 620000, 'Active', 4.5, 'https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000036', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000004', 'Áo Khoác Cardigan', 'Áo khoác cardigan len mỏng nhẹ', 380000, 'Active', 4.2, 'https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000037', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000004', 'Áo Khoác Phao', 'Áo khoác phao siêu nhẹ giữ ấm', 750000, 'Active', 4.6, 'https://images.unsplash.com/photo-1544923246-77307dd628b5?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000038', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000004', 'Áo Khoác Hoodie Zip', 'Áo khoác hoodie có khóa kéo', 420000, 'Active', 4.4, 'https://images.unsplash.com/photo-1556821840-3a63f95609a7?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000039', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000004', 'Áo Khoác Trench Coat', 'Áo khoác trench coat dáng dài', 980000, 'Active', 4.5, 'https://images.unsplash.com/photo-1539533018447-63fcce2678e3?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000040', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000004', 'Áo Khoác Thể Thao', 'Áo khoác thể thao chống gió', 450000, 'Active', 4.3, 'https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=800', GETDATE()),
    
    -- GIÀY (10 sản phẩm)
    ('30000000-0000-0000-0000-000000000041', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000005', 'Giày Sneaker Nam Trắng', 'Giày sneaker nam trắng classic', 850000, 'Active', 4.6, 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000042', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000005', 'Giày Chạy Bộ Nike', 'Giày chạy bộ Nike đệm êm', 1500000, 'Active', 4.7, 'https://images.unsplash.com/photo-1460353581641-37baddab0fa2?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000043', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000005', 'Giày Tây Nam Da Bò', 'Giày tây nam da bò thật 100%', 1200000, 'Active', 4.5, 'https://images.unsplash.com/photo-1614252369475-531eba835eb1?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000044', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000005', 'Giày Cao Gót Nữ', 'Giày cao gót nữ 7cm thanh lịch', 650000, 'Active', 4.3, 'https://images.unsplash.com/photo-1543163521-1bf539c55dd2?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000045', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000005', 'Giày Bệt Nữ', 'Giày bệt nữ êm chân đi làm', 420000, 'Active', 4.4, 'https://images.unsplash.com/photo-1551107696-a4b0c5a0d9a2?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000046', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000005', 'Giày Boot Cổ Cao', 'Giày boot cổ cao phong cách', 980000, 'Active', 4.5, 'https://images.unsplash.com/photo-1520639888713-7851133b1ed0?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000047', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000005', 'Giày Lười Nam', 'Giày lười nam da mềm tiện lợi', 550000, 'Active', 4.2, 'https://images.unsplash.com/photo-1533867617858-e7b97e060509?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000048', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000005', 'Giày Thể Thao Nữ', 'Giày thể thao nữ nhẹ thoáng', 780000, 'Active', 4.6, 'https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000049', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000005', 'Giày Sandal Nam', 'Giày sandal nam mùa hè thoáng mát', 380000, 'Active', 4.3, 'https://images.unsplash.com/photo-1603487742131-4160ec999306?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000050', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000005', 'Giày Oxford Nữ', 'Giày oxford nữ retro phong cách', 580000, 'Active', 4.4, 'https://images.unsplash.com/photo-1560343090-f0409e92791a?w=800', GETDATE()),
    
    -- DÉP (10 sản phẩm)
    ('30000000-0000-0000-0000-000000000051', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000006', 'Dép Quai Ngang Nam', 'Dép quai ngang nam đế êm', 180000, 'Active', 4.3, 'https://images.unsplash.com/photo-1603487742131-4160ec999306?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000052', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000006', 'Dép Xỏ Ngón Nữ', 'Dép xỏ ngón nữ đi biển', 120000, 'Active', 4.2, 'https://images.unsplash.com/photo-1535043934128-cf0b28d52f95?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000053', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000006', 'Dép Lê Đi Trong Nhà', 'Dép lê êm ái đi trong nhà', 85000, 'Active', 4.4, 'https://images.unsplash.com/photo-1604191784837-0f8da3e3d192?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000054', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000006', 'Dép Cao Gót Quai Trong', 'Dép cao gót quai trong sang trọng', 350000, 'Active', 4.3, 'https://images.unsplash.com/photo-1543163521-1bf539c55dd2?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000055', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000006', 'Dép Sandal Đế Xuồng', 'Dép sandal đế xuồng 5cm thoải mái', 280000, 'Active', 4.5, 'https://images.unsplash.com/photo-1570733117311-d990c3816c47?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000056', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000006', 'Dép Da Nam Cao Cấp', 'Dép da nam cao cấp cho công sở', 450000, 'Active', 4.6, 'https://images.unsplash.com/photo-1533867617858-e7b97e060509?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000057', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000006', 'Dép Massage', 'Dép massage chân thư giãn', 150000, 'Active', 4.2, 'https://images.unsplash.com/photo-1604191784837-0f8da3e3d192?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000058', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000006', 'Dép Quai Hậu Nữ', 'Dép quai hậu nữ thời trang', 220000, 'Active', 4.4, 'https://images.unsplash.com/photo-1570733117311-d990c3816c47?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000059', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000006', 'Dép Đi Mưa', 'Dép đi mưa chống trượt', 95000, 'Active', 4.1, 'https://images.unsplash.com/photo-1535043934128-cf0b28d52f95?w=800', GETDATE()),
    ('30000000-0000-0000-0000-000000000060', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000006', 'Dép Nữ Đính Đá', 'Dép nữ đính đá lấp lánh', 320000, 'Active', 4.3, 'https://images.unsplash.com/photo-1543163521-1bf539c55dd2?w=800', GETDATE());
GO

-- ============================================
-- 6. PRODUCT VARIANTS
-- ============================================
INSERT INTO product_variants (Id, ProductId, VariantName, Price, Size, Color, Stock, Sku, Status, ImageUrl)
VALUES
    -- QUẦN variants
    ('40000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001', 'Quần Jean Nam Slim Fit - Size 30 - Xanh', 450000, '30', 'Xanh', 50, 'JEAN-M-30-XANH', 'Active', 'https://images.unsplash.com/photo-1542272604-787c3835535d?w=800'),
    ('40000000-0000-0000-0000-000000000002', '30000000-0000-0000-0000-000000000001', 'Quần Jean Nam Slim Fit - Size 32 - Xanh', 450000, '32', 'Xanh', 45, 'JEAN-M-32-XANH', 'Active', 'https://images.unsplash.com/photo-1542272604-787c3835535d?w=800'),
    ('40000000-0000-0000-0000-000000000003', '30000000-0000-0000-0000-000000000002', 'Quần Kaki Nam - Size 30 - Be', 380000, '30', 'Be', 60, 'KAKI-M-30-BE', 'Active', 'https://images.unsplash.com/photo-1473966968600-fa801b869a1a?w=800'),
    ('40000000-0000-0000-0000-000000000004', '30000000-0000-0000-0000-000000000003', 'Quần Short Jean Nữ - Size S - Xanh', 280000, 'S', 'Xanh', 40, 'SHORT-F-S-XANH', 'Active', 'https://images.unsplash.com/photo-1591195853828-11db59a44f6b?w=800'),
    ('40000000-0000-0000-0000-000000000005', '30000000-0000-0000-0000-000000000004', 'Quần Jogger Nam - Size M - Đen', 320000, 'M', 'Đen', 55, 'JOG-M-M-DEN', 'Active', 'https://images.unsplash.com/photo-1552902865-b72c031ac5ea?w=800'),
    ('40000000-0000-0000-0000-000000000006', '30000000-0000-0000-0000-000000000005', 'Quần Tây Nam - Size 30 - Đen', 520000, '30', 'Đen', 35, 'TAY-M-30-DEN', 'Active', 'https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=800'),
    ('40000000-0000-0000-0000-000000000007', '30000000-0000-0000-0000-000000000006', 'Quần Legging Nữ - Size S - Đen', 250000, 'S', 'Đen', 70, 'LEG-F-S-DEN', 'Active', 'https://images.unsplash.com/photo-1506629082955-511b1aa562c8?w=800'),
    ('40000000-0000-0000-0000-000000000008', '30000000-0000-0000-0000-000000000007', 'Quần Culottes Nữ - Size M - Trắng', 350000, 'M', 'Trắng', 30, 'CUL-F-M-TRANG', 'Active', 'https://images.unsplash.com/photo-1509631179647-0177331693ae?w=800'),
    ('40000000-0000-0000-0000-000000000009', '30000000-0000-0000-0000-000000000008', 'Quần Cargo Nam - Size 32 - Xanh Rêu', 420000, '32', 'Xanh Rêu', 40, 'CARGO-M-32-XREU', 'Active', 'https://images.unsplash.com/photo-1517438476312-10d79c077509?w=800'),
    ('40000000-0000-0000-0000-000000000010', '30000000-0000-0000-0000-000000000009', 'Quần Palazzo Nữ - Size M - Đen', 380000, 'M', 'Đen', 25, 'PAL-F-M-DEN', 'Active', 'https://images.unsplash.com/photo-1551854838-212c50b4c184?w=800'),
    ('40000000-0000-0000-0000-000000000011', '30000000-0000-0000-0000-000000000010', 'Quần Thể Thao Unisex - Size L - Xám', 290000, 'L', 'Xám', 80, 'SPORT-U-L-XAM', 'Active', 'https://images.unsplash.com/photo-1556821840-3a63f95609a7?w=800'),
    
    -- ÁO variants
    ('40000000-0000-0000-0000-000000000012', '30000000-0000-0000-0000-000000000011', 'Áo Thun Nam Basic - Size M - Trắng', 180000, 'M', 'Trắng', 100, 'THUN-M-M-TRANG', 'Active', 'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=800'),
    ('40000000-0000-0000-0000-000000000013', '30000000-0000-0000-0000-000000000011', 'Áo Thun Nam Basic - Size L - Đen', 180000, 'L', 'Đen', 90, 'THUN-M-L-DEN', 'Active', 'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=800'),
    ('40000000-0000-0000-0000-000000000014', '30000000-0000-0000-0000-000000000012', 'Áo Sơ Mi Nam Trắng - Size M', 350000, 'M', 'Trắng', 60, 'SOMI-M-M-TRANG', 'Active', 'https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=800'),
    ('40000000-0000-0000-0000-000000000015', '30000000-0000-0000-0000-000000000013', 'Áo Polo Nam - Size L - Navy', 280000, 'L', 'Navy', 50, 'POLO-M-L-NAVY', 'Active', 'https://images.unsplash.com/photo-1625910513413-5fc42e712b4f?w=800'),
    ('40000000-0000-0000-0000-000000000016', '30000000-0000-0000-0000-000000000014', 'Áo Croptop Nữ - Size S - Hồng', 150000, 'S', 'Hồng', 45, 'CROP-F-S-HONG', 'Active', 'https://images.unsplash.com/photo-1503342217505-b0a15ec3261c?w=800'),
    ('40000000-0000-0000-0000-000000000017', '30000000-0000-0000-0000-000000000015', 'Áo Kiểu Nữ Công Sở - Size M - Trắng', 320000, 'M', 'Trắng', 35, 'KIEU-F-M-TRANG', 'Active', 'https://images.unsplash.com/photo-1564257631407-4deb1f99d992?w=800'),
    ('40000000-0000-0000-0000-000000000018', '30000000-0000-0000-0000-000000000016', 'Áo Len Dệt Kim - Size M - Xám', 420000, 'M', 'Xám', 30, 'LEN-U-M-XAM', 'Active', 'https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=800'),
    ('40000000-0000-0000-0000-000000000019', '30000000-0000-0000-0000-000000000017', 'Áo Tank Top Nữ - Size S - Đen', 130000, 'S', 'Đen', 55, 'TANK-F-S-DEN', 'Active', 'https://images.unsplash.com/photo-1503342394128-c104d54dba01?w=800'),
    ('40000000-0000-0000-0000-000000000020', '30000000-0000-0000-0000-000000000018', 'Áo Hoodie Unisex - Size L - Đen', 380000, 'L', 'Đen', 65, 'HOOD-U-L-DEN', 'Active', 'https://images.unsplash.com/photo-1556821840-3a63f95609a7?w=800'),
    ('40000000-0000-0000-0000-000000000021', '30000000-0000-0000-0000-000000000019', 'Áo Thun Oversize - Size XL - Trắng', 220000, 'XL', 'Trắng', 70, 'OVER-U-XL-TRANG', 'Active', 'https://images.unsplash.com/photo-1583744946564-b52ac1c389c8?w=800'),
    ('40000000-0000-0000-0000-000000000022', '30000000-0000-0000-0000-000000000020', 'Áo Sơ Mi Kẻ Sọc - Size M - Xanh', 300000, 'M', 'Xanh', 40, 'KESOC-U-M-XANH', 'Active', 'https://images.unsplash.com/photo-1598033129183-c4f50c736f10?w=800'),
    
    -- QUẦN ÁO BỘ variants
    ('40000000-0000-0000-0000-000000000023', '30000000-0000-0000-0000-000000000021', 'Bộ Đồ Ngủ Nữ Lụa - Size M - Hồng', 450000, 'M', 'Hồng', 25, 'DONGU-F-M-HONG', 'Active', 'https://images.unsplash.com/photo-1631947430066-48c30d57b943?w=800'),
    ('40000000-0000-0000-0000-000000000024', '30000000-0000-0000-0000-000000000022', 'Bộ Vest Nam - Size 50 - Đen', 1800000, '50', 'Đen', 15, 'VEST-M-50-DEN', 'Active', 'https://images.unsplash.com/photo-1507679799987-c73779587ccf?w=800'),
    ('40000000-0000-0000-0000-000000000025', '30000000-0000-0000-0000-000000000023', 'Bộ Thể Thao Nam - Size L - Đen', 520000, 'L', 'Đen', 40, 'BOTT-M-L-DEN', 'Active', 'https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?w=800'),
    ('40000000-0000-0000-0000-000000000026', '30000000-0000-0000-0000-000000000024', 'Bộ Đồ Ở Nhà Nữ - Size M - Hồng', 280000, 'M', 'Hồng', 50, 'DONHA-F-M-HONG', 'Active', 'https://images.unsplash.com/photo-1617137968427-85924c800a22?w=800'),
    ('40000000-0000-0000-0000-000000000027', '30000000-0000-0000-0000-000000000025', 'Bộ Jumpsuit Nữ - Size S - Đen', 480000, 'S', 'Đen', 20, 'JUMP-F-S-DEN', 'Active', 'https://images.unsplash.com/photo-1525507119028-ed4c629a60a3?w=800'),
    ('40000000-0000-0000-0000-000000000028', '30000000-0000-0000-0000-000000000026', 'Bộ Đồ Tập Yoga Nữ - Size M - Tím', 550000, 'M', 'Tím', 35, 'YOGA-F-M-TIM', 'Active', 'https://images.unsplash.com/photo-1518611012118-696072aa579a?w=800'),
    ('40000000-0000-0000-0000-000000000029', '30000000-0000-0000-0000-000000000027', 'Bộ Đầm Dự Tiệc - Size S - Đỏ', 850000, 'S', 'Đỏ', 15, 'DAM-F-S-DO', 'Active', 'https://images.unsplash.com/photo-1566174053879-31528523f8ae?w=800'),
    ('40000000-0000-0000-0000-000000000030', '30000000-0000-0000-0000-000000000028', 'Bộ Đồ Thể Thao Nữ - Size M - Xanh', 480000, 'M', 'Xanh', 30, 'BOTT-F-M-XANH', 'Active', 'https://images.unsplash.com/photo-1518459031867-a89b944bffe4?w=800'),
    ('40000000-0000-0000-0000-000000000031', '30000000-0000-0000-0000-000000000029', 'Bộ Pyjama Gia Đình - Size L - Xanh', 650000, 'L', 'Xanh', 25, 'PYJ-GD-L-XANH', 'Active', 'https://images.unsplash.com/photo-1571945153237-4929e783af4a?w=800'),
    ('40000000-0000-0000-0000-000000000032', '30000000-0000-0000-0000-000000000030', 'Bộ Đồ Công Sở Nữ - Size M - Đen', 720000, 'M', 'Đen', 20, 'CONGSO-F-M-DEN', 'Active', 'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=800'),
    
    -- ÁO KHOÁC variants
    ('40000000-0000-0000-0000-000000000033', '30000000-0000-0000-0000-000000000031', 'Áo Khoác Jean Nam - Size M - Xanh', 550000, 'M', 'Xanh', 35, 'AKJEAN-M-M-XANH', 'Active', 'https://images.unsplash.com/photo-1576995853123-5a10305d93c0?w=800'),
    ('40000000-0000-0000-0000-000000000034', '30000000-0000-0000-0000-000000000032', 'Áo Khoác Da Nam - Size L - Đen', 1200000, 'L', 'Đen', 20, 'AKDA-M-L-DEN', 'Active', 'https://images.unsplash.com/photo-1551028719-00167b16eac5?w=800'),
    ('40000000-0000-0000-0000-000000000035', '30000000-0000-0000-0000-000000000033', 'Áo Khoác Bomber - Size M - Xanh Rêu', 480000, 'M', 'Xanh Rêu', 40, 'BOMB-U-M-XREU', 'Active', 'https://images.unsplash.com/photo-1559551409-dadc959f76b8?w=800'),
    ('40000000-0000-0000-0000-000000000036', '30000000-0000-0000-0000-000000000034', 'Áo Khoác Gió Nhẹ - Size L - Đen', 350000, 'L', 'Đen', 60, 'AKGIO-U-L-DEN', 'Active', 'https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=800'),
    ('40000000-0000-0000-0000-000000000037', '30000000-0000-0000-0000-000000000035', 'Áo Khoác Blazer Nữ - Size S - Đen', 620000, 'S', 'Đen', 25, 'BLAZ-F-S-DEN', 'Active', 'https://images.unsplash.com/photo-1594938298603-c8148c4dae35?w=800'),
    ('40000000-0000-0000-0000-000000000038', '30000000-0000-0000-0000-000000000036', 'Áo Khoác Cardigan - Size M - Be', 380000, 'M', 'Be', 30, 'CARD-F-M-BE', 'Active', 'https://images.unsplash.com/photo-1434389677669-e08b4cac3105?w=800'),
    ('40000000-0000-0000-0000-000000000039', '30000000-0000-0000-0000-000000000037', 'Áo Khoác Phao - Size L - Đen', 750000, 'L', 'Đen', 35, 'PHAO-U-L-DEN', 'Active', 'https://images.unsplash.com/photo-1544923246-77307dd628b5?w=800'),
    ('40000000-0000-0000-0000-000000000040', '30000000-0000-0000-0000-000000000038', 'Áo Khoác Hoodie Zip - Size XL - Xám', 420000, 'XL', 'Xám', 50, 'HOODZ-U-XL-XAM', 'Active', 'https://images.unsplash.com/photo-1556821840-3a63f95609a7?w=800'),
    ('40000000-0000-0000-0000-000000000041', '30000000-0000-0000-0000-000000000039', 'Áo Khoác Trench Coat - Size M - Be', 980000, 'M', 'Be', 15, 'TRENCH-F-M-BE', 'Active', 'https://images.unsplash.com/photo-1539533018447-63fcce2678e3?w=800'),
    ('40000000-0000-0000-0000-000000000042', '30000000-0000-0000-0000-000000000040', 'Áo Khoác Thể Thao - Size L - Đen', 450000, 'L', 'Đen', 45, 'AKSPORT-U-L-DEN', 'Active', 'https://images.unsplash.com/photo-1591047139829-d91aecb6caea?w=800'),
    
    -- GIÀY variants
    ('40000000-0000-0000-0000-000000000043', '30000000-0000-0000-0000-000000000041', 'Giày Sneaker Nam Trắng - Size 42', 850000, '42', 'Trắng', 40, 'SNEAK-M-42-TRANG', 'Active', 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=800'),
    ('40000000-0000-0000-0000-000000000044', '30000000-0000-0000-0000-000000000041', 'Giày Sneaker Nam Trắng - Size 43', 850000, '43', 'Trắng', 35, 'SNEAK-M-43-TRANG', 'Active', 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=800'),
    ('40000000-0000-0000-0000-000000000045', '30000000-0000-0000-0000-000000000042', 'Giày Chạy Bộ Nike - Size 41 - Đen', 1500000, '41', 'Đen', 25, 'NIKE-M-41-DEN', 'Active', 'https://images.unsplash.com/photo-1460353581641-37baddab0fa2?w=800'),
    ('40000000-0000-0000-0000-000000000046', '30000000-0000-0000-0000-000000000043', 'Giày Tây Nam Da Bò - Size 42 - Đen', 1200000, '42', 'Đen', 20, 'TAY-M-42-DEN', 'Active', 'https://images.unsplash.com/photo-1614252369475-531eba835eb1?w=800'),
    ('40000000-0000-0000-0000-000000000047', '30000000-0000-0000-0000-000000000044', 'Giày Cao Gót Nữ - Size 37 - Đen', 650000, '37', 'Đen', 30, 'CAOGOT-F-37-DEN', 'Active', 'https://images.unsplash.com/photo-1543163521-1bf539c55dd2?w=800'),
    ('40000000-0000-0000-0000-000000000048', '30000000-0000-0000-0000-000000000045', 'Giày Bệt Nữ - Size 38 - Be', 420000, '38', 'Be', 35, 'BET-F-38-BE', 'Active', 'https://images.unsplash.com/photo-1551107696-a4b0c5a0d9a2?w=800'),
    ('40000000-0000-0000-0000-000000000049', '30000000-0000-0000-0000-000000000046', 'Giày Boot Cổ Cao - Size 39 - Đen', 980000, '39', 'Đen', 20, 'BOOT-F-39-DEN', 'Active', 'https://images.unsplash.com/photo-1520639888713-7851133b1ed0?w=800'),
    ('40000000-0000-0000-0000-000000000050', '30000000-0000-0000-0000-000000000047', 'Giày Lười Nam - Size 42 - Nâu', 550000, '42', 'Nâu', 25, 'LUOI-M-42-NAU', 'Active', 'https://images.unsplash.com/photo-1533867617858-e7b97e060509?w=800'),
    ('40000000-0000-0000-0000-000000000051', '30000000-0000-0000-0000-000000000048', 'Giày Thể Thao Nữ - Size 38 - Trắng', 780000, '38', 'Trắng', 40, 'THETHAO-F-38-TRANG', 'Active', 'https://images.unsplash.com/photo-1595950653106-6c9ebd614d3a?w=800'),
    ('40000000-0000-0000-0000-000000000052', '30000000-0000-0000-0000-000000000049', 'Giày Sandal Nam - Size 41 - Nâu', 380000, '41', 'Nâu', 30, 'SANDAL-M-41-NAU', 'Active', 'https://images.unsplash.com/photo-1603487742131-4160ec999306?w=800'),
    ('40000000-0000-0000-0000-000000000053', '30000000-0000-0000-0000-000000000050', 'Giày Oxford Nữ - Size 37 - Đen', 580000, '37', 'Đen', 25, 'OXFORD-F-37-DEN', 'Active', 'https://images.unsplash.com/photo-1560343090-f0409e92791a?w=800'),
    
    -- DÉP variants
    ('40000000-0000-0000-0000-000000000054', '30000000-0000-0000-0000-000000000051', 'Dép Quai Ngang Nam - Size 42 - Đen', 180000, '42', 'Đen', 60, 'QUAINGANG-M-42-DEN', 'Active', 'https://images.unsplash.com/photo-1603487742131-4160ec999306?w=800'),
    ('40000000-0000-0000-0000-000000000055', '30000000-0000-0000-0000-000000000052', 'Dép Xỏ Ngón Nữ - Size 38 - Hồng', 120000, '38', 'Hồng', 70, 'XONGON-F-38-HONG', 'Active', 'https://images.unsplash.com/photo-1535043934128-cf0b28d52f95?w=800'),
    ('40000000-0000-0000-0000-000000000056', '30000000-0000-0000-0000-000000000053', 'Dép Lê Đi Trong Nhà - Size 40 - Xám', 85000, '40', 'Xám', 100, 'DEPLE-U-40-XAM', 'Active', 'https://images.unsplash.com/photo-1604191784837-0f8da3e3d192?w=800'),
    ('40000000-0000-0000-0000-000000000057', '30000000-0000-0000-0000-000000000054', 'Dép Cao Gót Quai Trong - Size 37 - Trong', 350000, '37', 'Trong', 25, 'CAOGOTQUAI-F-37-TRONG', 'Active', 'https://images.unsplash.com/photo-1543163521-1bf539c55dd2?w=800'),
    ('40000000-0000-0000-0000-000000000058', '30000000-0000-0000-0000-000000000055', 'Dép Sandal Đế Xuồng - Size 38 - Be', 280000, '38', 'Be', 35, 'XUONG-F-38-BE', 'Active', 'https://images.unsplash.com/photo-1570733117311-d990c3816c47?w=800'),
    ('40000000-0000-0000-0000-000000000059', '30000000-0000-0000-0000-000000000056', 'Dép Da Nam Cao Cấp - Size 42 - Đen', 450000, '42', 'Đen', 20, 'DEPDA-M-42-DEN', 'Active', 'https://images.unsplash.com/photo-1533867617858-e7b97e060509?w=800'),
    ('40000000-0000-0000-0000-000000000060', '30000000-0000-0000-0000-000000000057', 'Dép Massage - Size 41 - Xanh', 150000, '41', 'Xanh', 50, 'MASSAGE-U-41-XANH', 'Active', 'https://images.unsplash.com/photo-1604191784837-0f8da3e3d192?w=800'),
    ('40000000-0000-0000-0000-000000000061', '30000000-0000-0000-0000-000000000058', 'Dép Quai Hậu Nữ - Size 38 - Đen', 220000, '38', 'Đen', 40, 'QUAIHAU-F-38-DEN', 'Active', 'https://images.unsplash.com/photo-1570733117311-d990c3816c47?w=800'),
    ('40000000-0000-0000-0000-000000000062', '30000000-0000-0000-0000-000000000059', 'Dép Đi Mưa - Size 40 - Xanh', 95000, '40', 'Xanh', 80, 'DIMUA-U-40-XANH', 'Active', 'https://images.unsplash.com/photo-1535043934128-cf0b28d52f95?w=800'),
    ('40000000-0000-0000-0000-000000000063', '30000000-0000-0000-0000-000000000060', 'Dép Nữ Đính Đá - Size 37 - Bạc', 320000, '37', 'Bạc', 30, 'DINHDA-F-37-BAC', 'Active', 'https://images.unsplash.com/photo-1543163521-1bf539c55dd2?w=800');
GO

-- ============================================
-- 7. CARTS
-- ============================================
INSERT INTO Carts (Id, UserId, Status, CreatedAt)
VALUES
    ('50000000-0000-0000-0000-000000000001', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'Active', GETDATE()),
    ('50000000-0000-0000-0000-000000000002', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Active', GETDATE());
GO

-- ============================================
-- 8. CART ITEMS
-- ============================================
INSERT INTO CartItems (Id, CartId, ProductVariantId, Quantity, CreatedAt)
VALUES
    ('60000000-0000-0000-0000-000000000001', '50000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000001', 1, GETDATE()),
    ('60000000-0000-0000-0000-000000000002', '50000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000008', 2, GETDATE()),
    ('60000000-0000-0000-0000-000000000003', '50000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000004', 1, GETDATE());
GO

-- ============================================
-- 9. ORDERS
-- ============================================
INSERT INTO orders (Id, UserId, TotalAmount, ShippingAddress, Status, CreatedAt)
VALUES
    ('70000000-0000-0000-0000-000000000001', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 25500000, '123 ABC Street, District 1, Ho Chi Minh City', 'Completed', DATEADD(day, -5, GETDATE())),
    ('70000000-0000-0000-0000-000000000002', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 800000, '456 XYZ Street, District 2, Ho Chi Minh City', 'Processing', DATEADD(day, -2, GETDATE())),
    ('70000000-0000-0000-0000-000000000003', 'ffffffff-ffff-ffff-ffff-ffffffffffff', 1200000, '789 DEF Street, District 3, Ho Chi Minh City', 'Pending', DATEADD(day, -1, GETDATE()));
GO

-- ============================================
-- 10. ORDER ITEMS
-- ============================================
INSERT INTO order_items (Id, OrderId, ProductVariantId, ProductName, Price, Quantity)
VALUES
    ('80000000-0000-0000-0000-000000000001', '70000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000001', 'iPhone 15 Pro 128GB - Titanium Blue', 25000000, 1),
    ('80000000-0000-0000-0000-000000000002', '70000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000008', 'Men Shirt - White - M', 500000, 1),
    
    ('80000000-0000-0000-0000-000000000003', '70000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000011', 'Women Jeans - Blue - Size 28', 800000, 1),
    
    ('80000000-0000-0000-0000-000000000004', '70000000-0000-0000-0000-000000000003', '40000000-0000-0000-0000-000000000014', 'Sports Shoes - White - Size 40', 1200000, 1);
GO

-- ============================================
-- 11. PAYMENTS
-- ============================================
INSERT INTO payments (Id, OrderId, Method, Amount, Status, TransactionCode, PaidAt)
VALUES
    ('90000000-0000-0000-0000-000000000001', '70000000-0000-0000-0000-000000000001', 'Credit Card', 25500000, 'Completed', 'TXN-' + FORMAT(GETDATE(), 'yyyyMMdd') + '-001', DATEADD(day, -5, GETDATE())),
    ('90000000-0000-0000-0000-000000000002', '70000000-0000-0000-0000-000000000002', 'Bank Transfer', 800000, 'Completed', 'TXN-' + FORMAT(GETDATE(), 'yyyyMMdd') + '-002', DATEADD(day, -2, GETDATE())),
    ('90000000-0000-0000-0000-000000000003', '70000000-0000-0000-0000-000000000003', 'COD', 1200000, 'Pending', 'TXN-' + FORMAT(GETDATE(), 'yyyyMMdd') + '-003', GETDATE());
GO

-- ============================================
-- 12. SHIPMENTS
-- ============================================
INSERT INTO shipments (Id, OrderId, Carrier, TrackingCode, Status, UpdatedAt)
VALUES
    ('A0000000-0000-0000-0000-000000000001', '70000000-0000-0000-0000-000000000001', 'Vietnam Post', 'VN123456789', 'Delivered', DATEADD(day, -3, GETDATE())),
    ('A0000000-0000-0000-0000-000000000002', '70000000-0000-0000-0000-000000000002', 'Giao Hang Nhanh', 'GHN987654321', 'In Transit', GETDATE()),
    ('A0000000-0000-0000-0000-000000000003', '70000000-0000-0000-0000-000000000003', 'Giao Hang Tiet Kiem', 'GHTK555666777', 'Pending', GETDATE());
GO

-- ============================================
-- 13. REVIEWS
-- ============================================
-- Note: Review entity includes ModerationReason, ModeratedAt, ModeratedBy fields
-- These are optional (nullable) fields, so they are set to NULL for approved reviews
INSERT INTO reviews (Id, UserId, ProductId, OrderItemId, Rating, Comment, Status, SpamScore, ToxicityScore, ModerationReason, ModeratedAt, ModeratedBy, CreatedAt)
VALUES
    ('B0000000-0000-0000-0000-000000000001', 'dddddddd-dddd-dddd-dddd-dddddddddddd', '30000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000001', 5, 'Great product, fast delivery!', 'Approved', 0, 0, NULL, NULL, NULL, DATEADD(day, -4, GETDATE())),
    ('B0000000-0000-0000-0000-000000000002', 'dddddddd-dddd-dddd-dddd-dddddddddddd', '30000000-0000-0000-0000-000000000004', '80000000-0000-0000-0000-000000000002', 4, 'Nice shirt, good quality', 'Approved', 0, 0, NULL, NULL, NULL, DATEADD(day, -4, GETDATE()));
GO

select * from users
select * from Carts
select * from CartItems
select * from orders where UserId = '2FF60FFD-D586-4E4B-8E2B-049F538919F3'
select * from order_items where OrderId = 'EACDC2EF-376A-4938-B45E-350911AF824F'
select * from payments
select * from Refunds
Update payments set Status = 'PAID' where Id = '9CA8DB48-5D83-4347-8E04-3E71E057DCED'
select * from products