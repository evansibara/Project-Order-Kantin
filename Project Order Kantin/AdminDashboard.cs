using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Project_Order_Kantin
{
    // AdminDashboard adalah pusat kendali untuk Admin.
    // Form ini dibagi menjadi beberapa tab:
    //   - Tab Menu     : CRUD menu kantin + kelola stok
    //   - Tab Kategori : tambah/hapus kategori
    //   - Tab Pesanan  : lihat semua transaksi yang sudah masuk
    //   - Laporan      : buka Crystal Report penjualan
    //   - Backup       : backup data menu ke CSV
    //
    // WEEK 8  - GUI Admin Dashboard
    // WEEK 10 - CRUD Menu & Kategori (ADO.NET II)
    // WEEK 11 - LINQ (filter daftar pesanan, hitung total omzet)
    // WEEK 14 - Backup file CSV
    public partial class AdminDashboard : Form
    {
        // _semuaMenu dan _semuaKategori: cache data yang diambil dari DB.
        // Disimpan di field supaya method lain bisa pakai tanpa query ulang ke DB.
        private List<MenuItemModel>     _semuaMenu         = new List<MenuItemModel>();
        private List<Category>          _semuaKategori     = new List<Category>();

        // _menuWithStockCache: versi lengkap MenuItemModel yang menyertakan info stok.
        // Dipakai sebagai DataSource di DataGridView menu agar kolom Stok bisa tampil.
        private List<MenuItemWithStock> _menuWithStockCache = new List<MenuItemWithStock>();

        // CONSTRUCTOR: daftarkan semua event handler untuk tombol-tombol di form.
        // Pendekatan ini lebih rapi daripada double-click tombol di Designer
        // karena semua wire-up ada di satu tempat yang mudah dibaca.
        public AdminDashboard()
        {
            InitializeComponent();
            this.Load += AdminDashboard_Load;

            // ===== Tombol Header =====
            // Lambda (s, e) => { ... } = event handler singkat tanpa perlu buat method terpisah
            this.btnLogout.Click += (s, e) =>
            {
                SessionManager.Logout();   // hapus sesi admin yang aktif
                this.Close();              // tutup dashboard, Form1 akan muncul lagi
            };

            // ===== Tab Menu =====
            this.btnTambahMenu.Click  += BtnTambahMenu_Click;
            this.btnEditMenu.Click    += BtnEditMenu_Click;
            this.btnHapusMenu.Click   += BtnHapusMenu_Click;
            this.btnRefreshMenu.Click += (s, e) => LoadMenu();    // refresh = load ulang dari DB
            this.btnKelolaStok.Click  += BtnKelolaStok_Click;

            // ===== Tab Kategori =====
            this.btnTambahKategori.Click += BtnTambahKategori_Click;
            this.btnHapusKategori.Click  += BtnHapusKategori_Click;

            // ===== Tab Pesanan =====
            this.btnRefreshOrders.Click    += (s, e) => LoadOrders();
            // Saat user klik baris di tabel pesanan, tampilkan detail item-nya di bawah
            this.dgvOrders.SelectionChanged += DgvOrders_SelectionChanged;

            // ===== Laporan =====
            this.btnBukaLaporan.Click += (s, e) =>
            {
                // 'using' = form laporan di-dispose otomatis saat ditutup
                using (var f = new LaporanPenjualan())
                    f.ShowDialog();
            };

            // ===== Backup =====
            this.btnBackup.Click   += BtnBackup_Click;
            this.btnLihatLog.Click += BtnLihatLog_Click;
        }

        // EVENT HANDLER: dipanggil saat form selesai dimuat (Form.Load).
        // Tampilkan nama admin di header, lalu load semua data.
        private void AdminDashboard_Load(object sender, EventArgs e)
        {
            // Tampilkan sapaan sesuai nama admin yang sedang login
            if (SessionManager.CurrentUser != null)
                lblWelcome.Text = SessionManager.CurrentUser.GetWelcomeMessage();

            // Load data untuk ketiga tab sekaligus saat form pertama dibuka
            LoadMenu();
            LoadKategori();
            LoadOrders();
        }

        // ============================ TAB MENU – CRUD ============================

        // PROCEDURE: ambil data menu dari DB dan tampilkan di DataGridView.
        // Menggunakan GetMenuWithStock() agar kolom Stok dan StatusStok ikut tampil.
        // Kolom yang tidak perlu disembunyikan agar grid tidak terlalu lebar.
        private void LoadMenu()
        {
            try
            {
                // ADO.NET + SP: ambil menu beserta info stok dari database
                _menuWithStockCache = DatabaseHelper.GetMenuWithStock();

                // Buat juga versi sederhana (_semuaMenu) untuk dipakai MenuEditor
                _semuaMenu = _menuWithStockCache.Select(m =>
                    new MenuItemModel(m.Id, m.Nama, m.Harga, m.Kategori, m.GambarUrl, m.Tersedia)
                ).ToList();

                // Reset DataSource dulu ke null sebelum isi ulang,
                // supaya DataGridView benar-benar refresh dari awal
                dgvMenu.DataSource = null;
                dgvMenu.DataSource = _menuWithStockCache;
                dgvMenu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Sembunyikan kolom yang tidak relevan untuk admin
                foreach (string col in new[] { "GambarUrl", "Harga", "HargaFormatted", "TersediaDisplay", "StokDisplay" })
                    if (dgvMenu.Columns[col] != null) dgvMenu.Columns[col].Visible = false;

                // Ganti nama kolom (header) jadi lebih ramah dibaca
                SetCol(dgvMenu, "Id",           "ID");
                SetCol(dgvMenu, "Nama",         "Nama Menu");
                SetCol(dgvMenu, "Kategori",     "Kategori");
                SetCol(dgvMenu, "Tersedia",     "Tersedia");
                SetCol(dgvMenu, "Stock",        "Stok");
                SetCol(dgvMenu, "StockMinimum", "Min. Stok");
                SetCol(dgvMenu, "StatusStok",   "Status Stok");

                // Pasang event untuk warnai baris berdasarkan status stok
                // (-= dulu untuk menghindari duplikat handler kalau LoadMenu dipanggil berkali-kali)
                dgvMenu.CellFormatting -= DgvMenu_CellFormatting;
                dgvMenu.CellFormatting += DgvMenu_CellFormatting;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load menu: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // PROCEDURE helper: ubah teks header kolom DataGridView.
        // Cek null dulu supaya tidak error kalau nama kolom tidak ada.
        private void SetCol(DataGridView dgv, string col, string header)
        {
            if (dgv.Columns[col] != null) dgv.Columns[col].HeaderText = header;
        }

        // EVENT HANDLER CellFormatting: warnai baris tabel menu sesuai status stok.
        // Dipanggil oleh framework setiap kali DataGridView merender satu sel.
        // HABIS  = merah (perlu segera isi stok)
        // RENDAH = kuning-oranye (stok hampir habis)
        // normal = putih
        private void DgvMenu_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _menuWithStockCache.Count) return;
            var m = _menuWithStockCache[e.RowIndex];

            switch (m.StatusStok)
            {
                case "HABIS":
                    dgvMenu.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                    dgvMenu.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
                    break;
                case "RENDAH":
                    dgvMenu.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 180);
                    dgvMenu.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkOrange;
                    break;
                default:
                    dgvMenu.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    dgvMenu.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    break;
            }
        }

        // FUNCTION: ambil objek MenuItemModel dari baris yang sedang dipilih di grid.
        // Return null kalau tidak ada baris yang dipilih.
        // DataBoundItem = objek C# di balik baris tersebut (MenuItemWithStock atau MenuItemModel).
        private MenuItemModel GetSelectedMenu()
        {
            if (dgvMenu.CurrentRow == null) return null;

            // DataSource kita adalah List<MenuItemWithStock>, jadi cast ke itu dulu
            var withStock = dgvMenu.CurrentRow.DataBoundItem as MenuItemWithStock;
            if (withStock != null)
                return new MenuItemModel(withStock.Id, withStock.Nama, withStock.Harga,
                                         withStock.Kategori, withStock.GambarUrl, withStock.Tersedia);

            // Fallback: kalau ternyata DataBoundItem adalah MenuItemModel biasa
            return dgvMenu.CurrentRow.DataBoundItem as MenuItemModel;
        }

        // EVENT HANDLER tombol Tambah Menu:
        // Buka MenuEditor dalam mode tambah (item = null).
        // Kalau dialog ditutup dengan OK (simpan berhasil), refresh tabel menu.
        private void BtnTambahMenu_Click(object sender, EventArgs e)
        {
            using (var f = new MenuEditor(null, _semuaKategori))
            {
                if (f.ShowDialog() == DialogResult.OK) LoadMenu();
            }
        }

        // EVENT HANDLER tombol Edit Menu:
        // Validasi ada item yang dipilih, lalu buka MenuEditor dalam mode edit.
        private void BtnEditMenu_Click(object sender, EventArgs e)
        {
            var m = GetSelectedMenu();
            if (m == null) { MessageBox.Show("Pilih menu yang akan diedit.", "Info"); return; }

            using (var f = new MenuEditor(m, _semuaKategori))
            {
                if (f.ShowDialog() == DialogResult.OK) LoadMenu();
            }
        }

        // EVENT HANDLER tombol Hapus Menu:
        // Konfirmasi dulu sebelum hapus (tidak bisa di-undo).
        // Kalau menu sudah pernah diorder, DELETE akan gagal karena foreign key constraint.
        // Solusinya: set Tersedia = false saja daripada hapus.
        private void BtnHapusMenu_Click(object sender, EventArgs e)
        {
            var m = GetSelectedMenu();
            if (m == null) { MessageBox.Show("Pilih menu yang akan dihapus.", "Info"); return; }

            // Konfirmasi dialog sebelum hapus permanen
            if (MessageBox.Show("Yakin hapus menu '" + m.Nama + "'?", "Konfirmasi",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                != DialogResult.Yes) return;

            try
            {
                DatabaseHelper.DeleteMenu(m.Id);
                MessageBox.Show("Menu dihapus.", "Sukses");
                LoadMenu();
            }
            catch (Exception ex)
            {
                // Pesan error yang menjelaskan kenapa hapus bisa gagal dan apa solusinya
                MessageBox.Show("Gagal hapus: " + ex.Message +
                    "\r\n(Menu yang sudah diorder tidak bisa dihapus. " +
                    "Set tersedia = false saja.)", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // EVENT HANDLER tombol Kelola Stok:
        // Buka form StockManager sebagai dialog.
        // Setelah ditutup, refresh tabel menu agar perubahan stok langsung terlihat.
        private void BtnKelolaStok_Click(object sender, EventArgs e)
        {
            using (var f = new StockManager())
            {
                f.ShowDialog();
            }
            // Refresh tabel menu supaya kolom Stok dan Status Stok terupdate
            LoadMenu();
        }

        // ============================ TAB KATEGORI – CRUD ============================

        // PROCEDURE: ambil semua kategori dari DB dan tampilkan di ListBox.
        private void LoadKategori()
        {
            try
            {
                _semuaKategori = DatabaseHelper.GetCategories();
                lstKategori.Items.Clear();
                foreach (var k in _semuaKategori)
                    lstKategori.Items.Add(k.Nama);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load kategori: " + ex.Message);
            }
        }

        // EVENT HANDLER tombol Tambah Kategori:
        // Baca nama dari TextBox, validasi tidak kosong, INSERT ke DB, refresh list.
        private void BtnTambahKategori_Click(object sender, EventArgs e)
        {
            string nama = txtKategoriBaru.Text.Trim();
            if (string.IsNullOrEmpty(nama)) { MessageBox.Show("Nama kategori kosong."); return; }

            try
            {
                DatabaseHelper.InsertCategory(nama);
                txtKategoriBaru.Clear();   // kosongkan input setelah berhasil
                LoadKategori();
            }
            catch (Exception ex) { MessageBox.Show("Gagal: " + ex.Message); }
        }

        // EVENT HANDLER tombol Hapus Kategori:
        // Ambil item yang dipilih di ListBox, konfirmasi, DELETE dari DB.
        private void BtnHapusKategori_Click(object sender, EventArgs e)
        {
            if (lstKategori.SelectedIndex < 0) return;  // tidak ada yang dipilih

            // Ambil objek Category yang sesuai dari cache (_semuaKategori)
            // berdasarkan index yang dipilih di ListBox
            var kat = _semuaKategori[lstKategori.SelectedIndex];

            if (MessageBox.Show("Hapus kategori '" + kat.Nama + "'?", "Konfirmasi",
                                MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            try { DatabaseHelper.DeleteCategory(kat.Id); LoadKategori(); }
            catch (Exception ex) { MessageBox.Show("Gagal: " + ex.Message); }
        }

        // ============================ TAB PESANAN ============================

        // PROCEDURE: ambil semua order dari DB dan tampilkan di DataGridView.
        // WEEK 11 – LINQ: pakai LINQ Sum() untuk hitung total omzet dan jumlah order.
        private void LoadOrders()
        {
            try
            {
                var orders = DatabaseHelper.GetOrders();

                // WEEK 11 – LINQ Sum(): hitung total omzet dari semua order
                // dan jumlah order dengan satu baris kode LINQ.
                int totalSemua  = orders.Sum(o => o.Total);
                int jumlahOrder = orders.Count;

                dgvOrders.DataSource = null;
                dgvOrders.DataSource = orders;
                dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Ganti header kolom agar lebih mudah dibaca
                if (dgvOrders.Columns["OrderNumber"] != null)
                    dgvOrders.Columns["OrderNumber"].HeaderText = "No Order";
                if (dgvOrders.Columns["MetodePembayaran"] != null)
                    dgvOrders.Columns["MetodePembayaran"].HeaderText = "Metode";

                // Tampilkan ringkasan omzet di label bawah tabel
                lblTotalOmzet.Text = "Total Order: " + jumlahOrder +
                    "  |  Total Omzet: Rp " + totalSemua.ToString("N0");
            }
            catch (Exception ex) { MessageBox.Show("Gagal load orders: " + ex.Message); }
        }

        // EVENT HANDLER: saat baris pesanan berubah, tampilkan detail item-nya di grid bawah.
        // Dipanggil otomatis oleh framework setiap kali SelectionChanged terpicu.
        private void DgvOrders_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvOrders.CurrentRow == null) return;
            var order = dgvOrders.CurrentRow.DataBoundItem as Order;
            if (order == null) return;

            try
            {
                // Ambil detail item dari order yang dipilih
                var items = DatabaseHelper.GetOrderItems(order.Id);
                dgvOrderItems.DataSource = null;
                dgvOrderItems.DataSource = items;
                dgvOrderItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch { /* abaikan error di detail, tidak kritis */ }
        }

        // ============================ BACKUP & LOG ============================

        // EVENT HANDLER tombol Backup:
        // Ambil semua menu dari DB dan ekspor ke file CSV via FileStreamHelper.
        // WEEK 14 – File Stream: BackupMenuToCsv menggunakan StreamWriter.
        private void BtnBackup_Click(object sender, EventArgs e)
        {
            try
            {
                var data = DatabaseHelper.GetAllMenuItems();
                string path = FileStreamHelper.BackupMenuToCsv(data);
                MessageBox.Show("Backup CSV tersimpan di:\r\n" + path,
                                "Backup Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show("Backup gagal: " + ex.Message); }
        }

        // EVENT HANDLER tombol Lihat Log:
        // Baca file log transaksi hari ini dan tampilkan di MessageBox.
        // WEEK 14 – File Stream: ReadLogToday menggunakan StreamReader.
        private void BtnLihatLog_Click(object sender, EventArgs e)
        {
            string isi = FileStreamHelper.ReadLogToday();
            MessageBox.Show(isi, "Log Transaksi Hari Ini",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
