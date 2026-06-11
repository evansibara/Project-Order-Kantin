namespace Project_Order_Kantin
{
    partial class Payment
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
            this.pnlHeader   = new System.Windows.Forms.Panel();
            this.lblJudul    = new System.Windows.Forms.Label();
            this.pnlBody     = new System.Windows.Forms.Panel();
            this.lblTotalLbl = new System.Windows.Forms.Label();
            this.lblTotal    = new System.Windows.Forms.Label();
            this.lblMetode   = new System.Windows.Forms.Label();
            this.cmbMetode   = new System.Windows.Forms.ComboBox();
            this.pnlActions  = new System.Windows.Forms.Panel();
            this.btnBatal    = new System.Windows.Forms.Button();
            this.btnBayar    = new System.Windows.Forms.Button();

            this.pnlHeader.SuspendLayout();
            this.pnlBody.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();

            // pnlHeader
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(22, 27, 34);
            this.pnlHeader.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height    = 56;
            this.pnlHeader.Controls.Add(this.lblJudul);

            this.lblJudul.AutoSize  = true;
            this.lblJudul.Font      = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblJudul.ForeColor = System.Drawing.Color.White;
            this.lblJudul.Location  = new System.Drawing.Point(20, 15);
            this.lblJudul.Text      = "💳  Pembayaran";

            // pnlBody
            this.pnlBody.BackColor = System.Drawing.Color.FromArgb(248, 247, 244);
            this.pnlBody.Dock      = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Controls.Add(this.lblTotalLbl);
            this.pnlBody.Controls.Add(this.lblTotal);
            this.pnlBody.Controls.Add(this.lblMetode);
            this.pnlBody.Controls.Add(this.cmbMetode);
            this.pnlBody.Controls.Add(this.pnlActions);

            this.lblTotalLbl.AutoSize  = true;
            this.lblTotalLbl.Font      = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblTotalLbl.ForeColor = System.Drawing.Color.FromArgb(100, 110, 125);
            this.lblTotalLbl.Location  = new System.Drawing.Point(30, 22);
            this.lblTotalLbl.Text      = "Total Pembayaran";

            // lblTotal - used by Payment.cs to show total amount
            this.lblTotal.AutoSize  = true;
            this.lblTotal.Font      = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(22, 27, 34);
            this.lblTotal.Location  = new System.Drawing.Point(28, 44);
            this.lblTotal.Text      = "Total: Rp 0";

            this.lblMetode.AutoSize  = true;
            this.lblMetode.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblMetode.ForeColor = System.Drawing.Color.FromArgb(40, 50, 65);
            this.lblMetode.Location  = new System.Drawing.Point(30, 100);
            this.lblMetode.Text      = "Metode Pembayaran";

            this.cmbMetode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMetode.BackColor     = System.Drawing.Color.White;
            this.cmbMetode.ForeColor     = System.Drawing.Color.FromArgb(22, 27, 34);
            this.cmbMetode.Font          = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbMetode.Location      = new System.Drawing.Point(30, 124);
            this.cmbMetode.Size          = new System.Drawing.Size(300, 32);
            this.cmbMetode.Items.AddRange(new string[] { "Cash", "QRIS", "Transfer Bank", "Debit", "Kredit" });

            // pnlActions
            this.pnlActions.BackColor = System.Drawing.Color.White;
            this.pnlActions.Dock      = System.Windows.Forms.DockStyle.Bottom;
            this.pnlActions.Height    = 72;
            this.pnlActions.Controls.Add(this.btnBatal);
            this.pnlActions.Controls.Add(this.btnBayar);

            this.btnBatal.BackColor = System.Drawing.Color.FromArgb(248, 247, 244);
            this.btnBatal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBatal.FlatAppearance.BorderSize  = 1;
            this.btnBatal.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(210, 208, 204);
            this.btnBatal.Font      = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnBatal.ForeColor = System.Drawing.Color.FromArgb(80, 90, 105);
            this.btnBatal.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnBatal.Location  = new System.Drawing.Point(20, 14);
            this.btnBatal.Size      = new System.Drawing.Size(120, 44);
            this.btnBatal.Text      = "Batal";

            this.btnBayar.BackColor = System.Drawing.Color.FromArgb(255, 107, 53);
            this.btnBayar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBayar.FlatAppearance.BorderSize = 0;
            this.btnBayar.Font      = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnBayar.ForeColor = System.Drawing.Color.White;
            this.btnBayar.Cursor    = System.Windows.Forms.Cursors.Hand;
            this.btnBayar.Location  = new System.Drawing.Point(155, 14);
            this.btnBayar.Size      = new System.Drawing.Size(190, 44);
            this.btnBayar.Text      = "Bayar Sekarang";

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor           = System.Drawing.Color.FromArgb(248, 247, 244);
            this.ClientSize          = new System.Drawing.Size(380, 310);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.Name            = "Payment";
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text            = "Pembayaran";

            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlBody.ResumeLayout(false);
            this.pnlBody.PerformLayout();
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel    pnlHeader;
        private System.Windows.Forms.Label    lblJudul;
        private System.Windows.Forms.Panel    pnlBody;
        private System.Windows.Forms.Label    lblTotalLbl;
        private System.Windows.Forms.Label    lblTotal;
        private System.Windows.Forms.Label    lblMetode;
        private System.Windows.Forms.ComboBox cmbMetode;
        private System.Windows.Forms.Panel    pnlActions;
        private System.Windows.Forms.Button   btnBayar;
        private System.Windows.Forms.Button   btnBatal;
    }
}
