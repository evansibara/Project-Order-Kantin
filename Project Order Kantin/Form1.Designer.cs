namespace Project_Order_Kantin
{
    partial class Form1
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
            this.pnlLeft        = new System.Windows.Forms.Panel();
            this.pnlBrand       = new System.Windows.Forms.Panel();
            this.lblBrandIcon   = new System.Windows.Forms.Label();
            this.lblBrandName   = new System.Windows.Forms.Label();
            this.lblBrandTagline= new System.Windows.Forms.Label();
            this.pnlRight       = new System.Windows.Forms.Panel();
            this.lblWelcome     = new System.Windows.Forms.Label();
            this.lblSubWelcome  = new System.Windows.Forms.Label();
            this.pnlDivider     = new System.Windows.Forms.Panel();
            this.btnMulaiPesanan= new System.Windows.Forms.Button();
            this.btnAdminLogin  = new System.Windows.Forms.Button();
            this.lblInfo        = new System.Windows.Forms.Label();
            this.pnlFooter      = new System.Windows.Forms.Panel();
            this.lblVersion     = new System.Windows.Forms.Label();

            this.pnlLeft.SuspendLayout();
            this.pnlBrand.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();

            // ── pnlLeft (dark brand panel) ──────────────────────────────
            this.pnlLeft.BackColor = System.Drawing.Color.FromArgb(22, 27, 34);
            this.pnlLeft.Dock      = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Width     = 220;
            this.pnlLeft.Controls.Add(this.pnlBrand);

            // ── pnlBrand (centered inside left) ─────────────────────────
            this.pnlBrand.BackColor  = System.Drawing.Color.Transparent;
            this.pnlBrand.Size       = new System.Drawing.Size(190, 200);
            this.pnlBrand.Location   = new System.Drawing.Point(15, 100);
            this.pnlBrand.Controls.Add(this.lblBrandIcon);
            this.pnlBrand.Controls.Add(this.lblBrandName);
            this.pnlBrand.Controls.Add(this.lblBrandTagline);

            this.lblBrandIcon.Text      = "🍽";
            this.lblBrandIcon.Font      = new System.Drawing.Font("Segoe UI", 40F);
            this.lblBrandIcon.ForeColor = System.Drawing.Color.FromArgb(255, 107, 53);
            this.lblBrandIcon.AutoSize  = true;
            this.lblBrandIcon.Location  = new System.Drawing.Point(60, 0);

            this.lblBrandName.Text      = "KANTIN";
            this.lblBrandName.Font      = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblBrandName.ForeColor = System.Drawing.Color.White;
            this.lblBrandName.AutoSize  = true;
            this.lblBrandName.Location  = new System.Drawing.Point(40, 75);

            this.lblBrandTagline.Text      = "Order System";
            this.lblBrandTagline.Font      = new System.Drawing.Font("Segoe UI", 10F);
            this.lblBrandTagline.ForeColor = System.Drawing.Color.FromArgb(140, 150, 165);
            this.lblBrandTagline.AutoSize  = true;
            this.lblBrandTagline.Location  = new System.Drawing.Point(52, 118);

            // ── pnlRight (main content area) ─────────────────────────────
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(248, 247, 244);
            this.pnlRight.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Controls.Add(this.lblWelcome);
            this.pnlRight.Controls.Add(this.lblSubWelcome);
            this.pnlRight.Controls.Add(this.pnlDivider);
            this.pnlRight.Controls.Add(this.btnMulaiPesanan);
            this.pnlRight.Controls.Add(this.btnAdminLogin);
            this.pnlRight.Controls.Add(this.lblInfo);
            this.pnlRight.Controls.Add(this.pnlFooter);

            this.lblWelcome.Text      = "Selamat Datang";
            this.lblWelcome.Font      = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(22, 27, 34);
            this.lblWelcome.AutoSize  = true;
            this.lblWelcome.Location  = new System.Drawing.Point(40, 70);

            this.lblSubWelcome.Text      = "Pilih mode penggunaan untuk melanjutkan";
            this.lblSubWelcome.Font      = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubWelcome.ForeColor = System.Drawing.Color.FromArgb(100, 110, 125);
            this.lblSubWelcome.AutoSize  = true;
            this.lblSubWelcome.Location  = new System.Drawing.Point(40, 108);

            this.pnlDivider.BackColor = System.Drawing.Color.FromArgb(230, 228, 224);
            this.pnlDivider.Location  = new System.Drawing.Point(40, 135);
            this.pnlDivider.Size      = new System.Drawing.Size(200, 1);

            // ── btnMulaiPesanan ──────────────────────────────────────────
            this.btnMulaiPesanan.BackColor = System.Drawing.Color.FromArgb(255, 107, 53);
            this.btnMulaiPesanan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMulaiPesanan.FlatAppearance.BorderSize = 0;
            this.btnMulaiPesanan.Font      = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnMulaiPesanan.ForeColor = System.Drawing.Color.White;
            this.btnMulaiPesanan.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnMulaiPesanan.Location  = new System.Drawing.Point(40, 160);
            this.btnMulaiPesanan.Name      = "btnMulaiPesanan";
            this.btnMulaiPesanan.Size      = new System.Drawing.Size(220, 56);
            this.btnMulaiPesanan.Text      = "🛒  Mulai Pesanan";
            this.btnMulaiPesanan.UseVisualStyleBackColor = false;
            this.btnMulaiPesanan.Click += new System.EventHandler(this.BtnMulaiPesanan_Click);

            // ── btnAdminLogin ────────────────────────────────────────────
            this.btnAdminLogin.BackColor = System.Drawing.Color.FromArgb(22, 27, 34);
            this.btnAdminLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdminLogin.FlatAppearance.BorderSize  = 1;
            this.btnAdminLogin.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(60, 70, 85);
            this.btnAdminLogin.Font      = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnAdminLogin.ForeColor = System.Drawing.Color.FromArgb(200, 210, 220);
            this.btnAdminLogin.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnAdminLogin.Location  = new System.Drawing.Point(40, 230);
            this.btnAdminLogin.Name      = "btnAdminLogin";
            this.btnAdminLogin.Size      = new System.Drawing.Size(220, 56);
            this.btnAdminLogin.Text      = "🔐  Admin Login";
            this.btnAdminLogin.UseVisualStyleBackColor = false;
            this.btnAdminLogin.Click += new System.EventHandler(this.BtnAdminLogin_Click);

            this.lblInfo.AutoSize  = true;
            this.lblInfo.Font      = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblInfo.ForeColor = System.Drawing.Color.FromArgb(160, 165, 175);
            this.lblInfo.Location  = new System.Drawing.Point(40, 305);
            this.lblInfo.Text      = "Admin: username=admin  |  password=admin123";

            // ── Footer ───────────────────────────────────────────────────
            this.pnlFooter.BackColor = System.Drawing.Color.FromArgb(235, 233, 229);
            this.pnlFooter.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Height    = 32;
            this.pnlFooter.Controls.Add(this.lblVersion);

            this.lblVersion.Text      = "Sistem Order Kantin  ·  v4.0";
            this.lblVersion.Font      = new System.Drawing.Font("Segoe UI", 8F);
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(140, 140, 140);
            this.lblVersion.AutoSize  = true;
            this.lblVersion.Location  = new System.Drawing.Point(12, 8);

            // ── Form1 ───────────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(248, 247, 244);
            this.ClientSize          = new System.Drawing.Size(560, 380);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlLeft);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.Name            = "Form1";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text            = "Sistem Order Kantin";

            this.pnlLeft.ResumeLayout(false);
            this.pnlBrand.ResumeLayout(false);
            this.pnlBrand.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.pnlRight.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel  pnlLeft;
        private System.Windows.Forms.Panel  pnlBrand;
        private System.Windows.Forms.Label  lblBrandIcon;
        private System.Windows.Forms.Label  lblBrandName;
        private System.Windows.Forms.Label  lblBrandTagline;
        private System.Windows.Forms.Panel  pnlRight;
        private System.Windows.Forms.Label  lblWelcome;
        private System.Windows.Forms.Label  lblSubWelcome;
        private System.Windows.Forms.Panel  pnlDivider;
        private System.Windows.Forms.Button btnMulaiPesanan;
        private System.Windows.Forms.Button btnAdminLogin;
        private System.Windows.Forms.Label  lblInfo;
        private System.Windows.Forms.Panel  pnlFooter;
        private System.Windows.Forms.Label  lblVersion;
    }
}
