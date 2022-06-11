using deAndrade_Project_III.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deAndrade_Project_III.Controllers
{
    /// <summary>
    /// order option controller
    /// </summary>
    public class OrderOptionController : Controller
    {
        /// <summary>
        /// Display all order options without any filtration
        /// </summary>
        /// <param name="id">string field to search</param>
        /// <param name="sortBy">int field to identify which column will order</param>
        /// <param name="isDesc">bool field that indicates the order is ascending or descending</param>
        /// <returns>List of order options object</returns>
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<OrderOption> orderOption;

            switch (sortBy)
            {
                case 0:
                default:
                    {
                        orderOption = isDesc ? context.OrderOptions.OrderByDescending(o => o.SalesTaxRate).ToList() :
                            context.OrderOptions.OrderBy(o => o.SalesTaxRate).ToList();
                        break;
                    }
                case 1:
                    {
                        orderOption = isDesc ? context.OrderOptions.OrderByDescending(o => o.FirstBookShipCharge).ToList() :
                            context.OrderOptions.OrderBy(o => o.FirstBookShipCharge).ToList();
                        break;
                    }
                case 2:
                    {
                        orderOption = isDesc ? context.OrderOptions.OrderByDescending(o => o.AdditionalBookShipCharge).ToList() :
                            context.OrderOptions.OrderBy(o => o.AdditionalBookShipCharge).ToList();
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(id))
            {
                id = id.Trim().ToLower();

                orderOption = orderOption.Where(o =>
                    o.SalesTaxRate.ToString().Contains(id) ||
                    o.FirstBookShipCharge.ToString().Contains(id) ||
                    o.AdditionalBookShipCharge.ToString().Contains(id)
                ).ToList();
            }

            orderOption = orderOption.Where(o => o.IsDeleted == false).ToList();

            return View(orderOption);
        }
    }
}