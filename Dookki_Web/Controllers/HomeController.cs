using Dookki_Web.App_Start;
using Dookki_Web.Models;
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
        DOOKKIEntities db = new DOOKKIEntities();
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
            ViewBag.error = "Tên đăng nhập hoặc mật khẩu không đúng";
            return View();
        }
        public ActionResult Order()
        {
            return View(db.Tickets.ToList());
        }
    }
}