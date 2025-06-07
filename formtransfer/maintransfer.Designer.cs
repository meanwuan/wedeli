namespace WeDeLi1.formtransfer
{
    partial class maintransfer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.revenue = new System.Windows.Forms.Button();
            this.about = new System.Windows.Forms.Button();
            this.changecar = new System.Windows.Forms.Button();
            this.logout = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // webView21
            // 
            this.webView21.AllowExternalDrop = true;
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Location = new System.Drawing.Point(12, 114);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(1158, 348);
            this.webView21.TabIndex = 0;
            this.webView21.ZoomFactor = 1D;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 468);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1158, 150);
            this.dataGridView1.TabIndex = 1;
            // 
            // revenue
            // 
            this.revenue.Location = new System.Drawing.Point(85, 34);
            this.revenue.Name = "revenue";
            this.revenue.Size = new System.Drawing.Size(124, 55);
            this.revenue.TabIndex = 2;
            this.revenue.Text = "Doanh Thu";
            this.revenue.UseVisualStyleBackColor = true;
            // 
            // about
            // 
            this.about.Location = new System.Drawing.Point(533, 34);
            this.about.Name = "about";
            this.about.Size = new System.Drawing.Size(124, 55);
            this.about.TabIndex = 3;
            this.about.Text = "Thông Tin";
            this.about.UseVisualStyleBackColor = true;
            // 
            // changecar
            // 
            this.changecar.Location = new System.Drawing.Point(299, 34);
            this.changecar.Name = "changecar";
            this.changecar.Size = new System.Drawing.Size(124, 55);
            this.changecar.TabIndex = 4;
            this.changecar.Text = "Điều Chỉnh Xe";
            this.changecar.UseVisualStyleBackColor = true;
            // 
            // logout
            // 
            this.logout.Location = new System.Drawing.Point(955, 34);
            this.logout.Name = "logout";
            this.logout.Size = new System.Drawing.Size(124, 55);
            this.logout.TabIndex = 5;
            this.logout.Text = "Đăng Xuất";
            this.logout.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(748, 34);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(124, 55);
            this.button1.TabIndex = 6;
            this.button1.Text = "Thêm Nhân viên";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // maintransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 602);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.logout);
            this.Controls.Add(this.changecar);
            this.Controls.Add(this.about);
            this.Controls.Add(this.revenue);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.webView21);
            this.Name = "maintransfer";
            this.Text = "maintransfer";
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button revenue;
        private System.Windows.Forms.Button about;
        private System.Windows.Forms.Button changecar;
        private System.Windows.Forms.Button logout;
        private System.Windows.Forms.Button button1;
    }
}