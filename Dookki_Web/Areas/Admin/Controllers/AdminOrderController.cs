﻿using Dookki_Web.Models;
using Dookki_Web.App_Start;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dookki_Web.Areas.Admin.Controllers
{
    [RoleUser]
    public class AdminOrderController : Controller
    {
        // GET: Admin/AdminOrder
        DOOKKIEntities db = new DOOKKIEntities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Display(string searchTerm)
        {
            var tickets = db.Tickets.AsQueryable();
            // Nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                tickets = tickets.Where(t => t.Name.ToLower().Contains(searchTerm));
            }
            
            return View(tickets.ToList());
        }
        [HttpGet]
        public ActionResult AddItem()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddItem(Ticket ticket)
        {
            // Kiểm tra xem tên vé đã tồn tại chưa
            if (db.Tickets.Any(t => t.Name == ticket.Name))
            {
                ModelState.AddModelError("Name", "Tên vé đã tồn tại");
            }
            // Nếu có lỗi, trả về view kèm model để hiển thị lại form với thông báo lỗi
            if (!ModelState.IsValid)
            {
                return View(ticket);
            }

            // Thêm vào cơ sở dữ liệu nếu hợp lệ
            db.Tickets.Add(ticket);
            db.SaveChanges();

            // Chuyển hướng đến trang hiển thị danh sách vé
            return RedirectToAction("Display");


        }

        [HttpGet]
        public ActionResult EditItem(int id)
        {
            // Lấy ra đối tượng sách theo mã
            Ticket ticket = db.Tickets.SingleOrDefault(n => n.ID == id);

            if (ticket == null)
            {
                Response.StatusCode = 404;
                return HttpNotFound();
            }

            return View(ticket);

        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditItem(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Tickets.AddOrUpdate(ticket); // Thêm mới bản ghi thay vì UpdateModel
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Display"); // Chuyển hướng nếu lưu thành công
                }
                catch (Exception ex)
                {
                    ViewBag.Thongbao = "Đã xảy ra lỗi khi lưu dữ liệu: " + ex.Message;
                    return View();
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult RemoveItem(int id)
        {
            // Lấy ra đối tượng ticket theo mã
            Ticket ticket = db.Tickets.SingleOrDefault(n => n.ID == id);
            if (ticket == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(ticket);
        }
        [HttpPost, ActionName("RemoveItem")]
        public ActionResult ApproveRemove(int id)
        {
            Ticket ticket = db.Tickets.SingleOrDefault(n => n.ID == id);
            try
            {
                if (ticket == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                db.Tickets.Remove(ticket);
                db.SaveChanges();
            } catch (Exception ex)
            {
                return View();
            }
            return RedirectToAction("Display");
        }
    }
}