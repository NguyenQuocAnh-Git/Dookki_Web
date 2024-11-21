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
        public decimal quantily { get; set; }
        public Cart(int ticketID) 
        {
            this.ticketID = ticketID;
            quantily = 1;
        }
    }
}