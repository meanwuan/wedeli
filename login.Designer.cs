namespace WeDeLi1
{
    partial class login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(login));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.clear = new System.Windows.Forms.Button();
            this.conf = new System.Windows.Forms.Button();
            this.register = new System.Windows.Forms.Button();
            this.username = new System.Windows.Forms.TextBox();
            this.pass = new System.Windows.Forms.TextBox();
            this.capcha = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.reloandcap = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 25.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(149, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(238, 48);
            this.label1.TabIndex = 0;
            this.label1.Text = "Đăng Nhập";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(68, 183);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 22);
            this.label2.TabIndex = 1;
            this.label2.Text = "Tên Đăng Nhập";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(68, 232);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Mật Khẩu";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(68, 310);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Capcha";
            // 
            // clear
            // 
            this.clear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clear.Location = new System.Drawing.Point(99, 471);
            this.clear.Name = "clear";
            this.clear.Size = new System.Drawing.Size(123, 65);
            this.clear.TabIndex = 4;
            this.clear.Text = "Xoá";
            this.clear.UseVisualStyleBackColor = false;
            this.clear.Click += new System.EventHandler(this.clear_Click);
            // 
            // conf
            // 
            this.conf.BackColor = System.Drawing.Color.SpringGreen;
            this.conf.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conf.Location = new System.Drawing.Point(274, 471);
            this.conf.Name = "conf";
            this.conf.Size = new System.Drawing.Size(113, 65);
            this.conf.TabIndex = 5;
            this.conf.Text = "Xác Nhận";
            this.conf.UseVisualStyleBackColor = false;
            this.conf.Click += new System.EventHandler(this.conf_Click);
            // 
            // register
            // 
            this.register.BackColor = System.Drawing.Color.Aqua;
            this.register.Location = new System.Drawing.Point(403, 542);
            this.register.Name = "register";
            this.register.Size = new System.Drawing.Size(98, 57);
            this.register.TabIndex = 6;
            this.register.Text = "Đăng Ký";
            this.register.UseVisualStyleBackColor = false;
            this.register.Click += new System.EventHandler(this.register_Click);
            // 
            // username
            // 
            this.username.Location = new System.Drawing.Point(278, 185);
            this.username.Name = "username";
            this.username.Size = new System.Drawing.Size(191, 22);
            this.username.TabIndex = 7;
            // 
            // pass
            // 
            this.pass.Location = new System.Drawing.Point(278, 230);
            this.pass.Name = "pass";
            this.pass.Size = new System.Drawing.Size(191, 22);
            this.pass.TabIndex = 8;
            // 
            // capcha
            // 
            this.capcha.Location = new System.Drawing.Point(257, 310);
            this.capcha.Name = "capcha";
            this.capcha.Size = new System.Drawing.Size(171, 22);
            this.capcha.TabIndex = 9;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(278, 269);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(127, 20);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "hiển thị mật khẩu";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // reloandcap
            // 
            this.reloandcap.Location = new System.Drawing.Point(353, 418);
            this.reloandcap.Name = "reloandcap";
            this.reloandcap.Size = new System.Drawing.Size(75, 34);
            this.reloandcap.TabIndex = 11;
            this.reloandcap.Text = "tải lại capcha";
            this.reloandcap.UseVisualStyleBackColor = true;
            this.reloandcap.Click += new System.EventHandler(this.reloandcap_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(257, 353);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(176, 59);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 605);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.reloandcap);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.capcha);
            this.Controls.Add(this.pass);
            this.Controls.Add(this.username);
            this.Controls.Add(this.register);
            this.Controls.Add(this.conf);
            this.Controls.Add(this.clear);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "login";
            this.Text = "WeDeLi";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button clear;
        private System.Windows.Forms.Button conf;
        private System.Windows.Forms.Button register;
        private System.Windows.Forms.TextBox username;
        private System.Windows.Forms.TextBox pass;
        private System.Windows.Forms.TextBox capcha;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button reloandcap;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}