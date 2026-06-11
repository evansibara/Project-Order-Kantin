-- =====================================================================
-- sql_setup_full.sql
-- Setup lengkap KantinDB + fitur STOK
-- Jalankan sekali di SSMS → Execute (F5)
-- =====================================================================

USE master;
GO

-- Buat database jika belum ada
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'KantinDB')
BEGIN
    CREATE DATABASE KantinDB;
    PRINT 'Database KantinDB dibuat.';
END
GO

USE KantinDB;
GO

-- -------------------------------------------------------
-- TABEL categories
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'categories')
BEGIN
    CREATE TABLE categories (
        id   INT IDENTITY(1,1) PRIMARY KEY,
        nama NVARCHAR(100) NOT NULL UNIQUE
    );
    PRINT 'Tabel categories dibuat.';
END
GO

-- -------------------------------------------------------
-- TABEL users
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'users')
BEGIN
    CREATE TABLE users (
        id       INT IDENTITY(1,1) PRIMARY KEY,
        username NVARCHAR(50)  NOT NULL UNIQUE,
        password NVARCHAR(100) NOT NULL,
        nama     NVARCHAR(100) NOT NULL,
        role     NVARCHAR(20)  NOT NULL DEFAULT 'Customer'
    );
    PRINT 'Tabel users dibuat.';
END
GO

-- -------------------------------------------------------
-- TABEL menu_items (dengan kolom stock & stock_minimum)
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'menu_items')
BEGIN
    CREATE TABLE menu_items (
        id            INT IDENTITY(1,1) PRIMARY KEY,
        nama          NVARCHAR(150) NOT NULL,
        harga         INT           NOT NULL,
        kategori      NVARCHAR(100) NOT NULL,
        gambar_url    NVARCHAR(500) NULL,
        tersedia      BIT           NOT NULL DEFAULT 1,
        stock         INT           NOT NULL DEFAULT 0,
        stock_minimum INT           NOT NULL DEFAULT 5
    );
    PRINT 'Tabel menu_items dibuat.';
END
ELSE
BEGIN
    -- Tambah kolom stock jika belum ada (upgrade dari versi lama)
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='menu_items' AND COLUMN_NAME='stock')
        ALTER TABLE menu_items ADD stock INT NOT NULL DEFAULT 0;
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='menu_items' AND COLUMN_NAME='stock_minimum')
        ALTER TABLE menu_items ADD stock_minimum INT NOT NULL DEFAULT 5;
    PRINT 'Tabel menu_items sudah ada, kolom stock dicek/ditambah.';
END
GO

-- -------------------------------------------------------
-- TABEL orders
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'orders')
BEGIN
    CREATE TABLE orders (
        id                INT IDENTITY(1,1) PRIMARY KEY,
        order_number      NVARCHAR(20)  NOT NULL,
        total             INT           NOT NULL,
        metode_pembayaran NVARCHAR(50)  NOT NULL,
        status            NVARCHAR(30)  NOT NULL DEFAULT 'SELESAI',
        created_at        DATETIME      NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Tabel orders dibuat.';
END
GO

-- -------------------------------------------------------
-- TABEL order_items
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'order_items')
BEGIN
    CREATE TABLE order_items (
        id           INT IDENTITY(1,1) PRIMARY KEY,
        order_id     INT           NOT NULL,
        menu_item_id INT           NULL,
        nama_item    NVARCHAR(150) NOT NULL,
        harga_satuan INT           NOT NULL,
        jumlah       INT           NOT NULL,
        subtotal     INT           NOT NULL,
        CONSTRAINT FK_OrderItems_Orders    FOREIGN KEY (order_id)     REFERENCES orders(id)     ON DELETE CASCADE,
        CONSTRAINT FK_OrderItems_MenuItems FOREIGN KEY (menu_item_id) REFERENCES menu_items(id) ON DELETE SET NULL
    );
    PRINT 'Tabel order_items dibuat.';
END
GO

-- -------------------------------------------------------
-- TABEL stock_log
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'stock_log')
BEGIN
    CREATE TABLE stock_log (
        id           INT IDENTITY(1,1) PRIMARY KEY,
        menu_item_id INT           NOT NULL,
        jenis        NVARCHAR(20)  NOT NULL,   -- MASUK | KELUAR | KOREKSI
        jumlah       INT           NOT NULL,
        stok_sebelum INT           NOT NULL,
        stok_sesudah INT           NOT NULL,
        keterangan   NVARCHAR(255) NULL,
        created_at   DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_StockLog_MenuItem FOREIGN KEY (menu_item_id)
            REFERENCES menu_items(id) ON DELETE CASCADE
    );
    PRINT 'Tabel stock_log dibuat.';
END
GO

-- -------------------------------------------------------
-- STORED PROCEDURE: sp_GetMenuWithStock
-- -------------------------------------------------------
IF OBJECT_ID('sp_GetMenuWithStock','P') IS NOT NULL DROP PROCEDURE sp_GetMenuWithStock;
GO
CREATE PROCEDURE sp_GetMenuWithStock AS
BEGIN
    SET NOCOUNT ON;
    SELECT id, nama, harga, kategori, gambar_url, tersedia, stock, stock_minimum,
        CASE
            WHEN stock = 0              THEN 'HABIS'
            WHEN stock <= stock_minimum THEN 'RENDAH'
            ELSE                             'CUKUP'
        END AS status_stok
    FROM menu_items ORDER BY id ASC;
END
GO

-- -------------------------------------------------------
-- STORED PROCEDURE: sp_UpdateStock
-- -------------------------------------------------------
IF OBJECT_ID('sp_UpdateStock','P') IS NOT NULL DROP PROCEDURE sp_UpdateStock;
GO
CREATE PROCEDURE sp_UpdateStock
    @menu_item_id INT,
    @jenis        NVARCHAR(20),
    @jumlah       INT,
    @keterangan   NVARCHAR(255) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @stok_lama INT;
    SELECT @stok_lama = stock FROM menu_items WHERE id = @menu_item_id;
    IF @stok_lama IS NULL BEGIN RAISERROR('Menu tidak ditemukan.',16,1); RETURN; END

    DECLARE @stok_baru INT, @delta INT;
    IF @jenis = 'MASUK'   BEGIN SET @stok_baru = @stok_lama + @jumlah;   SET @delta = @jumlah; END
    ELSE IF @jenis = 'KELUAR'
    BEGIN
        SET @stok_baru = @stok_lama - @jumlah;
        SET @delta = -@jumlah;
        IF @stok_baru < 0 BEGIN RAISERROR('Stok tidak cukup.',16,1); RETURN; END
    END
    ELSE IF @jenis = 'KOREKSI' BEGIN SET @stok_baru = @jumlah; SET @delta = @jumlah - @stok_lama; END
    ELSE BEGIN RAISERROR('Jenis tidak valid.',16,1); RETURN; END

    UPDATE menu_items SET stock = @stok_baru,
        tersedia = CASE WHEN @stok_baru > 0 THEN 1 ELSE 0 END
    WHERE id = @menu_item_id;

    INSERT INTO stock_log (menu_item_id, jenis, jumlah, stok_sebelum, stok_sesudah, keterangan)
    VALUES (@menu_item_id, @jenis, @delta, @stok_lama, @stok_baru, @keterangan);

    SELECT @stok_baru AS stok_baru;
END
GO

-- -------------------------------------------------------
-- STORED PROCEDURE: sp_GetStockLog
-- -------------------------------------------------------
IF OBJECT_ID('sp_GetStockLog','P') IS NOT NULL DROP PROCEDURE sp_GetStockLog;
GO
CREATE PROCEDURE sp_GetStockLog
    @menu_item_id INT      = NULL,
    @dari         DATETIME = NULL,
    @sampai       DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT sl.id, mi.nama AS nama_menu, sl.jenis, sl.jumlah,
           sl.stok_sebelum, sl.stok_sesudah, sl.keterangan, sl.created_at
    FROM stock_log sl
    INNER JOIN menu_items mi ON mi.id = sl.menu_item_id
    WHERE (@menu_item_id IS NULL OR sl.menu_item_id = @menu_item_id)
      AND (@dari    IS NULL OR sl.created_at >= @dari)
      AND (@sampai  IS NULL OR sl.created_at <= @sampai)
    ORDER BY sl.created_at DESC;
END
GO

-- -------------------------------------------------------
-- VIEW: v_StokRendah
-- -------------------------------------------------------
IF OBJECT_ID('v_StokRendah','V') IS NOT NULL DROP VIEW v_StokRendah;
GO
CREATE VIEW v_StokRendah AS
SELECT id, nama, kategori, stock AS stok_saat_ini,
       stock_minimum AS stok_minimum, tersedia
FROM menu_items WHERE stock <= stock_minimum;
GO

-- -------------------------------------------------------
-- DATA: categories
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM categories)
BEGIN
    INSERT INTO categories (nama) VALUES ('Makanan'),('Minuman'),('Camilan');
    PRINT 'Data kategori diisi.';
END
GO

-- -------------------------------------------------------
-- DATA: users (admin)
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM users WHERE username = 'admin')
    INSERT INTO users (username, password, nama, role) VALUES ('admin','admin123','Administrator Kantin','Admin');
IF NOT EXISTS (SELECT 1 FROM users WHERE username = 'admin2')
    INSERT INTO users (username, password, nama, role) VALUES ('admin2','admin123','Admin Kedua','Admin');
GO

-- -------------------------------------------------------
-- DATA: menu_items (sample 11 menu dengan stock awal 50)
-- -------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM menu_items)
BEGIN
    INSERT INTO menu_items (nama, harga, kategori, gambar_url, tersedia, stock, stock_minimum) VALUES
    ('Nasi Ayam Geprek', 20000, 'Makanan', '', 1, 50, 5),
    ('Nasi Goreng',      18000, 'Makanan', '', 1, 50, 5),
    ('Mie Goreng',       18000, 'Makanan', '', 1, 50, 5),
    ('Ayam Bakar',       25000, 'Makanan', '', 1, 30, 5),
    ('Teh Manis',         5000, 'Minuman', '', 1, 100, 10),
    ('Jus Jeruk',         8000, 'Minuman', '', 1, 60, 10),
    ('Es Teh',            4000, 'Minuman', '', 1, 100, 10),
    ('Kopi',              7000, 'Minuman', '', 1, 80, 10),
    ('Kerupuk',           3000, 'Camilan', '', 1, 200, 20),
    ('Tahu Goreng',       5000, 'Camilan', '', 1, 80, 10),
    ('Pisang Goreng',     6000, 'Camilan', '', 1, 60, 10);
    PRINT 'Data menu_items diisi (11 menu, stock awal 50 masing-masing).';
END
GO

-- -------------------------------------------------------
-- RINGKASAN
-- -------------------------------------------------------
SELECT nama, kategori, harga, stock AS stok, stock_minimum AS min_stok, tersedia FROM menu_items ORDER BY id;
GO

PRINT '';
PRINT '=== SETUP SELESAI ===';
PRINT 'DB     : KantinDB';
PRINT 'Login  : admin / admin123';
PRINT 'Tabel  : categories, users, menu_items, orders, order_items, stock_log';
PRINT 'SP     : sp_GetMenuWithStock, sp_UpdateStock, sp_GetStockLog';
PRINT 'View   : v_StokRendah';
GO
