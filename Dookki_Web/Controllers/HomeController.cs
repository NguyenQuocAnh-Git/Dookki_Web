using Dookki_Web.App_Start;
using Dookki_Web.Models;
using Dookki_Web.Models.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.error = "Please enter your full username and password.";
                return View();
            }

            var account = db.ACCOUNTs.SingleOrDefault(a => a.UserName == username && a.Password == password);

            if (account != null)
            {
                Session["Account"] = account;
                Session["UserID"] = account.ID;
                Session["UserName"] = account.UserName;
                Session["Role"] = account.Role;
                return RedirectToAction("index", "Home");
            }
            else
            {
                ViewBag.error = "Please enter your full username and password.";
                return View();
            }
        }
        public ActionResult Logout()
        {
            Session.Clear(); // Xóa tất cả session
            return RedirectToAction("Login", "Home"); // Quay lại trang Login
        }
        public ActionResult Order()
        {
            return View(db.Tickets.ToList());
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string username, string password, string name, string email, string address)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(name))
            {
                ViewBag.Error = "Phone, Password, and Name are required.";
                return View("Register");
            }

            if (db.ACCOUNTs.Any(a => a.UserName == username))
            {
                ViewBag.Error = "This phone number is already registered.";
                return View("Register");
            }

            var newAccount = new ACCOUNT
            {
                UserName = username,
                Password = password,
                Role = "customer"
            };

            db.ACCOUNTs.Add(newAccount);
            db.SaveChanges();

            int accountId = newAccount.ID;

            var newCustomer = new Customer
            {
                Phone = username,
                Name = name,
                Email = email,
                Address = address,
                IDAccount = accountId,
                Marks = 0,
            };

            db.Customers.Add(newCustomer);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Account created successfully. Please login.";
            return RedirectToAction("Login", "Home");
        }
    }
}