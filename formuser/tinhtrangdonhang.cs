using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeDeLi1.Dbase;
using WeDeLi1.service;
using WeDeLi1.style;

namespace WeDeLi1
{
    public partial class tinhtrangdonhang : Form
    {
        private readonly OrderStatusService orderStatusService;
        private readonly string sessionManager = sessionmanager.curentUser;
        private readonly string maDonHang;

        public tinhtrangdonhang(string maDonHang)
        {
            InitializeComponent();
            this.maDonHang = maDonHang;
            orderStatusService = new OrderStatusService();
            LoadOrderStatus();
        }

        private void LoadOrderStatus()
        {
            borderbutton.bogocbovien(changepr, 12);
            borderbutton.bogocbovien(cancelpro, 12);
            borderbutton.bogocbovien(followpro, 12); // Assuming label2 is a Label control for displaying order status
            try
            {
                var orderStatus = orderStatusService.GetOrderStatus(maDonHang);
                label2.Text = $"Trạng thái: {orderStatus.TrangThai}"; // Assuming statusLabel is a Label control
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void changepr_Click(object sender, EventArgs e)
        {
            try
            {
                var donHang = orderStatusService.GetOrderForEdit(maDonHang);
                var addProductForm = new addproduct(sessionManager, donHang);
                addProductForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cancelpro_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Bạn có chắc muốn hủy đơn hàng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    bool success = orderStatusService.CancelOrder(maDonHang);
                    if (success)
                    {
                        MessageBox.Show("Đơn hàng đã được hủy thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy đơn hàng để hủy.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tinhtrangdonhang_Load(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}