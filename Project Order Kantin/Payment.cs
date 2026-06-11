// =================================================================
// WEEK 8  - GUI : Form Payment untuk pilih metode pembayaran
// WEEK 9  - ADO.NET: SaveOrder menyimpan order + order_items ke DB
// WEEK 14 - File Stream: ExportInvoice (.txt) dan LogTransaction (.log)
// FITUR STOK: Validasi stok terakhir sebelum order disimpan ke DB
// =================================================================
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Project_Order_Kantin
{
    // Form Payment: langkah terakhir sebelum transaksi dikonfirmasi.
    // Menampilkan total, pilihan metode pembayaran, dan tombol bayar.
    //
    // Ini adalah lapisan validasi stok terakhir (last line of defense):
    // sebelum order disimpan ke database, stok setiap item dicek ulang
    // dari database untuk memastikan masih mencukupi.
    public partial class Payment : Form
    {
        // CONSTRUCTOR: pasang event handler Load dan tombol-tombol
        public Payment()
        {
            InitializeComponent();
            this.Load          += Payment_Load;
            this.btnBayar.Click += BtnBayar_Click;

            // Tombol Batal: tutup form, kembali ke Cart_Screen tanpa simpan apapun
            this.btnBatal.Click += (s, e) => this.Close();
        }

        // EVENT HANDLER Form.Load: tampilkan total harga dari keranjang
        private void Payment_Load(object sender, EventArgs e)
        {
            // CartManager.Total() = fungsi WEEK 5 yang menjumlahkan semua Subtotal
            lblTotal.Text = "Total: Rp " + CartManager.Total().ToString("N0");
        }

        // EVENT HANDLER tombol "Bayar Sekarang".
        //
        // ALUR LENGKAP:
        //   1. Validasi metode pembayaran dipilih
        //   2. FITUR STOK: cek stok semua item dari database (last defense)
        //   3. Simpan order ke database dalam satu transaction (ADO.NET WEEK 9-10)
        //   4. Export invoice ke file .txt di folder Invoices (FileStream WEEK 14)
        //   5. Catat log transaksi ke file .log di folder Logs (FileStream WEEK 14)
        //   6. Kosongkan keranjang dan tutup form
        private void BtnBayar_Click(object sender, EventArgs e)
        {
            // VALIDASI: metode pembayaran wajib dipilih dulu
            string metode = cmbMetode.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(metode))
            {
                MessageBox.Show("Pilih metode pembayaran.", "Info");
                return;
            }

            // ============================================================
            // FITUR STOK – Lapisan Validasi Ke-4 (terakhir)
            //
            // Lapisan validasi stok yang sudah ada sebelumnya:
            //   1. SearchMenuLinq()  → menu stok 0 tidak ditampilkan di kartu
            //   2. BuildMenuCard()   → tombol tambah dinonaktifkan kalau stok 0
            //   3. BtnPlus_Click()   → penambahan jumlah dibatasi sesuai stok DB
            //   4. Di sini           → cek ulang semua item sebelum simpan order
            //
            // Kenapa perlu 4 lapisan? Race condition: bisa saja stok habis
            // antara saat user pilih menu dan saat user klik bayar.
            // ============================================================
            try
            {
                // ADO.NET: ambil data stok terbaru dari database
                var semuaMenu = DatabaseHelper.GetAllMenuItems();

                // Buat Dictionary id → stok untuk lookup O(1) saat loop item keranjang
                var stokDb = new System.Collections.Generic.Dictionary<int, int>();
                foreach (var m in semuaMenu)
                    stokDb[m.Id] = m.Stock;

                // Kumpulkan semua item bermasalah dalam satu pesan
                // (lebih baik tampilkan semua sekaligus daripada satu-satu)
                var pesanStokHabis = new StringBuilder();

                foreach (var line in CartManager.Items.Values)
                {
                    if (stokDb.ContainsKey(line.MenuItemId))
                    {
                        int stokTersedia = stokDb[line.MenuItemId];

                        if (stokTersedia <= 0)
                        {
                            // Stok item ini habis total
                            pesanStokHabis.AppendLine($"• {line.Nama}: STOK HABIS");
                        }
                        else if (line.Jumlah > stokTersedia)
                        {
                            // Jumlah yang dipesan melebihi stok yang tersisa
                            pesanStokHabis.AppendLine(
                                $"• {line.Nama}: dipesan {line.Jumlah} porsi, " +
                                $"stok tersisa {stokTersedia} porsi");
                        }
                    }
                }

                // Ada item bermasalah → batalkan pembayaran, minta user sesuaikan keranjang
                if (pesanStokHabis.Length > 0)
                {
                    MessageBox.Show(
                        "Pembayaran dibatalkan.\nBeberapa item stoknya tidak mencukupi:\n\n" +
                        pesanStokHabis.ToString() +
                        "\nSilakan kembali ke keranjang dan sesuaikan pesanan Anda.",
                        "Stok Tidak Mencukupi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;   // HENTIKAN proses, jangan simpan order
                }
            }
            catch
            {
                // DB tidak bisa diakses saat cek stok – lanjutkan saja,
                // SaveOrder di bawah akan catch error yang lebih serius
            }

            // ============================================================
            // Stok oke → proses pembayaran
            // ============================================================
            try
            {
                int   total = CartManager.Total();
                var   lines = CartManager.Items.Values;

                // ADO.NET Transaction (WEEK 9 & 10):
                // Simpan order + order_items + update stok dalam satu transaction.
                // Kalau salah satu gagal, semua di-rollback.
                var order = DatabaseHelper.SaveOrder(total, metode, lines);

                // WEEK 14 – FILE STREAM: buat invoice .txt di folder Invoices/
                // Nama file: INV-{id}-{timestamp}.txt
                FileStreamHelper.ExportInvoice(order, lines);

                // WEEK 14 – FILE STREAM: tambahkan entri ke log harian di folder Logs/
                // File log di-append (tidak ditimpa), satu file per hari
                FileStreamHelper.LogTransaction(order);

                // Kosongkan keranjang setelah transaksi berhasil tersimpan
                CartManager.ClearCart();

                MessageBox.Show(
                    $"Pembayaran berhasil!\n" +
                    $"No. Order : {order.OrderNumber}\n" +
                    $"Total     : Rp {order.Total:N0}\n" +
                    $"Metode    : {order.MetodePembayaran}\n\n" +
                    $"Invoice tersimpan di folder Invoices.",
                    "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tutup form Payment → Cart_Screen mendeteksi keranjang kosong
                // dan ikut menutup diri, kembali ke Menu_Screen
                this.Close();
            }
            catch (Exception ex)
            {
                // Error saat simpan ke database (koneksi putus, dll.)
                MessageBox.Show("Gagal simpan order: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
