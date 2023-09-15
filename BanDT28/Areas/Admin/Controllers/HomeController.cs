using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BanDT28.Models;
using PagedList;

namespace BanDT28.Areas.Admin.Controllers
{
    public class HomeController : Controller

    {
        QLdienthoaiEntities db = new QLdienthoaiEntities();

        // GET: Admin/Home

        public ActionResult Index(int? page)
        {
            // 1. Tham số int? dùng để thể hiện null và kiểu int( số nguyên)
            // page có thể có giá trị là null ( rỗng) và kiểu int.

            // 2. Nếu page = null thì đặt lại là 1.
            if (page == null) page = 1;

            // 3. Tạo truy vấn sql, lưu ý phải sắp xếp theo trường nào đó, ví dụ OrderBy
            // theo Masp mới có thể phân trang.
            var sp = db.Sanphams.OrderBy(x => x.Mahang);

            // 4. Tạo kích thước trang (pageSize) hay là số sản phẩm hiển thị trên 1 trang
            int pageSize = 5;

            // 4.1 Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber.
            int pageNumber = (page ?? 1);

            // 5. Trả về các sản phẩm được phân trang theo kích thước và số trang.
            return View(sp.ToPagedList(pageNumber, pageSize));

        }

        // Xem chi tiết người dùng GET: Admin/Home/Details/5 
        public ActionResult Details(int id)
        {
            var dt = db.Sanphams.Find(id);
            return View(dt);
        }

        // Tạo sản phẩm mới phương thức GET: Admin/Home/Create
        public ActionResult Create()
        {
            return View(new Sanpham() { Soluong = 0, Bonhotrong = 0, Thesim = 0, Ram = 0 });
        }

        // Tạo sản phẩm mới phương thức POST: Admin/Home/Create
        [HttpPost]
        public ActionResult Create(Sanpham sanpham, HttpPostedFileBase fileAnh)
        {
            try
            {
                if(fileAnh == null)
                {
                    ModelState.AddModelError("", "Chưa chọn file ảnh");
                    return View(sanpham);
                }

                if (fileAnh.ContentLength == 0)
                {
                    ModelState.AddModelError("", "File ảnh không có nội dung");
                    return View(sanpham);
                }
                var urlTuongDoi = "/HinhanhSP/";
                var urlTuyetDoi = Server.MapPath(urlTuongDoi);

                fileAnh.SaveAs(urlTuyetDoi + fileAnh.FileName);

                sanpham.Anhbia = fileAnh.FileName;

                //Thêm  sản phẩm mới
                db.Sanphams.Add(sanpham);
                // Lưu lại
                db.SaveChanges();
                // Thành công chuyển đến trang index
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // Sửa sản phẩm GET lấy ra ID sản phẩm: Admin/Home/Edit/5
        public ActionResult Edit(int id)
        {
            // Hiển thị dropdownlist
            var dt = db.Sanphams.Find(id);
            ViewBag.tenAnh = dt.Anhbia;
            // 
            return View(dt);

        }

        // POST: Admin/Home/Edit/5
        [HttpPost]
        public ActionResult Edit(Sanpham sanpham, FormCollection collection, HttpPostedFileBase fileAnh)
        {
            try
            {
                 var oldItem = db.Sanphams.Find(sanpham.Masp);
                if (fileAnh == null)
                {
                    oldItem.Tensp = sanpham.Tensp;
                    oldItem.Giatien = sanpham.Giatien;
                    oldItem.Soluong = sanpham.Soluong;
                    oldItem.Mota = sanpham.Mota;
                    oldItem.Bonhotrong = sanpham.Bonhotrong;
                    oldItem.Ram = sanpham.Ram;
                    oldItem.Thesim = sanpham.Thesim;
                    oldItem.Mahang = sanpham.Mahang;
                    oldItem.Mahdh = sanpham.Mahdh;

                    db.SaveChanges();
                    // xong chuyển qua index
                    return RedirectToAction("Index");
                }
                else
                {
                    if (fileAnh.ContentLength == 0)
                    {
                        ModelState.AddModelError("", "File ảnh không có nội dung");
                        return View(sanpham);
                    }
                    var urlTuongDoi = "/HinhanhSP/";
                    var urlTuyetDoi = Server.MapPath(urlTuongDoi);

                    string fullDuongDan = urlTuyetDoi + fileAnh.FileName;
                    string fullDuongDanCanXoa = urlTuyetDoi + oldItem.Anhbia;

                    if (System.IO.File.Exists(fullDuongDanCanXoa))
                    {
                        System.IO.File.Delete(fullDuongDanCanXoa);
                    }
                    int i = 1;
                    while (System.IO.File.Exists(fullDuongDan) == true)
                    {
                        string ten = Path.GetFileNameWithoutExtension(fileAnh.FileName);
                        string duoi = Path.GetExtension(fileAnh.FileName);
                        fullDuongDan = urlTuyetDoi + ten + "-" + i + duoi;
                        i++;
                    }
                    fileAnh.SaveAs(fullDuongDan);

                    // Sửa sản phẩm theo mã sản phẩm
                    oldItem.Tensp = sanpham.Tensp;
                    oldItem.Giatien = sanpham.Giatien;
                    oldItem.Soluong = sanpham.Soluong;
                    oldItem.Mota = sanpham.Mota;
                    oldItem.Anhbia = Path.GetFileName(fullDuongDan);
                    oldItem.Bonhotrong = sanpham.Bonhotrong;
                    oldItem.Ram = sanpham.Ram;
                    oldItem.Thesim = sanpham.Thesim;
                    oldItem.Mahang = sanpham.Mahang;
                    oldItem.Mahdh = sanpham.Mahdh;
                    // lưu lại
                    db.SaveChanges();
                    // xong chuyển qua index
                    return RedirectToAction("Index");
                } 
            }
            catch
            {
                return View();
            }
        }


        // Xoá sản phẩm phương thức GET: Admin/Home/Delete/5
        public ActionResult Delete(int id)
        {
            var dt = db.Sanphams.Find(id);
            return View(dt);
        }

        // Xoá sản phẩm phương thức POST: Admin/Home/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                //Lấy được thông tin sản phẩm theo ID(mã sản phẩm)
                var dt = db.Sanphams.Find(id);

                var urlTuongDoi = "/HinhanhSP/";
                var urlTuyetDoi = Server.MapPath(urlTuongDoi);

                string fullDuongDan = urlTuyetDoi + dt.Anhbia;
                if(System.IO.File.Exists(fullDuongDan))
                {
                    System.IO.File.Delete(fullDuongDan);
                }    
                // Xoá
                db.Sanphams.Remove(dt);
                // Lưu lại
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Search(string SearchStringAdmin)
        {
            if (String.IsNullOrWhiteSpace(SearchStringAdmin))
            {
                ViewBag.CountProduct = 0;
                ViewBag.KeySearch = SearchStringAdmin;
                return View();

            }

            var lsp = db.Sanphams.Where(n => n.Tensp.Contains(SearchStringAdmin)).ToList();
            ViewBag.CountProduct = lsp.Count();
            ViewBag.KeySearch = SearchStringAdmin;
            return View(lsp);
        }
    }
}
