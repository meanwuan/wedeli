using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WeDeLi1.Dbase;

namespace WeDeLi1.formtransfer
{
    public partial class listxe : Form
    {
        public listxe()
        {
            InitializeComponent();
            // Gán sự kiện cho các control
            this.Load += listxe_Load;
            this.button1.Click += (sender, e) => this.Close(); // Nút Trở Lại
            // Giả sử bạn đã thêm buttonAdd trong designer
            // this.buttonAdd.Click += buttonAdd_Click; 
            this.dataGridView1.CellContentClick += dataGridView1_CellContentClick;
        }

        private void listxe_Load(object sender, EventArgs e)
        {
            SetupDataGridView();
            LoadData();
        }

        // Cấu hình cho DataGridView
        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Columns.Clear();

            // Thêm các cột dữ liệu
            AddTextColumn("MaPhuongTien", "Mã Phương Tiện");
            AddTextColumn("BienSo", "Biển Số");
            AddTextColumn("LoaiHang", "Loại Hàng Chuyên Chở");
            AddTextColumn("TaiTrong", "Tải Trọng (kg)");
            AddTextColumn("TrangThai", "Trạng Thái");

            // Thêm cột nút "Sửa"
            var editButtonColumn = new DataGridViewButtonColumn
            {
                Name = "EditButton",
                HeaderText = "Sửa",
                Text = "Sửa",
                UseColumnTextForButtonValue = true,
                Width = 60
            };
            dataGridView1.Columns.Add(editButtonColumn);

            // Thêm cột nút "Xóa"
            var deleteButtonColumn = new DataGridViewButtonColumn
            {
                Name = "DeleteButton",
                HeaderText = "Xóa",
                Text = "Xóa",
                UseColumnTextForButtonValue = true,
                Width = 60
            };
            dataGridView1.Columns.Add(deleteButtonColumn);

            dataGridView1.Columns["LoaiHang"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        // Hàm hỗ trợ thêm cột Text
        private void AddTextColumn(string dataPropertyName, string headerText)
        {
            var column = new DataGridViewTextBoxColumn
            {
                DataPropertyName = dataPropertyName,
                Name = dataPropertyName,
                HeaderText = headerText,
                ReadOnly = true
            };
            dataGridView1.Columns.Add(column);
        }

        // Tải dữ liệu từ database và hiển thị lên DataGridView
        private void LoadData()
        {
            try
            {
                using (var dbContext = new databases())
                {
                    var currentNhaXeId = sessionmanager.currentTransport;
                    if (string.IsNullOrEmpty(currentNhaXeId))
                    {
                        MessageBox.Show("Không thể xác định nhà xe hiện tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var vehicleList = dbContext.PhuongTiens
                                               .Where(p => p.MaNhaXe == currentNhaXeId)
                                               .ToList();
                    dataGridView1.DataSource = vehicleList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu xe: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sự kiện click vào nút Thêm Xe (cần tạo button này trong Designer)
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            // Mở form addxe dưới dạng dialog
            using (var addForm = new addxe())
            {
                addForm.ShowDialog();
            }
            // Tải lại dữ liệu sau khi form addxe đóng
            LoadData();
        }

        // Xử lý sự kiện khi click vào một ô trong DataGridView (đặc biệt là các nút)
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Bỏ qua nếu click vào header
            if (e.RowIndex < 0) return;

            string maPhuongTien = dataGridView1.Rows[e.RowIndex].Cells["MaPhuongTien"].Value.ToString();
            string columnName = dataGridView1.Columns[e.ColumnIndex].Name;

            // Nếu nhấn nút Sửa
            if (columnName == "EditButton")
            {
                using (var changeForm = new changexe(maPhuongTien))
                {
                    changeForm.ShowDialog();
                }
                LoadData(); // Tải lại dữ liệu sau khi sửa
            }
            // Nếu nhấn nút Xóa
            else if (columnName == "DeleteButton")
            {
                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa xe có mã '{maPhuongTien}' không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        using (var dbContext = new databases())
                        {
                            var vehicleToDelete = dbContext.PhuongTiens.Find(maPhuongTien);
                            if (vehicleToDelete != null)
                            {
                                // Kiểm tra xem xe có ràng buộc với đơn hàng không
                                bool hasOrders = dbContext.DonHangs.Any(d => d.MaPhuongTien == maPhuongTien);
                                if (hasOrders)
                                {
                                    MessageBox.Show("Không thể xóa xe này vì đã có đơn hàng liên quan.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                                dbContext.PhuongTiens.Remove(vehicleToDelete);
                                dbContext.SaveChanges();
                                LoadData(); // Tải lại dữ liệu
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa xe: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}