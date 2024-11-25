using Dookki_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Parser.SyntaxTree;

namespace Dookki_Web.Controllers
{
    public class OrderController : Controller
    {
        DOOKKIEntities db = new DOOKKIEntities();
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DisplayCart()
        {
            List<Cart> cart = GetCart();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index");
            }
            ViewBag.NumProduct = NumProduct();
            ViewBag.Total = Total();
            return View(cart);
        }
        public ActionResult DisplayCartPartial()
        {
            var num = NumProduct();
            if (num != 0)
            {
                ViewBag.NumProduct = num;
            }
            else
            {
                ViewBag.NumProduct = "";
            }

            return PartialView();
        }
        public List<Cart> GetCart()
        {
            List<Cart> cart = Session["Cart"] as List<Cart>;
            if(cart == null)
            {
                cart = new List<Cart>();
                Session["Cart"] = cart;
            }
            return cart;
        }
        public ActionResult AddtoCart(int ticketID)
        {
            List<Cart> cart = GetCart();
            Cart tmp  = cart.Find(n => n.ticketID == ticketID);
            if (tmp == null) {
                tmp = new Cart(ticketID);
                cart.Add(tmp);
            }
            else
            {
                tmp.quantily++;
            }
            return RedirectToAction("DisplayCart", "Order");
        }
        private int NumProduct()
        {
            int sum = 0;
            List<Cart> cart = Session["Cart"] as List<Cart>;
            if (cart != null)
            {
                sum = cart.Sum(n => n.quantily);
            }
            return sum;
        }
        private double Total()
        {
            double sum= 0;
            List<Cart> cart = Session["Cart"] as List<Cart>;
            if (cart != null)
            {
                sum = double.Parse(cart.Sum(n => n.quantily * n.price).ToString());
            }
            return sum;
        }
        public ActionResult UpdateCart(int ID, FormCollection f)
        {
            List<Cart> cart = GetCart();

            Cart item = cart.SingleOrDefault(n => n.ticketID == ID);

            if (item != null)
            {
                item.quantily= int.Parse(f["quantity"].ToString());
            }

            return RedirectToAction("DisplayCart");
        }
        public ActionResult DeleteCart(int ID)
        {
            List<Cart> cart = GetCart();

            if (cart.Count == 0)
            {
                return RedirectToAction("DisplayCart");
            }

            Cart item = cart.SingleOrDefault(n => n.ticketID == ID);


            if (item != null)
            {
                cart.RemoveAll(n => n.ticketID == ID);
            }
            return RedirectToAction("DisplayCart");
        }
        [HttpGet]
        public ActionResult FillInfo()
        {
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<Cart> cart = GetCart();
            ViewBag.NumProduct = NumProduct();
            ViewBag.Total = Total();
            return View(cart);
        }
        [HttpPost]
        public ActionResult FillInfo(FormCollection collection)
        {
            var firstName = collection["FirstName"];
            var lastName = collection["LastName"];
            var address = collection["Address"];
            var phone = collection["Phone"];
            var email = collection["Email"];
            if(String.IsNullOrEmpty(firstName))
            {
                ViewData["firstName"] = "This is required information";
            }
            else if(String.IsNullOrEmpty(lastName))
            {
                ViewData["lastName"] = "This is required information";
            }
            else if (String.IsNullOrEmpty(address))
            {
                ViewData["address"] = "This is required information";
            }
            else if (String.IsNullOrEmpty(phone))
            {
                ViewData["phone"] = "This is required information";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewData["email"] = "This is required information";
            }
            else
            {
                //Add customer (info)
                Customer cus =  new Customer();
                cus.Name = firstName + lastName;
                cus.Email = email;
                cus.Address = address;
                cus.Phone = phone;
                cus.Marks = 0;
                cus.IDAccount = 0;
                //Add tmp account
                ACCOUNT tmp = new ACCOUNT();
                tmp.UserName = cus.Phone;
                tmp.Password = cus.Phone;
                tmp.Role = "temp";
                //Save tmp account
                Session["Account"] = tmp;
                if(db.Customers.SingleOrDefault(n=>n.Phone == cus.Phone) == null)
                {
                    db.Customers.Add(cus);
                    db.SaveChanges();
                }
                return RedirectToAction("Confirm");
            }
            return this.FillInfo();
        }
        [HttpGet]
        public ActionResult Confirm()
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ACCOUNT acc = (ACCOUNT)Session["Account"];
            Customer cus = db.Customers.SingleOrDefault(x => x.Phone == acc.UserName);
            ViewBag.Name = cus.Name;
            ViewBag.Phone = cus.Phone;
            ViewBag.Email = cus.Email;
            ViewBag.Address = cus.Address;

            // Lay gio hang tu Session
            List<Cart> cart = GetCart();
            ViewBag.NumProduct = NumProduct();
            ViewBag.Total = Total();
            return View(cart);
        }
        public ActionResult Confirm(FormCollection collection)
        {
            //True = Cash, False = transfer
            bool cash = Convert.ToBoolean(collection["Payment"].Split(',')[0]);

            Order newOrder = new Order();

            ACCOUNT acc = (ACCOUNT)Session["Account"];
            Customer client = db.Customers.SingleOrDefault(x => x.Phone == acc.UserName);
            List<Cart> cart = GetCart();

            newOrder.customerID = client.ID;
            newOrder.Time = DateTime.Now.TimeOfDay;
            newOrder.discount = 0;

            Payment payment = new Payment();
            payment.amount = decimal.Parse(Total().ToString());
            payment.day = DateTime.Now;
            if (cash)
            {
                payment.paymentMethodID = 1;
            }
            else
            {
                payment.paymentMethodID = 2;
            }
            db.Payments.Add(payment);
            db.Orders.Add(newOrder);
            db.SaveChanges();


            foreach (var item in cart)
            {
                OrderDetail detail = new OrderDetail();

                detail.quantily= item.quantily;
                detail.ticketID = item.ticketID;
                detail.paymentID = payment.ID;
                detail.orderID = newOrder.ID;

                db.OrderDetails.Add(detail);
                db.SaveChanges();
            }

            return RedirectToAction("DisplayBill", "Order");
        }
        public ActionResult DisplayBill()
        {
            ViewBag.Total = Total();
            ViewBag.NumProduct = NumProduct();
            List<Cart> cart = GetCart();
            ViewBag.Cart = cart;
            Session["Cart"] = null;
            return View();
        }
        [HttpGet]
        public ActionResult BookTable()
        {
            //ViewBag.StatusTable = "Available";
            //if(false)
            //{
            //    return RedirectToAction("FullTable");
            //}
            return View();
        }
        [HttpPost]
        public ActionResult BookTable(FormCollection collection)
        {
            var firstName = collection["FirstName"];
            var lastName = collection["LastName"];
            var phone = collection["Phone"];
            var seat = collection["Seat"];
            var date = collection["Date"];
            var time = collection["Time"];
            if (String.IsNullOrEmpty(firstName))
            {
                ViewData["firstName"] = "This is required information";
            }
            else if (String.IsNullOrEmpty(lastName))
            {
                ViewData["lastName"] = "This is required information";
            }
            else if (String.IsNullOrEmpty(phone))
            {
                ViewData["phone"] = "This is required information";
            }
            else if (String.IsNullOrEmpty(seat))
            {
                ViewData["seat"] = "This is required information";
            }
            else if (String.IsNullOrEmpty(date))
            {
                ViewData["date"] = "This is required information";
            }
            else if (String.IsNullOrEmpty(time))
            {
                ViewData["time"] = "This is required information";
            }
            else
            {
                Table t = new Table();
                t.Name = firstName + " " + lastName;
                t.Seat = int.Parse(seat);
                t.Phone = phone;
                t.Date = DateTime.Parse(date);
                t.Time = TimeSpan.Parse(time);

                BookingRequest bkq = new BookingRequest();
                bkq.Name = t.Name;
                bkq.Phone = t.Phone;
                bkq.Time = t.Time;
                bkq.NumberOfSeat = t.Seat;
                bkq.Date = t.Date;
                bkq.Status = "Pending";
                db.BookingRequests.Add(bkq);
                db.SaveChanges();
                Session["Table"] = t;
                return RedirectToAction("DisplaySucces");
            }
            return this.BookTable();
        }
        public ActionResult DisplaySucces()
        {
            Table t = (Table)Session["Table"];
            ViewBag.Total = t.Seat * db.Tickets.SingleOrDefault(n => n.ID == 2).Price;
            ViewBag.Name = t.Name;
            ViewBag.Phone = t.Phone;
            ViewBag.Seat = t.Seat;
            ViewBag.Time = t.Time;
            ViewBag.Date = t.Date;
            return View();
        }
    }
}