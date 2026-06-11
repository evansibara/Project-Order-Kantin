// =================================================================
// WEEK 8  - GUI : Cart_Screen menampilkan isi keranjang belanja
// WEEK 6  - Dictionary/List : membaca CartManager.Items
// WEEK 5  - Function & Procedure: RenderCart, UpdateTotal, BuildCartRow
// FITUR STOK: Tombol + di cart dibatasi agar tidak melebihi stok di DB
// =================================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Project_Order_Kantin
{
    // Cart_Screen menampilkan semua item yang sudah masuk keranjang.
    // Pelanggan bisa ubah jumlah (+/-) setiap item atau langsung checkout.
    public partial class Cart_Screen : Form
    {
        // CONSTRUCTOR: pasang event handler untuk tombol Kembali dan Checkout
        public Cart_Screen()
        {
            InitializeComponent();
            this.bttnKembali.Click  += BttnKembali_Click;
            this.bttnCheckout.Click += BttnCheckout_Click;
        }

        // EVENT HANDLER Form.Shown: render isi keranjang saat form pertama tampil.
        // Shown (bukan Load) karena ClientSize FlowLayout sudah tersedia saat Shown.
        private void Cart_Screen_Shown(object sender, EventArgs e)
        {
            RenderCart();
        }

        // PROCEDURE: bersihkan FlowLayout, lalu render ulang satu baris per item keranjang.
        // Dipanggil saat form tampil dan setiap kali jumlah item berubah.
        private void RenderCart()
        {
            flowLayoutCart.Controls.Clear();

            // WEEK 6 – DICTIONARY: iterasi semua key (MenuItemId) di CartManager.Items.
            // Buat salinan key dulu (new List<int>(...)) supaya aman kalau isi
            // Dictionary berubah saat iterasi (walau di sini seharusnya tidak berubah).
            foreach (var key in new List<int>(CartManager.Items.Keys))
                flowLayoutCart.Controls.Add(BuildCartRow(CartManager.Items[key]));

            UpdateTotal();
        }

        // PROCEDURE: update label total harga di bagian bawah form.
        // CartManager.Total() menjumlahkan semua Subtotal (HargaSatuan × Jumlah).
        private void UpdateTotal()
        {
            lblTotalHarga.Text = "Rp " + CartManager.Total().ToString("N0");
        }

        // FUNCTION (return Panel): bangun satu baris kartu untuk item di keranjang.
        // Isi setiap baris: gambar kecil, nama item, harga satuan, tombol -/+, jumlah, subtotal.
        private Panel BuildCartRow(CartLine line)
        {
            // Hitung lebar baris agar mengisi FlowLayout, dengan fallback 940px
            int rowWidth = flowLayoutCart.ClientSize.Width - 20;
            if (rowWidth < 200) rowWidth = flowLayoutCart.Width - 30;
            if (rowWidth < 200) rowWidth = 940;

            var pnl = new Panel
            {
                Size        = new Size(rowWidth, 100),
                BackColor   = Color.White,
                Margin      = new Padding(5, 5, 5, 0),
                BorderStyle = BorderStyle.FixedSingle,
                Tag         = line.MenuItemId   // simpan id agar event handler tahu item ini milik siapa
            };

            // ---- Gambar item (ambil dari DB berdasarkan MenuItemId) ----
            var pic = new PictureBox
            {
                Location  = new Point(10, 10),
                Size      = new Size(110, 78),
                SizeMode  = PictureBoxSizeMode.Zoom,
                BackColor = Color.LightGray
            };
            try
            {
                // Ambil URL gambar dari DB lalu load secara async
                var menus = DatabaseHelper.GetAllMenuItems();
                foreach (var m in menus)
                {
                    if (m.Id == line.MenuItemId && !string.IsNullOrEmpty(m.GambarUrl))
                    {
                        SupabaseImageHelper.LoadImageAsync(m.GambarUrl, pic);
                        break;
                    }
                }
            }
            catch { /* abaikan kalau gambar gagal load */ }
            pnl.Controls.Add(pic);

            // ---- Nama item ----
            var lblNama = new Label
            {
                Text      = line.Nama,
                Font      = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location  = new Point(135, 14),
                AutoSize  = true,
                ForeColor = Color.FromArgb(30, 30, 30)
            };
            pnl.Controls.Add(lblNama);

            // ---- Harga satuan (kecil, abu-abu) ----
            var lblHarga = new Label
            {
                Text      = "Rp " + line.HargaSatuan.ToString("N0"),
                Font      = new Font("Segoe UI", 10F),
                ForeColor = Color.Gray,
                Location  = new Point(135, 48),
                AutoSize  = true
            };
            pnl.Controls.Add(lblHarga);

            // ---- Tombol KURANGI (-) ----
            // Anchor kanan agar tombol tetap rata kanan saat form di-resize
            var btnMin = new Button
            {
                Text      = "-",
                Size      = new Size(38, 38),
                Location  = new Point(rowWidth - 290, 30),
                BackColor = Color.FromArgb(230, 230, 230),
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
                Cursor    = Cursors.Hand,
                Tag       = line.MenuItemId,
                Anchor    = AnchorStyles.Top | AnchorStyles.Right
            };
            btnMin.FlatAppearance.BorderSize = 0;
            btnMin.Click += BtnMin_Click;
            pnl.Controls.Add(btnMin);

            // ---- Label jumlah porsi (di tengah antara - dan +) ----
            var lblJumlah = new Label
            {
                Text        = line.Jumlah.ToString(),
                Size        = new Size(52, 38),
                Location    = new Point(rowWidth - 248, 30),
                Font        = new Font("Segoe UI", 13F, FontStyle.Bold),
                TextAlign   = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle,
                Anchor      = AnchorStyles.Top | AnchorStyles.Right
            };
            pnl.Controls.Add(lblJumlah);

            // ---- Tombol TAMBAH (+) ----
            // FITUR STOK: event handler BtnPlus_Click akan cek stok DB sebelum tambah
            var btnPlus = new Button
            {
                Text      = "+",
                Size      = new Size(38, 38),
                Location  = new Point(rowWidth - 192, 30),
                BackColor = Color.FromArgb(230, 230, 230),
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 13F, FontStyle.Bold),
                Cursor    = Cursors.Hand,
                Tag       = line.MenuItemId,
                Anchor    = AnchorStyles.Top | AnchorStyles.Right
            };
            btnPlus.FlatAppearance.BorderSize = 0;
            btnPlus.Click += BtnPlus_Click;
            pnl.Controls.Add(btnPlus);

            // ---- Subtotal (harga × jumlah, warna biru) ----
            var lblSubtotal = new Label
            {
                Text      = "Rp " + line.Subtotal.ToString("N0"),
                Font      = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.RoyalBlue,
                Size      = new Size(160, 38),
                Location  = new Point(rowWidth - 170, 30),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor    = AnchorStyles.Top | AnchorStyles.Right
            };
            pnl.Controls.Add(lblSubtotal);

            return pnl;
        }

        // EVENT HANDLER tombol + (tambah 1 porsi).
        // FITUR STOK: sebelum tambah, cek stok terkini dari DB.
        // Ini mencegah user memesan lebih dari yang tersedia.
        private void BtnPlus_Click(object sender, EventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            if (!CartManager.Items.ContainsKey(id)) return;

            try
            {
                // Cek stok terkini dari DB (bukan dari cache)
                var semuaMenu = DatabaseHelper.GetAllMenuItems();
                int stokDb = 0;
                string namaMenu = CartManager.Items[id].Nama;

                foreach (var m in semuaMenu)
                {
                    if (m.Id == id) { stokDb = m.Stock; break; }
                }

                int jumlahSekarang = CartManager.Items[id].Jumlah;

                // SELECTION: cek apakah jumlah di keranjang sudah mentok stok
                if (jumlahSekarang >= stokDb)
                {
                    MessageBox.Show(
                        $"Jumlah '{namaMenu}' di keranjang sudah mencapai batas stok ({stokDb} porsi).",
                        "Batas Stok", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;   // batalkan penambahan
                }
            }
            catch
            {
                // Kalau DB tidak bisa diakses saat cek stok, biarkan lanjut
                // (tidak blokir user karena masalah koneksi sementara)
            }

            // Stok masih cukup → tambah 1 porsi
            CartManager.SetJumlah(id, CartManager.Items[id].Jumlah + 1);
            RenderCart();
        }

        // EVENT HANDLER tombol - (kurangi 1 porsi).
        // CartManager.SetJumlah otomatis hapus item dari Dictionary kalau jumlah jadi 0.
        private void BtnMin_Click(object sender, EventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            if (CartManager.Items.ContainsKey(id))
            {
                CartManager.SetJumlah(id, CartManager.Items[id].Jumlah - 1);
                RenderCart();
            }
        }

        // EVENT HANDLER tombol Kembali: tutup form cart dan kembali ke Menu_Screen
        private void BttnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // EVENT HANDLER tombol Checkout:
        // Buka Payment sebagai modal. Setelah bayar, kalau keranjang sudah kosong
        // (pembayaran berhasil), tutup cart juga.
        private void BttnCheckout_Click(object sender, EventArgs e)
        {
            // Guard clause: tidak bisa checkout keranjang kosong
            if (CartManager.Items.Count == 0)
            {
                MessageBox.Show("Keranjang kosong, tidak bisa checkout.",
                                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var pay = new Payment())
            {
                this.Hide();        // sembunyikan cart selama payment tampil
                pay.ShowDialog();
                this.Show();        // tampilkan kembali setelah payment selesai
            }

            // SELECTION: cek hasil pembayaran berdasarkan isi keranjang
            if (CartManager.Items.Count == 0)
                // Keranjang kosong = pembayaran berhasil → tutup cart, balik ke menu
                this.Close();
            else
                // Keranjang masih ada = pembayaran dibatalkan → render ulang
                RenderCart();
        }

        // EVENT HANDLER Paint panel bawah: gambar garis separator di atas panel total.
        private void PnlBottom_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            using (var pen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(230, 228, 224), 1))
                e.Graphics.DrawLine(pen, 0, 0, ((System.Windows.Forms.Panel)sender).Width, 0);
        }
    }
}
