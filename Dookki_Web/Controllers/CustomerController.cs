﻿using Dookki_Web.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dookki_Web.Controllers
{
    public class CustomerController : Controller
    {
        DOOKKIEntities db = new DOOKKIEntities();
        // GET: Customer
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
            return RedirectToAction("Login", "Customer"); // Quay lại trang Login
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
            return RedirectToAction("Login", "Customer");
        }
        public ActionResult EditProfile()
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("Login", "Customer"); // Nếu chưa đăng nhập, quay lại Login
            }

            int accountId = (int)Session["UserID"];
            var customer = db.Customers.FirstOrDefault(c => c.IDAccount == accountId);

            if (customer != null)
            {
                return View(customer); // Truyền dữ liệu khách hàng vào View
            }
            else
            {
                return RedirectToAction("Index", "Home"); // Nếu không tìm thấy khách hàng, quay về trang chính
            }
        }
        [HttpPost]
        public ActionResult EditProfile(Customer updatedCustomer)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem số điện thoại đã tồn tại cho khách hàng khác hay không
                var duplicatePhone = db.Customers
                    .FirstOrDefault(c => c.Phone == updatedCustomer.Phone && c.ID != updatedCustomer.ID);

                if (duplicatePhone != null)
                {
                    ViewBag.Error = "The phone number is already in use by another customer.";
                    return View(updatedCustomer);
                }

                // Lấy bản ghi khách hàng hiện tại từ database
                var existingCustomer = db.Customers.FirstOrDefault(c => c.ID == updatedCustomer.ID);

                if (existingCustomer != null)
                {
                    // Cập nhật thông tin khách hàng
                    existingCustomer.Name = updatedCustomer.Name;
                    existingCustomer.Phone = updatedCustomer.Phone;
                    existingCustomer.Email = updatedCustomer.Email;
                    existingCustomer.Address = updatedCustomer.Address;

                    // Lưu thay đổi
                    db.SaveChanges();
                    ViewBag.Success = "Profile updated successfully!";
                }
                else
                {
                    ViewBag.Error = "Customer not found.";
                }
            }
            else
            {
                ViewBag.Error = "Invalid data.";
            }

            return View(updatedCustomer);
        }
        
        public ActionResult HistoryOrder()
        {
            int accountID = int.Parse(Session["UserID"].ToString());
            var cus = db.Customers.FirstOrDefault(c => c.IDAccount == accountID);

            var orders = db.Orders
                .Where(o => o.Status == "Finish" && o.customerID == cus.ID)
                .Include(o => o.OrderDetails.Select(od => od.Ticket))  // Include Ticket in OrderDetails
                .Include(o => o.OrderDetails.Select(od => od.Payment))  // Include Payment in OrderDetails
                .ToList();

            List<OrderViewModel> ordersViewModel = orders.Select(o => new OrderViewModel
            {
                IDOrder = o.ID,
                Day = o.OrderDetails.ElementAt(0).Payment.day,  
                Time = o.Time,
                Status = o.Status,
                Money = o.OrderDetails.Sum(od => od.quantily * (od.Ticket != null ? od.Ticket.Price : 0)) 
            }).ToList();

            return View(ordersViewModel);
        }
        public ActionResult CurrentOrder()
        {
            int accountID = int.Parse(Session["UserID"].ToString());
            var cus = db.Customers.FirstOrDefault(c => c.IDAccount == accountID);

            var orders = db.Orders
                .Where(o => (o.Status == "Pending" || o.Status == "Accepted" || o.Status == "Delivering") && o.customerID == cus.ID)
                .Include(o => o.OrderDetails.Select(od => od.Ticket))  // Include Ticket in OrderDetails
                .Include(o => o.OrderDetails.Select(od => od.Payment))  // Include Payment in OrderDetails
                .ToList();

            List<OrderViewModel> ordersViewModel = orders.Select(o => new OrderViewModel
            {
                IDOrder = o.ID,
                Day = o.OrderDetails.ElementAt(0).Payment.day,
                Time = o.Time,
                Status = o.Status,
                Money = o.OrderDetails.Sum(od => od.quantily * (od.Ticket != null ? od.Ticket.Price : 0))
            }).ToList();

            return View(ordersViewModel);
        }
        public ActionResult DetailOrder(int id)
        {
            var order = db.Orders.Where(o => o.ID == id)
                .Include(o => o.OrderDetails.Select(od => od.Ticket))
                .Include(o=> o.OrderDetails.Select(od => od.Payment)).SingleOrDefault();

            return View(order);
        }
        public ActionResult CancelOrder(int id)
        {
            var order = db.Orders.FirstOrDefault(o => o.ID == id);
            order.Status = "Deleted";
            db.Orders.AddOrUpdate(order);
            db.SaveChanges();
            return RedirectToAction("CurrentOrder","Customer");
        }
        public ActionResult EditOrder(int id)
        {
            var order = db.Orders.Where(o=>o.ID == id)
                .Include(o=>o.OrderDetails)
                .Include(o=>o.OrderDetails.Select(od=>od.Ticket))
                .SingleOrDefault();
            var orderDetails = order.OrderDetails;
            List<Cart> cart = new List<Cart>();
            foreach (var orderDetail in orderDetails)
            {
                cart.Add(new Cart(orderDetail.Ticket.ID, (int)orderDetail.quantily));
            }
            Session["Cart"] = cart;
            return RedirectToAction("DisplayCart","Order", new { isUpdate = true, idOrderUpdate = id });
        }
    }
}