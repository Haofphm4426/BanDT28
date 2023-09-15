using BanDT28.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanDT28.Controllers
{
    public class SanphamController : Controller
    {
        // GET: Sanpham
        QLdienthoaiEntities db = new QLdienthoaiEntities();

        // GET: Sanpham

        public ActionResult Index(string SearchString)
        {
            if (String.IsNullOrWhiteSpace(SearchString))
            {
                ViewBag.CountProduct = 0;
                ViewBag.KeySearch = SearchString;
                return View();

            }

            var lsp = db.Sanphams.Where(n => n.Tensp.Contains(SearchString)).ToList();
            ViewBag.CountProduct = lsp.Count();
            ViewBag.KeySearch = SearchString;
            return View(lsp);
        }
        public ActionResult dtiphonepartial()
        {
            var ip = db.Sanphams.Where(n => n.Mahang == 2).Take(4).ToList();
            return PartialView(ip);
        }
        public ActionResult dtsamsungpartial()
        {
            var ss = db.Sanphams.Where(n => n.Mahang == 1).Take(4).ToList();
            return PartialView(ss);
        }
        public ActionResult dtxiaomipartial()
        {
            var mi = db.Sanphams.Where(n => n.Mahang == 3).Take(4).ToList();
            return PartialView(mi);
        }

        public ActionResult xemchitiet(int Masp = 0)
        {
            var chitiet = db.Sanphams.SingleOrDefault(n => n.Masp == Masp);
            if (chitiet == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(chitiet);
        }

        public ActionResult danhsach(int Mahang)
        {
            var mahang = db.Sanphams.Where(n => n.Mahang == Mahang).ToList();
            return View(mahang);
        }

        public ActionResult trangcapnhat()
        {
            return View();
        }
    }
}