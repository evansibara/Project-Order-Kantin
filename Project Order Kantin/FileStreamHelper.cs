using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Project_Order_Kantin
{
    // =================================================================
    // WEEK 14 - File Stream
    // Kelas ini mengurus semua operasi baca-tulis file:
    //   1. Export Invoice (.txt) per transaksi   – StreamWriter
    //   2. Log transaksi harian (.log)            – FileStream + StreamWriter (append)
    //   3. Backup semua menu ke CSV               – StreamWriter
    //   4. Baca log hari ini untuk ditampilkan    – StreamReader
    // =================================================================

    // FileStreamHelper adalah kelas static – tidak perlu di-instansiasi,
    // cukup panggil langsung: FileStreamHelper.ExportInvoice(...)
    // Semua file disimpan di subfolder dalam folder aplikasi (BaseDir).
    public static class FileStreamHelper
    {
        // AppDomain.CurrentDomain.BaseDirectory = folder tempat .exe berjalan.
        // Di mode Debug: biasanya ...bin\Debug\
        // Ini adalah root semua folder file (Invoices, Logs, Backup).
        private static string BaseDir => AppDomain.CurrentDomain.BaseDirectory;

        // ============================================================
        // Export Invoice (.txt) — menggunakan StreamWriter
        // Setiap transaksi menghasilkan satu file invoice tersendiri.
        // ============================================================

        // PROCEDURE: buat file invoice teks untuk satu order yang baru selesai.
        // File disimpan di folder "Invoices" dengan nama unik berdasarkan Id dan waktu.
        // Return value = path lengkap file yang dibuat (untuk ditampilkan ke user).
        public static string ExportInvoice(Order order, IEnumerable<CartLine> lines)
        {
            // Buat folder Invoices kalau belum ada
            // Directory.CreateDirectory aman dipanggil berulang – tidak error kalau sudah ada
            string folder = Path.Combine(BaseDir, "Invoices");
            Directory.CreateDirectory(folder);

            // Nama file unik: INV-{id}-{timestamp}.txt, contoh: INV-5-20260615_143022.txt
            string fileName = $"INV-{order.Id}-{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string path     = Path.Combine(folder, fileName);

            // StreamWriter: nulis teks ke file. Parameter kedua (false) = tidak append,
            // artinya file baru dibuat dari awal (bukan ditambah ke akhir file lama).
            // Encoding.UTF8 supaya karakter Indonesia (é, ñ, dll.) tersimpan benar.
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                sw.WriteLine("==============================");
                sw.WriteLine("       KANTIN INVOICE         ");
                sw.WriteLine("==============================");
                sw.WriteLine($"No. Order   : {order.OrderNumber}");
                sw.WriteLine($"Tanggal     : {order.CreatedAt:dd/MM/yyyy HH:mm}");
                sw.WriteLine($"Pembayaran  : {order.MetodePembayaran}");
                sw.WriteLine("------------------------------");

                // Loop setiap item di keranjang dan tulis baris detail-nya
                foreach (var line in lines)
                    sw.WriteLine($"  {line.Nama,-25} x{line.Jumlah}  Rp {line.Subtotal:N0}");

                sw.WriteLine("------------------------------");
                sw.WriteLine($"  TOTAL                        Rp {order.Total:N0}");
                sw.WriteLine("==============================");
                sw.WriteLine("   Terima kasih sudah memesan!");
                sw.WriteLine("==============================");
            }
            return path;
        }

        // ============================================================
        // Log Transaksi Harian — menggunakan FileStream (mode Append)
        // Satu file log per hari: transaksi_20260615.log
        // Setiap panggilan MENAMBAHKAN satu baris di akhir file yang sudah ada,
        // tidak menimpa isi lama.
        // ============================================================

        // PROCEDURE: catat satu transaksi ke file log hari ini.
        // FileMode.Append = buka file dan taruh cursor di akhir, lalu tulis.
        // Berbeda dengan StreamWriter biasa yang default-nya timpa file.
        public static void LogTransaction(Order order)
        {
            string folder = Path.Combine(BaseDir, "Logs");
            Directory.CreateDirectory(folder);

            // Nama file berdasarkan tanggal hari ini, jadi tiap hari bikin file baru
            string fileName = $"transaksi_{DateTime.Today:yyyyMMdd}.log";
            string path     = Path.Combine(folder, fileName);

            // Format entri log: [HH:mm:ss] Order=#1001 Total=Rp50.000 Metode=Cash
            string entry = $"[{DateTime.Now:HH:mm:ss}] Order={order.OrderNumber} " +
                           $"Total=Rp{order.Total:N0} Metode={order.MetodePembayaran}" +
                           Environment.NewLine;

            // FileStream dengan FileMode.Append → hanya menambah di akhir, tidak timpa
            // FileAccess.Write → akses tulis saja (tidak perlu baca)
            using (var fs = new FileStream(path, FileMode.Append, FileAccess.Write))
            using (var sw = new StreamWriter(fs, Encoding.UTF8))
            {
                sw.Write(entry);
            }
        }

        // ============================================================
        // Backup Menu ke CSV — StreamWriter
        // Backup snapshot semua menu saat ini ke format CSV
        // yang bisa dibuka di Excel.
        // ============================================================

        // PROCEDURE: ekspor semua data menu ke file .csv.
        // Nama file menggunakan timestamp agar setiap backup punya nama unik
        // dan tidak menimpa backup sebelumnya.
        // Return value = path file backup yang dibuat.
        public static string BackupMenuToCsv(IEnumerable<MenuItemModel> items)
        {
            string folder = Path.Combine(BaseDir, "Backup");
            Directory.CreateDirectory(folder);

            // Contoh nama: backup_menu_20260615_143022.csv
            string fileName = $"backup_menu_{DateTime.Today:yyyyMMdd}_{DateTime.Now:HHmmss}.csv";
            string path     = Path.Combine(folder, fileName);

            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                // Header baris pertama CSV
                sw.WriteLine("id,nama,harga,kategori,gambar_url,tersedia");

                // Tulis satu baris per item menu
                // Kolom teks dibungkus tanda kutip ganda agar koma dalam nama
                // tidak dianggap sebagai pemisah kolom CSV.
                foreach (var m in items)
                    sw.WriteLine($"{m.Id},\"{m.Nama}\",{m.Harga},\"{m.Kategori}\"," +
                                 $"\"{m.GambarUrl}\",{m.Tersedia}");
            }
            return path;
        }

        // ============================================================
        // Baca Log Hari Ini — StreamReader
        // Ditampilkan di AdminDashboard saat tombol "Lihat Log" diklik.
        // ============================================================

        // FUNCTION: baca seluruh isi file log hari ini sebagai satu string.
        // Jika file belum ada (belum ada transaksi hari ini), kembalikan pesan info.
        public static string ReadLogToday()
        {
            string folder   = Path.Combine(BaseDir, "Logs");
            string fileName = $"transaksi_{DateTime.Today:yyyyMMdd}.log";
            string path     = Path.Combine(folder, fileName);

            // Guard clause: kalau file tidak ada, tidak perlu lanjut
            if (!File.Exists(path))
                return "Belum ada transaksi hari ini.";

            // StreamReader.ReadToEnd() membaca semua isi file sekaligus ke satu string.
            // Cocok untuk file log yang ukurannya tidak terlalu besar.
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
