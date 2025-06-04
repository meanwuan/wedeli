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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.revenue = new System.Windows.Forms.Button();
            this.about = new System.Windows.Forms.Button();
            this.changecar = new System.Windows.Forms.Button();
            this.logout = new System.Windows.Forms.Button();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.webView21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
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
            this.revenue.Location = new System.Drawing.Point(171, 33);
            this.revenue.Name = "revenue";
            this.revenue.Size = new System.Drawing.Size(124, 55);
            this.revenue.TabIndex = 2;
            this.revenue.Text = "Doanh Thu";
            this.revenue.UseVisualStyleBackColor = true;
            // 
            // about
            // 
            this.about.Location = new System.Drawing.Point(619, 33);
            this.about.Name = "about";
            this.about.Size = new System.Drawing.Size(124, 55);
            this.about.TabIndex = 3;
            this.about.Text = "Thông Tin";
            this.about.UseVisualStyleBackColor = true;
            // 
            // changecar
            // 
            this.changecar.Location = new System.Drawing.Point(385, 33);
            this.changecar.Name = "changecar";
            this.changecar.Size = new System.Drawing.Size(124, 55);
            this.changecar.TabIndex = 4;
            this.changecar.Text = "Điều Chỉnh Xe";
            this.changecar.UseVisualStyleBackColor = true;
            // 
            // logout
            // 
            this.logout.Location = new System.Drawing.Point(841, 33);
            this.logout.Name = "logout";
            this.logout.Size = new System.Drawing.Size(124, 55);
            this.logout.TabIndex = 5;
            this.logout.Text = "Đăng Xuất";
            this.logout.UseVisualStyleBackColor = true;
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(431, 440);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(22, 10);
            this.chart1.TabIndex = 6;
            this.chart1.Text = "chart1";
            // 
            // maintransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 602);
            this.Controls.Add(this.chart1);
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
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button revenue;
        private System.Windows.Forms.Button about;
        private System.Windows.Forms.Button changecar;
        private System.Windows.Forms.Button logout;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    }
}