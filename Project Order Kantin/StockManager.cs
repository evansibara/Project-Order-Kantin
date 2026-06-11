using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Project_Order_Kantin
{
    // Form StockManager: pusat pengelolaan stok menu untuk Admin.
    //
    // Fitur yang tersedia:
    //   1. Lihat stok semua menu + status (CUKUP / RENDAH / HABIS)
    //   2. Tambah Stok (MASUK): tambahkan jumlah ke stok yang ada
    //   3. Koreksi Stok: set stok langsung ke nilai tertentu
    //   4. Set Minimum: ubah ambang batas peringatan stok rendah
    //   5. Riwayat perubahan stok (log semua transaksi stok)
    //
    // Semua operasi stok menggunakan Stored Procedure di SQL Server
    // agar logika bisnis terpusat dan konsisten.
    public class StockManager : Form
    {
        // Kontrol-kontrol form dideklarasikan sebagai field
        // agar bisa diakses dari method manapun di kelas ini.
        private DataGridView dgvStok;
        private DataGridView dgvLog;
        private TabControl   tabMain;
        private TabPage      tabStok;
        private TabPage      tabLog;
        private Panel        pnlTop;
        private Panel        pnlAksi;
        private Label        lblTitle;
        private Label        lblPeringatan;   // banner kuning kalau ada stok rendah
        private Button       btnTambahStok;
        private Button       btnKoreksiStok;
        private Button       btnSetMinimum;
        private Button       btnRefresh;
        private Button       btnRefreshLog;
        private Button       btnTutup;
        private ComboBox     cmbFilterLog;
        private Label        lblFilterLog;

        // Cache data stok terakhir yang diambil dari DB.
        // Dipakai oleh cmbFilterLog untuk mapping nama menu → id.
        private List<MenuItemWithStock> _dataStok = new List<MenuItemWithStock>();

        // CONSTRUCTOR: bangun UI dan pasang event handler Load
        public StockManager()
        {
            InitUI();
            // Load data saat form pertama dibuka
            this.Load += (s, e) => { LoadStok(); LoadLog(); };
        }

        // PROCEDURE: bangun semua elemen UI form secara programmatic.
        private void InitUI()
        {
            this.Text          = "Kelola Stok Menu";
            this.Size          = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimumSize   = new Size(900, 560);
            this.BackColor     = Color.WhiteSmoke;
            this.Font          = new Font("Segoe UI", 10F);

            // ---- Header (DarkSlateBlue) ----
            pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.DarkSlateBlue };
            lblTitle = new Label
            {
                AutoSize  = true,
                Text      = "📦  KELOLA STOK MENU",
                Font      = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Location  = new Point(16, 14)
            };
            pnlTop.Controls.Add(lblTitle);

            // ---- Panel Tombol Aksi (baris tombol di bawah header) ----
            pnlAksi = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = Color.White };

            // MakeBtn() = helper untuk membuat tombol dengan warna dan posisi tertentu
            btnTambahStok  = MakeBtn("+ TAMBAH STOK",   Color.SeaGreen,   10,  10);
            btnKoreksiStok = MakeBtn("✎ KOREKSI STOK",  Color.DarkOrange, 195, 10);
            btnSetMinimum  = MakeBtn("⚙ SET MINIMUM",   Color.SteelBlue,  380, 10);
            btnRefresh     = MakeBtn("↻ REFRESH",        Color.Gray,       565, 10);
            btnTutup       = MakeBtn("✕ TUTUP",          Color.IndianRed,  750, 10);

            // Pasang event handler tiap tombol
            btnTambahStok.Click  += BtnTambahStok_Click;
            btnKoreksiStok.Click += BtnKoreksiStok_Click;
            btnSetMinimum.Click  += BtnSetMinimum_Click;
            btnRefresh.Click     += (s, e) => { LoadStok(); LoadLog(); };
            btnTutup.Click       += (s, e) => this.Close();

            pnlAksi.Controls.AddRange(new Control[]
                { btnTambahStok, btnKoreksiStok, btnSetMinimum, btnRefresh, btnTutup });

            // ---- Banner Peringatan Stok Rendah ----
            // Tampil di atas tab kalau ada menu stok rendah/habis.
            // Awalnya disembunyikan (Visible = false), ditampilkan oleh LoadStok().
            lblPeringatan = new Label
            {
                Dock      = DockStyle.Top,
                Height    = 26,
                BackColor = Color.LightYellow,
                ForeColor = Color.DarkRed,
                Font      = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Text      = "",
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(10, 0, 0, 0),
                Visible   = false
            };

            // ---- Tab Control (dua tab: Stok Menu & Riwayat) ----
            tabMain = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
            tabStok = new TabPage("Stok Menu");
            tabLog  = new TabPage("Riwayat Perubahan Stok");
            tabMain.Controls.AddRange(new TabPage[] { tabStok, tabLog });

            // ---- DataGridView Tab Stok ----
            // BuildDgv() = helper untuk buat DataGridView dengan style seragam
            dgvStok = BuildDgv();
            tabStok.Controls.Add(dgvStok);
            tabStok.BackColor = Color.White;

            // ---- Tab Log: panel filter + DataGridView ----
            Panel pnlLogTop = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Color.White };
            lblFilterLog = new Label { AutoSize = true, Text = "Filter Menu:", Location = new Point(10, 13) };
            cmbFilterLog = new ComboBox
            {
                Location      = new Point(100, 9),
                Size          = new Size(260, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 10F)
            };
            cmbFilterLog.Items.Add("-- Semua Menu --");
            cmbFilterLog.SelectedIndex = 0;
            // Setiap kali filter berubah, reload log
            cmbFilterLog.SelectedIndexChanged += (s, e) => LoadLog();

            btnRefreshLog       = MakeBtn("↻ REFRESH", Color.Gray, 380, 7);
            btnRefreshLog.Size  = new Size(120, 32);
            btnRefreshLog.Click += (s, e) => LoadLog();

            pnlLogTop.Controls.AddRange(new Control[] { lblFilterLog, cmbFilterLog, btnRefreshLog });
            dgvLog = BuildDgv();
            tabLog.Controls.Add(dgvLog);
            tabLog.Controls.Add(pnlLogTop);
            tabLog.BackColor = Color.White;

            // ---- Susun semua kontrol ke form ----
            // Urutan Add penting untuk DockStyle karena yang ditambah terakhir
            // berada di posisi terdalam (Fill mengisi sisa ruang)
            this.Controls.Add(tabMain);
            this.Controls.Add(lblPeringatan);
            this.Controls.Add(pnlAksi);
            this.Controls.Add(pnlTop);
        }

        // FUNCTION helper: buat Button dengan style seragam.
        // Semua tombol di panel aksi punya tampilan yang konsisten.
        private Button MakeBtn(string text, Color color, int x, int y)
        {
            return new Button
            {
                Text             = text,
                BackColor        = color,
                ForeColor        = Color.White,
                Font             = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                FlatStyle        = FlatStyle.Flat,
                FlatAppearance   = { BorderSize = 0 },
                Cursor           = Cursors.Hand,
                Location         = new Point(x, y),
                Size             = new Size(170, 36),
                UseVisualStyleBackColor = false
            };
        }

        // FUNCTION helper: buat DataGridView dengan style seragam.
        // Header ungu gelap, baris read-only, satu baris dipilih sekaligus.
        private DataGridView BuildDgv()
        {
            var dgv = new DataGridView
            {
                Dock                  = DockStyle.Fill,
                ReadOnly              = true,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect           = false,
                BackgroundColor       = Color.White,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible     = false,
                BorderStyle           = BorderStyle.None,
                EnableHeadersVisualStyles = false
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkSlateBlue;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
            return dgv;
        }

        // ================================================================
        // LOAD DATA
        // ================================================================

        // PROCEDURE: ambil data stok dari DB dan tampilkan di dgvStok.
        // Juga update ComboBox filter log dan banner peringatan.
        private void LoadStok()
        {
            try
            {
                // ADO.NET + SP: panggil sp_GetMenuWithStock yang menghitung StatusStok
                _dataStok = DatabaseHelper.GetMenuWithStock();

                dgvStok.DataSource = null;
                dgvStok.DataSource = _dataStok;

                // Sembunyikan kolom teknis yang tidak perlu ditampilkan ke admin
                HideCol(dgvStok, "GambarUrl", "Tersedia", "HargaFormatted", "TersediaDisplay", "StokDisplay");

                // Ganti nama kolom dengan teks yang lebih mudah dipahami
                SetHeader(dgvStok, "Id",           "ID");
                SetHeader(dgvStok, "Nama",          "Nama Menu");
                SetHeader(dgvStok, "Harga",         "Harga (Rp)");
                SetHeader(dgvStok, "Kategori",      "Kategori");
                SetHeader(dgvStok, "Stock",         "Stok");
                SetHeader(dgvStok, "StockMinimum",  "Min. Stok");
                SetHeader(dgvStok, "StatusStok",    "Status");

                // Pasang event warnai baris berdasarkan status stok
                dgvStok.CellFormatting -= DgvStok_CellFormatting;
                dgvStok.CellFormatting += DgvStok_CellFormatting;

                // Update ComboBox filter log dengan nama menu terkini
                // Simpan index yang dipilih dulu agar tidak reset ke "Semua" setiap refresh
                var saved = cmbFilterLog.SelectedIndex;
                cmbFilterLog.Items.Clear();
                cmbFilterLog.Items.Add("-- Semua Menu --");
                foreach (var m in _dataStok) cmbFilterLog.Items.Add(m.Nama);
                cmbFilterLog.SelectedIndex = (saved >= 0 && saved < cmbFilterLog.Items.Count) ? saved : 0;

                // Tampilkan banner peringatan kalau ada menu stok rendah atau habis
                // LINQ: filter menu yang stoknya di bawah atau sama dengan minimum
                var rendah = _dataStok.Where(m => m.Stock <= m.StockMinimum).ToList();
                if (rendah.Count > 0)
                {
                    lblPeringatan.Text    = $"⚠  {rendah.Count} menu stok RENDAH / HABIS: " +
                                            string.Join(", ", rendah.Select(m => $"{m.Nama} ({m.Stock})"));
                    lblPeringatan.Visible = true;
                }
                else
                {
                    lblPeringatan.Text    = "";
                    lblPeringatan.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load stok: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // EVENT HANDLER CellFormatting: warnai baris dgvStok sesuai status stok.
        // Sama dengan di AdminDashboard:
        //   HABIS  = merah, RENDAH = kuning-oranye, lainnya = putih
        private void DgvStok_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _dataStok.Count) return;
            var m = _dataStok[e.RowIndex];

            switch (m.StatusStok)
            {
                case "HABIS":
                    dgvStok.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                    dgvStok.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkRed;
                    break;
                case "RENDAH":
                    dgvStok.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 180);
                    dgvStok.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkOrange;
                    break;
                default:
                    dgvStok.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    dgvStok.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    break;
            }
        }

        // PROCEDURE: load riwayat perubahan stok dari DB ke dgvLog.
        // Bisa difilter per menu berdasarkan pilihan cmbFilterLog.
        private void LoadLog()
        {
            try
            {
                // Kalau filter bukan "Semua Menu", cari Id menu yang namanya cocok
                int menuId = 0;
                if (cmbFilterLog.SelectedIndex > 0)
                {
                    string namaPilih = cmbFilterLog.SelectedItem.ToString();
                    var found = _dataStok.FirstOrDefault(m => m.Nama == namaPilih);
                    if (found != null) menuId = found.Id;
                }

                // menuId = 0 = ambil log semua menu
                var log = DatabaseHelper.GetStockLog(menuId);
                dgvLog.DataSource = null;
                dgvLog.DataSource = log;

                // Sembunyikan kolom yang tidak perlu, tampilkan yang relevan saja
                HideCol(dgvLog, "Id", "Jumlah", "StokSebelum", "StokSesudah", "CreatedAt");
                SetHeader(dgvLog, "NamaMenu",      "Nama Menu");
                SetHeader(dgvLog, "Jenis",         "Jenis");
                SetHeader(dgvLog, "JumlahDisplay", "Perubahan");
                SetHeader(dgvLog, "StokSebelum",   "Stok Sebelum");
                SetHeader(dgvLog, "StokSesudah",   "Stok Sesudah");
                SetHeader(dgvLog, "Keterangan",    "Keterangan");
                SetHeader(dgvLog, "TanggalDisplay","Tanggal");

                // ShowOnly: tampilkan hanya kolom-kolom ini, sembunyikan sisanya
                ShowOnly(dgvLog, "NamaMenu", "Jenis", "JumlahDisplay",
                         "StokSebelum", "StokSesudah", "Keterangan", "TanggalDisplay");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load log: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================================================================
        // TOMBOL AKSI
        // ================================================================

        // FUNCTION helper: ambil item yang sedang dipilih di dgvStok.
        // Return null kalau tidak ada baris yang dipilih.
        private MenuItemWithStock GetSelected()
        {
            if (dgvStok.CurrentRow == null) return null;
            return dgvStok.CurrentRow.DataBoundItem as MenuItemWithStock;
        }

        // EVENT HANDLER tombol Tambah Stok (MASUK):
        // Buka dialog input jumlah, lalu panggil SP sp_UpdateStock jenis MASUK.
        private void BtnTambahStok_Click(object sender, EventArgs e)
        {
            var m = GetSelected();
            if (m == null) { MessageBox.Show("Pilih menu terlebih dahulu.", "Info"); return; }

            using (var dlg = new StockInputDialog(m, "MASUK"))
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                try
                {
                    // ADO.NET + SP: panggil sp_UpdateStock, return stok baru
                    int stokBaru = DatabaseHelper.UpdateStock(m.Id, "MASUK", dlg.Jumlah, dlg.Keterangan);
                    MessageBox.Show(
                        $"Stok '{m.Nama}' berhasil ditambah.\nStok sekarang: {stokBaru} porsi",
                        "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStok(); LoadLog();   // refresh kedua tab setelah perubahan
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal: " + ex.Message, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // EVENT HANDLER tombol Koreksi Stok:
        // Buka dialog input, lalu panggil SP dengan jenis KOREKSI (set langsung ke nilai baru).
        private void BtnKoreksiStok_Click(object sender, EventArgs e)
        {
            var m = GetSelected();
            if (m == null) { MessageBox.Show("Pilih menu terlebih dahulu.", "Info"); return; }

            using (var dlg = new StockInputDialog(m, "KOREKSI"))
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                try
                {
                    int stokBaru = DatabaseHelper.UpdateStock(m.Id, "KOREKSI", dlg.Jumlah, dlg.Keterangan);
                    MessageBox.Show(
                        $"Stok '{m.Nama}' dikoreksi ke {stokBaru} porsi.",
                        "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStok(); LoadLog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal: " + ex.Message, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // EVENT HANDLER tombol Set Minimum:
        // Buka dialog input nilai minimum baru, lalu update kolom stock_minimum di DB.
        private void BtnSetMinimum_Click(object sender, EventArgs e)
        {
            var m = GetSelected();
            if (m == null) { MessageBox.Show("Pilih menu terlebih dahulu.", "Info"); return; }

            using (var dlg = new MinimumInputDialog(m))
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                try
                {
                    DatabaseHelper.UpdateStockMinimum(m.Id, dlg.Minimum);
                    MessageBox.Show($"Stok minimum '{m.Nama}' diset ke {dlg.Minimum} porsi.", "Sukses");
                    LoadStok();   // refresh tab stok untuk perbarui kolom Min. Stok
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Gagal: " + ex.Message, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ================================================================
        // HELPER METHODS
        // ================================================================

        // PROCEDURE: sembunyikan beberapa kolom DataGridView sekaligus.
        // params string[] = bisa terima jumlah argumen fleksibel
        private void HideCol(DataGridView dgv, params string[] names)
        {
            foreach (var n in names)
                if (dgv.Columns[n] != null) dgv.Columns[n].Visible = false;
        }

        // PROCEDURE: tampilkan hanya kolom tertentu, sembunyikan semua sisanya.
        // Berguna untuk tab log agar hanya kolom relevan yang tampil.
        private void ShowOnly(DataGridView dgv, params string[] names)
        {
            foreach (DataGridViewColumn col in dgv.Columns)
                col.Visible = false;
            foreach (var n in names)
                if (dgv.Columns[n] != null) dgv.Columns[n].Visible = true;
        }

        // PROCEDURE: set teks header satu kolom DataGridView.
        private void SetHeader(DataGridView dgv, string col, string header)
        {
            if (dgv.Columns[col] != null) dgv.Columns[col].HeaderText = header;
        }
    }

    // ====================================================================
    // Dialog Input Stok – untuk mode MASUK dan KOREKSI
    // ====================================================================
    // Dialog kecil yang muncul saat tombol Tambah Stok atau Koreksi Stok diklik.
    // Berisi NumericUpDown untuk input jumlah dan TextBox untuk keterangan opsional.
    public class StockInputDialog : Form
    {
        // Hasil input disimpan di properti publik agar StockManager bisa membaca nilainya
        // setelah dialog ditutup dengan OK.
        public int    Jumlah     { get; private set; }
        public string Keterangan { get; private set; }

        private NumericUpDown numJumlah;
        private TextBox       txtKet;
        private Button        btnOk;
        private Button        btnBatal;

        // CONSTRUCTOR: terima data menu yang akan diubah stoknya dan jenis operasi
        public StockInputDialog(MenuItemWithStock menu, string jenis)
        {
            this.Text            = jenis == "MASUK" ? "Tambah Stok" : "Koreksi Stok";
            this.Size            = new Size(420, 300);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = Color.White;
            this.Font            = new Font("Segoe UI", 10F);

            int y = 16;

            // Teks penjelasan berbeda untuk MASUK vs KOREKSI
            this.Controls.Add(new Label
            {
                AutoSize = false, Size = new Size(380, 60), Location = new Point(16, y),
                Text = jenis == "MASUK"
                    ? $"Menu         : {menu.Nama}\nStok Saat Ini : {menu.Stock} porsi\n\nMasukkan jumlah yang ditambahkan:"
                    : $"Menu         : {menu.Nama}\nStok Saat Ini : {menu.Stock} porsi\n\nMasukkan stok baru (set langsung):"
            });
            y += 75;

            // NumericUpDown: input angka dengan tombol naik/turun.
            // MASUK: minimum 1 (harus tambah minimal 1), KOREKSI: minimum 0 (bisa set ke 0)
            numJumlah = new NumericUpDown
            {
                Location = new Point(16, y), Size = new Size(120, 30),
                Minimum  = jenis == "MASUK" ? 1 : 0, Maximum = 9999,
                Value    = jenis == "MASUK" ? 1 : menu.Stock,   // default = stok saat ini untuk KOREKSI
                Font     = new Font("Segoe UI", 13F, FontStyle.Bold)
            };
            this.Controls.Add(numJumlah);
            y += 48;

            this.Controls.Add(new Label { AutoSize = true, Text = "Keterangan (opsional):", Location = new Point(16, y) });
            y += 22;

            txtKet = new TextBox
            {
                Location = new Point(16, y), Size = new Size(380, 28),
                Font     = new Font("Segoe UI", 10F)
            };
            this.Controls.Add(txtKet);
            y += 44;

            // Tombol SIMPAN: ambil nilai dari NumericUpDown dan TextBox,
            // set ke properti publik, lalu tutup dialog dengan OK
            btnOk = new Button
            {
                Text = "SIMPAN", Location = new Point(16, y), Size = new Size(120, 36),
                BackColor = Color.SeaGreen, ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderSize = 0 }, Cursor = Cursors.Hand
            };
            btnOk.Click += (s, e) =>
            {
                Jumlah     = (int)numJumlah.Value;
                Keterangan = txtKet.Text.Trim();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            btnBatal = new Button
            {
                Text = "BATAL", Location = new Point(150, y), Size = new Size(120, 36),
                BackColor = Color.IndianRed, ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] { btnOk, btnBatal });
            this.AcceptButton = btnOk;
            this.CancelButton = btnBatal;
        }
    }

    // ====================================================================
    // Dialog Set Stok Minimum
    // ====================================================================
    // Dialog kecil untuk mengubah nilai ambang batas stok rendah satu menu.
    public class MinimumInputDialog : Form
    {
        // Nilai minimum baru yang diinput user
        public int Minimum { get; private set; }

        private NumericUpDown numMin;
        private Button        btnOk;
        private Button        btnBatal;

        public MinimumInputDialog(MenuItemWithStock menu)
        {
            this.Text            = "Set Stok Minimum";
            this.Size            = new Size(380, 220);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = Color.White;
            this.Font            = new Font("Segoe UI", 10F);

            int y = 16;

            this.Controls.Add(new Label
            {
                AutoSize = false, Size = new Size(340, 50), Location = new Point(16, y),
                Text = $"Menu                : {menu.Nama}\nStok Minimum Saat Ini : {menu.StockMinimum} porsi\n\nMasukkan nilai minimum baru:"
            });
            y += 70;

            numMin = new NumericUpDown
            {
                Location = new Point(16, y), Size = new Size(120, 30),
                Minimum  = 0, Maximum = 9999,
                Value    = menu.StockMinimum,   // default = nilai minimum saat ini
                Font     = new Font("Segoe UI", 13F, FontStyle.Bold)
            };
            this.Controls.Add(numMin);
            y += 52;

            btnOk = new Button
            {
                Text = "SIMPAN", Location = new Point(16, y), Size = new Size(120, 36),
                BackColor = Color.SteelBlue, ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderSize = 0 }, Cursor = Cursors.Hand
            };
            btnOk.Click += (s, e) =>
            {
                Minimum = (int)numMin.Value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            btnBatal = new Button
            {
                Text = "BATAL", Location = new Point(150, y), Size = new Size(120, 36),
                BackColor = Color.IndianRed, ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand, DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] { btnOk, btnBatal });
            this.AcceptButton = btnOk;
            this.CancelButton = btnBatal;
        }
    }
}
