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

namespace WeDeLi1
{
    public partial class tinhtrangdonhang : Form
    {
        private readonly addorders addOrderService = new addorders();
        private readonly string sessionManager = sessionmanager.curentUser; // Assuming sessionmanager is a class that manages user sessions
        private readonly string maDonHang; // Assuming maDonHang is a string that represents the order ID
        private readonly XacNhanDonhang xacNhanDonhangService = new XacNhanDonhang(); // Assuming XacNhanDonhang is a class that handles order confirmation
        private readonly conf_addpro confAddProService = new conf_addpro(); // Assuming conf_addpro is a class that handles order confirmation logic 
        public tinhtrangdonhang()
        {
            InitializeComponent();
        }

        private void changepr_Click(object sender, EventArgs e)
        {
            try
            {
                var orders = addOrderService.Getdonhangformlog();
                var donHang = orders.Find(o => o.MaDonHang == maDonHang);
                if (donHang != null)
                {
                    var addProductForm = new addproduct(sessionManager, donHang);
                    addProductForm.ShowDialog(); // Hiển thị form để chỉnh sửa
                }
                else
                {
                    MessageBox.Show("Không tìm thấy đơn hàng để sửa!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi sửa đơn hàng: " + ex.Message);
            }
        }

        private void cancelpro_Click(object sender, EventArgs e)
        {
            try
            {
                confAddProService.XacNhanDonHang(maDonHang, "TuChoi");
                MessageBox.Show("Đơn hàng đã bị hủy thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show("Lỗi khi hủy đơn hàng: " + ex.Message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
