using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanDT28.Models
{
    public class Cart
    {
        QLdienthoaiEntities db = new QLdienthoaiEntities();
        public int iMasp { get; set; }
        public string sTensp { get; set; }
        public string sAnhBia { get; set; }
        public double dDonGia { get; set; }
        public int iSoLuong { get; set; }
        public double ThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }
        //Hàm tạo cho giỏ hàng
        public Cart(int Masp)
        {
            iMasp = Masp;
            Sanpham sp = db.Sanphams.Single(n => n.Masp == iMasp);
            sTensp = sp.Tensp;
            sAnhBia = sp.Anhbia;
            dDonGia = double.Parse(sp.Giatien.ToString());
            iSoLuong = 1;
        }
    }
}