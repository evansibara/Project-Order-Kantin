namespace Project_Order_Kantin
{
    partial class Menu_Screen
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlTop         = new System.Windows.Forms.Panel();
            this.pnlTopRow1     = new System.Windows.Forms.Panel();
            this.pnlTopRow2     = new System.Windows.Forms.Panel();
            this.lblMenuTitle   = new System.Windows.Forms.Label();
            this.lblJumlahMenu  = new System.Windows.Forms.Label();
            this.lblCartCount   = new System.Windows.Forms.Label();
            this.btnCart        = new System.Windows.Forms.Button();
            this.txtSearch      = new System.Windows.Forms.TextBox();
            this.cmbKategori    = new System.Windows.Forms.ComboBox();
            this.cmbSort        = new System.Windows.Forms.ComboBox();
            this.btnRefresh     = new System.Windows.Forms.Button();
            this.flowLayoutMenu = new System.Windows.Forms.FlowLayoutPanel();

            this.pnlTop.SuspendLayout();
            this.pnlTopRow1.SuspendLayout();
            this.pnlTopRow2.SuspendLayout();
            this.SuspendLayout();

            // ── pnlTop ───────────────────────────────────────────────────
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(22, 27, 34);
            this.pnlTop.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Height    = 108;
            this.pnlTop.Controls.Add(this.pnlTopRow1);
            this.pnlTop.Controls.Add(this.pnlTopRow2);

            // ── Row1 ─────────────────────────────────────────────────────
            this.pnlTopRow1.BackColor = System.Drawing.Color.Transparent;
            this.pnlTopRow1.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlTopRow1.Height    = 58;
            this.pnlTopRow1.Controls.Add(this.lblMenuTitle);
            this.pnlTopRow1.Controls.Add(this.lblJumlahMenu);
            this.pnlTopRow1.Controls.Add(this.lblCartCount);
            this.pnlTopRow1.Controls.Add(this.btnCart);

            this.lblMenuTitle.AutoSize  = true;
            this.lblMenuTitle.Font      = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblMenuTitle.ForeColor = System.Drawing.Color.White;
            this.lblMenuTitle.Location  = new System.Drawing.Point(18, 8);
            this.lblMenuTitle.Text      = "🍽  Menu Kantin";

            this.lblJumlahMenu.AutoSize  = true;
            this.lblJumlahMenu.Font      = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblJumlahMenu.ForeColor = System.Drawing.Color.FromArgb(140, 150, 165);
            this.lblJumlahMenu.Location  = new System.Drawing.Point(20, 38);
            this.lblJumlahMenu.Text      = "Memuat menu...";

            this.lblCartCount.AutoSize  = true;
            this.lblCartCount.Font      = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblCartCount.ForeColor = System.Drawing.Color.FromArgb(255, 107, 53);
            this.lblCartCount.Text      = "🛒 0 item";
            this.lblCartCount.Anchor    = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.lblCartCount.Location  = new System.Drawing.Point(820, 18);

            this.btnCart.BackColor = System.Drawing.Color.FromArgb(255, 107, 53);
            this.btnCart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCart.FlatAppearance.BorderSize = 0;
            this.btnCart.Font      = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnCart.ForeColor = System.Drawing.Color.White;
            this.btnCart.Text      = "🛒  Keranjang";
            this.btnCart.Size      = new System.Drawing.Size(158, 36);
            this.btnCart.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnCart.Anchor    = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.btnCart.Location  = new System.Drawing.Point(920, 11);

            // ── Row2 ─────────────────────────────────────────────────────
            this.pnlTopRow2.BackColor = System.Drawing.Color.FromArgb(30, 37, 47);
            this.pnlTopRow2.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlTopRow2.Height    = 50;
            this.pnlTopRow2.Controls.Add(this.txtSearch);
            this.pnlTopRow2.Controls.Add(this.cmbKategori);
            this.pnlTopRow2.Controls.Add(this.cmbSort);
            this.pnlTopRow2.Controls.Add(this.btnRefresh);

            this.txtSearch.BackColor  = System.Drawing.Color.FromArgb(42, 50, 62);
            this.txtSearch.ForeColor  = System.Drawing.Color.White;
            this.txtSearch.BorderStyle= System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Font       = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSearch.Location   = new System.Drawing.Point(14, 12);
            this.txtSearch.Size       = new System.Drawing.Size(200, 26);

            this.cmbKategori.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKategori.BackColor     = System.Drawing.Color.FromArgb(42, 50, 62);
            this.cmbKategori.ForeColor     = System.Drawing.Color.White;
            this.cmbKategori.Font          = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbKategori.Location      = new System.Drawing.Point(226, 12);
            this.cmbKategori.Size          = new System.Drawing.Size(160, 26);

            this.cmbSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSort.BackColor     = System.Drawing.Color.FromArgb(42, 50, 62);
            this.cmbSort.ForeColor     = System.Drawing.Color.White;
            this.cmbSort.Font          = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbSort.Location      = new System.Drawing.Point(398, 12);
            this.cmbSort.Size          = new System.Drawing.Size(175, 26);

            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(42, 50, 62);
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.FlatAppearance.BorderSize  = 1;
            this.btnRefresh.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(70, 80, 95);
            this.btnRefresh.Font      = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.FromArgb(200, 210, 220);
            this.btnRefresh.Location  = new System.Drawing.Point(585, 10);
            this.btnRefresh.Size      = new System.Drawing.Size(105, 30);
            this.btnRefresh.Text      = "↻  Refresh";
            this.btnRefresh.Cursor    = System.Windows.Forms.Cursors.Hand;

            // ── flowLayoutMenu ───────────────────────────────────────────
            this.flowLayoutMenu.Dock       = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutMenu.BackColor  = System.Drawing.Color.FromArgb(248, 247, 244);
            this.flowLayoutMenu.AutoScroll = true;
            this.flowLayoutMenu.Padding    = new System.Windows.Forms.Padding(14);

            // ── Form ────────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(248, 247, 244);
            this.ClientSize          = new System.Drawing.Size(1100, 700);
            this.MinimumSize         = new System.Drawing.Size(900, 550);
            this.Controls.Add(this.flowLayoutMenu);
            this.Controls.Add(this.pnlTop);
            this.Name          = "Menu_Screen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text          = "Menu Kantin";

            this.pnlTopRow2.ResumeLayout(false);
            this.pnlTopRow1.ResumeLayout(false);
            this.pnlTopRow1.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel           pnlTop;
        private System.Windows.Forms.Panel           pnlTopRow1;
        private System.Windows.Forms.Panel           pnlTopRow2;
        private System.Windows.Forms.Label           lblMenuTitle;
        private System.Windows.Forms.Label           lblJumlahMenu;
        private System.Windows.Forms.Label           lblCartCount;
        private System.Windows.Forms.Button          btnCart;
        private System.Windows.Forms.TextBox         txtSearch;
        private System.Windows.Forms.ComboBox        cmbKategori;
        private System.Windows.Forms.ComboBox        cmbSort;
        private System.Windows.Forms.Button          btnRefresh;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutMenu;
    }
}
