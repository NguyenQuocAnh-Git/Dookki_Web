//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dookki_Web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customer
    {
        public Customer()
        {
            this.Orders = new HashSet<Order>();
        }
    
        public int customerID { get; set; }
        public string customerUserName { get; set; }
        public string customerPassword { get; set; }
        public string customerPhone { get; set; }
        public string customerEmail { get; set; }
        public string customerAddress { get; set; }
        public string customerName { get; set; }
    
        public virtual ICollection<Order> Orders { get; set; }
    }
}
