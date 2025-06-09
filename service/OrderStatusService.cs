using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WeDeLi1.Dbase;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace WeDeLi1.service
{
    public class OrderStatusService
    {
        private readonly string logfile = "don_hang_cho_xacnhan_log.json";
        private readonly string pendingLogfile = "don_hang_tam_log.json";
        private readonly addorders addOrderService;

        public OrderStatusService()
        {
            addOrderService = new addorders();
        }

        public dynamic GetOrderStatus(string maDonHang)
        {
            try
            {
                var orders = addOrderService.Getdonhangformlog();
                var pendingOrders = addOrderService.GetPendingOrders();

                var order = orders.FirstOrDefault(o => o.MaDonHang == maDonHang);
                var pendingOrder = pendingOrders.FirstOrDefault(o => o.MaDonHangTam == maDonHang);

                if (order != null)
                {
                    return new
                    {
                        MaDonHang = order.MaDonHang,
                        TrangThai = order.TrangThai,
                        LoaiDon = order.LoaiDon,
                        KhoiLuong = order.KhoiLuong,
                        DiaChiLayHang = order.DiaChiLayHang,
                        DiaChiGiaoHang = order.DiaChiGiaoHang,
                        tenNguoiNhan = order.tenNguoiNhan,
                        TongTien = order.TongTien,
                        PhuongThucThanhToan = order.PhuongThucThanhToan
                    };
                }
                else if (pendingOrder != null)
                {
                    return new
                    {
                        MaDonHang = pendingOrder.MaDonHangTam,
                        TrangThai = pendingOrder.TrangThai,
                        LoaiDon = pendingOrder.LoaiDon,
                        KhoiLuong = pendingOrder.KhoiLuong,
                        DiaChiLayHang = pendingOrder.DiaChiLayHang,
                        DiaChiGiaoHang = pendingOrder.DiaChiGiaoHang,
                        tenNguoiNhan = pendingOrder.tenNguoiNhan,
                        TongTien = pendingOrder.TongTien,
                        PhuongThucThanhToan = pendingOrder.PhuongThucThanhToan
                    };
                }
                else
                {
                    throw new Exception("Không tìm thấy đơn hàng.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy trạng thái đơn hàng: {ex.Message}");
            }
        }

        public bool CancelOrder(string maDonHang)
        {
            try
            {
                return addOrderService.CancelOrder(maDonHang);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi hủy đơn hàng: {ex.Message}");
            }
        }

        public DonHang GetOrderForEdit(string maDonHang)
        {
            try
            {
                var orders = addOrderService.Getdonhangformlog();
                var pendingOrders = addOrderService.GetPendingOrders();
                var order = orders.FirstOrDefault(o => o.MaDonHang == maDonHang);
                var pendingOrder = pendingOrders.FirstOrDefault(o => o.MaDonHangTam == maDonHang);

                DonHang editDonHang = order;
                if (pendingOrder != null)
                {
                    editDonHang = new DonHang
                    {
                        MaDonHang = pendingOrder.MaDonHangTam,
                        LoaiDon = pendingOrder.LoaiDon,
                        KhoiLuong = pendingOrder.KhoiLuong,
                        tenNguoiNhan = pendingOrder.tenNguoiNhan,
                        DiaChiLayHang = pendingOrder.DiaChiLayHang,
                        DiaChiGiaoHang = pendingOrder.DiaChiGiaoHang,
                        ThoiGianLayHang = pendingOrder.ThoiGianLayHang,
                        ThoiGianGiaoHang = pendingOrder.ThoiGianGiaoHang,
                        TongTien = pendingOrder.TongTien,
                        PhuongThucThanhToan = pendingOrder.PhuongThucThanhToan
                    };
                }

                if (editDonHang == null)
                {
                    throw new Exception("Không tìm thấy đơn hàng để chỉnh sửa.");
                }

                return editDonHang;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin đơn hàng để chỉnh sửa: {ex.Message}");
            }
        }
    }
}