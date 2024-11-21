using Dookki_Web.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dookki_Web.Areas.Admin.Controllers
{
    [RoleUser]
    public class AdminOrderController : Controller
    {
        // GET: Admin/AdminOrder
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Display()
        {
            return View();
        }
        public ActionResult AddItem()
        {
            return View();
        }
        public ActionResult EditItem()
        {
            return View();
        }
        public ActionResult RemoveItem()
        {
            return View();
        }
    }
}