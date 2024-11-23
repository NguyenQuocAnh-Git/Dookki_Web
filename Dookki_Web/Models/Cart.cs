using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dookki_Web.Models
{
    public class Cart
    {
        DOOKKIEntities db = new DOOKKIEntities();
        public int ticketID { get; set; }
        public string name { get; set; }
        public float price { get; set; }
        public int quantily { get; set; }
        public Cart(int ticketID) 
        {
            this.ticketID = ticketID;
            var ticket  = db.Tickets.Single(n=>n.ID == ticketID);
            name = ticket.Name;
            price = (int)ticket.Price;
            quantily = 1;
        }
    }
}