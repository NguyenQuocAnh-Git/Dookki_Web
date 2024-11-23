using Dookki_Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult Checkout()
        {
            //if (Session["Cart"] == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            //// Lay gio hang tu Session
            //List<Cart> cart = GetCart();
            //ViewBag.NumProduct = NumProduct();
            //ViewBag.Total = Total();
            //return View(cart);
            return View();
        }
        public ActionResult Checkout(FormCollection collection)
        {
            // Them Don hang
            Order newOrder = new Order();

            Customer client = (Customer)Session["Account"];
            List<Cart> cart = GetCart();

            newOrder.customerID = client.ID;
            newOrder.Time = DateTime.Now.TimeOfDay;
            newOrder.discount = 0;

            db.Orders.Add(newOrder);
            db.SaveChanges();

            // Them chi tiet don hang
            foreach (var item in cart)
            {
                OrderDetail detail = new OrderDetail();

                detail.quantily= item.quantily;
                detail.ticketID = item.ticketID;
                detail.paymentID = int.Parse(collection["payment"]);
                detail.orderID = newOrder.ID;

                db.OrderDetails.Add(detail);
                db.SaveChanges();
            }

            Session["Giohang"] = null;
            return RedirectToAction("ConfirmOrder", "Order");
        }
    }
}