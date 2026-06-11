namespace Project_Order_Kantin
{
    partial class AdminDashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Designer code

        private void InitializeComponent()
        {
            this.pnlHeader   = new System.Windows.Forms.Panel();
            this.lblTitle    = new System.Windows.Forms.Label();
            this.lblWelcome  = new System.Windows.Forms.Label();
            this.btnLogout   = new System.Windows.Forms.Button();

            this.tabControl  = new System.Windows.Forms.TabControl();
            this.tabMenu     = new System.Windows.Forms.TabPage();
            this.tabKategori = new System.Windows.Forms.TabPage();
            this.tabPesanan  = new System.Windows.Forms.TabPage();
            this.tabLaporan  = new System.Windows.Forms.TabPage();

            this.dgvMenu        = new System.Windows.Forms.DataGridView();
            this.pnlMenuBtn     = new System.Windows.Forms.Panel();
            this.btnTambahMenu  = new System.Windows.Forms.Button();
            this.btnEditMenu    = new System.Windows.Forms.Button();
            this.btnHapusMenu   = new System.Windows.Forms.Button();
            this.btnRefreshMenu = new System.Windows.Forms.Button();
            this.btnKelolaStok  = new System.Windows.Forms.Button();

            this.lstKategori      = new System.Windows.Forms.ListBox();
            this.txtKategoriBaru  = new System.Windows.Forms.TextBox();
            this.btnTambahKategori= new System.Windows.Forms.Button();
            this.btnHapusKategori = new System.Windows.Forms.Button();
            this.lblInfoKategori  = new System.Windows.Forms.Label();

            this.dgvOrders       = new System.Windows.Forms.DataGridView();
            this.dgvOrderItems   = new System.Windows.Forms.DataGridView();
            this.splitOrders     = new System.Windows.Forms.SplitContainer();
            this.btnRefreshOrders= new System.Windows.Forms.Button();
            this.lblTotalOmzet   = new System.Windows.Forms.Label();
            this.pnlOrderTop     = new System.Windows.Forms.Panel();

            this.btnBukaLaporan = new System.Windows.Forms.Button();
            this.btnBackup      = new System.Windows.Forms.Button();
            this.btnLihatLog    = new System.Windows.Forms.Button();
            this.lblLapInfo     = new System.Windows.Forms.Label();

            this.pnlHeader.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabMenu.SuspendLayout();
            this.tabKategori.SuspendLayout();
            this.tabPesanan.SuspendLayout();
            this.tabLaporan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitOrders)).BeginInit();
            this.splitOrders.Panel1.SuspendLayout();
            this.splitOrders.Panel2.SuspendLayout();
            this.splitOrders.SuspendLayout();
            this.SuspendLayout();

            // ═══════════════════════════════════════════════════════
            //  HEADER
            // ═══════════════════════════════════════════════════════
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(22, 27, 34);
            this.pnlHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height    = 64;
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblWelcome);
            this.pnlHeader.Controls.Add(this.btnLogout);

            this.lblTitle.AutoSize  = true;
            this.lblTitle.Font      = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location  = new System.Drawing.Point(20, 10);
            this.lblTitle.Text      = "Admin Dashboard  ·  Kantin";

            this.lblWelcome.AutoSize  = true;
            this.lblWelcome.Font      = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(140, 150, 165);
            this.lblWelcome.Location  = new System.Drawing.Point(22, 38);
            this.lblWelcome.Text      = "Selamat datang, Admin";

            this.btnLogout.Anchor    = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(185, 40, 40);
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font      = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.Location  = new System.Drawing.Point(1080, 16);
            this.btnLogout.Size      = new System.Drawing.Size(100, 32);
            this.btnLogout.Text      = "Logout";
            this.btnLogout.UseVisualStyleBackColor = false;

            // ═══════════════════════════════════════════════════════
            //  TAB CONTROL
            // ═══════════════════════════════════════════════════════
            this.tabControl.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font      = new System.Drawing.Font("Segoe UI", 10F);
            this.tabControl.Controls.Add(this.tabMenu);
            this.tabControl.Controls.Add(this.tabKategori);
            this.tabControl.Controls.Add(this.tabPesanan);
            this.tabControl.Controls.Add(this.tabLaporan);

            // ═══════════════════════════════════════════════════════
            //  TAB MENU
            //  Button layout (w=152, gap=10, start=12):
            //    + Tambah Menu : x=12   right=164
            //    Edit          : x=174  right=326
            //    Hapus         : x=336  right=488
            //    ↻ Refresh     : x=498  right=650
            //    📦 Kelola Stok: x=660  right=812
            // ═══════════════════════════════════════════════════════
            this.tabMenu.Text      = "  Kelola Menu  ";
            this.tabMenu.BackColor = System.Drawing.Color.FromArgb(248, 247, 244);
            this.tabMenu.Controls.Add(this.dgvMenu);
            this.tabMenu.Controls.Add(this.pnlMenuBtn);

            this.pnlMenuBtn.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlMenuBtn.Height    = 58;
            this.pnlMenuBtn.BackColor = System.Drawing.Color.White;
            this.pnlMenuBtn.Controls.Add(this.btnTambahMenu);
            this.pnlMenuBtn.Controls.Add(this.btnEditMenu);
            this.pnlMenuBtn.Controls.Add(this.btnHapusMenu);
            this.pnlMenuBtn.Controls.Add(this.btnRefreshMenu);
            this.pnlMenuBtn.Controls.Add(this.btnKelolaStok);

            // w=152, gap=10 → each step = 162
            StyleAdminButton(this.btnTambahMenu,  "+ Tambah Menu",   System.Drawing.Color.FromArgb(22, 130, 80),   12,  13, 152);
            StyleAdminButton(this.btnEditMenu,    "Edit",             System.Drawing.Color.FromArgb(50, 100, 200),  174, 13, 152);
            StyleAdminButton(this.btnHapusMenu,   "Hapus",           System.Drawing.Color.FromArgb(185, 40, 40),   336, 13, 152);
            StyleAdminButton(this.btnRefreshMenu, "↻ Refresh",       System.Drawing.Color.FromArgb(75, 85, 100),   498, 13, 152);
            StyleAdminButton(this.btnKelolaStok,  "📦 Kelola Stok", System.Drawing.Color.FromArgb(90, 60, 160),   660, 13, 152);

            StyleDataGrid(this.dgvMenu);
            this.dgvMenu.Dock = System.Windows.Forms.DockStyle.Fill;

            // ═══════════════════════════════════════════════════════
            //  TAB KATEGORI
            //  Row 1 (y=18): lblInfoKategori
            //  Row 2 (y=52): txtKategoriBaru(w=240) | btnTambah(x=252,w=130) | btnHapus(x=392,w=150)
            //  Row 3 (y=100): lstKategori
            // ═══════════════════════════════════════════════════════
            this.tabKategori.Text      = "  Kategori  ";
            this.tabKategori.BackColor = System.Drawing.Color.FromArgb(248, 247, 244);
            this.tabKategori.Controls.Add(this.lblInfoKategori);
            this.tabKategori.Controls.Add(this.txtKategoriBaru);
            this.tabKategori.Controls.Add(this.btnTambahKategori);
            this.tabKategori.Controls.Add(this.btnHapusKategori);
            this.tabKategori.Controls.Add(this.lstKategori);

            this.lblInfoKategori.AutoSize  = true;
            this.lblInfoKategori.Font      = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblInfoKategori.ForeColor = System.Drawing.Color.FromArgb(22, 27, 34);
            this.lblInfoKategori.Location  = new System.Drawing.Point(20, 18);
            this.lblInfoKategori.Text      = "Kelola Kategori Menu";

            this.txtKategoriBaru.BackColor  = System.Drawing.Color.White;
            this.txtKategoriBaru.BorderStyle= System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtKategoriBaru.Font       = new System.Drawing.Font("Segoe UI", 11F);
            this.txtKategoriBaru.Location   = new System.Drawing.Point(20, 54);
            this.txtKategoriBaru.Size       = new System.Drawing.Size(240, 30);

            // btnTambah: x = 20+240+12 = 272, w=130
            // btnHapus:  x = 272+130+12 = 414, w=160
            StyleAdminButton(this.btnTambahKategori, "+ Tambah",      System.Drawing.Color.FromArgb(22, 130, 80),  272, 52, 130);
            StyleAdminButton(this.btnHapusKategori,  "Hapus Pilihan", System.Drawing.Color.FromArgb(185, 40, 40),  414, 52, 160);

            this.lstKategori.BackColor   = System.Drawing.Color.White;
            this.lstKategori.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstKategori.Font        = new System.Drawing.Font("Segoe UI", 11F);
            this.lstKategori.Location    = new System.Drawing.Point(20, 100);
            this.lstKategori.Size        = new System.Drawing.Size(560, 460);

            // ═══════════════════════════════════════════════════════
            //  TAB PESANAN
            // ═══════════════════════════════════════════════════════
            this.tabPesanan.Text      = "  Daftar Pesanan  ";
            this.tabPesanan.BackColor = System.Drawing.Color.FromArgb(248, 247, 244);
            this.tabPesanan.Controls.Add(this.splitOrders);
            this.tabPesanan.Controls.Add(this.pnlOrderTop);

            this.pnlOrderTop.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlOrderTop.Height    = 54;
            this.pnlOrderTop.BackColor = System.Drawing.Color.White;
            this.pnlOrderTop.Controls.Add(this.btnRefreshOrders);
            this.pnlOrderTop.Controls.Add(this.lblTotalOmzet);

            StyleAdminButton(this.btnRefreshOrders, "↻ Refresh", System.Drawing.Color.FromArgb(50, 100, 200), 12, 11, 130);

            this.lblTotalOmzet.AutoSize  = true;
            this.lblTotalOmzet.Font      = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTotalOmzet.ForeColor = System.Drawing.Color.FromArgb(22, 130, 80);
            this.lblTotalOmzet.Location  = new System.Drawing.Point(158, 17);
            this.lblTotalOmzet.Text      = "Total Order: 0  |  Total Omzet: Rp 0";

            this.splitOrders.Dock             = System.Windows.Forms.DockStyle.Fill;
            this.splitOrders.Orientation      = System.Windows.Forms.Orientation.Horizontal;
            this.splitOrders.SplitterDistance = 280;
            this.splitOrders.Panel1.Controls.Add(this.dgvOrders);
            this.splitOrders.Panel2.Controls.Add(this.dgvOrderItems);

            StyleDataGrid(this.dgvOrders);
            this.dgvOrders.Dock = System.Windows.Forms.DockStyle.Fill;

            StyleDataGrid(this.dgvOrderItems);
            this.dgvOrderItems.Dock = System.Windows.Forms.DockStyle.Fill;

            // ═══════════════════════════════════════════════════════
            //  TAB LAPORAN
            //  Button layout (w=240, gap=18, start=20):
            //    Buka Crystal Report : x=20   right=260
            //    Backup Menu (.CSV)  : x=278  right=518
            //    Lihat Log Transaksi : x=536  right=776
            // ═══════════════════════════════════════════════════════
            this.tabLaporan.Text      = "  Laporan & Backup  ";
            this.tabLaporan.BackColor = System.Drawing.Color.FromArgb(248, 247, 244);
            this.tabLaporan.Controls.Add(this.lblLapInfo);
            this.tabLaporan.Controls.Add(this.btnBukaLaporan);
            this.tabLaporan.Controls.Add(this.btnBackup);
            this.tabLaporan.Controls.Add(this.btnLihatLog);

            this.lblLapInfo.AutoSize  = false;
            this.lblLapInfo.Font      = new System.Drawing.Font("Segoe UI", 10F);
            this.lblLapInfo.ForeColor = System.Drawing.Color.FromArgb(80, 90, 105);
            this.lblLapInfo.Location  = new System.Drawing.Point(20, 20);
            this.lblLapInfo.Size      = new System.Drawing.Size(800, 80);
            this.lblLapInfo.Text      = "Crystal Report : Buka laporan penjualan dengan filter tanggal, kategori, " +
                                        "dan metode pembayaran.\r\n\r\n" +
                                        "Backup CSV : Simpan seluruh data menu ke file CSV (folder /Backup).\r\n" +
                                        "Log Transaksi : Lihat log transaksi hari ini (folder /Logs).";

            StyleAdminButton(this.btnBukaLaporan, "Buka Crystal Report", System.Drawing.Color.FromArgb(50, 100, 200),  20,  120, 240);
            StyleAdminButton(this.btnBackup,      "Backup Menu (.CSV)",  System.Drawing.Color.FromArgb(22, 130, 80),   278, 120, 240);
            StyleAdminButton(this.btnLihatLog,    "Lihat Log Transaksi", System.Drawing.Color.FromArgb(200, 110, 20),  536, 120, 240);

            this.btnBukaLaporan.Size = new System.Drawing.Size(240, 56);
            this.btnBackup.Size      = new System.Drawing.Size(240, 56);
            this.btnLihatLog.Size    = new System.Drawing.Size(240, 56);

            // ═══════════════════════════════════════════════════════
            //  FORM
            // ═══════════════════════════════════════════════════════
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(248, 247, 244);
            this.ClientSize          = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.pnlHeader);
            this.MinimumSize     = new System.Drawing.Size(900, 600);
            this.Name            = "AdminDashboard";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text            = "Admin Dashboard";

            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabMenu.ResumeLayout(false);
            this.tabKategori.ResumeLayout(false);
            this.tabKategori.PerformLayout();
            this.tabPesanan.ResumeLayout(false);
            this.tabLaporan.ResumeLayout(false);
            this.tabLaporan.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderItems)).EndInit();
            this.splitOrders.Panel1.ResumeLayout(false);
            this.splitOrders.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitOrders)).EndInit();
            this.splitOrders.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        // w parameter added so each call site can pass its own width
        private void StyleAdminButton(System.Windows.Forms.Button btn, string text,
                                      System.Drawing.Color color, int x, int y, int w = 152)
        {
            btn.BackColor                 = color;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatStyle                 = System.Windows.Forms.FlatStyle.Flat;
            btn.Font                      = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            btn.ForeColor                 = System.Drawing.Color.White;
            btn.Cursor                    = System.Windows.Forms.Cursors.Hand;
            btn.Location                  = new System.Drawing.Point(x, y);
            btn.Size                      = new System.Drawing.Size(w, 32);
            btn.Text                      = text;
            btn.UseVisualStyleBackColor   = false;
        }

        private void StyleDataGrid(System.Windows.Forms.DataGridView dgv)
        {
            dgv.AllowUserToAddRows    = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly              = true;
            dgv.SelectionMode         = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect           = false;
            dgv.BackgroundColor       = System.Drawing.Color.White;
            dgv.BorderStyle           = System.Windows.Forms.BorderStyle.None;
            dgv.GridColor             = System.Drawing.Color.FromArgb(235, 233, 229);
            dgv.RowTemplate.Height    = 30;
            dgv.Font                  = new System.Drawing.Font("Segoe UI", 9.5F);
            dgv.ColumnHeadersDefaultCellStyle.BackColor  = System.Drawing.Color.FromArgb(22, 27, 34);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor  = System.Drawing.Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font       = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding    = new System.Windows.Forms.Padding(8, 0, 0, 0);
            dgv.ColumnHeadersHeight                      = 36;
            dgv.ColumnHeadersBorderStyle                 = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dgv.EnableHeadersVisualStyles                = false;
            dgv.DefaultCellStyle.SelectionBackColor      = System.Drawing.Color.FromArgb(255, 240, 232);
            dgv.DefaultCellStyle.SelectionForeColor      = System.Drawing.Color.FromArgb(22, 27, 34);
            dgv.DefaultCellStyle.Padding                 = new System.Windows.Forms.Padding(6, 0, 0, 0);
            dgv.AlternatingRowsDefaultCellStyle.BackColor= System.Drawing.Color.FromArgb(252, 251, 249);
        }

        #endregion

        private System.Windows.Forms.Panel        pnlHeader;
        private System.Windows.Forms.Label        lblTitle;
        private System.Windows.Forms.Label        lblWelcome;
        private System.Windows.Forms.Button       btnLogout;
        private System.Windows.Forms.TabControl   tabControl;
        private System.Windows.Forms.TabPage      tabMenu;
        private System.Windows.Forms.TabPage      tabKategori;
        private System.Windows.Forms.TabPage      tabPesanan;
        private System.Windows.Forms.TabPage      tabLaporan;
        private System.Windows.Forms.DataGridView dgvMenu;
        private System.Windows.Forms.Panel        pnlMenuBtn;
        private System.Windows.Forms.Button       btnTambahMenu;
        private System.Windows.Forms.Button       btnEditMenu;
        private System.Windows.Forms.Button       btnHapusMenu;
        private System.Windows.Forms.Button       btnRefreshMenu;
        private System.Windows.Forms.Button       btnKelolaStok;
        private System.Windows.Forms.ListBox      lstKategori;
        private System.Windows.Forms.TextBox      txtKategoriBaru;
        private System.Windows.Forms.Button       btnTambahKategori;
        private System.Windows.Forms.Button       btnHapusKategori;
        private System.Windows.Forms.Label        lblInfoKategori;
        private System.Windows.Forms.DataGridView dgvOrders;
        private System.Windows.Forms.DataGridView dgvOrderItems;
        private System.Windows.Forms.SplitContainer splitOrders;
        private System.Windows.Forms.Button       btnRefreshOrders;
        private System.Windows.Forms.Label        lblTotalOmzet;
        private System.Windows.Forms.Panel        pnlOrderTop;
        private System.Windows.Forms.Button       btnBukaLaporan;
        private System.Windows.Forms.Button       btnBackup;
        private System.Windows.Forms.Button       btnLihatLog;
        private System.Windows.Forms.Label        lblLapInfo;
    }
}
