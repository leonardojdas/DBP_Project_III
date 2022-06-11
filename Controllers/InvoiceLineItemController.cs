using deAndrade_Project_III.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deAndrade_Project_III.Controllers
{
    /// <summary>
    /// Invoice line item controller
    /// </summary>
    public class InvoiceLineItemController : Controller
    {
        /// <summary>
        /// Display all invoice line items without any filtration
        /// </summary>
        /// <param name="invoiceID">Invoice ID</param>
        /// <param name="id">int field to identify which column will order</param>
        /// <returns>List of invoices line items object</returns>
        public ActionResult All(int invoiceID, string id)
        {
            BooksEntities context = new BooksEntities();
            List<InvoiceLineItem> invoiceLineItems;

            invoiceLineItems = context.InvoiceLineItems.Where(i => i.InvoiceID == invoiceID && i.IsDeleted == false).ToList();

            if (!string.IsNullOrEmpty(id))
            {
                id = id.Replace("-", ".");
                id = id.Trim().ToLower();

                invoiceLineItems = invoiceLineItems.Where(i =>
                    i.ProductCode.ToLower().Contains(id) ||
                    i.UnitPrice.ToString().Contains(id) ||
                    i.Quantity.ToString().Contains(id) ||
                    i.ItemTotal.ToString().Contains(id)
                ).ToList();
            }

            return View(invoiceLineItems);
        }

        /// <summary>
        /// Used to retrieve a invoicelineitem and view its details
        /// </summary>
        /// <param name="id">Invoice line item field</param>
        /// <returns>Invoice line item object or redirect to action "All" to view all invoices</returns>
        [HttpGet]
        public ActionResult Upsert(string id)
        {
            int invoiceID = Int32.Parse(id.Split('-')[0]);
            string productCode = id.Split('-')[1];

            BooksEntities context = new BooksEntities();
            InvoiceLineItem invoiceLineItem = context.InvoiceLineItems.Where(i => i.InvoiceID == invoiceID && i.ProductCode == productCode).FirstOrDefault();

            // create a list of invoiceLineItems to obtain the products of the invoice
            List<string> productCodes = context.InvoiceLineItems.Where(i => i.InvoiceID == invoiceID).Select(i => i.ProductCode.Trim()).ToList();

            // create a empty list of products
            List<Product> newProducts = new List<Product>();

            // create a list of products temp
            List<Product> products = context.Products.Where(p => p.IsDeleted == false).ToList();

            // create a list of products present in the invoice line items
            foreach (var product in products)
            {
                foreach (var code in productCodes)
                {
                    if (product.ProductCode.Trim() == code) {
                        newProducts.Add(product);
                    }
                }
            }

            // remove than from the main product list
            foreach (var product in newProducts)
            {
                if (products.Contains(product))
                {
                    products.Remove(product);
                }
            }

            // update the list of products with the products that are not present in the invoice

            UpsertInvoiceLineItemModel viewModel = new UpsertInvoiceLineItemModel()
            {
                InvoiceLineItem = invoiceLineItem,
                Products = products
            };

            return View(viewModel);
        }

        /// <summary>
        /// Used to update/insert a invoice line item
        /// </summary>
        /// <param name="invoiceLineItem">invoice line item object</param>
        /// <returns>Redirect to action "All" to view all invoices</returns>
        [HttpPost]
        public ActionResult Upsert(InvoiceLineItem invoiceLineItem)
        {
            InvoiceLineItem newInvoiceLineItem = invoiceLineItem;
            BooksEntities context = new BooksEntities();

            try
            {
                decimal itemTotalOld = 0;
                decimal itemTotalNew = 0;
                var totalItems = 0;
                decimal salesTaxRate = 0;
                decimal shippingPrice = 0;

                if (context.InvoiceLineItems.Where(i => i.InvoiceID == invoiceLineItem.InvoiceID && i.ProductCode == invoiceLineItem.ProductCode).Count() > 0)
                {
                    var invoiceLineItemToSave = context.InvoiceLineItems.Where(i => 
                            i.InvoiceID == invoiceLineItem.InvoiceID && 
                            i.ProductCode == invoiceLineItem.ProductCode
                        ).ToList()[0];

                    if (invoiceLineItemToSave.IsDeleted == false)
                    {
                        invoiceLineItemToSave.Quantity = invoiceLineItem.Quantity;
                        itemTotalOld = invoiceLineItemToSave.ItemTotal;
                        invoiceLineItemToSave.ItemTotal = invoiceLineItemToSave.UnitPrice * invoiceLineItemToSave.Quantity;
                        itemTotalNew = invoiceLineItemToSave.ItemTotal;
                    }
                }
                else
                {
                    var product = context.Products.Where(p => p.ProductCode == invoiceLineItem.ProductCode).ToList()[0];
                    
                    newInvoiceLineItem.InvoiceID = invoiceLineItem.InvoiceID;
                    newInvoiceLineItem.ProductCode = invoiceLineItem.ProductCode;
                    newInvoiceLineItem.UnitPrice = product.UnitPrice;
                    newInvoiceLineItem.Quantity = invoiceLineItem.Quantity;
                    newInvoiceLineItem.ItemTotal = product.UnitPrice * invoiceLineItem.Quantity;
                    itemTotalNew = newInvoiceLineItem.ItemTotal;

                    newInvoiceLineItem.IsActive = true;
                    newInvoiceLineItem.IsDeleted = false;

                    context.InvoiceLineItems.Add(newInvoiceLineItem);
                }

                context.SaveChanges();

                // update invoice product total, salex tax, shipping and invoice total
                totalItems = context.InvoiceLineItems.Where(i => i.InvoiceID == invoiceLineItem.InvoiceID && i.IsDeleted == false).Count();
                var orderOptions = context.OrderOptions.Where(o => o.IsDeleted == false).ToList()[0];
                salesTaxRate = orderOptions.SalesTaxRate;

                switch (totalItems)
                {
                    case 0:
                    case 1:
                    default:
                        shippingPrice = orderOptions.FirstBookShipCharge;
                        break;
                    case 2:
                        shippingPrice = orderOptions.FirstBookShipCharge + orderOptions.AdditionalBookShipCharge;
                        break;
                    case 3:
                        shippingPrice = orderOptions.FirstBookShipCharge + (orderOptions.AdditionalBookShipCharge * (totalItems - 1));
                        break;
                }

                var invoiceToSave = context.Invoices.Where(i2 => i2.InvoiceID == invoiceLineItem.InvoiceID).ToList()[0];
                invoiceToSave.ProductTotal = invoiceToSave.ProductTotal - itemTotalOld + itemTotalNew;
                invoiceToSave.SalesTax = invoiceToSave.ProductTotal * salesTaxRate;
                invoiceToSave.Shipping = shippingPrice;
                invoiceToSave.InvoiceTotal = invoiceToSave.ProductTotal + invoiceToSave.SalesTax + invoiceToSave.Shipping;

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("Upsert/" + invoiceLineItem.InvoiceID, "Invoice");
        }

        /// <summary>
        /// Used to delete a invoice line item
        /// </summary>
        /// <param name="id">Invoice line item ids</param>
        /// <returns>Redirect to action "All" to view all invoices</returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {
            int invoiceID = Int32.Parse(id.Split('-')[0]);
            string productCode = id.Split('-')[1];

            BooksEntities context = new BooksEntities();

            try
            {
                var invoiceLineItem = context.InvoiceLineItems.Where(i => i.InvoiceID == invoiceID && i.ProductCode == productCode).ToList()[0];
                invoiceLineItem.IsDeleted = true;
                decimal invoiceLineItemTotal = invoiceLineItem.ItemTotal;

                context.SaveChanges();

                // update invoice product total, salex tax, shipping and invoice total
                var totalItems = context.InvoiceLineItems.Where(i => i.InvoiceID == invoiceID && i.IsDeleted == false).Count();
                var orderOptions = context.OrderOptions.Where(o => o.IsDeleted == false).ToList()[0];
                decimal salesTaxRate = orderOptions.SalesTaxRate;
                decimal shippingPrice = 0;

                switch (totalItems)
                {
                    case 0:
                    default:
                        shippingPrice = 0;
                        break;
                    case 1:
                        shippingPrice = orderOptions.FirstBookShipCharge;
                        break;
                    case 2:
                        shippingPrice = orderOptions.FirstBookShipCharge + orderOptions.AdditionalBookShipCharge;
                        break;
                    case 3:
                        shippingPrice = orderOptions.FirstBookShipCharge + (orderOptions.AdditionalBookShipCharge * (totalItems - 1));
                        break;
                }

                var invoice = context.Invoices.Where(i => i.InvoiceID == invoiceID).ToList()[0];
                invoice.ProductTotal = invoice.ProductTotal - invoiceLineItemTotal;
                invoice.SalesTax = invoice.ProductTotal * salesTaxRate;
                invoice.Shipping = shippingPrice;
                invoice.InvoiceTotal = invoice.ProductTotal + invoice.SalesTax + invoice.Shipping;

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("Upsert/" + invoiceID, "Invoice");
        }
    }
}