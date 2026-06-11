# 🍽️ Project Order Kantin

Aplikasi pemesanan makanan berbasis desktop (Windows Forms) yang dibangun dengan **C# .NET Framework 4.7.2** dan **SQL Server** sebagai database. Aplikasi ini dirancang untuk memudahkan proses pemesanan di kantin — mulai dari manajemen menu, keranjang belanja, pembayaran, hingga laporan penjualan.

---

## 📋 Daftar Isi

- [Fitur Utama](#-fitur-utama)
- [Teknologi yang Digunakan](#-teknologi-yang-digunakan)
- [Struktur Proyek](#-struktur-proyek)
- [Persyaratan Sistem](#-persyaratan-sistem)
- [Instalasi & Setup](#-instalasi--setup)
- [Konfigurasi Database](#-konfigurasi-database)
- [Cara Menjalankan](#-cara-menjalankan)
- [Akun Default](#-akun-default)

---

## ✨ Fitur Utama

| Fitur | Keterangan |
|---|---|
| 🔐 **Login Multi-Role** | Dukungan role Admin dan Customer dengan hak akses berbeda |
| 🍱 **Menu Screen** | Tampilkan daftar menu kantin dengan gambar dan harga |
| 🛒 **Cart / Keranjang** | Kelola item pesanan sebelum checkout |
| 💳 **Payment** | Proses pembayaran dengan kalkulasi total otomatis |
| 📊 **Admin Dashboard** | Kelola seluruh data dari satu tempat |
| 📝 **Menu Editor** | Tambah, edit, dan hapus menu (Admin) |
| 📦 **Stock Manager** | Pantau dan perbarui stok bahan/menu |
| 📈 **Laporan Penjualan** | Generate laporan penjualan dengan Crystal Reports |
| 🖼️ **Upload Gambar** | Dukungan gambar menu via Supabase Storage |

---

## 🛠️ Teknologi yang Digunakan

- **Bahasa:** C# (.NET Framework 4.7.2)
- **UI Framework:** Windows Forms (WinForms)
- **Database:** Microsoft SQL Server (SQL Server Express)
- **ORM/Data Access:** ADO.NET (`SqlConnection`, `SqlCommand`, `SqlTransaction`)
- **Reporting:** SAP Crystal Reports
- **Cloud Storage:** Supabase (untuk gambar menu)
- **IDE:** Visual Studio 2019 / 2022

---

## 📁 Struktur Proyek

```
Final Project Order Kantin/
├── Project Order Kantin.sln          # Solution file Visual Studio
├── Setup Database SQL Server.sql     # Script setup database lengkap
└── Project Order Kantin/
    ├── Program.cs                    # Entry point aplikasi
    ├── App.config                    # Konfigurasi koneksi database
    ├── Models.cs                     # Model data (User, Admin, Customer, Menu, dll.)
    ├── DatabaseHelper.cs             # Semua operasi database (ADO.NET)
    ├── SessionManager.cs             # Manajemen sesi pengguna yang login
    ├── CartManager.cs                # Logika keranjang belanja
    ├── FileStreamHelper.cs           # Helper untuk operasi file/stream
    ├── SupabaseImageHelper.cs        # Integrasi Supabase untuk gambar
    ├── Form1.cs / .Designer.cs       # Form splash / koneksi awal
    ├── LoginAdmin.cs                 # Form login
    ├── Menu Screen.cs / .Designer.cs # Layar utama menu makanan
    ├── Cart Screen.cs / .Designer.cs # Layar keranjang belanja
    ├── Payment.cs / .Designer.cs     # Layar pembayaran
    ├── AdminDashboard.cs             # Dashboard administrator
    ├── MenuEditor.cs                 # Form edit/tambah menu
    ├── StockManager.cs               # Manajemen stok
    ├── LaporanPenjualan.cs           # Laporan penjualan
    └── LaporanPenjualan.rpt          # Template Crystal Reports
```

---

## 💻 Persyaratan Sistem

Pastikan perangkat kamu sudah terinstal:

- **Windows** 10 / 11
- **Visual Studio** 2019 atau 2022 (dengan workload *.NET desktop development*)
- **SQL Server Express** (gratis) — [Download di sini](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- **SQL Server Management Studio (SSMS)** — [Download di sini](https://aka.ms/ssmsfullsetup)
- **SAP Crystal Reports** for Visual Studio — [Download di sini](https://www.sap.com/indonesia/products/technology-platform/crystal-reports.html)
- **.NET Framework 4.7.2** (biasanya sudah ada di Windows 10+)

---

## ⚙️ Instalasi & Setup

### 1. Clone atau Download Proyek

```bash
[https://github.com/evansibara/Project-Order-Kantin.git]
```

Atau download ZIP lalu ekstrak ke folder pilihan kamu.

### 2. Setup Database

1. Buka **SQL Server Management Studio (SSMS)**
2. Hubungkan ke server: `.\SQLEXPRESS`
3. Klik **New Query**
4. Buka file `Setup Database SQL Server.sql` dari folder proyek
5. Klik **Execute** (F5) — script akan otomatis membuat database `KantinDB` beserta semua tabelnya

### 3. Buka Proyek di Visual Studio

1. Buka file `Project Order Kantin.sln`
2. Visual Studio akan memuat seluruh proyek secara otomatis
3. Tunggu proses *restore packages* selesai

### 4. Verifikasi Koneksi Database

Buka file `App.config` dan pastikan connection string sudah sesuai dengan instalasi SQL Server kamu:

```xml
<add name="KantinDB"
     connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=KantinDB;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=30"
     providerName="System.Data.SqlClient" />
```

> Ganti `.\SQLEXPRESS` dengan nama instance SQL Server kamu jika berbeda (misal: `localhost` atau `DESKTOP-XXX\SQLEXPRESS`).

---

## 🗄️ Konfigurasi Database

Database yang digunakan bernama **`KantinDB`** dan terdiri dari tabel-tabel berikut:

- `categories` — Kategori menu makanan
- `menus` — Data menu beserta harga dan stok
- `users` — Data pengguna (Admin & Customer)
- `orders` — Data pesanan
- `order_items` — Detail item per pesanan
- `payments` — Data transaksi pembayaran

Semua tabel dibuat otomatis saat kamu menjalankan script `Setup Database SQL Server.sql`.

---

## ▶️ Cara Menjalankan

1. Pastikan **SQL Server Express** sedang berjalan di background
2. Buka solusi di Visual Studio
3. Tekan **F5** atau klik tombol **Start** untuk menjalankan aplikasi
4. Aplikasi akan otomatis mengecek koneksi database saat pertama dibuka
5. Login menggunakan akun default di bawah

---

## 👤 Akun Default

| Role | Username | Password |
|---|---|---|
| Admin | `admin` | `admin123` |
| Customer | *(registrasi via app)* | — |

> ⚠️ Disarankan untuk mengganti password admin setelah pertama kali login.

---

## 📸 Screenshot

> *(Tambahkan screenshot tampilan aplikasi di sini)*

---

## 🤝 Kontribusi

Pull request sangat disambut! Untuk perubahan besar, harap buka *issue* terlebih dahulu untuk mendiskusikan apa yang ingin kamu ubah.

---

## 📄 Lisensi

Proyek ini dibuat sebagai **Final Project** mata kuliah Pemrograman Berbasis Objek.  
© 2026 — Dibuat dengan ❤️ menggunakan C# & WinForms.
