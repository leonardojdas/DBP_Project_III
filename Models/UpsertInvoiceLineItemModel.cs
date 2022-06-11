using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deAndrade_Project_III.Models
{
    /// <summary>
    /// upsert invoice line item model
    /// </summary>
    public class UpsertInvoiceLineItemModel
    {
        public InvoiceLineItem InvoiceLineItem { get; set; }
        public List<Product> Products { get; set; }

        /// <summary>
        /// Check if the object is null
        /// </summary>
        /// <returns>Bool = true for null, false for not null</returns>
        public bool IsNull()
        {
            bool r = false;
            if (InvoiceLineItem is null) r = true;
            return r;
        }
    }
}