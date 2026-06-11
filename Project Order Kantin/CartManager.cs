// =================================================================
// WEEK 5 - Function & Procedure: AddItem, RemoveItem, Total, dll.
// WEEK 6 - Dictionary          : Items = Dictionary<int, CartLine>
//          Key   = MenuItemId (int)
//          Value = CartLine (nama, harga, jumlah, subtotal)
// =================================================================
using System.Collections.Generic;
using System.Linq;

namespace Project_Order_Kantin
{
    // CartManager menyimpan isi keranjang belanja selama aplikasi berjalan.
    // Dibuat 'static' agar bisa diakses dari form mana saja tanpa perlu
    // passing objek – Menu_Screen, Cart_Screen, dan Payment semuanya
    // baca/tulis ke CartManager yang sama.
    //
    // Struktur data utama: Dictionary<int, CartLine>
    //   Key   = MenuItemId → pencarian item tertentu sangat cepat (O(1))
    //   Value = CartLine   → menyimpan nama, harga, jumlah, dan subtotal
    //
    // Kenapa Dictionary, bukan List? Karena saat user klik "Tambah" berkali-kali
    // untuk menu yang sama, kita hanya perlu Update Jumlah (bukan buat entry baru).
    // Dengan Dictionary, Items[menuId].Jumlah += 1 langsung kena.
    // Dengan List, kita harus loop dulu untuk cari apakah item sudah ada.
    public static class CartManager
    {
        // WEEK 6 – DICTIONARY: tempat nyimpan semua item di keranjang.
        // Readonly = referensi Dictionary-nya tidak bisa diganti,
        // tapi isi Dictionary-nya (pasangan key-value) tetap bisa dimodifikasi.
        public static Dictionary<int, CartLine> Items { get; } = new Dictionary<int, CartLine>();

        // PROCEDURE (WEEK 5): tambahkan item ke keranjang.
        // SELECTION (if-else): dua kondisi berbeda tergantung apakah item sudah ada.
        //   - Sudah ada → cukup naikkan Jumlah (tidak duplikat)
        //   - Belum ada → buat CartLine baru dan masukkan ke Dictionary
        // Parameter jumlah punya default value 1, artinya kalau dipanggil
        // CartManager.AddItem(menu) tanpa jumlah, otomatis ditambahkan 1 porsi.
        public static void AddItem(MenuItemModel menu, int jumlah = 1)
        {
            if (Items.ContainsKey(menu.Id))
            {
                // Item sudah ada di keranjang – tambah jumlahnya saja
                Items[menu.Id].Jumlah += jumlah;
            }
            else
            {
                // Item baru – buat CartLine dan simpan ke Dictionary dengan key = Id menu
                Items[menu.Id] = new CartLine(menu.Id, menu.Nama, menu.Harga, jumlah);
            }
        }

        // PROCEDURE (WEEK 5): ubah jumlah item yang sudah ada di keranjang.
        // Dipakai oleh tombol +/- di Cart_Screen.
        // SELECTION: kalau jumlah baru <= 0, hapus item dari keranjang
        // karena tidak masuk akal punya 0 atau negatif porsi.
        public static void SetJumlah(int menuItemId, int jumlah)
        {
            if (jumlah <= 0)
                // Kuantitas nol atau minus = hapus dari keranjang
                Items.Remove(menuItemId);
            else if (Items.ContainsKey(menuItemId))
                Items[menuItemId].Jumlah = jumlah;
        }

        // PROCEDURE (WEEK 5): hapus satu item dari keranjang berdasarkan id-nya.
        // Dictionary.Remove(key) otomatis aman – tidak error kalau key tidak ada.
        public static void RemoveItem(int menuItemId)
        {
            Items.Remove(menuItemId);
        }

        // FUNCTION (WEEK 5): hitung total harga semua item di keranjang.
        // LINQ Sum() menjumlahkan nilai Subtotal dari setiap CartLine di Dictionary.
        // Lebih ringkas daripada nulis loop for/foreach + variabel akumulator manual.
        // Return int karena harga dalam rupiah tidak pakai desimal.
        public static int Total()
        {
            // Items.Values = semua CartLine (bukan key-nya)
            // line.Subtotal = HargaSatuan × Jumlah (computed property di CartLine)
            return Items.Values.Sum(line => line.Subtotal);
        }

        // FUNCTION (WEEK 5): hitung total porsi (bukan total harga) di keranjang.
        // Dipakai untuk nampilkan counter "🛒 N item" di header Menu_Screen.
        // Contoh: Nasi Goreng 2 + Es Teh 1 + Bakso 3 = 6 item total.
        public static int TotalItems()
        {
            return Items.Values.Sum(line => line.Jumlah);
        }

        // PROCEDURE: kosongkan semua isi keranjang sekaligus.
        // Dipanggil oleh Payment.cs setelah transaksi berhasil disimpan ke database.
        // Dictionary.Clear() menghapus semua key-value sekaligus.
        public static void ClearCart()
        {
            Items.Clear();
        }
    }
}
