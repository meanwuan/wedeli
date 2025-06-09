namespace WeDeLi1
{
    partial class tinhtrangdonhang
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.changepr = new System.Windows.Forms.Button();
            this.cancelpro = new System.Windows.Forms.Button();
            this.followpro = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(66, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(327, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tình Trạng đơn hàng của bạn";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(43, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(350, 45);
            this.label2.TabIndex = 1;
            this.label2.Text = "Chờ Nhà Xe Xác Nhận";
            // 
            // changepr
            // 
            this.changepr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.changepr.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.changepr.Location = new System.Drawing.Point(86, 154);
            this.changepr.Name = "changepr";
            this.changepr.Size = new System.Drawing.Size(247, 81);
            this.changepr.TabIndex = 2;
            this.changepr.Text = "Chỉnh Sửa Đơn Hàng";
            this.changepr.UseVisualStyleBackColor = false;
            this.changepr.Click += new System.EventHandler(this.changepr_Click);
            // 
            // cancelpro
            // 
            this.cancelpro.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.cancelpro.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelpro.Location = new System.Drawing.Point(86, 241);
            this.cancelpro.Name = "cancelpro";
            this.cancelpro.Size = new System.Drawing.Size(247, 81);
            this.cancelpro.TabIndex = 3;
            this.cancelpro.Text = "Huỷ Đơn Hàng";
            this.cancelpro.UseVisualStyleBackColor = false;
            this.cancelpro.Click += new System.EventHandler(this.cancelpro_Click);
            // 
            // followpro
            // 
            this.followpro.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.followpro.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.followpro.Location = new System.Drawing.Point(86, 328);
            this.followpro.Name = "followpro";
            this.followpro.Size = new System.Drawing.Size(247, 81);
            this.followpro.TabIndex = 4;
            this.followpro.Text = "Theo dõi đơn hàng của bạn";
            this.followpro.UseVisualStyleBackColor = false;
            // 
            // tinhtrangdonhang
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 433);
            this.Controls.Add(this.followpro);
            this.Controls.Add(this.cancelpro);
            this.Controls.Add(this.changepr);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "tinhtrangdonhang";
            this.Text = "tinhtrangdonhang";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button changepr;
        private System.Windows.Forms.Button cancelpro;
        private System.Windows.Forms.Button followpro;
    }
}