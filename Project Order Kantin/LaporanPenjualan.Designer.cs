namespace Project_Order_Kantin
{
    // SESUDAH:
    partial class LaporanPenjualan : System.Windows.Forms.Form
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
            this.pnlFilter           = new System.Windows.Forms.Panel();
            this.lblDari             = new System.Windows.Forms.Label();
            this.dtpDari             = new System.Windows.Forms.DateTimePicker();
            this.lblSampai           = new System.Windows.Forms.Label();
            this.dtpSampai           = new System.Windows.Forms.DateTimePicker();
            this.lblKategori         = new System.Windows.Forms.Label();
            this.cmbKategori         = new System.Windows.Forms.ComboBox();
            this.lblMetode           = new System.Windows.Forms.Label();
            this.cmbMetode           = new System.Windows.Forms.ComboBox();
            this.btnTampilkan        = new System.Windows.Forms.Button();
            this.btnTutup            = new System.Windows.Forms.Button();
            this.crystalReportViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();

            this.pnlFilter.SuspendLayout();
            this.SuspendLayout();

            // pnlFilter
            this.pnlFilter.BackColor = System.Drawing.Color.White;
            this.pnlFilter.Dock      = System.Windows.Forms.DockStyle.Top;
            this.pnlFilter.Height    = 58;
            this.pnlFilter.Controls.Add(this.lblDari);
            this.pnlFilter.Controls.Add(this.dtpDari);
            this.pnlFilter.Controls.Add(this.lblSampai);
            this.pnlFilter.Controls.Add(this.dtpSampai);
            this.pnlFilter.Controls.Add(this.lblKategori);
            this.pnlFilter.Controls.Add(this.cmbKategori);
            this.pnlFilter.Controls.Add(this.lblMetode);
            this.pnlFilter.Controls.Add(this.cmbMetode);
            this.pnlFilter.Controls.Add(this.btnTampilkan);
            this.pnlFilter.Controls.Add(this.btnTutup);

            int y = 14;
            this.lblDari.AutoSize = true; this.lblDari.Location = new System.Drawing.Point(8, y); this.lblDari.Text = "Dari:";
            this.dtpDari.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDari.Location = new System.Drawing.Point(50, y - 2); this.dtpDari.Size = new System.Drawing.Size(120, 26);

            this.lblSampai.AutoSize = true; this.lblSampai.Location = new System.Drawing.Point(182, y); this.lblSampai.Text = "Sampai:";
            this.dtpSampai.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpSampai.Location = new System.Drawing.Point(240, y - 2); this.dtpSampai.Size = new System.Drawing.Size(120, 26);

            this.lblKategori.AutoSize = true; this.lblKategori.Location = new System.Drawing.Point(374, y); this.lblKategori.Text = "Kategori:";
            this.cmbKategori.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKategori.Location = new System.Drawing.Point(440, y - 2); this.cmbKategori.Size = new System.Drawing.Size(140, 26);

            this.lblMetode.AutoSize = true; this.lblMetode.Location = new System.Drawing.Point(592, y); this.lblMetode.Text = "Metode:";
            this.cmbMetode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMetode.Location = new System.Drawing.Point(650, y - 2); this.cmbMetode.Size = new System.Drawing.Size(130, 26);

            this.btnTampilkan.BackColor = System.Drawing.Color.RoyalBlue;
            this.btnTampilkan.ForeColor = System.Drawing.Color.White;
            this.btnTampilkan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTampilkan.FlatAppearance.BorderSize = 0;
            this.btnTampilkan.Font     = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnTampilkan.Location = new System.Drawing.Point(796, 9);
            this.btnTampilkan.Size     = new System.Drawing.Size(120, 34);
            this.btnTampilkan.Text     = "TAMPILKAN";
            this.btnTampilkan.Cursor   = System.Windows.Forms.Cursors.Hand;

            this.btnTutup.BackColor = System.Drawing.Color.IndianRed;
            this.btnTutup.ForeColor = System.Drawing.Color.White;
            this.btnTutup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTutup.FlatAppearance.BorderSize = 0;
            this.btnTutup.Font      = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnTutup.Location  = new System.Drawing.Point(928, 9);
            this.btnTutup.Size      = new System.Drawing.Size(80, 34);
            this.btnTutup.Text      = "TUTUP";
            this.btnTutup.Cursor    = System.Windows.Forms.Cursors.Hand;

            // CrystalReportViewer
            this.crystalReportViewer.Dock        = System.Windows.Forms.DockStyle.Fill;
            this.crystalReportViewer.Name        = "crystalReportViewer";
            this.crystalReportViewer.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode       = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize          = new System.Drawing.Size(1060, 680);
            this.Controls.Add(this.crystalReportViewer);
            this.Controls.Add(this.pnlFilter);
            this.Name          = "LaporanPenjualan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text          = "Laporan Penjualan";
            this.WindowState   = System.Windows.Forms.FormWindowState.Maximized;

            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel           pnlFilter;
        private System.Windows.Forms.Label           lblDari;
        private System.Windows.Forms.DateTimePicker  dtpDari;
        private System.Windows.Forms.Label           lblSampai;
        private System.Windows.Forms.DateTimePicker  dtpSampai;
        private System.Windows.Forms.Label           lblKategori;
        private System.Windows.Forms.ComboBox        cmbKategori;
        private System.Windows.Forms.Label           lblMetode;
        private System.Windows.Forms.ComboBox        cmbMetode;
        private System.Windows.Forms.Button          btnTampilkan;
        private System.Windows.Forms.Button          btnTutup;
        private CrystalDecisions.Windows.Forms.CrystalReportViewer crystalReportViewer;
    }
}
