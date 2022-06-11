using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deAndrade_Project_III.Models
{
    /// <summary>
    /// upsert product model
    /// </summary>
    public class UpsertProductModel
    {
        public Product Product { get; set; }

        /// <summary>
        /// Check if the object is null
        /// </summary>
        /// <returns>Bool = true for null, false for not null</returns>
        public bool IsNull()
        {
            bool r = false;
            if (Product is null) r = true;
            return r;
        }
    }
}