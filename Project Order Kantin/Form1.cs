using System;
using System.Drawing;
using System.Windows.Forms;

namespace Project_Order_Kantin
{
    // Form1 adalah home screen aplikasi – layar pertama yang dilihat saat
    // program dibuka. Berisi dua pilihan utama:
    //   1. "Mulai Pesanan" → buka Menu_Screen (halaman pelanggan)
    //   2. "Admin Login"   → buka LoginAdmin (halaman admin)
    //
    // WEEK 1 - WinForms Project Pertama: ini adalah form awal yang dibuat
    // saat project WinForms baru dibuat di Visual Studio.
    public partial class Form1 : Form
    {
        // CONSTRUCTOR: InitializeComponent() membaca desain form dari Form1.Designer.cs
        // dan membangun semua kontrol (tombol, label, dll.) secara otomatis.
        public Form1()
        {
            InitializeComponent();
        }

        // EVENT HANDLER: dijalankan saat tombol "Mulai Pesanan" diklik.
        // Sebelum buka Menu_Screen, coba koneksi ke database dulu.
        // Kalau gagal, tampilkan pesan error yang informatif agar user tahu
        // apa yang harus diperbaiki (biasanya: SQL Server belum jalan).
        private void BtnMulaiPesanan_Click(object sender, EventArgs e)
        {
            string error;
            if (!DatabaseHelper.TestConnection(out error))
            {
                // Koneksi gagal – tampilkan pesan error dan petunjuk solusi
                MessageBox.Show("Tidak bisa terhubung ke database:\n" + error +
                                "\n\nPastikan SQL Server berjalan dan sudah menjalankan sql_setup_full.sql",
                                "Koneksi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;  // Hentikan di sini, jangan buka Menu_Screen
            }

            // Koneksi OK – buka Menu_Screen
            var menuScreen = new Menu_Screen();
            menuScreen.Show();

            // Sembunyikan Form1 sementara Menu_Screen aktif.
            // Saat Menu_Screen ditutup, Form1 tampil lagi (FormClosed event).
            this.Hide();
            menuScreen.FormClosed += (s, ev) => this.Show();
        }

        // EVENT HANDLER: dijalankan saat tombol "Admin Login" diklik.
        // Sama seperti di atas, cek koneksi database dulu sebelum buka LoginAdmin.
        private void BtnAdminLogin_Click(object sender, EventArgs e)
        {
            string error;
            if (!DatabaseHelper.TestConnection(out error))
            {
                MessageBox.Show("Tidak bisa terhubung ke database:\n" + error,
                                "Koneksi Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Buka form Login Admin
            var loginForm = new LoginAdmin();
            loginForm.Show();
            this.Hide();
            loginForm.FormClosed += (s, ev) => this.Show();
        }
    }
}
