namespace Project_Order_Kantin
{
    partial class Cart_Screen
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
            this.flowLayoutCart = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlBottom      = new System.Windows.Forms.Panel();
            this.lblTotal       = new System.Windows.Forms.Label();
            this.lblTotalHarga  = new System.Windows.Forms.Label();
            this.bttnKembali    = new System.Windows.Forms.Button();
            this.bttnCheckout   = new System.Windows.Forms.Button();
            this.lblCartScreen  = new System.Windows.Forms.Label();

            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();

            // ── flowLayoutCart ──────────────────────────────────────────
            this.flowLayoutCart.Dock          = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutCart.Name          = "flowLayoutCart";
            this.flowLayoutCart.BackColor     = System.Drawing.Color.FromArgb(248, 247, 244);
            this.flowLayoutCart.AutoScroll    = true;
            this.flowLayoutCart.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutCart.WrapContents  = false;
            this.flowLayoutCart.Padding       = new System.Windows.Forms.Padding(16);

            // ── pnlBottom ───────────────────────────────────────────────
            this.pnlBottom.BackColor    = System.Drawing.Color.White;
            this.pnlBottom.BorderStyle  = System.Windows.Forms.BorderStyle.None;
            this.pnlBottom.Controls.Add(this.lblTotal);
            this.pnlBottom.Controls.Add(this.lblTotalHarga);
            this.pnlBottom.Controls.Add(this.bttnKembali);
            this.pnlBottom.Controls.Add(this.bttnCheckout);
            this.pnlBottom.Dock         = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Name         = "pnlBottom";
            this.pnlBottom.Size         = new System.Drawing.Size(1000, 82);
            this.pnlBottom.Paint       += new System.Windows.Forms.PaintEventHandler(this.PnlBottom_Paint);

            this.lblTotal.AutoSize  = true;
            this.lblTotal.Font      = new System.Drawing.Font("Segoe UI", 11F);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(100, 110, 125);
            this.lblTotal.Location  = new System.Drawing.Point(480, 30);
            this.lblTotal.Name      = "lblTotal";
            this.lblTotal.Text      = "Total Pembayaran";

            this.lblTotalHarga.AutoSize  = true;
            this.lblTotalHarga.Font      = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTotalHarga.ForeColor = System.Drawing.Color.FromArgb(22, 27, 34);
            this.lblTotalHarga.Location  = new System.Drawing.Point(480, 48);
            this.lblTotalHarga.Name      = "lblTotalHarga";
            this.lblTotalHarga.Text      = "Rp 0";

            // bttnKembali
            this.bttnKembali.BackColor = System.Drawing.Color.FromArgb(248, 247, 244);
            this.bttnKembali.ForeColor = System.Drawing.Color.FromArgb(60, 70, 85);
            this.bttnKembali.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.bttnKembali.Location  = new System.Drawing.Point(16, 16);
            this.bttnKembali.Name      = "bttnKembali";
            this.bttnKembali.Size      = new System.Drawing.Size(160, 48);
            this.bttnKembali.Text      = "← Kembali";
            this.bttnKembali.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bttnKembali.FlatAppearance.BorderSize  = 1;
            this.bttnKembali.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(210, 208, 204);
            this.bttnKembali.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.bttnKembali.UseVisualStyleBackColor = false;

            // bttnCheckout
            this.bttnCheckout.BackColor = System.Drawing.Color.FromArgb(255, 107, 53);
            this.bttnCheckout.ForeColor = System.Drawing.Color.White;
            this.bttnCheckout.Font      = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.bttnCheckout.Location  = new System.Drawing.Point(800, 16);
            this.bttnCheckout.Name      = "bttnCheckout";
            this.bttnCheckout.Size      = new System.Drawing.Size(184, 48);
            this.bttnCheckout.Text      = "Checkout  →";
            this.bttnCheckout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bttnCheckout.FlatAppearance.BorderSize = 0;
            this.bttnCheckout.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.bttnCheckout.Anchor    = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            this.bttnCheckout.UseVisualStyleBackColor = false;

            // ── lblCartScreen (top header bar) ──────────────────────────
            this.lblCartScreen.BackColor  = System.Drawing.Color.FromArgb(22, 27, 34);
            this.lblCartScreen.Dock       = System.Windows.Forms.DockStyle.Top;
            this.lblCartScreen.Font       = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblCartScreen.ForeColor  = System.Drawing.Color.White;
            this.lblCartScreen.Name       = "lblCartScreen";
            this.lblCartScreen.Size       = new System.Drawing.Size(1000, 48);
            this.lblCartScreen.Text       = "🛒  Keranjang Pesanan";
            this.lblCartScreen.TextAlign  = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCartScreen.Padding    = new System.Windows.Forms.Padding(18, 0, 0, 0);

            // ── Cart_Screen ─────────────────────────────────────────────
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(248, 247, 244);
            this.ClientSize          = new System.Drawing.Size(1000, 680);
            this.Controls.Add(this.flowLayoutCart);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.lblCartScreen);
            this.MinimumSize    = new System.Drawing.Size(800, 500);
            this.Name           = "Cart_Screen";
            this.StartPosition  = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text           = "Keranjang";
            this.Shown         += new System.EventHandler(this.Cart_Screen_Shown);

            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutCart;
        private System.Windows.Forms.Panel           pnlBottom;
        private System.Windows.Forms.Label           lblTotal;
        private System.Windows.Forms.Label           lblTotalHarga;
        private System.Windows.Forms.Button          bttnKembali;
        private System.Windows.Forms.Button          bttnCheckout;
        private System.Windows.Forms.Label           lblCartScreen;
    }
}
