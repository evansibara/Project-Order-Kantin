using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_Order_Kantin
{
    // Helper untuk download dan cache gambar menu dari Supabase Storage.
    //
    // Kenapa perlu helper ini?
    // Gambar menu disimpan di cloud (Supabase Storage), bukan di lokal.
    // Setiap kali kartu menu dirender, gambar harus diunduh dari internet.
    // Kalau langsung diunduh di UI thread, aplikasi jadi freeze sampai
    // semua gambar selesai didownload.
    //
    // Solusinya dua:
    //   1. Async: download di background thread, update PictureBox setelah selesai
    //   2. Cache: simpan file gambar ke disk, pakai lagi di-run berikutnya
    //      tanpa perlu download ulang (hemat bandwidth + lebih cepat)
    //
    // Cache disimpan di folder %TEMP%\KantinImageCache\ (folder temp Windows).
    public static class SupabaseImageHelper
    {
        // Path folder cache di disk. Path.GetTempPath() = folder %TEMP% Windows.
        // Tiap user Windows punya folder temp sendiri, jadi tidak tabrakan.
        private static readonly string CacheDir =
            Path.Combine(Path.GetTempPath(), "KantinImageCache");

        // Static constructor: dijalankan sekali waktu kelas pertama kali diakses.
        // Pastikan folder cache sudah ada sebelum digunakan.
        static SupabaseImageHelper()
        {
            Directory.CreateDirectory(CacheDir);
        }

        // PROCEDURE: load gambar dari URL ke PictureBox secara asinkron (non-blocking).
        // Alurnya:
        //   1. Cek apakah gambar sudah ada di cache disk
        //   2. Kalau ada → langsung load dari cache (cepat)
        //   3. Kalau belum → download dari URL, simpan ke cache, lalu tampilkan
        //   4. Update PictureBox dilakukan di UI thread via Invoke() agar aman
        public static void LoadImageAsync(string url, PictureBox target)
        {
            // Guard clause: kalau URL kosong, tidak perlu download apa-apa
            if (string.IsNullOrWhiteSpace(url)) return;

            // Task.Run() = jalankan kode berikut di background thread (bukan UI thread).
            // UI tetap responsif selama download berlangsung.
            Task.Run(() =>
            {
                try
                {
                    string cachePath = GetCachePath(url);
                    Image img;

                    if (File.Exists(cachePath))
                    {
                        // Cache hit: gambar sudah pernah didownload, pakai dari disk langsung.
                        // Ini menghindari request internet yang tidak perlu.
                        img = Image.FromFile(cachePath);
                    }
                    else
                    {
                        // Cache miss: download gambar dari internet.
                        // WebClient.DownloadData() mengunduh file sebagai array byte.
                        // Lalu disimpan ke disk cache untuk pemakaian berikutnya.
                        using (var wc = new WebClient())
                        {
                            byte[] data = wc.DownloadData(url);

                            // Simpan ke cache disk dulu
                            File.WriteAllBytes(cachePath, data);

                            // Buat objek Image dari array byte (bukan dari file),
                            // karena file yang baru ditulis mungkin masih terkunci.
                            using (var ms = new MemoryStream(data))
                            {
                                img = Image.FromStream(ms);
                            }
                        }
                    }

                    // Cek apakah PictureBox masih valid sebelum update UI.
                    // Bisa saja form sudah ditutup sementara download masih jalan,
                    // sehingga IsDisposed = true. Kalau langsung Invoke ke disposed
                    // control, aplikasi akan crash.
                    if (target.IsHandleCreated && !target.IsDisposed)
                    {
                        // Invoke() = pindah eksekusi ke UI thread.
                        // WinForms melarang update kontrol dari thread selain UI thread.
                        // (Action)() => { ... } = lambda yang dibungkus sebagai delegate Action
                        target.Invoke((Action)(() =>
                        {
                            target.Image = img;
                        }));
                    }
                }
                catch
                {
                    // Kalau download gagal (no internet, URL salah, dll.),
                    // biarkan PictureBox tetap menampilkan background abu-abu bawaan.
                    // Tidak perlu crash aplikasi hanya karena satu gambar gagal load.
                }
            });
        }

        // FUNCTION (private): buat nama file cache yang unik berdasarkan URL.
        // URL tidak bisa langsung dijadikan nama file karena mengandung karakter
        // yang tidak valid (/, :, ?, =, dll.). Solusinya: encode URL ke Base64,
        // lalu ganti karakter Base64 yang tidak valid di nama file.
        // Panjang maksimal dibatasi 120 karakter supaya path tidak terlalu panjang.
        private static string GetCachePath(string url)
        {
            string safeName = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(url))
                .Replace("/", "_").Replace("+", "-").Replace("=", "");
            if (safeName.Length > 120) safeName = safeName.Substring(0, 120);
            return Path.Combine(CacheDir, safeName + ".png");
        }

        // PROCEDURE: hapus semua file cache gambar.
        // Berguna kalau gambar di Supabase diupdate tapi masih tampil versi lama.
        // Bisa dipanggil dari menu Settings atau saat logout.
        public static void ClearCache()
        {
            foreach (var f in Directory.GetFiles(CacheDir))
                try { File.Delete(f); } catch { }
        }
    }
}
