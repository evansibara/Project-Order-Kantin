using System;
using System.Windows.Forms;

namespace Project_Order_Kantin
{
    // Titik masuk (entry point) aplikasi WinForms.
    // Framework .NET memanggil method Main() pertama kali saat program dijalankan.
    static class Program
    {
        // [STAThread] = Single-Threaded Apartment, wajib ada untuk aplikasi WinForms.
        // Ini memberitahu Windows bahwa UI berjalan di satu thread utama,
        // bukan multi-thread, agar komponen COM (seperti file dialog) berfungsi benar.
        [STAThread]
        static void Main()
        {
            // EnableVisualStyles() = aktifkan tampilan visual modern (rounded buttons, dll.)
            // SetCompatibleTextRenderingDefault(false) = pakai GDI+ untuk render teks
            // (lebih konsisten di berbagai ukuran layar)
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Application.Run() membuka Form1 sebagai jendela utama dan menjaga
            // aplikasi tetap berjalan. Saat Form1 ditutup, program selesai.
            Application.Run(new Form1());
        }
    }
}
