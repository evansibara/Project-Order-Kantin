namespace Project_Order_Kantin
{
    // SessionManager adalah "tempat titip" informasi user yang sedang login.
    // Saat Admin berhasil login lewat LoginAdmin.cs, objek Admin disimpan di sini
    // agar form-form lain (AdminDashboard, dll.) bisa tahu siapa yang sedang aktif
    // tanpa perlu passing objek manual dari form ke form.
    //
    // Pola ini disebut "Static Session Storage" – sederhana dan cukup untuk
    // aplikasi desktop single-user seperti ini.
    public static class SessionManager
    {
        // CurrentUser menyimpan Admin yang sedang login.
        // 'private set' = hanya bisa diubah dari dalam kelas ini sendiri,
        // tidak bisa diubah dari luar. Ini mencegah form lain memanipulasi
        // session secara sembarangan.
        public static Admin CurrentUser { get; private set; }

        // PROCEDURE: dipanggil setelah verifikasi login berhasil.
        // Menyimpan objek Admin ke CurrentUser agar bisa diakses di seluruh aplikasi.
        public static void Login(Admin admin)
        {
            CurrentUser = admin;
        }

        // PROCEDURE: dipanggil saat tombol Logout diklik di AdminDashboard.
        // Mengosongkan CurrentUser (set ke null) = sesi dianggap berakhir.
        public static void Logout()
        {
            CurrentUser = null;
        }

        // FUNCTION: properti pintas untuk cek apakah ada user yang sedang login.
        // Lebih bersih daripada cek CurrentUser != null di mana-mana.
        // Contoh pemakaian: if (SessionManager.IsLoggedIn) { ... }
        public static bool IsLoggedIn => CurrentUser != null;
    }
}
