using Dookki_Web.Models;
using System;
using System.Collections.Generic;
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
        public ActionResult EditProfile()
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("Login", "Home"); // Nếu chưa đăng nhập, quay lại Login
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
    }
}