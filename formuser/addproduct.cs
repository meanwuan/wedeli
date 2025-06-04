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
    public partial class addproduct : Form
    {
        private readonly addorders addOrderService = new addorders();
        private readonly string sessionManager = sessionmanager.curentUser; // Assuming sessionmanager is a class that manages user sessions
        private DonHang donHang; // Assuming DonHang is a class that represents an order
        public addproduct(string maNguoiDung = null, DonHang donHang = null)
        {
            InitializeComponent();
            this.donHang = donHang;
            if (maNguoiDung != null) sessionManager = maNguoiDung; // Gán mã người dùng nếu có
            //LoadNhaXeComboBox();
            GanThongTinDonHang();
        }


        private void addproduct_Load(object sender, EventArgs e)
        {

        }

        private void turnback_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("bạn có muốn dừng lại quá trình đặt đơn không", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void conf_Click(object sender, EventArgs e)
        {
            try
            {
                DonHang donhang = new DonHang
                {
                    LoaiDon = loaidon.Text.Trim(),
                    KhoiLuong = double.TryParse(trongtai.Text, out double KhoiLuong)? KhoiLuong : (double?)null,
                    tenNguoiNhan = tennguoinhan.Text.Trim(),
                    DiaChiLayHang = diachinhanhang.Text.Trim(),
                    DiaChiGiaoHang = diachinhanhang.Text.Trim(),
                    // thanh toan
                    MaNguoiDung = sessionmanager.curentUser,
                    //MaPhuongTien = sessionmanager.curentPhuongTien,
                };
                if (string.IsNullOrWhiteSpace(donhang.LoaiDon) || donhang.KhoiLuong == null || string.IsNullOrWhiteSpace(donhang.tenNguoiNhan) || string.IsNullOrWhiteSpace(donhang.DiaChiLayHang) || string.IsNullOrWhiteSpace(donhang.DiaChiGiaoHang))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin đơn hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string maDonHang = addOrderService.themdonhang(donhang);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void GanThongTinDonHang()
        {
            if (donHang != null)
            {
                loaidon.Text = donHang.LoaiDon;
                trongtai.Text = donHang.KhoiLuong?.ToString();
                diachinhanhang.Text = donHang.DiaChiLayHang;
                diachinhanhang.Text = donHang.DiaChiGiaoHang;
                tennguoinhan.Text = donHang.tenNguoiNhan;
                //txtThanhTien.Text = donHang.ThanhTien?.ToString();
                //cboNhaXe.SelectedValue = donHang.MaNhaXe;
            }
        }
    }
}
