using Dookki_Web.Models.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dookki_Web.Contents
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            mapAccount map = new mapAccount();
            var user = map.find(username, password);

            //1. Co: sang trang dashboard admin
            if(user != null)
            {
                return Redirect("/Admin/AdminHome/Index");
            }

            //2. ko co: Quay lai trang login, bao loi
            ViewBag.error = "Tên đăng nhập hoặc mật khẩu không đúng";
            return View();
        }
    }
}