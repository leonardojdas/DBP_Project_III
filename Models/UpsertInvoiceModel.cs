using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deAndrade_Project_III.Models
{
    /// <summary>
    /// upsert invoice model
    /// </summary>
    public class UpsertInvoiceModel : Controller
    {
        public Invoice Invoice { get; set; }
        public List<Customer> Customer { get; set; }

        public OrderOption OrderOption { get; set; }

        /// <summary>
        /// Check if the object is null
        /// </summary>
        /// <returns>Bool = true for null, false for not null</returns>
        public bool IsNull()
        {
            bool r = false;
            if (Invoice is null) r = true;
            return r;
        }
    }
}