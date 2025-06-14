using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WeDeLi1.Dbase;

namespace WeDeLi1.formtransfer
{
    public partial class listnhanvien : Form
    {
        // Lớp private để định nghĩa các bậc lương cần thiết
        private class RequiredSalaryLevel
        {
            public double BacLuong { get; set; }
            public string MaLuong { get; set; }
            public string TenChucVu { get; set; }
        }

        public listnhanvien()
        {
            InitializeComponent();
            this.Load += listnhanvien_Load;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            button1.Click += (sender, e) => this.Close();
            button3.Click += button3_Click;
        }

        private void listnhanvien_Load(object sender, EventArgs e)
        {
            // MỚI: Gọi hàm kiểm tra và thiết lập bậc lương
            EnsureSalaryLevelsExist();

            SetupDataGridView();
            LoadData();
        }

        // ====================================================================
        // MỚI: HÀM TỰ ĐỘNG THIẾT LẬP BẬC LƯƠNG
        // ====================================================================
        private void EnsureSalaryLevelsExist()
        {
            var currentNhaXeId = sessionmanager.currentTransport;
            if (string.IsNullOrEmpty(currentNhaXeId))
            {
                MessageBox.Show("Không thể xác định nhà xe hiện tại.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Định nghĩa các bậc lương mà hệ thống yêu cầu
            var requiredLevels = new List<RequiredSalaryLevel>
            {
                new RequiredSalaryLevel { BacLuong = 1.0, MaLuong = "NV", TenChucVu = "Nhân viên" },
                new RequiredSalaryLevel { BacLuong = 2.0, MaLuong = "QL", TenChucVu = "Quản lý" }
            };

            try
            {
                using (var dbContext = new databases())
                {
                    bool needsSave = false;
                    foreach (var level in requiredLevels)
                    {
                        // Kiểm tra xem bậc lương này đã tồn tại cho nhà xe chưa
                        bool levelExists = dbContext.MucLuongs.Any(ml => ml.MaNhaXe == currentNhaXeId && ml.BacLuong == level.BacLuong);

                        if (!levelExists)
                        {
                            // Nếu chưa tồn tại, tạo mới
                            var newMucLuong = new MucLuong
                            {
                                MaNhaXe = currentNhaXeId,
                                BacLuong = level.BacLuong,
                                MaLuong = level.MaLuong
                            };
                            dbContext.MucLuongs.Add(newMucLuong);
                            needsSave = true;
                            Console.WriteLine($"Đã tạo bậc lương cho {level.TenChucVu} của nhà xe {currentNhaXeId}.");
                        }
                    }

                    // Chỉ lưu vào CSDL nếu có thay đổi
                    if (needsSave)
                    {
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tự động thiết lập bậc lương: {ex.Message}", "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Các hàm còn lại giữ nguyên ---

        private void SetupDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Columns.Clear();
            AddTextColumn("MaNhanVien", "Mã Nhân Viên");
            AddTextColumn("HoTen", "Họ và Tên");
            AddTextColumn("SoDienThoai", "Số Điện Thoại");

            var editButtonColumn = new DataGridViewButtonColumn { Name = "EditButton", HeaderText = "Sửa", Text = "Sửa", UseColumnTextForButtonValue = true, FlatStyle = FlatStyle.Flat, DefaultCellStyle = { BackColor = Color.Gold, ForeColor = Color.Black }, Width = 60, AutoSizeMode = DataGridViewAutoSizeColumnMode.None };
            dataGridView1.Columns.Add(editButtonColumn);
            var deleteButtonColumn = new DataGridViewButtonColumn { Name = "DeleteButton", HeaderText = "Xóa", Text = "Xóa", UseColumnTextForButtonValue = true, FlatStyle = FlatStyle.Flat, DefaultCellStyle = { BackColor = Color.Tomato, ForeColor = Color.White }, Width = 60, AutoSizeMode = DataGridViewAutoSizeColumnMode.None };
            dataGridView1.Columns.Add(deleteButtonColumn);
            dataGridView1.Columns["HoTen"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void AddTextColumn(string dataPropertyName, string headerText)
        {
            var column = new DataGridViewTextBoxColumn { DataPropertyName = dataPropertyName, Name = dataPropertyName, HeaderText = headerText, ReadOnly = true };
            dataGridView1.Columns.Add(column);
        }

        private void LoadData()
        {
            try
            {
                using (var dbContext = new databases())
                {
                    var currentNhaXeId = sessionmanager.currentTransport;
                    if (string.IsNullOrEmpty(currentNhaXeId)) return;

                    var employeeList = dbContext.NhanViens.Where(nv => nv.MaNhaXe == currentNhaXeId).ToList();
                    dataGridView1.DataSource = employeeList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string selectedMaNV = dataGridView1.Rows[e.RowIndex].Cells["MaNhanVien"].Value.ToString();
            string columnName = dataGridView1.Columns[e.ColumnIndex].Name;

            if (columnName == "EditButton")
            {
                using (var changeForm = new changenhanvien(selectedMaNV)) { changeForm.ShowDialog(); }
                LoadData();
            }
            else if (columnName == "DeleteButton")
            {
                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa nhân viên '{selectedMaNV}' không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        using (var dbContext = new databases())
                        {
                            var employeeToDelete = dbContext.NhanViens.Include(nv => nv.Luongs).FirstOrDefault(nv => nv.MaNhanVien == selectedMaNV);
                            if (employeeToDelete != null)
                            {
                                dbContext.Luongs.RemoveRange(employeeToDelete.Luongs);
                                dbContext.NhanViens.Remove(employeeToDelete);
                                dbContext.SaveChanges();
                                LoadData();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi xóa nhân viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (var adnhanvien = new addnhanvien()) { adnhanvien.ShowDialog(); }
            LoadData();
        }
    }
}