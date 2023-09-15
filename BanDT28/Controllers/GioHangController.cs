using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BanDT28.Models;
using System.Transactions;

namespace BanDT28.Controllers
{
    public class GioHangController : Controller
    {
        QLdienthoaiEntities db = new QLdienthoaiEntities();
        // GET: GioHang

        //Lấy giỏ hàng 
        public List<Cart> LayGioHang()
        {
            List<Cart> lstGioHang = Session["GioHang"] as List<Cart>;
            if (lstGioHang == null)
            {
                //Nếu giỏ hàng chưa tồn tại thì mình tiến hành khởi tao list giỏ hàng (sessionGioHang)
                lstGioHang = new List<Cart>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        //Thêm giỏ hàng
        public ActionResult ThemGioHang(int iMasp, string strURL)
        {
            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp == iMasp);
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy ra session giỏ hàng
            List<Cart> lstGioHang = LayGioHang();
            //Kiểm tra sp này đã tồn tại trong session[giohang] chưa
            Cart sanpham = lstGioHang.Find(n => n.iMasp == iMasp);
            if (sanpham == null)
            {
                sanpham = new Cart(iMasp);
                //Add sản phẩm mới thêm vào list
                lstGioHang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoLuong++;
                return Redirect(strURL);
            }
        }
        //Cập nhật giỏ hàng 
        public ActionResult CapNhatGioHang(int iMaSP, FormCollection f)
        {
            //Kiểm tra masp
            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp == iMaSP);
            //Nếu get sai masp thì sẽ trả về trang lỗi 404
            if (sp == null)
            {
                return null;
            }
            //Lấy giỏ hàng ra từ session
            List<Cart> lstGioHang = LayGioHang();
            //Kiểm tra sp có tồn tại trong session["GioHang"]
            Cart sanpham = lstGioHang.SingleOrDefault(n => n.iMasp == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            if (sanpham != null)
            {
                int number;
                if(int.TryParse(f["txtSoLuong"].ToString(), out number))
                {
                    sanpham.iSoLuong = int.Parse(f["txtSoLuong"].ToString());

                }
                else
                {
                    ModelState.AddModelError("", "Vui lòng nhập số");
                    return View("SuaGioHang", lstGioHang);
                }


            }
            return RedirectToAction("GioHang");
        }
        //Xây dựng 1 view cho người dùng chỉnh sửa giỏ hàng
        public ActionResult SuaGioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<Cart> lstGioHang = LayGioHang();
            return View(lstGioHang);

        }
        //Xóa giỏ hàng
        public ActionResult XoaGioHang(int iMaSP)
        {
            //Kiểm tra masp
            Sanpham sp = db.Sanphams.SingleOrDefault(n => n.Masp == iMaSP);
            //Nếu get sai masp thì sẽ trả về trang lỗi 404
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Lấy giỏ hàng ra từ session
            List<Cart> lstGioHang = LayGioHang();
            Cart sanpham = lstGioHang.SingleOrDefault(n => n.iMasp == iMaSP);
            //Nếu mà tồn tại thì chúng ta cho sửa số lượng
            if (sanpham != null)
            {
                lstGioHang.RemoveAll(n => n.iMasp == iMaSP);

            }
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("GioHang");
        }
        //Xây dựng trang giỏ hàng
        public ActionResult GioHang()
        {
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<Cart> lstGioHang = LayGioHang();
            double totalCount = 0;
            foreach(var item in lstGioHang)
            {
                totalCount = totalCount + item.ThanhTien;
            }
            ViewBag.totalCount = totalCount;
            return View(lstGioHang);
        }
        //Tính tổng số lượng và tổng tiền
        //Tính tổng số lượng
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Cart> lstGioHang = Session["GioHang"] as List<Cart>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        //Tính tổng thành tiền
        private double TongTien()
        {
            double dTongTien = 0;
            List<Cart> lstGioHang = Session["GioHang"] as List<Cart>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.ThanhTien);
            }
            return dTongTien;
        }
        //tạo partial giỏ hàng
        public ActionResult GioHangPartial()
        {
            if (TongSoLuong() == 0)
            {
                return PartialView();
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        

        #region // Mới hoàn thiện
        //Xây dựng chức năng đặt hàng
        [HttpPost]
        public ActionResult DatHang(FormCollection donhang)
        {
            //Kiểm tra đăng đăng nhập
            if (Session["use"] == null || Session["use"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }
            //Kiểm tra giỏ hàng
            if (Session["GioHang"] == null)
            {
                RedirectToAction("Index", "Home");
            }
            using (TransactionScope tranScope = new TransactionScope())
            {
                try
                {
                    //
                    //Thêm đơn hàng
                    Donhang ddh = new Donhang();
                    Nguoidung kh = (Nguoidung)Session["use"];
                    List<Cart> gh = LayGioHang();
                    if(String.IsNullOrEmpty(donhang["Diachinhanhang"].ToString()))
                    {
                        return RedirectToAction("ThanhToanDonHang");
                    } 
                    
                    ddh.MaNguoidung = kh.MaNguoiDung;
                    ddh.Ngaydat = DateTime.Now;
                    int ptthanhtoan = Int32.Parse(donhang["MaTT"].ToString());
                    ddh.Phuongthucthanhtoan = ptthanhtoan;
                    ddh.Diachinhanhang = donhang["Diachinhanhang"].ToString();
                    db.Donhangs.Add(ddh);
                    db.SaveChanges();
                    //Thêm chi tiết đơn hàng
                    foreach (var item in gh)
                    {
                        Chitietdonhang ctDH = new Chitietdonhang();
                        decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                        ctDH.Madon = ddh.Madon;
                        ctDH.Masp = item.iMasp;
                        ctDH.Soluong = item.iSoLuong;
                        ctDH.Dongia = (decimal)item.dDonGia;
                        ctDH.Thanhtien = (decimal)thanhtien;
                        db.Chitietdonhangs.Add(ctDH);
                    }
                    db.SaveChanges();
                    Session["GioHang"] = null;
                    tranScope.Complete();
                }
                catch (Exception)
                {
                    tranScope.Dispose();
                    throw;
                }
            }
            return RedirectToAction("Index", "Donhangs");
        }
        #endregion

        public ActionResult ThanhToanDonHang()
        {

            ViewBag.MaTT = new SelectList(new[]
                {
                    new { MaTT = 1, TenPT="Thanh toán khi nhận hàng" },
                    new { MaTT = 2, TenPT="Thanh toán chuyển khoản" },
                }, "MaTT", "TenPT", 1);
            

            //Kiểm tra đăng đăng nhập
            if (Session["use"] == null || Session["use"].ToString() == "")
            {
                return RedirectToAction("Dangnhap", "User");
            }
            //Kiểm tra giỏ hàng
            if (Session["GioHang"] == null)
            {
                RedirectToAction("Index", "Home");
            }
            //Thêm đơn hàng
            Donhang ddh = new Donhang();
            Nguoidung kh = (Nguoidung)Session["use"];
            List<Cart> gh = LayGioHang();
            decimal tongtien = 0;
            foreach (var item in gh)
            {
                decimal thanhtien = item.iSoLuong * (decimal)item.dDonGia;
                tongtien += thanhtien;
            }
            ViewBag.tenNguoidung = kh.Hoten;
            ddh.MaNguoidung = kh.MaNguoiDung;
            ddh.Ngaydat = DateTime.Now;
            ddh.Diachinhanhang = kh.Diachi;
            Chitietdonhang ctDH = new Chitietdonhang();
            ViewBag.tongtien = tongtien;
            return View(ddh);

        }

    }
}