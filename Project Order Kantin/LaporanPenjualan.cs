using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace Project_Order_Kantin
{
    // Form LaporanPenjualan: menampilkan laporan penjualan menggunakan Crystal Reports.
    //
    // WEEK 12 & 13 - Crystal Reports:
    //   - File .rpt (LaporanPenjualan.rpt) adalah template laporan yang dibuat
    //     di Crystal Reports Designer Visual Studio.
    //   - Data dari database diambil via GetLaporanPenjualan() lalu dimasukkan
    //     ke DataSet (dsLaporanPenjualan) yang kemudian dikirim ke ReportDocument.
    //   - Filter tersedia: rentang tanggal, kategori menu, dan metode pembayaran.
    //
    // Alur kerja Crystal Reports:
    //   1. User pilih filter → klik Tampilkan
    //   2. Ambil data dari DB (DatabaseHelper.GetLaporanPenjualan)
    //   3. Masukkan data ke typed DataSet (dsLaporanPenjualan)
    //   4. Load file .rpt dan set DataSource-nya
    //   5. CrystalReportViewer merender laporan di layar
    public partial class LaporanPenjualan : Form
    {
        // ReportDocument = objek utama Crystal Reports untuk load .rpt dan set data.
        // Di-dispose saat form ditutup (OnFormClosed) agar tidak ada memory leak.
        private ReportDocument _report = new ReportDocument();

        // CONSTRUCTOR: pasang event handler Load dan tombol-tombol
        public LaporanPenjualan()
        {
            InitializeComponent();
            this.Load += LaporanPenjualan_Load;
            this.btnTampilkan.Click += BtnTampilkan_Click;
            this.btnTutup.Click     += (s, e) => this.Close();
        }

        // EVENT HANDLER: inisialisasi filter saat form pertama dibuka.
        // Default range tanggal = satu bulan terakhir sampai hari ini.
        private void LaporanPenjualan_Load(object sender, EventArgs e)
        {
            dtpDari.Value   = DateTime.Today.AddMonths(-1);  // satu bulan lalu
            dtpSampai.Value = DateTime.Today;                // hari ini

            // Isi ComboBox Kategori dengan data dari database
            cmbKategori.Items.Add("-- Semua --");
            try
            {
                foreach (var k in DatabaseHelper.GetCategories())
                    cmbKategori.Items.Add(k.Nama);
            }
            catch { /* abaikan jika database tidak bisa diakses saat load */ }
            cmbKategori.SelectedIndex = 0;  // default = semua kategori

            // Isi ComboBox Metode Pembayaran (data statis, tidak dari DB)
            cmbMetode.Items.AddRange(new string[]
                { "-- Semua --", "Cash", "QRIS", "Transfer Bank", "Debit", "Kredit" });
            cmbMetode.SelectedIndex = 0;

            // Langsung tampilkan laporan dengan filter default
            TampilkanLaporan();
        }

        // EVENT HANDLER: tampilkan ulang laporan saat tombol "Tampilkan" diklik
        private void BtnTampilkan_Click(object sender, EventArgs e)
        {
            TampilkanLaporan();
        }

        // PROCEDURE: inti dari fitur laporan – ambil data, set ke Crystal Reports, render.
        private void TampilkanLaporan()
        {
            try
            {
                // Cari file .rpt di folder yang sama dengan .exe
                string rptPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "LaporanPenjualan.rpt");

                // Cek apakah file .rpt sudah ada dan ukurannya wajar (> 100 byte)
                // File .rpt yang valid biasanya berukuran puluhan KB atau lebih.
                if (!File.Exists(rptPath) || new FileInfo(rptPath).Length < 100)
                {
                    MessageBox.Show(
                        "File laporan 'LaporanPenjualan.rpt' belum tersedia atau belum dikonfigurasi.\n\n" +
                        "Silakan buat file .rpt melalui Crystal Reports Designer di Visual Studio,\n" +
                        "kemudian letakkan di folder: " + AppDomain.CurrentDomain.BaseDirectory,
                        "File Laporan Tidak Ditemukan",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Baca nilai filter dari ComboBox
                // Nilai "-- Semua --" = tidak ada filter, set ke null
                string kategori = cmbKategori.SelectedItem?.ToString();
                if (kategori == "-- Semua --") kategori = null;

                string metode = cmbMetode.SelectedItem?.ToString();
                if (metode == "-- Semua --") metode = null;

                // Ambil data laporan dari database dengan filter yang dipilih
                // Hasilnya berupa DataTable yang berisi semua transaksi yang cocok
                var dt = DatabaseHelper.GetLaporanPenjualan(
                    dtpDari.Value, dtpSampai.Value, kategori, metode);

                // Masukkan DataTable ke dalam typed DataSet (dsLaporanPenjualan).
                // Crystal Reports butuh DataSet bertipe khusus yang field-nya cocok
                // dengan nama kolom di file .rpt. dsLaporanPenjualan sudah dibuat
                // di Visual Studio agar field-fieldnya sesuai.
                var ds = new dsLaporanPenjualan();
                ds.LaporanPenjualan.Merge(dt);   // salin data dari DataTable biasa ke typed table

                // Load template .rpt dan set DataSource-nya ke DataSet yang sudah terisi
                _report.Load(rptPath);
                _report.SetDataSource(ds);

                // Tampilkan laporan di CrystalReportViewer yang sudah ada di form (dari Designer)
                crystalReportViewer.ReportSource = _report;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load laporan: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Override OnFormClosed untuk memastikan ReportDocument di-dispose dengan benar.
        // Crystal Reports punya resource internal (koneksi, memori) yang harus dilepas
        // secara eksplisit, tidak cukup hanya mengandalkan garbage collector.
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _report.Close();
            _report.Dispose();
            base.OnFormClosed(e);
        }
    }
}
