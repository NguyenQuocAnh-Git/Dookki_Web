using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dookki_Web.Controllers
{
    public class AdminManagementController : Controller
    {
        // GET: AdminManagement
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