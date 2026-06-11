using System;
using System.Drawing;
using System.Windows.Forms;

namespace Project_Order_Kantin
{
    // Form LoginAdmin: halaman login khusus untuk Admin.
    // Seluruh UI dibangun secara dinamis di kode (bukan di Designer.cs)
    // agar lebih mudah dikustomisasi tampilannya.
    //
    // WEEK 2 - If-Else: validasi username dan password sebelum proses login
    // WEEK 8 - GUI Form: desain form dengan panel kartu, TextBox, dan Button
    public class LoginAdmin : Form
    {
        // Deklarasi semua kontrol yang dibutuhkan.
        // Dideklarasikan sebagai field kelas agar bisa diakses dari method manapun
        // di dalam kelas ini (tidak hanya dari InitUI).
        private Panel  pnlCard;
        private Label  lblJudul;
        private Label  lblUsername;
        private Label  lblPassword;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnBatal;
        private Label  lblStatus;   // label merah untuk pesan error login

        // CONSTRUCTOR: panggil InitUI() untuk membangun tampilan form
        public LoginAdmin()
        {
            InitUI();
        }

        // PROCEDURE: membangun semua elemen UI secara kode (programmatic UI).
        // Pendekatan ini lebih fleksibel untuk kustomisasi warna dan layout,
        // tapi lebih verbose dibanding drag-drop di Designer.
        private void InitUI()
        {
            // Konfigurasi dasar jendela form
            this.Text            = "Login Admin";
            this.Size            = new Size(440, 400);
            this.StartPosition   = FormStartPosition.CenterScreen;  // muncul di tengah layar
            this.FormBorderStyle = FormBorderStyle.FixedDialog;      // ukuran tidak bisa diubah user
            this.MaximizeBox     = false;   // nonaktifkan tombol maximize
            this.MinimizeBox     = false;   // nonaktifkan tombol minimize
            this.BackColor       = Color.FromArgb(22, 27, 34);   // background gelap
            this.Font            = new Font("Segoe UI", 10F);

            // Strip oranye di sisi kiri sebagai aksen visual
            var pnlAccent = new Panel
            {
                BackColor = Color.FromArgb(255, 107, 53),
                Location  = new Point(0, 0),
                Size      = new Size(6, 400),
                Dock      = DockStyle.Left
            };
            this.Controls.Add(pnlAccent);

            // Panel kartu (card) yang berisi semua input
            pnlCard = new Panel
            {
                Size      = new Size(390, 360),
                Location  = new Point(28, 20),
                BackColor = Color.FromArgb(30, 37, 47),
                Padding   = new Padding(20)
            };

            // Judul form dengan ikon kunci
            lblJudul = new Label
            {
                AutoSize  = true,
                Text      = "🔐  Admin Login",
                Font      = new Font("Segoe UI", 15F, FontStyle.Bold),
                ForeColor = Color.White,
                Location  = new Point(20, 18)
            };

            // Label dan TextBox untuk Username
            lblUsername = new Label
            {
                AutoSize  = true,
                Text      = "Username",
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(140, 150, 165),
                Location  = new Point(20, 65)
            };
            txtUsername = new TextBox
            {
                Location   = new Point(20, 83),
                Size       = new Size(320, 30),
                Font       = new Font("Segoe UI", 11F),
                BackColor  = Color.FromArgb(42, 50, 62),
                ForeColor  = Color.White,
                BorderStyle= BorderStyle.FixedSingle
            };

            // Label dan TextBox untuk Password
            // PasswordChar = '●' → karakter yang diketik diganti bulat hitam
            // agar password tidak terlihat di layar
            lblPassword = new Label
            {
                AutoSize  = true,
                Text      = "Password",
                Font      = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(140, 150, 165),
                Location  = new Point(20, 128)
            };
            txtPassword = new TextBox
            {
                Location     = new Point(20, 146),
                Size         = new Size(320, 30),
                Font         = new Font("Segoe UI", 11F),
                PasswordChar = '●',           // sembunyikan karakter password
                BackColor    = Color.FromArgb(42, 50, 62),
                ForeColor    = Color.White,
                BorderStyle  = BorderStyle.FixedSingle
            };

            // Tombol Login (oranye, warna utama)
            btnLogin = new Button
            {
                Text             = "Login",
                Location         = new Point(20, 200),
                Size             = new Size(200, 44),
                BackColor        = Color.FromArgb(255, 107, 53),
                ForeColor        = Color.White,
                Font             = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle        = FlatStyle.Flat,
                FlatAppearance   = { BorderSize = 0 },
                Cursor           = Cursors.Hand    // cursor berubah jadi tangan saat hover
            };
            btnLogin.Click += BtnLogin_Click;

            // Tombol Batal (abu-abu, warna sekunder)
            // DialogResult.Cancel = menutup form dengan hasil "dibatalkan"
            btnBatal = new Button
            {
                Text           = "Batal",
                Location       = new Point(235, 200),
                Size           = new Size(105, 44),
                BackColor      = Color.FromArgb(42, 50, 62),
                ForeColor      = Color.FromArgb(180, 190, 205),
                Font           = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle      = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 1, BorderColor = Color.FromArgb(70, 80, 95) },
                Cursor         = Cursors.Hand,
                DialogResult   = DialogResult.Cancel
            };

            // Label untuk pesan error (awalnya kosong, diisi saat login gagal)
            lblStatus = new Label
            {
                AutoSize  = false,
                Size      = new Size(340, 28),
                Location  = new Point(20, 256),
                ForeColor = Color.FromArgb(255, 107, 53),
                Font      = new Font("Segoe UI", 9.5F),
                BackColor = Color.Transparent
            };

            // Tambahkan semua kontrol ke panel kartu
            pnlCard.Controls.AddRange(new Control[]
                { lblJudul, lblUsername, txtUsername,
                  lblPassword, txtPassword, btnLogin, btnBatal, lblStatus });

            this.Controls.Add(pnlCard);

            // AcceptButton = tombol yang aktif saat user tekan Enter
            // CancelButton = tombol yang aktif saat user tekan Escape
            this.AcceptButton = btnLogin;
            this.CancelButton = btnBatal;
        }

        // EVENT HANDLER: dijalankan saat tombol Login diklik (atau Enter ditekan).
        // WEEK 2 – IF-ELSE: validasi bertahap, satu kondisi per baris.
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();    // .Trim() buang spasi di awal/akhir
            string pass = txtPassword.Text;

            // Validasi 1: field tidak boleh kosong
            // WEEK 2 - If-Else: pengecekan kondisi sederhana dengan return early
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                lblStatus.Text = "Username dan password tidak boleh kosong.";
                return;   // Hentikan di sini, jangan lanjut ke database
            }

            try
            {
                // ADO.NET: cari user di database yang cocok username + password + role Admin
                Admin admin = DatabaseHelper.LoginAdmin(user, pass);

                if (admin != null)
                {
                    // Login berhasil: simpan sesi dan buka dashboard
                    SessionManager.Login(admin);
                    var dashboard = new AdminDashboard();
                    dashboard.Show();
                    this.Close();    // tutup form login, tidak perlu lagi
                }
                else
                {
                    // Login gagal: username/password salah atau bukan Admin
                    lblStatus.Text = "Username atau password salah.";
                    txtPassword.Clear();   // hapus password yang salah
                    txtPassword.Focus();   // pindahkan fokus ke field password
                }
            }
            catch (Exception ex)
            {
                // Error database (SQL Server mati, connection string salah, dll.)
                lblStatus.Text = "Error: " + ex.Message;
            }
        }
    }
}
