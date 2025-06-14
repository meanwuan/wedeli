namespace WeDeLi1
{
    partial class userform
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
            this.components = new System.ComponentModel.Container();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.signout = new System.Windows.Forms.Button();
            this.add_goods = new System.Windows.Forms.Button();
            this.history_diliver = new System.Windows.Forms.Button();
            this.edit_info = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // webView21
            // 
            this.webView21.AllowExternalDrop = true;
            this.webView21.CreationProperties = null;
            this.webView21.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webView21.Location = new System.Drawing.Point(24, 64);
            this.webView21.Margin = new System.Windows.Forms.Padding(2);
            this.webView21.Name = "webView21";
            this.webView21.Size = new System.Drawing.Size(951, 357);
            this.webView21.TabIndex = 0;
            this.webView21.ZoomFactor = 1D;
            this.webView21.Click += new System.EventHandler(this.webview_Click);
            // 
            // signout
            // 
            this.signout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.signout.Location = new System.Drawing.Point(783, 17);
            this.signout.Margin = new System.Windows.Forms.Padding(2);
            this.signout.Name = "signout";
            this.signout.Size = new System.Drawing.Size(80, 34);
            this.signout.TabIndex = 5;
            this.signout.Text = "Đăng Xuất";
            this.signout.UseVisualStyleBackColor = false;
            this.signout.Click += new System.EventHandler(this.signout_Click);
            // 
            // add_goods
            // 
            this.add_goods.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.add_goods.Location = new System.Drawing.Point(105, 17);
            this.add_goods.Margin = new System.Windows.Forms.Padding(2);
            this.add_goods.Name = "add_goods";
            this.add_goods.Size = new System.Drawing.Size(92, 34);
            this.add_goods.TabIndex = 6;
            this.add_goods.Text = "Thêm Đơn Hàng";
            this.add_goods.UseVisualStyleBackColor = false;
            this.add_goods.Click += new System.EventHandler(this.add_goods_Click);
            // 
            // history_diliver
            // 
            this.history_diliver.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.history_diliver.Location = new System.Drawing.Point(298, 17);
            this.history_diliver.Margin = new System.Windows.Forms.Padding(2);
            this.history_diliver.Name = "history_diliver";
            this.history_diliver.Size = new System.Drawing.Size(106, 34);
            this.history_diliver.TabIndex = 7;
            this.history_diliver.Text = "Lịch Sử Giao Hàng";
            this.history_diliver.UseVisualStyleBackColor = false;
            this.history_diliver.Click += new System.EventHandler(this.history_diliver_Click);
            // 
            // edit_info
            // 
            this.edit_info.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.edit_info.Location = new System.Drawing.Point(561, 17);
            this.edit_info.Margin = new System.Windows.Forms.Padding(2);
            this.edit_info.Name = "edit_info";
            this.edit_info.Size = new System.Drawing.Size(105, 34);
            this.edit_info.TabIndex = 9;
            this.edit_info.Text = "Chỉnh Sửa Thông Tin";
            this.edit_info.UseVisualStyleBackColor = false;
            this.edit_info.Click += new System.EventHandler(this.edit_info_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(24, 430);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(951, 185);
            this.dataGridView1.TabIndex = 10;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // userform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1005, 625);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.edit_info);
            this.Controls.Add(this.history_diliver);
            this.Controls.Add(this.add_goods);
            this.Controls.Add(this.signout);
            this.Controls.Add(this.webView21);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "userform";
            this.Text = "userform";
            this.Load += new System.EventHandler(this.userform_Load);
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.Button signout;
        private System.Windows.Forms.Button add_goods;
        private System.Windows.Forms.Button history_diliver;
        private System.Windows.Forms.Button edit_info;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
    }
}