-- ============================================
-- E-Commerce Platform Ass2 - Seed Data Script
-- ============================================
-- This script creates sample data for the local database
-- Run this script after running migrations
-- Password for all users: "123456" (BCrypt hash)

USE [ECommercePlatformDB2]; -- Database name from appsettings.json
GO

-- Delete old data (if needed) - Order matters due to foreign key constraints
DELETE FROM Refunds;
DELETE FROM EKycVerifications;
DELETE FROM Wallets;
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
GO

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
-- Password: "123456" (BCrypt hash: $2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC)
INSERT INTO users (Id, Name, PasswordHash, Email, RoleId, Status, EmailVerified, CreatedAt)
VALUES
    -- Admin
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Admin User', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'admin@example.com', '11111111-1111-1111-1111-111111111111', 1, 1, GETDATE()),
    
    -- Sellers
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Shop Owner 1', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'seller1@example.com', '33333333-3333-3333-3333-333333333333', 1, 1, GETDATE()),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Shop Owner 2', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'seller2@example.com', '33333333-3333-3333-3333-333333333333', 1, 1, GETDATE()),
    
    -- Customers
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'Customer 1', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'customer1@example.com', '22222222-2222-2222-2222-222222222222', 1, 1, GETDATE()),
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Customer 2', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'customer2@example.com', '22222222-2222-2222-2222-222222222222', 1, 1, GETDATE()),
    ('ffffffff-ffff-ffff-ffff-ffffffffffff', 'Customer 3', '$2a$11$tVSQZ.QyXTekMK9jnqwhWuM69Hnwiubpy1whI.uLRR4.HYRaJPwwC', 'customer3@example.com', '22222222-2222-2222-2222-222222222222', 1, 1, GETDATE());
GO

-- ============================================
-- 3. WALLETS
-- ============================================
-- Create wallets for all users
INSERT INTO Wallets (WalletId, UserId, Balance, LastChangeAmount, LastChangeType, UpdatedAt)
VALUES
    ('W0000000-0000-0000-0000-000000000001', 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 0, NULL, NULL, GETDATE()),
    ('W0000000-0000-0000-0000-000000000002', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 5000000, 5000000, 'INITIAL', GETDATE()),
    ('W0000000-0000-0000-0000-000000000003', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 3000000, 3000000, 'INITIAL', GETDATE()),
    ('W0000000-0000-0000-0000-000000000004', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 10000000, 10000000, 'INITIAL', GETDATE()),
    ('W0000000-0000-0000-0000-000000000005', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 5000000, 5000000, 'INITIAL', GETDATE()),
    ('W0000000-0000-0000-0000-000000000006', 'ffffffff-ffff-ffff-ffff-ffffffffffff', 2000000, 2000000, 'INITIAL', GETDATE());
GO

-- ============================================
-- 4. E-KYC VERIFICATIONS
-- ============================================
-- Sample E-KYC verifications (optional - some users may not have verified)
INSERT INTO EKycVerifications (Id, UserId, CccdNumber, FullName, FaceMatchScore, Liveness, Status, CreatedAt)
VALUES
    ('K0000000-0000-0000-0000-000000000001', 'dddddddd-dddd-dddd-dddd-dddddddddddd', '001234567890', 'Customer 1', 0.95, 1, 'VERIFIED', DATEADD(day, -10, GETDATE())),
    ('K0000000-0000-0000-0000-000000000002', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '001234567891', 'Customer 2', 0.92, 1, 'VERIFIED', DATEADD(day, -8, GETDATE())),
    ('K0000000-0000-0000-0000-000000000003', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '001234567892', 'Shop Owner 1', 0.98, 1, 'VERIFIED', DATEADD(day, -15, GETDATE()));
GO

-- ============================================
-- 5. CATEGORIES
-- ============================================
INSERT INTO categories (Id, Name, Status)
VALUES
    ('10000000-0000-0000-0000-000000000001', 'Electronics', 'Active'),
    ('10000000-0000-0000-0000-000000000002', 'Fashion', 'Active'),
    ('10000000-0000-0000-0000-000000000003', 'Home & Kitchen', 'Active'),
    ('10000000-0000-0000-0000-000000000004', 'Books', 'Active'),
    ('10000000-0000-0000-0000-000000000005', 'Sports', 'Active'),
    ('10000000-0000-0000-0000-000000000006', 'Beauty & Personal Care', 'Active'),
    ('10000000-0000-0000-0000-000000000007', 'Toys & Games', 'Active');
GO

-- ============================================
-- 6. SHOPS
-- ============================================
INSERT INTO shops (Id, UserId, ShopName, Description, Status, CreatedAt)
VALUES
    ('20000000-0000-0000-0000-000000000001', 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Tech Store', 'Trusted electronics store with latest gadgets', 'Active', GETDATE()),
    ('20000000-0000-0000-0000-000000000002', 'cccccccc-cccc-cccc-cccc-cccccccccccc', 'Fashion Shop', 'Men and women fashion - trendy and affordable', 'Active', GETDATE());
GO

-- ============================================
-- 7. PRODUCTS
-- ============================================
INSERT INTO products (Id, ShopId, CategoryId, Name, Description, BasePrice, Status, AvgRating, ImageUrl, CreatedAt)
VALUES
    -- Electronics
    ('30000000-0000-0000-0000-000000000001', '20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'iPhone 15 Pro', 'Premium smartphone with A17 Pro chip', 25000000, 'Active', 4.5, 'https://example.com/iphone15.jpg', GETDATE()),
    ('30000000-0000-0000-0000-000000000002', '20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'Samsung Galaxy S24', 'Android flagship phone with AI features', 22000000, 'Active', 4.3, 'https://example.com/galaxy-s24.jpg', GETDATE()),
    ('30000000-0000-0000-0000-000000000003', '20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'MacBook Pro M3', 'Professional laptop for creators', 45000000, 'Active', 4.8, 'https://example.com/macbook-pro.jpg', GETDATE()),
    ('30000000-0000-0000-0000-000000000007', '20000000-0000-0000-0000-000000000001', '10000000-0000-0000-0000-000000000001', 'AirPods Pro 2', 'Wireless earbuds with noise cancellation', 6500000, 'Active', 4.6, 'https://example.com/airpods-pro.jpg', GETDATE()),
    
    -- Fashion
    ('30000000-0000-0000-0000-000000000004', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Men Shirt', 'Premium office shirt - comfortable fit', 500000, 'Active', 4.2, 'https://example.com/ao-so-mi.jpg', GETDATE()),
    ('30000000-0000-0000-0000-000000000005', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Women Jeans', 'Fashion jeans - stretchy and durable', 800000, 'Active', 4.0, 'https://example.com/quan-jean.jpg', GETDATE()),
    ('30000000-0000-0000-0000-000000000006', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Sports Shoes', 'Multi-purpose running shoes', 1200000, 'Active', 4.5, 'https://example.com/giay-the-thao.jpg', GETDATE()),
    ('30000000-0000-0000-0000-000000000008', '20000000-0000-0000-0000-000000000002', '10000000-0000-0000-0000-000000000002', 'Leather Jacket', 'Classic leather jacket for men', 2500000, 'Active', 4.7, 'https://example.com/ao-khoac-da.jpg', GETDATE());
GO

-- ============================================
-- 8. PRODUCT VARIANTS
-- ============================================
INSERT INTO product_variants (Id, ProductId, VariantName, Price, Size, Color, Stock, Sku, Status, ImageUrl)
VALUES
    -- iPhone 15 Pro variants
    ('40000000-0000-0000-0000-000000000001', '30000000-0000-0000-0000-000000000001', 'iPhone 15 Pro 128GB - Titanium Blue', 25000000, '', 'Blue', 50, 'IP15P-128-BLUE', 'Active', 'https://example.com/iphone15-blue.jpg'),
    ('40000000-0000-0000-0000-000000000002', '30000000-0000-0000-0000-000000000001', 'iPhone 15 Pro 256GB - Titanium Blue', 28000000, '', 'Blue', 30, 'IP15P-256-BLUE', 'Active', 'https://example.com/iphone15-blue.jpg'),
    ('40000000-0000-0000-0000-000000000003', '30000000-0000-0000-0000-000000000001', 'iPhone 15 Pro 128GB - Titanium Black', 25000000, '', 'Black', 40, 'IP15P-128-BLACK', 'Active', 'https://example.com/iphone15-black.jpg'),
    
    -- Samsung Galaxy S24 variants
    ('40000000-0000-0000-0000-000000000004', '30000000-0000-0000-0000-000000000002', 'Galaxy S24 256GB - Phantom Black', 22000000, '', 'Black', 35, 'GS24-256-BLACK', 'Active', 'https://example.com/galaxy-s24-black.jpg'),
    ('40000000-0000-0000-0000-000000000005', '30000000-0000-0000-0000-000000000002', 'Galaxy S24 512GB - Phantom Black', 25000000, '', 'Black', 20, 'GS24-512-BLACK', 'Active', 'https://example.com/galaxy-s24-black.jpg'),
    
    -- MacBook Pro variants
    ('40000000-0000-0000-0000-000000000006', '30000000-0000-0000-0000-000000000003', 'MacBook Pro M3 14" 512GB', 45000000, '14 inch', 'Space Gray', 15, 'MBP-M3-14-512', 'Active', 'https://example.com/macbook-pro.jpg'),
    ('40000000-0000-0000-0000-000000000007', '30000000-0000-0000-0000-000000000003', 'MacBook Pro M3 16" 1TB', 55000000, '16 inch', 'Space Gray', 10, 'MBP-M3-16-1TB', 'Active', 'https://example.com/macbook-pro.jpg'),
    
    -- AirPods Pro variants
    ('40000000-0000-0000-0000-000000000016', '30000000-0000-0000-0000-000000000007', 'AirPods Pro 2 - White', 6500000, '', 'White', 100, 'APP2-WHITE', 'Active', 'https://example.com/airpods-pro.jpg'),
    
    -- Men Shirt variants
    ('40000000-0000-0000-0000-000000000008', '30000000-0000-0000-0000-000000000004', 'Men Shirt - White - M', 500000, 'M', 'White', 100, 'ASM-WHITE-M', 'Active', 'https://example.com/ao-so-mi-white.jpg'),
    ('40000000-0000-0000-0000-000000000009', '30000000-0000-0000-0000-000000000004', 'Men Shirt - White - L', 500000, 'L', 'White', 80, 'ASM-WHITE-L', 'Active', 'https://example.com/ao-so-mi-white.jpg'),
    ('40000000-0000-0000-0000-000000000010', '30000000-0000-0000-0000-000000000004', 'Men Shirt - Blue - M', 500000, 'M', 'Blue', 90, 'ASM-BLUE-M', 'Active', 'https://example.com/ao-so-mi-blue.jpg'),
    
    -- Women Jeans variants
    ('40000000-0000-0000-0000-000000000011', '30000000-0000-0000-0000-000000000005', 'Women Jeans - Blue - Size 28', 800000, '28', 'Blue', 60, 'QJ-BLUE-28', 'Active', 'https://example.com/quan-jean-blue.jpg'),
    ('40000000-0000-0000-0000-000000000012', '30000000-0000-0000-0000-000000000005', 'Women Jeans - Blue - Size 30', 800000, '30', 'Blue', 70, 'QJ-BLUE-30', 'Active', 'https://example.com/quan-jean-blue.jpg'),
    ('40000000-0000-0000-0000-000000000013', '30000000-0000-0000-0000-000000000005', 'Women Jeans - Black - Size 28', 800000, '28', 'Black', 50, 'QJ-BLACK-28', 'Active', 'https://example.com/quan-jean-black.jpg'),
    
    -- Sports Shoes variants
    ('40000000-0000-0000-0000-000000000014', '30000000-0000-0000-0000-000000000006', 'Sports Shoes - White - Size 40', 1200000, '40', 'White', 45, 'GT-WHITE-40', 'Active', 'https://example.com/giay-white.jpg'),
    ('40000000-0000-0000-0000-000000000015', '30000000-0000-0000-0000-000000000006', 'Sports Shoes - Black - Size 42', 1200000, '42', 'Black', 55, 'GT-BLACK-42', 'Active', 'https://example.com/giay-black.jpg'),
    
    -- Leather Jacket variants
    ('40000000-0000-0000-0000-000000000017', '30000000-0000-0000-0000-000000000008', 'Leather Jacket - Black - M', 2500000, 'M', 'Black', 25, 'AKD-BLACK-M', 'Active', 'https://example.com/ao-khoac-da-black.jpg'),
    ('40000000-0000-0000-0000-000000000018', '30000000-0000-0000-0000-000000000008', 'Leather Jacket - Black - L', 2500000, 'L', 'Black', 20, 'AKD-BLACK-L', 'Active', 'https://example.com/ao-khoac-da-black.jpg');
GO

-- ============================================
-- 9. CARTS
-- ============================================
INSERT INTO Carts (Id, UserId, Status, CreatedAt)
VALUES
    ('50000000-0000-0000-0000-000000000001', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 'Active', GETDATE()),
    ('50000000-0000-0000-0000-000000000002', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Active', GETDATE());
GO

-- ============================================
-- 10. CART ITEMS
-- ============================================
INSERT INTO CartItems (Id, CartId, ProductVariantId, Quantity, CreatedAt)
VALUES
    ('60000000-0000-0000-0000-000000000001', '50000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000001', 1, GETDATE()),
    ('60000000-0000-0000-0000-000000000002', '50000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000008', 2, GETDATE()),
    ('60000000-0000-0000-0000-000000000003', '50000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000004', 1, GETDATE());
GO

-- ============================================
-- 11. ORDERS
-- ============================================
INSERT INTO orders (Id, UserId, TotalAmount, ShippingAddress, Status, CreatedAt)
VALUES
    ('70000000-0000-0000-0000-000000000001', 'dddddddd-dddd-dddd-dddd-dddddddddddd', 25500000, '123 ABC Street, District 1, Ho Chi Minh City', 'Completed', DATEADD(day, -5, GETDATE())),
    ('70000000-0000-0000-0000-000000000002', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 800000, '456 XYZ Street, District 2, Ho Chi Minh City', 'Processing', DATEADD(day, -2, GETDATE())),
    ('70000000-0000-0000-0000-000000000003', 'ffffffff-ffff-ffff-ffff-ffffffffffff', 1200000, '789 DEF Street, District 3, Ho Chi Minh City', 'Pending', DATEADD(day, -1, GETDATE()));
GO

-- ============================================
-- 12. ORDER ITEMS
-- ============================================
INSERT INTO order_items (Id, OrderId, ProductVariantId, ProductName, Price, Quantity)
VALUES
    ('80000000-0000-0000-0000-000000000001', '70000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000001', 'iPhone 15 Pro 128GB - Titanium Blue', 25000000, 1),
    ('80000000-0000-0000-0000-000000000002', '70000000-0000-0000-0000-000000000001', '40000000-0000-0000-0000-000000000008', 'Men Shirt - White - M', 500000, 1),
    
    ('80000000-0000-0000-0000-000000000003', '70000000-0000-0000-0000-000000000002', '40000000-0000-0000-0000-000000000011', 'Women Jeans - Blue - Size 28', 800000, 1),
    
    ('80000000-0000-0000-0000-000000000004', '70000000-0000-0000-0000-000000000003', '40000000-0000-0000-0000-000000000014', 'Sports Shoes - White - Size 40', 1200000, 1);
GO

-- ============================================
-- 13. PAYMENTS
-- ============================================
INSERT INTO payments (Id, OrderId, Method, Amount, Status, TransactionCode, PaidAt)
VALUES
    ('90000000-0000-0000-0000-000000000001', '70000000-0000-0000-0000-000000000001', 'Credit Card', 25500000, 'Completed', 'TXN-' + FORMAT(GETDATE(), 'yyyyMMdd') + '-001', DATEADD(day, -5, GETDATE())),
    ('90000000-0000-0000-0000-000000000002', '70000000-0000-0000-0000-000000000002', 'Wallet', 800000, 'Completed', 'TXN-' + FORMAT(GETDATE(), 'yyyyMMdd') + '-002', DATEADD(day, -2, GETDATE())),
    ('90000000-0000-0000-0000-000000000003', '70000000-0000-0000-0000-000000000003', 'Momo', 1200000, 'Pending', 'TXN-' + FORMAT(GETDATE(), 'yyyyMMdd') + '-003', NULL);
GO

-- ============================================
-- 14. SHIPMENTS
-- ============================================
INSERT INTO shipments (Id, OrderId, Carrier, TrackingCode, Status, UpdatedAt)
VALUES
    ('A0000000-0000-0000-0000-000000000001', '70000000-0000-0000-0000-000000000001', 'Vietnam Post', 'VN123456789', 'Delivered', DATEADD(day, -3, GETDATE())),
    ('A0000000-0000-0000-0000-000000000002', '70000000-0000-0000-0000-000000000002', 'Giao Hang Nhanh', 'GHN987654321', 'In Transit', GETDATE()),
    ('A0000000-0000-0000-0000-000000000003', '70000000-0000-0000-0000-000000000003', 'Giao Hang Tiet Kiem', 'GHTK555666777', 'Pending', GETDATE());
GO

-- ============================================
-- 15. REVIEWS
-- ============================================
INSERT INTO reviews (Id, UserId, ProductId, OrderItemId, Rating, Comment, Status, SpamScore, ToxicityScore, ModerationReason, ModeratedAt, ModeratedBy, CreatedAt)
VALUES
    ('B0000000-0000-0000-0000-000000000001', 'dddddddd-dddd-dddd-dddd-dddddddddddd', '30000000-0000-0000-0000-000000000001', '80000000-0000-0000-0000-000000000001', 5, 'Great product, fast delivery! Highly recommend!', 'Approved', 0, 0, NULL, NULL, NULL, DATEADD(day, -4, GETDATE())),
    ('B0000000-0000-0000-0000-000000000002', 'dddddddd-dddd-dddd-dddd-dddddddddddd', '30000000-0000-0000-0000-000000000004', '80000000-0000-0000-0000-000000000002', 4, 'Nice shirt, good quality and fits well', 'Approved', 0, 0, NULL, NULL, NULL, DATEADD(day, -4, GETDATE())),
    ('B0000000-0000-0000-0000-000000000003', 'eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', '30000000-0000-0000-0000-000000000005', '80000000-0000-0000-0000-000000000003', 5, 'Perfect jeans, very comfortable!', 'Approved', 0, 0, NULL, NULL, NULL, DATEADD(day, -1, GETDATE()));
GO

-- ============================================
-- VERIFICATION QUERIES
-- ============================================
-- Uncomment to verify data insertion
/*
SELECT 'Roles' AS TableName, COUNT(*) AS Count FROM roles
UNION ALL
SELECT 'Users', COUNT(*) FROM users
UNION ALL
SELECT 'Wallets', COUNT(*) FROM Wallets
UNION ALL
SELECT 'EKycVerifications', COUNT(*) FROM EKycVerifications
UNION ALL
SELECT 'Categories', COUNT(*) FROM categories
UNION ALL
SELECT 'Shops', COUNT(*) FROM shops
UNION ALL
SELECT 'Products', COUNT(*) FROM products
UNION ALL
SELECT 'ProductVariants', COUNT(*) FROM product_variants
UNION ALL
SELECT 'Carts', COUNT(*) FROM Carts
UNION ALL
SELECT 'CartItems', COUNT(*) FROM CartItems
UNION ALL
SELECT 'Orders', COUNT(*) FROM orders
UNION ALL
SELECT 'OrderItems', COUNT(*) FROM order_items
UNION ALL
SELECT 'Payments', COUNT(*) FROM payments
UNION ALL
SELECT 'Shipments', COUNT(*) FROM shipments
UNION ALL
SELECT 'Reviews', COUNT(*) FROM reviews;
*/

PRINT 'Seed data insertion completed successfully!';
PRINT 'All users password: 123456';
PRINT 'Admin: admin@example.com';
PRINT 'Seller 1: seller1@example.com';
PRINT 'Seller 2: seller2@example.com';
PRINT 'Customer 1: customer1@example.com';
PRINT 'Customer 2: customer2@example.com';
PRINT 'Customer 3: customer3@example.com';
GO
