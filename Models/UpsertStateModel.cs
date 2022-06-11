using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deAndrade_Project_III.Models
{
    /// <summary>
    /// upsert state model
    /// </summary>
    public class UpsertStateModel
    {
        public State State { get; set; }

        /// <summary>
        /// Check if the object is null
        /// </summary>
        /// <returns>Bool = true for null, false for not null</returns>
        public bool IsNull()
        {
            bool r = false;
            if (State is null) r = true;
            return r;
        }
    }
}