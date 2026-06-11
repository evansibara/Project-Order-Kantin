using System;
using System.Collections.Generic;

namespace Project_Order_Kantin
{
    // =================================================================
    // WEEK 3  - Class & Object      : Mendefinisikan kelas-kelas model
    // WEEK 4  - Constructor, Getter/Setter : Setiap kelas punya constructor
    //           dan auto-property { get; set; }
    // WEEK 5  - Function & Procedure: Method seperti HargaFormatted(),
    //           GetWelcomeMessage(), IsStokHabis() dll.
    // WEEK 7  - Inheritance         : Admin dan Customer mewarisi User
    // =================================================================

    // ---- Base class User (WEEK 7: Inheritance – parent class) ----
    // 'User' adalah kelas induk yang jadi fondasi sistem.
    // Kelas Admin dan Customer MEWARISI User, artinya mereka otomatis
    // punya semua properti dan method yang ada di sini tanpa perlu
    // mendefinisikannya ulang.
    public class User
    {
        // AUTO-PROPERTY: cara ringkas C# untuk mendefinisikan properti.
        // { get; set; } artinya properti bisa dibaca dan diubah dari luar kelas.
        // Di balik layar, compiler otomatis bikin private field + getter + setter.
        // Contoh: 'public int Id { get; set; }' = field _id + property Id
        public int    Id       { get; set; }
        public string Username { get; set; }
        public string Nama     { get; set; }

        // Role menentukan hak akses pengguna di sistem: "Admin" atau "Customer".
        // Dipakai oleh SessionManager dan form-form untuk memutuskan tampilan apa
        // yang boleh diakses.
        public string Role     { get; set; }

        // CONSTRUCTOR default (parameterless) – wajib ada karena dibutuhkan
        // beberapa mekanisme internal WinForms/serialisasi.
        public User() { }

        // CONSTRUCTOR dengan parameter – dipakai untuk membuat objek User
        // sekaligus mengisi semua propertinya dalam satu baris.
        // Contoh: new User(1, "budi", "Budi Santoso", "Admin")
        public User(int id, string username, string nama, string role)
        {
            Id = id; Username = username; Nama = nama; Role = role;
        }

        // WEEK 7: method virtual – penanda bahwa method ini BOLEH diganti
        // oleh kelas turunan (Admin/Customer) dengan implementasi yang berbeda.
        // Keyword 'virtual' berarti "ada versi default, tapi boleh dioverride".
        public virtual string GetWelcomeMessage()
        {
            return "Selamat datang, " + Nama;
        }
    }

    // ---- Admin : User (WEEK 7 – Inheritance) ----
    // Tanda titik dua ':' berarti "mewarisi dari".
    // Admin otomatis punya Id, Username, Nama, Role dari User.
    // Yang membedakan Admin adalah sapaan sambutan yang lebih spesifik
    // dan Role yang selalu "Admin" (di-hardcode di constructor).
    public class Admin : User
    {
        // Constructor Admin memanggil constructor User dulu lewat ':base(...)'.
        // Ini wajib karena User tidak punya parameterless constructor sendiri
        // (constructor User butuh parameter). ':base()' = "panggil constructor
        // milik parent (User) dengan argumen yang diberikan".
        public Admin() : base() { Role = "Admin"; }

        public Admin(int id, string username, string nama)
            : base(id, username, nama, "Admin") { }

        // WEEK 7: override – mengganti implementasi GetWelcomeMessage milik User.
        // Karena method di User pakai 'virtual', kelas turunan BISA menggantinya
        // dengan 'override'. Tanpa 'override', versi User yang dipakai.
        public override string GetWelcomeMessage()
        {
            return "Selamat datang Admin " + Nama;
        }
    }

    // ---- Customer : User (WEEK 7 – Inheritance) ----
    // Customer punya properti tambahan NomorMeja yang tidak dimiliki oleh
    // User biasa maupun Admin. Ini contoh Inheritance sekaligus Extension –
    // mewarisi lalu menambah fitur baru.
    public class Customer : User
    {
        // Properti khusus Customer: nomor meja tempat pelanggan duduk.
        // Ditampilkan di header Menu_Screen supaya pelanggan tahu pesanannya
        // akan dikirim ke meja yang benar.
        public string NomorMeja { get; set; }

        public Customer() : base() { Role = "Customer"; }

        public Customer(int id, string username, string nama, string nomorMeja)
            : base(id, username, nama, "Customer")
        {
            NomorMeja = nomorMeja;
        }

        // WEEK 7: override – sapaan khusus customer menyertakan nomor meja
        // supaya lebih personal dan membantu pelanggan mengonfirmasi mejanya.
        public override string GetWelcomeMessage()
        {
            return "Halo, " + Nama + " (Meja " + NomorMeja + ")";
        }
    }

    // ---- Category (WEEK 3 – Class & Object) ----
    // Merepresentasikan satu kategori menu, contoh: Makanan, Minuman, Snack.
    // Dipakai oleh ComboBox filter di Menu_Screen dan form MenuEditor.
    public class Category
    {
        public int    Id   { get; set; }
        public string Nama { get; set; }

        public Category() { }
        public Category(int id, string nama) { Id = id; Nama = nama; }

        // Override ToString() supaya ketika objek Category masuk ke ComboBox,
        // yang tampil adalah namanya, bukan nama tipe kelas ("Category").
        public override string ToString() => Nama;
    }

    // ---- MenuItemModel (WEEK 3, 4) ----
    // Satu objek = satu baris di tabel menu_items database.
    // Dipakai hampir di semua tempat: kartu menu, keranjang, halaman admin.
    // Sengaja dibuat terpisah dari MenuItemWithStock agar kelas ini tetap
    // ringan (tidak bawa data stok yang tidak selalu dibutuhkan).
    public class MenuItemModel
    {
        public int    Id        { get; set; }
        public string Nama      { get; set; }
        public int    Harga     { get; set; }
        public string Kategori  { get; set; }
        public string GambarUrl { get; set; }

        // Tersedia = true/false, diset oleh Admin lewat form AdminDashboard.
        // Jika false, menu tidak muncul di kartu pelanggan meskipun stok ada.
        public bool   Tersedia  { get; set; }

        // Stock diambil dari kolom 'stock' di database.
        // Nilai 0 atau negatif = stok habis → tombol "Tambah" dinonaktifkan.
        // Defaultnya 0 supaya data lama (sebelum fitur stok ada) tidak error.
        public int    Stock     { get; set; }

        public MenuItemModel() { }

        // CONSTRUCTOR lengkap – parameter stock punya nilai default 0
        // agar kode lama yang belum memakai parameter stock tetap bisa jalan.
        public MenuItemModel(int id, string nama, int harga, string kategori,
                             string gambarUrl, bool tersedia, int stock = 0)
        {
            Id = id; Nama = nama; Harga = harga;
            Kategori = kategori; GambarUrl = gambarUrl;
            Tersedia = tersedia; Stock = stock;
        }

        // WEEK 5: FUNCTION (return value) – memformat harga jadi string rupiah.
        // Contoh: Harga = 15000 → "Rp 15.000"
        // Format "N0" = angka dengan pemisah ribuan, tanpa desimal.
        public string HargaFormatted() => "Rp " + Harga.ToString("N0");

        // FUNCTION: mengecek apakah stok habis (kurang dari atau sama dengan 0).
        // Dipakai oleh BuildMenuCard() untuk menonaktifkan tombol tambah.
        public bool IsStokHabis() => Stock <= 0;

        // FUNCTION: menentukan teks label stok untuk ditampilkan di kartu menu.
        // SELECTION (if-else bertingkat): logika berbeda untuk tiap kondisi stok.
        // Stok cukup (>5) → tidak perlu label → kembalikan string kosong.
        public string GetStatusStokLabel()
        {
            if (Stock <= 0)  return "Stok Habis";
            if (Stock <= 5)  return $"Stok: {Stock} (Terbatas)";
            return "";       // Stok aman, tidak perlu peringatan di kartu
        }
    }

    // ---- MenuItemWithStock (versi lengkap dengan info stok minimum) ----
    // Dipakai khusus oleh StockManager dan AdminDashboard (tab menu).
    // Dipisah dari MenuItemModel supaya kelas dasar tetap sederhana,
    // sementara form admin yang butuh detail stok memakai versi ini.
    public class MenuItemWithStock
    {
        public int    Id           { get; set; }
        public string Nama         { get; set; }
        public int    Harga        { get; set; }
        public string Kategori     { get; set; }
        public string GambarUrl    { get; set; }
        public bool   Tersedia     { get; set; }
        public int    Stock        { get; set; }

        // StockMinimum = ambang batas peringatan. Jika Stock <= StockMinimum,
        // sistem akan menampilkan peringatan stok rendah di AdminDashboard.
        public int    StockMinimum { get; set; }

        // StatusStok: "CUKUP" | "RENDAH" | "HABIS" – dihitung oleh stored procedure
        // di SQL Server, bukan di C#, agar logikanya terpusat di satu tempat.
        public string StatusStok   { get; set; }

        // Read-only computed properties (getter only, tidak bisa di-set dari luar).
        // Dipakai untuk tampilan di DataGridView tanpa perlu kolom formula terpisah.
        public string HargaFormatted  => "Rp " + Harga.ToString("N0");
        public string TersediaDisplay => Tersedia ? "✔" : "✘";
        public string StokDisplay     => Stock + " porsi";
    }

    // ---- StockLogEntry (riwayat perubahan stok) ----
    // Satu record = satu kejadian perubahan stok yang disimpan di tabel stock_log.
    // Ditampilkan di tab "Riwayat" pada form StockManager.
    public class StockLogEntry
    {
        public int      Id          { get; set; }
        public string   NamaMenu    { get; set; }

        // Jenis perubahan: "MASUK" (stok bertambah dari pengisian),
        // "KELUAR" (stok berkurang karena ada order), "KOREKSI" (admin set langsung).
        public string   Jenis       { get; set; }
        public int      Jumlah      { get; set; }
        public int      StokSebelum { get; set; }
        public int      StokSesudah { get; set; }
        public string   Keterangan  { get; set; }
        public DateTime CreatedAt   { get; set; }

        // Format tanggal yang lebih mudah dibaca manusia: "15/06/2026 14:30"
        public string TanggalDisplay => CreatedAt.ToString("dd/MM/yyyy HH:mm");

        // Perubahan jumlah ditampilkan dengan tanda +/- agar lebih jelas
        // arah perubahannya. Contoh: +10 (stok masuk) atau -3 (stok keluar).
        public string JumlahDisplay  => (Jumlah >= 0 ? "+" : "") + Jumlah;
    }

    // ---- CartLine (WEEK 6 – Array & List/Dictionary) ----
    // Satu objek CartLine = satu baris di keranjang belanja.
    // Disimpan sebagai value di dalam Dictionary<int, CartLine> milik CartManager.
    // Key Dictionary-nya adalah MenuItemId, sehingga cari item tertentu O(1).
    public class CartLine
    {
        public int    MenuItemId  { get; set; }
        public string Nama        { get; set; }
        public int    HargaSatuan { get; set; }
        public int    Jumlah      { get; set; }

        // WEEK 5: FUNCTION – Subtotal dihitung otomatis, bukan disimpan terpisah.
        // Expression body (=>) berarti "nilai ini selalu HargaSatuan × Jumlah",
        // jadi tidak perlu update manual setiap kali Jumlah berubah.
        public int    Subtotal    => HargaSatuan * Jumlah;

        public CartLine() { }

        public CartLine(int menuItemId, string nama, int hargaSatuan, int jumlah)
        {
            MenuItemId = menuItemId; Nama = nama;
            HargaSatuan = hargaSatuan; Jumlah = jumlah;
        }
    }

    // ---- Order (satu transaksi yang sudah selesai) ----
    // Satu record = satu kali proses pembayaran yang berhasil.
    // Disimpan di tabel 'orders' oleh DatabaseHelper.SaveOrder().
    public class Order
    {
        public int      Id                { get; set; }

        // OrderNumber = nomor cantik seperti "#1001", "#1002", dst.
        // Dihitung dari 1000 + Id database agar nomornya tidak mulai dari 1.
        public string   OrderNumber       { get; set; }
        public int      Total             { get; set; }
        public string   MetodePembayaran  { get; set; }

        // Status selalu "SELESAI" dalam versi ini karena tidak ada fitur
        // antrian atau konfirmasi kasir.
        public string   Status            { get; set; }
        public DateTime CreatedAt         { get; set; }
    }

    // ---- OrderItem (detail item dalam satu Order) ----
    // Satu Order bisa punya banyak OrderItem (relasi one-to-many).
    // Disimpan di tabel 'order_items'.
    // Kolom NamaItem sengaja disalin dari menu_items saat order dibuat,
    // supaya riwayat tetap terjaga walau nama menu di-update nanti.
    public class OrderItem
    {
        public int    Id          { get; set; }
        public int    OrderId     { get; set; }
        public int    MenuItemId  { get; set; }
        public string NamaItem    { get; set; }
        public int    HargaSatuan { get; set; }
        public int    Jumlah      { get; set; }
        public int    Subtotal    { get; set; }
    }
}
