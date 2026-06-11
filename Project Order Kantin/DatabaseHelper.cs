using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Project_Order_Kantin
{
    // DatabaseHelper adalah kelas static yang menjadi satu-satunya pintu masuk
    // ke database. Semua form yang butuh data berbicara ke sini, bukan langsung
    // membuat koneksi sendiri.
    //
    // Keuntungan pola ini (Repository/Helper Pattern):
    //   - Kalau struktur SQL berubah, cukup ubah di satu tempat
    //   - Setiap method punya tanggung jawab yang jelas (satu method = satu operasi)
    //   - Pola 'using' di setiap method memastikan koneksi selalu ditutup otomatis
    //
    // WEEK 9  - ADO.NET I : SqlConnection, SqlCommand, ExecuteReader (READ)
    // WEEK 10 - ADO.NET II: ExecuteNonQuery (INSERT/UPDATE/DELETE),
    //           SqlTransaction (SaveOrder), SqlCommand.CommandType = StoredProcedure
    // WEEK 11 - LINQ      : SearchMenuLinq (Where, OrderBy, ToList)
    public static class DatabaseHelper
    {
        // Connection string diambil dari App.config, bukan di-hardcode di sini.
        // Alasannya: kalau nama server atau kredensial berubah, cukup edit App.config
        // tanpa perlu recompile kode. Ini juga lebih aman (password tidak ada di kode).
        // Name "KantinDB" harus cocok dengan atribut name di App.config.
        private static string ConnStr
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["KantinDB"];
                if (cs == null)
                    throw new InvalidOperationException(
                        "Connection string 'KantinDB' tidak ditemukan di App.config.");
                return cs.ConnectionString;
            }
        }

        // FUNCTION: coba buka koneksi ke database untuk validasi.
        // Dipanggil oleh Form1 sebelum membuka halaman mana pun,
        // agar error "SQL Server tidak jalan" terdeteksi lebih awal
        // daripada pas user sudah ada di halaman menu.
        // Return: true = berhasil terhubung, false = gagal (pesan error di 'error')
        public static bool TestConnection(out string error)
        {
            error = null;
            try
            {
                using (var con = new SqlConnection(ConnStr))
                {
                    con.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        // =================================================================
        // WEEK 9 - ADO.NET I: READ data dari database
        // Pola umum: buka koneksi → buat command → execute reader →
        //            baca row per row → tutup semua (otomatis via 'using')
        // =================================================================

        // FUNCTION: ambil semua menu dari database termasuk kolom 'stock'.
        // Kolom stock penting untuk cek stok habis di kartu menu.
        // Dipakai oleh: LoadMenu di AdminDashboard, cart screen, backup.
        public static List<MenuItemModel> GetAllMenuItems()
        {
            var list = new List<MenuItemModel>();

            // Query mengambil semua kolom yang dibutuhkan, termasuk 'stock'
            // yang tidak ada di versi lama. ORDER BY id supaya urutan konsisten.
            string sql = "SELECT id, nama, harga, kategori, gambar_url, tersedia, stock FROM menu_items ORDER BY id ASC";

            // 'using' memastikan SqlConnection dan SqlCommand di-dispose (ditutup)
            // secara otomatis saat blok selesai, bahkan kalau ada exception.
            // Ini mencegah connection leak yang bisa bikin database kehabisan koneksi.
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    // ExecuteReader() mengembalikan SqlDataReader – kursor yang bergerak
                    // maju satu baris per kali (forward-only cursor).
                    // rd.Read() = geser ke baris berikutnya, return false kalau habis.
                    while (rd.Read())
                    {
                        list.Add(new MenuItemModel(
                            Convert.ToInt32(rd["id"]),
                            rd["nama"].ToString(),
                            Convert.ToInt32(rd["harga"]),
                            rd["kategori"].ToString(),
                            rd["gambar_url"] == DBNull.Value ? "" : rd["gambar_url"].ToString(),
                            Convert.ToBoolean(rd["tersedia"]),
                            // rd["stock"] == DBNull.Value → data lama tidak punya kolom stock,
                            // default ke 0 agar tidak error
                            rd["stock"] == DBNull.Value ? 0 : Convert.ToInt32(rd["stock"])
                        ));
                    }
                }
            }
            return list;
        }

        // FUNCTION: ambil menu yang tersedia (tersedia = 1) saja,
        // dengan filter kategori opsional.
        // Parameter 'kategori = null' berarti opsional – kalau tidak diisi,
        // query berjalan tanpa filter kategori.
        public static List<MenuItemModel> GetMenuItems(string kategori = null)
        {
            var list = new List<MenuItemModel>();
            string sql = "SELECT id, nama, harga, kategori, gambar_url, tersedia " +
                         "FROM menu_items WHERE tersedia = 1";

            // Tambahkan kondisi kategori ke SQL hanya kalau ada nilainya
            if (!string.IsNullOrEmpty(kategori))
                sql += " AND kategori = @kategori";
            sql += " ORDER BY id ASC";

            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                // Parameter @kategori ditambahkan ke command HANYA kalau ada nilainya.
                // Ini mencegah SQL Injection – nilai tidak digabung langsung ke string SQL.
                if (!string.IsNullOrEmpty(kategori))
                    cmd.Parameters.AddWithValue("@kategori", kategori);

                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new MenuItemModel(
                            Convert.ToInt32(rd["id"]),
                            rd["nama"].ToString(),
                            Convert.ToInt32(rd["harga"]),
                            rd["kategori"].ToString(),
                            rd["gambar_url"] == DBNull.Value ? "" : rd["gambar_url"].ToString(),
                            Convert.ToBoolean(rd["tersedia"])
                        ));
                    }
                }
            }
            return list;
        }

        // =================================================================
        // WEEK 11 - LINQ: Filter dan Sort di memori (in-memory)
        // Data sudah diambil dari DB, lalu difilter/diurutkan pakai LINQ
        // tanpa perlu bolak-balik ke database.
        // =================================================================

        // FUNCTION: cari dan filter menu dengan LINQ.
        // Dipanggil oleh Menu_Screen.FilterMenu() setiap kali user
        // mengetik di kolom pencarian atau mengubah filter.
        //
        // LINQ WHERE digunakan untuk tiga kondisi:
        //   1. Hanya menu yang tersedia (tersedia = true)
        //   2. Hanya menu yang stok masih ada (Stock > 0)
        //   3. Filter kategori (opsional)
        //   4. Filter keyword di nama (opsional, case-insensitive)
        public static List<MenuItemModel> SearchMenuLinq(string keyword, string kategori,
                                                        string sortBy)
        {
            var semua = GetAllMenuItems();

            // LINQ WHERE: kombinasi dua kondisi wajib (tersedia DAN stok > 0).
            // Menu yang tidak tersedia atau stok habis tidak ditampilkan ke pelanggan.
            // IEnumerable<T> sebagai tipe variabel memungkinkan chaining query LINQ
            // tanpa eksekusi di setiap langkah (deferred execution).
            IEnumerable<MenuItemModel> q = semua.Where(m => m.Tersedia && m.Stock > 0);

            // Filter kategori (opsional) – hanya tambahkan kalau ada nilainya
            if (!string.IsNullOrEmpty(kategori) && kategori != "Semua")
                q = q.Where(m => m.Kategori == kategori);

            // Filter keyword (opsional) – Contains() = seperti LIKE '%keyword%' di SQL
            // ToLower() di kedua sisi = case-insensitive search
            if (!string.IsNullOrEmpty(keyword))
            {
                string k = keyword.ToLower();
                q = q.Where(m => m.Nama.ToLower().Contains(k));
            }

            // Sort berdasarkan pilihan user di ComboBox
            // OrderBy = ascending, OrderByDescending = descending
            switch (sortBy)
            {
                case "Harga Termurah":  q = q.OrderBy(m => m.Harga); break;
                case "Harga Termahal":  q = q.OrderByDescending(m => m.Harga); break;
                case "Nama A-Z":        q = q.OrderBy(m => m.Nama); break;
                case "Nama Z-A":        q = q.OrderByDescending(m => m.Nama); break;
                default:                q = q.OrderBy(m => m.Id); break;  // urutan default dari DB
            }

            // ToList() = eksekusi semua query LINQ yang tertunda dan hasilkan List
            return q.ToList();
        }

        // FUNCTION: filter menu berdasarkan harga maksimal.
        // Contoh penggunaan: tampilkan menu yang bisa dibeli dengan budget 20.000.
        // Menggunakan method chaining LINQ yang ringkas.
        public static List<MenuItemModel> FilterByMaxPrice(int maxPrice)
        {
            return GetAllMenuItems()
                    .Where(m => m.Harga <= maxPrice && m.Tersedia)
                    .ToList();
        }

        // =================================================================
        // WEEK 10 - ADO.NET II: CREATE, UPDATE, DELETE (CUD dari CRUD)
        // ExecuteNonQuery() dipakai untuk INSERT/UPDATE/DELETE
        // – operasi yang tidak mengembalikan baris data.
        // =================================================================

        // FUNCTION: tambah menu baru ke database.
        // Return: Id yang digenerate database (SCOPE_IDENTITY()).
        // SCOPE_IDENTITY() lebih aman daripada @@IDENTITY karena tidak
        // terpengaruh trigger yang mungkin insert ke tabel lain.
        public static int InsertMenu(MenuItemModel item)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "INSERT INTO menu_items (nama, harga, kategori, gambar_url, tersedia, stock, stock_minimum) " +
                "VALUES (@nm, @hg, @kt, @gb, @ts, @st, @stmin); SELECT SCOPE_IDENTITY();", con))
            {
                cmd.Parameters.AddWithValue("@nm", item.Nama);
                cmd.Parameters.AddWithValue("@hg", item.Harga);
                cmd.Parameters.AddWithValue("@kt", item.Kategori);
                // (object)item.GambarUrl ?? DBNull.Value = kalau GambarUrl null,
                // kirim DBNull.Value (bukan null C#) ke SQL Server agar tersimpan sebagai NULL
                cmd.Parameters.AddWithValue("@gb", (object)item.GambarUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ts", item.Tersedia);
                cmd.Parameters.AddWithValue("@st", 0);      // stok awal = 0 (diisi lewat StockManager)
                cmd.Parameters.AddWithValue("@stmin", 5);   // stok minimum default = 5
                con.Open();
                // ExecuteScalar() untuk query yang hasilnya satu nilai (SCOPE_IDENTITY)
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // PROCEDURE: update data menu yang sudah ada di database.
        // Tidak mengupdate kolom 'stock' karena stok punya jalur sendiri (StockManager).
        public static void UpdateMenu(MenuItemModel item)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "UPDATE menu_items SET nama=@nm, harga=@hg, kategori=@kt, " +
                "gambar_url=@gb, tersedia=@ts WHERE id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", item.Id);
                cmd.Parameters.AddWithValue("@nm", item.Nama);
                cmd.Parameters.AddWithValue("@hg", item.Harga);
                cmd.Parameters.AddWithValue("@kt", item.Kategori);
                cmd.Parameters.AddWithValue("@gb", (object)item.GambarUrl ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ts", item.Tersedia);
                con.Open();
                // ExecuteNonQuery() untuk INSERT/UPDATE/DELETE – tidak mengembalikan data
                cmd.ExecuteNonQuery();
            }
        }

        // PROCEDURE: hapus menu dari database berdasarkan Id.
        // Catatan: akan error kalau menu sudah pernah diorder (foreign key constraint).
        // Solusi yang lebih baik adalah set Tersedia = false daripada hapus.
        public static void DeleteMenu(int id)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("DELETE FROM menu_items WHERE id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ============ CRUD KATEGORI ============

        // FUNCTION: ambil semua kategori dari database, diurutkan alfabetis.
        public static List<Category> GetCategories()
        {
            var list = new List<Category>();
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT id, nama FROM categories ORDER BY nama ASC", con))
            {
                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new Category(
                            Convert.ToInt32(rd["id"]),
                            rd["nama"].ToString()));
                    }
                }
            }
            return list;
        }

        // FUNCTION: tambah kategori baru ke database.
        // Return: Id kategori baru.
        public static int InsertCategory(string nama)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "INSERT INTO categories (nama) VALUES (@nm); SELECT SCOPE_IDENTITY();", con))
            {
                cmd.Parameters.AddWithValue("@nm", nama);
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // PROCEDURE: hapus kategori berdasarkan Id.
        public static void DeleteCategory(int id)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("DELETE FROM categories WHERE id=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ============ LOGIN ADMIN ============

        // FUNCTION: verifikasi kredensial admin ke database.
        // Return: objek Admin kalau cocok, null kalau tidak cocok.
        // Menggunakan parameterized query untuk mencegah SQL Injection:
        // input user TIDAK digabung langsung ke string SQL.
        public static Admin LoginAdmin(string username, string password)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT id, username, nama FROM users " +
                "WHERE username=@u AND password=@p AND role='Admin'", con))
            {
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        // Ada baris yang cocok = login berhasil
                        return new Admin(
                            Convert.ToInt32(rd["id"]),
                            rd["username"].ToString(),
                            rd["nama"].ToString());
                    }
                }
            }
            // Tidak ada baris yang cocok = login gagal
            return null;
        }

        // =================================================================
        // WEEK 10 - ADO.NET TRANSACTION: SaveOrder
        //
        // Transaction memastikan semua operasi berhasil SEPENUHNYA atau
        // GAGAL SEPENUHNYA (atomik). Tidak ada kondisi setengah-setengah.
        //
        // Tanpa transaction: kalau INSERT order_items berhasil tapi
        // sp_UpdateStock gagal, stok tidak berkurang tapi order sudah tercatat.
        // Data jadi tidak konsisten.
        //
        // Dengan transaction: kalau ada satu yang gagal, ROLLBACK mengembalikan
        // semua perubahan seolah-olah tidak pernah terjadi.
        // =================================================================

        // FUNCTION: simpan satu transaksi order ke database secara atomik.
        // Langkah-langkah di dalam satu transaction:
        //   1. INSERT ke tabel 'orders' → dapatkan newOrderId
        //   2. UPDATE order_number dengan format "#" + (1000 + id)
        //   3. LOOP: INSERT tiap item keranjang ke tabel 'order_items'
        //   4. LOOP: panggil sp_UpdateStock untuk kurangi stok
        //   5. COMMIT (simpan semua) atau ROLLBACK (batalkan semua kalau ada error)
        public static Order SaveOrder(int total, string metodePembayaran, IEnumerable<CartLine> cartLines)
        {
            using (var con = new SqlConnection(ConnStr))
            {
                con.Open();

                // BeginTransaction() = mulai "rekam" semua perubahan.
                // Perubahan belum masuk DB sampai tx.Commit() dipanggil.
                using (var tx = con.BeginTransaction())
                {
                    try
                    {
                        int newOrderId;

                        // Langkah 1: INSERT order baru, ambil Id yang digenerate
                        using (var cmd = new SqlCommand(
                            "INSERT INTO orders (order_number, total, metode_pembayaran, status) " +
                            "VALUES (@on, @tot, @mp, @st); " +
                            "SELECT SCOPE_IDENTITY();", con, tx))  // tx = pakai transaction ini
                        {
                            cmd.Parameters.AddWithValue("@on", "TEMP");    // sementara, diupdate di langkah 2
                            cmd.Parameters.AddWithValue("@tot", total);
                            cmd.Parameters.AddWithValue("@mp", metodePembayaran);
                            cmd.Parameters.AddWithValue("@st", "SELESAI");
                            newOrderId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Langkah 2: UPDATE order_number dengan format yang cantik
                        // Contoh: id=5 → "#1005"
                        string orderNumber = "#" + (1000 + newOrderId);
                        using (var cmd = new SqlCommand(
                            "UPDATE orders SET order_number=@on WHERE id=@id", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@on", orderNumber);
                            cmd.Parameters.AddWithValue("@id", newOrderId);
                            cmd.ExecuteNonQuery();
                        }

                        // Langkah 3 & 4: loop tiap item di keranjang
                        foreach (var line in cartLines)
                        {
                            // INSERT detail item ke order_items
                            using (var cmd = new SqlCommand(
                                "INSERT INTO order_items " +
                                "(order_id, menu_item_id, nama_item, harga_satuan, jumlah, subtotal) " +
                                "VALUES (@oid, @mid, @nm, @hs, @jml, @sub)", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@oid", newOrderId);
                                cmd.Parameters.AddWithValue("@mid", line.MenuItemId);
                                cmd.Parameters.AddWithValue("@nm",  line.Nama);
                                cmd.Parameters.AddWithValue("@hs",  line.HargaSatuan);
                                cmd.Parameters.AddWithValue("@jml", line.Jumlah);
                                cmd.Parameters.AddWithValue("@sub", line.Subtotal);
                                cmd.ExecuteNonQuery();
                            }

                            // Kurangi stok via Stored Procedure sp_UpdateStock.
                            // Dibungkus try-catch tersendiri: kalau SP belum ada di database,
                            // proses tetap lanjut (tidak batalkan seluruh order hanya karena SP).
                            try
                            {
                                using (var cmdStok = new SqlCommand("sp_UpdateStock", con, tx))
                                {
                                    cmdStok.CommandType = CommandType.StoredProcedure;
                                    cmdStok.Parameters.AddWithValue("@menu_item_id", line.MenuItemId);
                                    cmdStok.Parameters.AddWithValue("@jenis", "KELUAR");
                                    cmdStok.Parameters.AddWithValue("@jumlah", line.Jumlah);
                                    cmdStok.Parameters.AddWithValue("@keterangan", "Order " + orderNumber);
                                    cmdStok.ExecuteScalar();
                                }
                            }
                            catch { /* SP belum ada – abaikan, order tetap tersimpan */ }
                        }

                        // Semua langkah berhasil → COMMIT = jadikan semua perubahan permanen
                        tx.Commit();

                        // Kembalikan objek Order sebagai bukti transaksi berhasil
                        return new Order
                        {
                            Id = newOrderId,
                            OrderNumber = orderNumber,
                            Total = total,
                            MetodePembayaran = metodePembayaran,
                            Status = "SELESAI",
                            CreatedAt = DateTime.Now
                        };
                    }
                    catch
                    {
                        // Ada yang gagal → ROLLBACK = batalkan SEMUA perubahan dalam transaction ini
                        // Database kembali ke kondisi sebelum SaveOrder dipanggil
                        tx.Rollback();
                        throw;  // lempar ulang exception agar pemanggil tahu ada error
                    }
                }
            }
        }

        // FUNCTION: ambil daftar order dengan filter tanggal opsional.
        // Dipakai di AdminDashboard (tab Pesanan) dan LaporanPenjualan.
        // StringBuilder dipakai untuk bangun SQL secara dinamis agar lebih bersih
        // daripada string concatenation dengan banyak if.
        public static List<Order> GetOrders(DateTime? dari = null, DateTime? sampai = null)
        {
            var list = new List<Order>();
            var sql  = new StringBuilder();
            sql.Append("SELECT id, order_number, total, metode_pembayaran, status, created_at ");
            sql.Append("FROM orders WHERE 1=1 ");  // "WHERE 1=1" = kondisi selalu true, agar AND berikutnya valid

            if (dari.HasValue)   sql.Append("AND created_at >= @dari ");
            if (sampai.HasValue) sql.Append("AND created_at <= @sampai ");
            sql.Append("ORDER BY created_at DESC");  // terbaru di atas

            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql.ToString(), con))
            {
                if (dari.HasValue)   cmd.Parameters.AddWithValue("@dari", dari.Value.Date);
                if (sampai.HasValue) cmd.Parameters.AddWithValue("@sampai",
                                        sampai.Value.Date.AddDays(1).AddSeconds(-1));  // sampai akhir hari

                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new Order
                        {
                            Id               = Convert.ToInt32(rd["id"]),
                            OrderNumber      = rd["order_number"].ToString(),
                            Total            = Convert.ToInt32(rd["total"]),
                            MetodePembayaran = rd["metode_pembayaran"].ToString(),
                            Status           = rd["status"].ToString(),
                            CreatedAt        = Convert.ToDateTime(rd["created_at"])
                        });
                    }
                }
            }
            return list;
        }

        // FUNCTION: ambil detail item dari satu order tertentu.
        // Dipakai di AdminDashboard saat user klik satu baris di tabel pesanan.
        public static List<OrderItem> GetOrderItems(int orderId)
        {
            var list = new List<OrderItem>();
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "SELECT id, order_id, menu_item_id, nama_item, harga_satuan, jumlah, subtotal " +
                "FROM order_items WHERE order_id=@id ORDER BY id", con))
            {
                cmd.Parameters.AddWithValue("@id", orderId);
                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new OrderItem
                        {
                            Id          = Convert.ToInt32(rd["id"]),
                            OrderId     = Convert.ToInt32(rd["order_id"]),
                            // menu_item_id bisa NULL kalau menu sudah dihapus dari database
                            MenuItemId  = rd["menu_item_id"] == DBNull.Value ? 0
                                            : Convert.ToInt32(rd["menu_item_id"]),
                            NamaItem    = rd["nama_item"].ToString(),
                            HargaSatuan = Convert.ToInt32(rd["harga_satuan"]),
                            Jumlah      = Convert.ToInt32(rd["jumlah"]),
                            Subtotal    = Convert.ToInt32(rd["subtotal"])
                        });
                    }
                }
            }
            return list;
        }

        // =================================================================
        // FITUR KELOLA STOK – Stored Procedure
        // Logika bisnis stok (hitung status, update log) diletakkan di SQL Server
        // sebagai Stored Procedure agar konsisten antara berbagai klien.
        // =================================================================

        // FUNCTION: ambil semua menu beserta info stok via SP sp_GetMenuWithStock.
        // Stored Procedure di SQL Server yang menghitung StatusStok (CUKUP/RENDAH/HABIS).
        public static List<MenuItemWithStock> GetMenuWithStock()
        {
            var list = new List<MenuItemWithStock>();
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("sp_GetMenuWithStock", con))
            {
                // CommandType.StoredProcedure memberitahu ADO.NET bahwa ini adalah
                // nama SP, bukan string SQL biasa. ADO.NET akan otomatis bungkus
                // dengan EXEC sp_GetMenuWithStock.
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new MenuItemWithStock
                        {
                            Id           = Convert.ToInt32(rd["id"]),
                            Nama         = rd["nama"].ToString(),
                            Harga        = Convert.ToInt32(rd["harga"]),
                            Kategori     = rd["kategori"].ToString(),
                            GambarUrl    = rd["gambar_url"] == DBNull.Value ? "" : rd["gambar_url"].ToString(),
                            Tersedia     = Convert.ToBoolean(rd["tersedia"]),
                            Stock        = Convert.ToInt32(rd["stock"]),
                            StockMinimum = Convert.ToInt32(rd["stock_minimum"]),
                            StatusStok   = rd["status_stok"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // FUNCTION: update stok menu via SP sp_UpdateStock.
        // Jenis: "MASUK" (tambah stok), "KELUAR" (kurang stok), "KOREKSI" (set langsung).
        // Return: nilai stok baru setelah perubahan.
        // SP di SQL Server juga otomatis mencatat ke tabel stock_log.
        public static int UpdateStock(int menuItemId, string jenis, int jumlah, string keterangan = null)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("sp_UpdateStock", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@menu_item_id", menuItemId);
                cmd.Parameters.AddWithValue("@jenis",        jenis);
                cmd.Parameters.AddWithValue("@jumlah",       jumlah);
                // Keterangan opsional – kalau null, kirim DBNull.Value ke SQL
                cmd.Parameters.AddWithValue("@keterangan", (object)keterangan ?? DBNull.Value);
                con.Open();
                var result = cmd.ExecuteScalar();
                return result == null ? 0 : Convert.ToInt32(result);
            }
        }

        // PROCEDURE: set nilai stok minimum (ambang peringatan) untuk satu menu.
        // Ketika Stock <= StockMinimum, sistem menampilkan peringatan di dashboard.
        public static void UpdateStockMinimum(int menuItemId, int minimum)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(
                "UPDATE menu_items SET stock_minimum = @min WHERE id = @id", con))
            {
                cmd.Parameters.AddWithValue("@min", minimum);
                cmd.Parameters.AddWithValue("@id",  menuItemId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // FUNCTION: ambil riwayat perubahan stok via SP sp_GetStockLog.
        // Parameter opsional: menuItemId = 0 berarti semua menu.
        // Filter tanggal juga opsional.
        public static List<StockLogEntry> GetStockLog(int menuItemId = 0,
                                                      DateTime? dari = null,
                                                      DateTime? sampai = null)
        {
            var list = new List<StockLogEntry>();
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand("sp_GetStockLog", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Parameter yang null dikirim sebagai DBNull.Value ke SP,
                // agar SP bisa deteksi "tidak ada filter" dan query semua data.
                cmd.Parameters.AddWithValue("@menu_item_id",
                    menuItemId > 0 ? (object)menuItemId : DBNull.Value);
                cmd.Parameters.AddWithValue("@dari",
                    dari.HasValue ? (object)dari.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@sampai",
                    sampai.HasValue ? (object)sampai.Value.AddDays(1).AddSeconds(-1) : DBNull.Value);

                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new StockLogEntry
                        {
                            Id          = Convert.ToInt32(rd["id"]),
                            NamaMenu    = rd["nama_menu"].ToString(),
                            Jenis       = rd["jenis"].ToString(),
                            Jumlah      = Convert.ToInt32(rd["jumlah"]),
                            StokSebelum = Convert.ToInt32(rd["stok_sebelum"]),
                            StokSesudah = Convert.ToInt32(rd["stok_sesudah"]),
                            Keterangan  = rd["keterangan"] == DBNull.Value ? "" : rd["keterangan"].ToString(),
                            CreatedAt   = Convert.ToDateTime(rd["created_at"])
                        });
                    }
                }
            }
            return list;
        }

        // FUNCTION: ambil menu yang stoknya di bawah atau sama dengan minimum.
        // Menggunakan LINQ (WEEK 11) untuk filter dan sort di memori.
        // Dipakai untuk menampilkan peringatan stok rendah di AdminDashboard.
        public static List<MenuItemWithStock> GetMenuStokRendah()
        {
            return GetMenuWithStock()
                    .Where(m => m.Stock <= m.StockMinimum)   // filter: stok di bawah minimum
                    .OrderBy(m => m.Stock)                    // sort: paling rendah di atas
                    .ToList();
        }

        // =================================================================
        // LAPORAN PENJUALAN (Week 12 & 13 - Crystal Report)
        // Query JOIN antara order_items, orders, dan menu_items.
        // DataTable yang dikembalikan langsung dipakai oleh Crystal Reports.
        // =================================================================

        // FUNCTION: ambil data laporan penjualan dengan filter lengkap.
        // Hasilnya DataTable (bukan List<T>) karena Crystal Reports butuh format DataTable/DataSet.
        // JOIN ke menu_items pakai LEFT JOIN karena menu bisa sudah dihapus setelah diorder.
        public static DataTable GetLaporanPenjualan(
            DateTime tanggalDari,
            DateTime tanggalSampai,
            string kategori,
            string metodePembayaran)
        {
            // Normalisasi tanggal: dari = awal hari (00:00:00), sampai = akhir hari (23:59:59)
            DateTime dari   = tanggalDari.Date;
            DateTime sampai = tanggalSampai.Date.AddDays(1).AddSeconds(-1);

            // Bangun query dengan StringBuilder karena ada kondisi opsional
            var sql = new StringBuilder();
            sql.Append("SELECT ");
            sql.Append("  oi.id                    AS OrderItemId, ");
            sql.Append("  o.id                     AS OrderId, ");
            sql.Append("  o.order_number           AS OrderNumber, ");
            sql.Append("  o.created_at             AS Tanggal, ");
            sql.Append("  oi.nama_item             AS NamaItem, ");
            // ISNULL = kalau menu sudah dihapus (NULL), ganti dengan '-'
            sql.Append("  ISNULL(mi.kategori, '-') AS Kategori, ");
            sql.Append("  oi.harga_satuan          AS HargaSatuan, ");
            sql.Append("  oi.jumlah                AS Jumlah, ");
            sql.Append("  oi.subtotal              AS Subtotal, ");
            sql.Append("  o.metode_pembayaran      AS MetodePembayaran, ");
            sql.Append("  o.status                 AS Status ");
            sql.Append("FROM order_items oi ");
            sql.Append("INNER JOIN orders o      ON o.id  = oi.order_id ");     // harus ada order
            sql.Append("LEFT  JOIN menu_items mi ON mi.id = oi.menu_item_id "); // boleh tidak ada menu (sudah dihapus)
            sql.Append("WHERE o.created_at BETWEEN @dari AND @sampai ");

            if (!string.IsNullOrEmpty(kategori))
                sql.Append("  AND mi.kategori = @kategori ");
            if (!string.IsNullOrEmpty(metodePembayaran))
                sql.Append("  AND o.metode_pembayaran = @metode ");

            sql.Append("ORDER BY o.created_at ASC, o.id ASC, oi.id ASC");

            var dt = new DataTable("LaporanPenjualan");

            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(sql.ToString(), con))
            {
                cmd.Parameters.AddWithValue("@dari",   dari);
                cmd.Parameters.AddWithValue("@sampai", sampai);
                if (!string.IsNullOrEmpty(kategori))
                    cmd.Parameters.AddWithValue("@kategori", kategori);
                if (!string.IsNullOrEmpty(metodePembayaran))
                    cmd.Parameters.AddWithValue("@metode", metodePembayaran);

                // SqlDataAdapter.Fill() lebih cocok untuk Crystal Reports daripada
                // ExecuteReader karena langsung mengisi DataTable tanpa loop manual.
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }
    }
}
