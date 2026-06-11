using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Project_Order_Kantin
{
    // Form MenuEditor: dipakai untuk TAMBAH menu baru dan EDIT menu yang sudah ada.
    // Satu form, dua mode – dibedakan lewat parameter 'item' di constructor.
    //
    // Mode TAMBAH: item == null  → judul "Tambah Menu Baru", tombol "SIMPAN"
    // Mode EDIT  : item != null → judul "Edit Menu", tombol "UPDATE",
    //              field diisi dulu dengan data menu yang dipilih (PreFill)
    //
    // WEEK 8  - GUI Form Modal: form ini dibuka sebagai dialog (ShowDialog)
    //           sehingga AdminDashboard tidak bisa diklik sampai form ini ditutup.
    // WEEK 10 - CRUD: memanggil DatabaseHelper.InsertMenu() atau UpdateMenu()
    public class MenuEditor : Form
    {
        // _editItem menyimpan data menu yang sedang diedit.
        // readonly = tidak bisa diubah setelah constructor selesai.
        // Kalau null = mode tambah baru.
        private readonly MenuItemModel _editItem;
        private readonly bool          _isEdit;   // shortcut untuk cek _editItem != null

        // Deklarasi semua kontrol form
        private TextBox   txtNama;
        private TextBox   txtHarga;
        private ComboBox  cmbKategori;
        private TextBox   txtGambarUrl;
        private CheckBox  chkTersedia;
        private Button    btnSimpan;
        private Button    btnBatal;
        private Button    btnPreview;
        private PictureBox picPreview;
        private Label     lblStatus;

        // CONSTRUCTOR: terima item yang akan diedit (null = tambah baru)
        // dan list kategori untuk mengisi ComboBox.
        public MenuEditor(MenuItemModel item, List<Category> kategoriList)
        {
            _editItem = item;
            _isEdit   = item != null;

            InitUI(kategoriList);

            // Kalau mode edit, isi form dengan data menu yang dipilih
            if (_isEdit) PreFill();
        }

        // PROCEDURE: bangun semua elemen UI form secara kode.
        private void InitUI(List<Category> kategoriList)
        {
            // Judul form berbeda tergantung mode
            this.Text            = _isEdit ? "Edit Menu" : "Tambah Menu Baru";
            this.Size            = new Size(540, 530);
            this.StartPosition   = FormStartPosition.CenterParent;  // muncul di tengah parent
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.BackColor       = Color.White;
            this.Font            = new Font("Segoe UI", 10F);

            // Koordinat awal: lx = kolom kiri (label), fx = kolom kanan (input), fw = lebar input
            int lx = 20, fx = 160, fw = 330, y = 20;

            AddLabel("Nama Menu:", lx, y);
            txtNama = AddTextBox(fx, y, fw); y += 45;

            AddLabel("Harga (Rp):", lx, y);
            txtHarga = AddTextBox(fx, y, fw); y += 45;

            AddLabel("Kategori:", lx, y);
            cmbKategori = new ComboBox
            {
                Location      = new Point(fx, y),
                Size          = new Size(fw, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,  // hanya bisa pilih, tidak bisa ketik
                Font          = new Font("Segoe UI", 10F)
            };
            // Isi ComboBox dengan nama kategori dari database
            foreach (var k in kategoriList) cmbKategori.Items.Add(k.Nama);
            if (cmbKategori.Items.Count > 0) cmbKategori.SelectedIndex = 0;
            this.Controls.Add(cmbKategori);
            y += 45;

            AddLabel("Gambar URL:", lx, y);
            txtGambarUrl = AddTextBox(fx, y, fw); y += 45;

            // Tombol preview: load gambar dari URL yang diketik ke PictureBox
            btnPreview = new Button
            {
                Text      = "Preview Gambar",
                Location  = new Point(fx, y),
                Size      = new Size(150, 30),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor    = Cursors.Hand
            };
            btnPreview.Click += (s, e) =>
            {
                string url = txtGambarUrl.Text.Trim();
                if (!string.IsNullOrEmpty(url))
                    SupabaseImageHelper.LoadImageAsync(url, picPreview);
            };
            this.Controls.Add(btnPreview);
            y += 40;

            // PictureBox untuk preview gambar sebelum disimpan
            picPreview = new PictureBox
            {
                Location    = new Point(fx, y),
                Size        = new Size(200, 120),
                SizeMode    = PictureBoxSizeMode.Zoom,   // zoom in/out tanpa distorsi
                BackColor   = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(picPreview);
            y += 130;

            // CheckBox tersedia: menentukan apakah menu tampil di halaman pelanggan
            chkTersedia = new CheckBox
            {
                Text     = "Tersedia (tampil di menu customer)",
                Location = new Point(fx, y),
                AutoSize = true,
                Checked  = true,    // default: menu baru langsung tersedia
                Font     = new Font("Segoe UI", 10F)
            };
            this.Controls.Add(chkTersedia);
            y += 40;

            // Label untuk pesan validasi/error
            lblStatus = new Label
            {
                AutoSize  = false,
                Size      = new Size(480, 22),
                Location  = new Point(lx, y),
                ForeColor = Color.Red,
                Text      = ""
            };
            this.Controls.Add(lblStatus);
            y += 28;

            // Tombol SIMPAN / UPDATE (warna hijau)
            btnSimpan = new Button
            {
                Text      = _isEdit ? "UPDATE" : "SIMPAN",
                Location  = new Point(lx, y),
                Size      = new Size(140, 38),
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor    = Cursors.Hand
            };
            btnSimpan.Click += BtnSimpan_Click;

            // Tombol BATAL (warna merah), menutup form tanpa menyimpan
            btnBatal = new Button
            {
                Text         = "BATAL",
                Location     = new Point(175, y),
                Size         = new Size(140, 38),
                BackColor    = Color.IndianRed,
                ForeColor    = Color.White,
                Font         = new Font("Segoe UI", 11F, FontStyle.Bold),
                FlatStyle    = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor       = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };

            this.Controls.AddRange(new Control[] { btnSimpan, btnBatal });
            this.AcceptButton = btnSimpan;
            this.CancelButton = btnBatal;
        }

        // PROCEDURE: isi field-field form dengan data dari menu yang sedang diedit.
        // Dipanggil hanya saat mode edit (_isEdit = true).
        private void PreFill()
        {
            txtNama.Text        = _editItem.Nama;
            txtHarga.Text       = _editItem.Harga.ToString();
            txtGambarUrl.Text   = _editItem.GambarUrl;
            chkTersedia.Checked = _editItem.Tersedia;

            // Cari dan pilih kategori yang sesuai di ComboBox
            for (int i = 0; i < cmbKategori.Items.Count; i++)
            {
                if (cmbKategori.Items[i].ToString() == _editItem.Kategori)
                {
                    cmbKategori.SelectedIndex = i;
                    break;
                }
            }

            // Load preview gambar kalau URL sudah ada
            if (!string.IsNullOrEmpty(_editItem.GambarUrl))
                SupabaseImageHelper.LoadImageAsync(_editItem.GambarUrl, picPreview);
        }

        // EVENT HANDLER: dijalankan saat tombol SIMPAN/UPDATE diklik.
        // Validasi dulu semua input sebelum simpan ke database.
        // WEEK 2 – If-Else: validasi bertahap dengan return early.
        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            string nama     = txtNama.Text.Trim();
            string hargaStr = txtHarga.Text.Trim();
            string kategori = cmbKategori.SelectedItem?.ToString();

            // Validasi 1: nama menu wajib diisi
            if (string.IsNullOrEmpty(nama))
            { lblStatus.Text = "Nama menu wajib diisi."; return; }

            // Validasi 2: harga harus angka positif
            // int.TryParse = parse string ke int, return false kalau bukan angka
            if (!int.TryParse(hargaStr, out int harga) || harga < 0)
            { lblStatus.Text = "Harga harus angka positif."; return; }

            // Validasi 3: kategori harus dipilih
            if (string.IsNullOrEmpty(kategori))
            { lblStatus.Text = "Pilih kategori."; return; }

            // Semua validasi lolos – buat objek MenuItemModel dari input user
            var item = new MenuItemModel
            {
                Nama      = nama,
                Harga     = harga,
                Kategori  = kategori,
                GambarUrl = txtGambarUrl.Text.Trim(),
                Tersedia  = chkTersedia.Checked
            };

            try
            {
                if (_isEdit)
                {
                    // Mode EDIT: update data yang sudah ada di database
                    item.Id = _editItem.Id;
                    DatabaseHelper.UpdateMenu(item);
                    MessageBox.Show("Menu berhasil diupdate.", "Sukses",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Mode TAMBAH: insert data baru ke database
                    DatabaseHelper.InsertMenu(item);
                    MessageBox.Show("Menu baru berhasil ditambahkan.", "Sukses",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Set DialogResult = OK agar AdminDashboard tahu operasi berhasil
                // dan bisa refresh tabel menu
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
            }
        }

        // FUNCTION helper: buat Label dengan teks dan posisi tertentu.
        // Dipakai berulang kali di InitUI() agar kode lebih ringkas.
        private Label AddLabel(string text, int x, int y)
        {
            var lbl = new Label
            {
                AutoSize = true,
                Text     = text,
                Location = new Point(x, y + 5),
                Font     = new Font("Segoe UI", 10F)
            };
            this.Controls.Add(lbl);
            return lbl;
        }

        // FUNCTION helper: buat TextBox dengan posisi dan lebar tertentu.
        private TextBox AddTextBox(int x, int y, int width)
        {
            var txt = new TextBox
            {
                Location = new Point(x, y),
                Size     = new Size(width, 30),
                Font     = new Font("Segoe UI", 11F)
            };
            this.Controls.Add(txt);
            return txt;
        }
    }
}
