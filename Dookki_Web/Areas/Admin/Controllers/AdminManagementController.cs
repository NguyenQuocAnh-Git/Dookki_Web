using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dookki_Web.Areas.Admin.Controllers
{
    public class AdminManagementController : Controller
    {
        // GET: Admin/AdminManagement
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddAdmin()
        {
            return View();
        }
        public ActionResult EditAdmin()
        {
            return View();
        }
        public ActionResult RemoveAdmin()
        {
            return View();
        }
    }
}