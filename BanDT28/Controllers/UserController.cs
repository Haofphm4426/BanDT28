using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BanDT28.Models;
namespace BanDT28.Controllers
{
    public class UserController : Controller
    {
        QLdienthoaiEntities db = new QLdienthoaiEntities();
        // ĐĂNG KÝ
        public ActionResult Dangky()
        {
            return View();
        }

        // ĐĂNG KÝ PHƯƠNG THỨC POST
        [HttpPost]
        public ActionResult Dangky(Nguoidung nguoidung, FormCollection user)
        {
            try
            {
                string confirmPassword = user["confirmPassword"].ToString();

                if (string.IsNullOrEmpty(nguoidung.Hoten) == true)
                {
                    ModelState.AddModelError("", "Họ và tên không được để trống");
                    return View(nguoidung);
                }
                if (string.IsNullOrEmpty(nguoidung.Email) == true)
                {
                    ModelState.AddModelError("", "Tên đăng nhập không được để trống");
                    return View(nguoidung);
                }
                if (string.IsNullOrEmpty(nguoidung.Matkhau) == true)
                {
                    ModelState.AddModelError("", "Mật khẩu không được để trống");
                    return View(nguoidung);

                }
                var userCreated = db.Nguoidungs.SingleOrDefault(x => x.Email.Equals(nguoidung.Email));
                if (userCreated != null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại !");
                    return View(nguoidung);
                }
                if (nguoidung.Matkhau.Equals(confirmPassword) == false)
                {
                    ModelState.AddModelError("", "Mật khẩu nhập lại không chính xác!");
                    return View(nguoidung);
                }
                nguoidung.IDQuyen = 1;
                // Thêm người dùng  mới
                db.Nguoidungs.Add(nguoidung);
                // Lưu lại vào cơ sở dữ liệu
                db.SaveChanges();
                // Nếu dữ liệu đúng thì trả về trang đăng nhập
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Dangnhap");
                }
                return View("Dangky");

            }
            catch
            {
                return View();
            }
        }

        public ActionResult Dangnhap()
        {
            return View();

        }


        [HttpPost]
        public ActionResult Dangnhap(Nguoidung nguoidung)
        {
            //string userMail = userlog["Email"].ToString();

            var islogin = db.Nguoidungs.SingleOrDefault(x => x.Email.Equals(nguoidung.Email) && x.Matkhau.Equals(nguoidung.Matkhau));

            if (string.IsNullOrEmpty(nguoidung.Email) == true)
            {
                ModelState.AddModelError("", "Tên đăng nhập không được để trống");
                return View(nguoidung);
            }

            if (string.IsNullOrEmpty(nguoidung.Matkhau) == true)
            {
                ModelState.AddModelError("", "Mật khẩu không được để trống");
                return View(nguoidung);
            }

            

            

            if (islogin != null)
            {
                if (nguoidung.Email == "Admin@gmail.com")
                {
                    Session["use"] = islogin;
                    return RedirectToAction("Index", "Admin/Home");
                }
                else
                {
                    Session["use"] = islogin;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Đăng nhập thất bại. Vui lòng kiểm tra lại Email hoặc Mật khẩu");
                return View("Dangnhap");
            }

        }
        public ActionResult DangXuat()
        {
            Session["use"] = null;
            return RedirectToAction("Index", "Home");

        }


    }
}