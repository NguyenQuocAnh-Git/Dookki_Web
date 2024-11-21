using Dookki_Web.App_Start;
using Dookki_Web.Models;
using Dookki_Web.Models.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dookki_Web.Areas.Admin.Controllers
{
    [RoleUser]
    public class AdminManagementController : Controller
    {
        private readonly DOOKKIEntities _context = new DOOKKIEntities();
        // GET: Admin/AdminManagement
        public ActionResult Index()
        {
            return View(new mapAccount().ListAccount());
        }

        [HttpGet]
        public ActionResult AddAdmin()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddAdmin(ACCOUNT account, string phone, string name)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Đặt Role mặc định là "admin"
                    account.Role = "admin";

                    // 2. Thêm ACCOUNT vào database
                    _context.ACCOUNTs.Add(account);
                    _context.SaveChanges(); // Lưu để lấy IDAccount cho bảng Admin

                    // 3. Tạo bản ghi cho bảng Admin
                    Dookki_Web.Models.Admin admin = new Dookki_Web.Models.Admin
                     {
                        IDAccount = account.ID, // ID vừa được tạo trong bảng ACCOUNT
                        Phone = phone,
                        Name = name
                    };
                    _context.Admins.Add(admin);

                    // 4. Lưu cả hai bảng
                    _context.SaveChanges();

                    // 5. Chuyển hướng sau khi thêm thành công
                    return RedirectToAction("Index"); // Redirect đến danh sách admin
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu cần thiết
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            return View(account);
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