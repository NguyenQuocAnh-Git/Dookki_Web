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
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DisplayCart()
        {
            //List<Cart> cart = GetCart();
            //if (cart.Count == 0)
            //{
            //    return RedirectToAction("Index");
            //}
            //return View(cart);
            return View();
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
            return RedirectToAction("Order", "Home");
        }
        
    }
}