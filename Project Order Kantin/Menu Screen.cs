// =================================================================
// WEEK 8  - GUI : Menu_Screen menampilkan kartu menu dalam FlowLayout
// WEEK 11 - LINQ: SearchMenuLinq() untuk search, filter, sort
// FITUR STOK: Menu dengan stok habis (stock = 0) tidak ditampilkan
//             (difilter di DatabaseHelper.SearchMenuLinq).
//             Kartu menu yang stok terbatas (1-5) menampilkan badge peringatan.
// =================================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Project_Order_Kantin
{
    // Menu_Screen adalah halaman utama pelanggan.
    // Menampilkan kartu-kartu menu yang bisa di-filter, dicari, dan diurutkan.
    // Setiap kartu punya tombol "Tambah ke Keranjang" yang menambah item
    // ke CartManager (Dictionary).
    public partial class Menu_Screen : Form
    {
        // Win32 API untuk placeholder text pada TextBox (teks abu-abu sebelum user ketik).
        // Di .NET Framework 4.7.2, TextBox tidak punya properti PlaceholderText bawaan.
        // Solusinya: kirim pesan EM_SETCUEBANNER ke Win32 API secara manual.
        // DllImport = import fungsi dari DLL Windows (user32.dll)
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        // Konstanta kode pesan Win32 untuk memasang teks placeholder pada TextBox
        private const int EM_SETCUEBANNER = 0x1501;

        // PROCEDURE: pasang teks placeholder pada TextBox menggunakan Win32 API.
        // Dipanggil di constructor untuk txtSearch.
        private static void SetPlaceholder(TextBox tb, string text)
        {
            SendMessage(tb.Handle, EM_SETCUEBANNER, (IntPtr)1, text);
        }

        // LIST (WEEK 6): cache semua menu yang diambil dari database.
        // Disimpan di memori agar FilterMenu() bisa filter dengan LINQ
        // tanpa perlu query DB setiap kali user mengetik.
        private List<MenuItemModel> _semuaMenu = new List<MenuItemModel>();

        // CONSTRUCTOR: set up semua event handler dan inisialisasi form.
        public Menu_Screen()
        {
            InitializeComponent();

            // Pasang placeholder teks di kotak pencarian (fitur Win32)
            SetPlaceholder(this.txtSearch, "Cari menu...");

            // Wire-up tombol keranjang dan refresh
            this.btnCart.Click    += (s, e) => BukaCart();
            this.btnRefresh.Click += (s, e) => LoadMenu();

            // Saat form di-resize, reposisi tombol keranjang ke kanan
            this.Resize += (s, e) => AturPosisiKanan();

            // Filter realtime: setiap kali input berubah, langsung filter kartu menu
            this.txtSearch.TextChanged            += (s, e) => FilterMenu();
            this.cmbKategori.SelectedIndexChanged += (s, e) => FilterMenu();
            this.cmbSort.SelectedIndexChanged     += (s, e) => FilterMenu();

            // Load data saat form pertama tampil
            this.Load += Menu_Screen_Load;
        }

        // PROCEDURE: pastikan btnCart dan lblCartCount selalu rata kanan header.
        // Dipanggil saat form di-resize atau teks counter berubah (jumlah item berubah).
        private void AturPosisiKanan()
        {
            int margin = 14;
            int panelW = pnlTopRow1.ClientSize.Width;

            // Posisi tombol keranjang = lebar panel dikurangi lebar tombol dikurangi margin
            btnCart.Left      = panelW - btnCart.Width - margin;

            // Label counter diletakkan 10px di kiri tombol keranjang
            lblCartCount.Left = btnCart.Left - lblCartCount.Width - 10;
        }

        // EVENT HANDLER Form.Load: inisialisasi ComboBox filter dan load menu pertama kali.
        private void Menu_Screen_Load(object sender, EventArgs e)
        {
            AturPosisiKanan();   // posisikan tombol keranjang

            // Isi ComboBox Kategori dari database
            cmbKategori.Items.Clear();
            cmbKategori.Items.Add("Semua");    // pilihan default = tampilkan semua kategori
            try
            {
                var kat = DatabaseHelper.GetCategories();
                foreach (var k in kat) cmbKategori.Items.Add(k.Nama);
            }
            catch { /* abaikan jika kategori gagal dimuat */ }
            cmbKategori.SelectedIndex = 0;

            // Isi ComboBox Sort dengan pilihan pengurutan
            cmbSort.Items.Clear();
            cmbSort.Items.AddRange(new string[]
                { "Default", "Nama A-Z", "Nama Z-A", "Harga Termurah", "Harga Termahal" });
            cmbSort.SelectedIndex = 0;

            LoadMenu();
        }

        // PROCEDURE: ambil semua data menu dari DB ke _semuaMenu,
        // lalu panggil FilterMenu() untuk render kartu.
        // Dipanggil saat form load dan tombol Refresh diklik.
        private void LoadMenu()
        {
            try
            {
                // ADO.NET (WEEK 9): ambil semua menu termasuk kolom 'stock'
                _semuaMenu = DatabaseHelper.GetAllMenuItems();
                FilterMenu();    // render ulang dengan filter yang sedang aktif
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load menu: " + ex.Message);
            }
        }

        // PROCEDURE: baca nilai filter saat ini dari UI, jalankan LINQ query,
        // lalu render hasilnya sebagai kartu di FlowLayout.
        // Dipanggil setiap kali TextBox atau ComboBox berubah nilai.
        private void FilterMenu()
        {
            string keyword  = txtSearch.Text.Trim();
            string kategori = cmbKategori.SelectedItem?.ToString();
            string sort     = cmbSort.SelectedItem?.ToString() ?? "Default";

            // WEEK 11 – LINQ: SearchMenuLinq melakukan Where + OrderBy di memori (_semuaMenu).
            // Sudah termasuk filter: hanya menu tersedia DAN stok > 0.
            var hasil = DatabaseHelper.SearchMenuLinq(keyword, kategori, sort);

            RenderMenu(hasil);
        }

        // PROCEDURE: hapus semua kartu lama dari FlowLayout,
        // lalu render ulang satu kartu per item.
        private void RenderMenu(List<MenuItemModel> items)
        {
            flowLayoutMenu.Controls.Clear();

            foreach (var item in items)
                flowLayoutMenu.Controls.Add(BuildMenuCard(item));

            lblJumlahMenu.Text = $"Menampilkan {items.Count} menu";
        }

        // FUNCTION (return Panel): bangun satu kartu menu secara dinamis (programmatic UI).
        // Setiap kartu berisi: gambar, badge kategori, nama, harga, dan tombol tambah.
        //
        // FITUR STOK:
        //   - Stok terbatas (1-5): tampilkan badge kuning di kartu
        //   - Stok habis (0): tombol "Tambah" dinonaktifkan (disabled), teks jadi "Stok Habis"
        //     (sebenarnya kondisi ini sudah dicegah di SearchMenuLinq, ini lapisan ke-2)
        private Panel BuildMenuCard(MenuItemModel item)
        {
            var card = new Panel
            {
                Size      = new Size(210, 275),
                BackColor = Color.White,
                Margin    = new Padding(10),
                Tag       = item    // simpan referensi objek menu di Tag agar bisa diakses event handler
            };

            // EVENT PAINT: gambar border tipis di sekeliling kartu
            card.Paint += (s, e) =>
            {
                var g = e.Graphics;
                using (var pen = new System.Drawing.Pen(Color.FromArgb(228, 226, 221), 1))
                    g.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            // ---- Gambar menu (PictureBox) ----
            // Load gambar dari Supabase secara async agar UI tidak freeze
            var pic = new PictureBox
            {
                Location  = new Point(0, 0),
                Size      = new Size(210, 138),
                SizeMode  = PictureBoxSizeMode.Zoom,    // zoom tanpa distorsi
                BackColor = Color.FromArgb(238, 236, 232)
            };
            if (!string.IsNullOrEmpty(item.GambarUrl))
                SupabaseImageHelper.LoadImageAsync(item.GambarUrl, pic);
            card.Controls.Add(pic);

            // ---- Badge Kategori (teks kecil oranye) ----
            var lblKat = new Label
            {
                Text      = item.Kategori.ToUpper(),
                Font      = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 107, 53),
                BackColor = Color.FromArgb(255, 244, 240),
                Location  = new Point(10, 148),
                AutoSize  = true,
                Padding   = new Padding(4, 2, 4, 2)
            };
            card.Controls.Add(lblKat);

            // ---- Nama Menu ----
            card.Controls.Add(new Label
            {
                Text      = item.Nama,
                Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location  = new Point(10, 172),
                Size      = new Size(190, 36),
                ForeColor = Color.FromArgb(22, 27, 34)
            });

            // ---- Harga ----
            card.Controls.Add(new Label
            {
                Text      = "Rp " + item.Harga.ToString("N0"),
                Font      = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 130, 80),
                Location  = new Point(10, 207),
                AutoSize  = true
            });

            // ---- FITUR STOK: Badge peringatan stok terbatas ----
            // GetStatusStokLabel() mengembalikan string kosong kalau stok cukup (> 5)
            string statusStok = item.GetStatusStokLabel();
            if (!string.IsNullOrEmpty(statusStok))
            {
                // Tampilkan badge kuning di sebelah kanan harga
                var lblStok = new Label
                {
                    Text      = statusStok,
                    Font      = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(150, 90, 0),
                    BackColor = Color.FromArgb(255, 243, 205),
                    Location  = new Point(90, 210),
                    AutoSize  = true,
                    Padding   = new Padding(4, 1, 4, 1)
                };
                card.Controls.Add(lblStok);
            }

            // ---- Tombol Tambah ke Keranjang ----
            // Cek stok: kalau habis, nonaktifkan tombol (defence-in-depth)
            bool stokHabis = item.IsStokHabis();

            var btnAdd = new Button
            {
                Text      = stokHabis ? "Stok Habis" : "+ Tambah ke Keranjang",
                Location  = new Point(0, 245),
                Size      = new Size(210, 30),
                BackColor = stokHabis ? Color.FromArgb(180, 180, 180)
                                      : Color.FromArgb(22, 27, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Font      = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor    = stokHabis ? Cursors.No : Cursors.Hand,
                Enabled   = !stokHabis,    // disabled = tidak bisa diklik kalau stok habis
                Tag       = item
            };

            // Efek hover: warna tombol berubah oranye saat mouse masuk
            // (hanya aktif kalau stok ada)
            if (!stokHabis)
            {
                btnAdd.MouseEnter += (s, e) => { btnAdd.BackColor = Color.FromArgb(255, 107, 53); };
                btnAdd.MouseLeave += (s, e) => { btnAdd.BackColor = Color.FromArgb(22, 27, 34); };
            }

            // EVENT HANDLER tombol tambah ke keranjang
            btnAdd.Click += (s, e) =>
            {
                // Validasi ulang stok dari objek menu (lapisan ke-2 setelah SearchMenuLinq)
                if (item.IsStokHabis())
                {
                    MessageBox.Show($"Maaf, stok '{item.Nama}' telah habis.",
                                    "Stok Habis", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // WEEK 6 – Dictionary: CartManager.AddItem menambah ke Dictionary<int, CartLine>
                CartManager.AddItem(item);

                // Update counter keranjang di header
                int total = CartManager.TotalItems();
                lblCartCount.Text = $"🛒 {total} item";
                AturPosisiKanan();    // reposisi karena lebar teks berubah

                MessageBox.Show($"'{item.Nama}' ditambahkan ke keranjang.",
                                "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            card.Controls.Add(btnAdd);

            return card;
        }

        // PROCEDURE: buka Cart_Screen sebagai modal dialog.
        // Guard clause: tidak buka cart kalau masih kosong.
        private void BukaCart()
        {
            if (CartManager.Items.Count == 0)
            {
                MessageBox.Show("Keranjang masih kosong.", "Info",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var cart = new Cart_Screen())
            {
                cart.ShowDialog();
            }

            // Sinkronkan counter setelah cart ditutup (mungkin ada item yang dihapus)
            int total = CartManager.TotalItems();
            lblCartCount.Text = $"🛒 {total} item";
            AturPosisiKanan();
        }
    }
}
