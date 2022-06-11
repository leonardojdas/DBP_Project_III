using deAndrade_Project_III.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deAndrade_Project_III.Controllers
{
    /// <summary>
    /// Invoice Controller
    /// </summary>
    public class InvoiceController : Controller
    {
        /// <summary>
        /// Display all invoices without any filtration
        /// </summary>
        /// <param name="id">string field to search</param>
        /// <param name="sortBy">int field to identify which column will order</param>
        /// <param name="isDesc">bool field that indicates the order is ascending or descending</param>
        /// <returns>List of invoices object</returns>
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Invoice> invoices;

            switch (sortBy)
            {
                case 0:
                default:
                    {
                        invoices = isDesc ? context.Invoices.OrderByDescending(i => i.InvoiceID).ToList() :
                            context.Invoices.OrderBy(i => i.InvoiceID).ToList();
                        break;
                    }
                case 1:
                    {
                        invoices = isDesc ? context.Invoices.OrderByDescending(i => i.Customer.Name).ToList() :
                            context.Invoices.OrderBy(i => i.Customer.Name).ToList();
                        break;
                    }
                case 2:
                    {
                        invoices = isDesc ? context.Invoices.OrderByDescending(i => i.InvoiceDate).ToList() :
                            context.Invoices.OrderBy(i => i.InvoiceDate).ToList();
                        break;
                    }
                case 3:
                    {
                        invoices = isDesc ? context.Invoices.OrderByDescending(i => i.ProductTotal).ToList() :
                            context.Invoices.OrderBy(i => i.ProductTotal).ToList();
                        break;
                    }
                case 4:
                    {
                        invoices = isDesc ? context.Invoices.OrderByDescending(i => i.SalesTax).ToList() :
                            context.Invoices.OrderBy(i => i.SalesTax).ToList();
                        break;
                    }
                case 5:
                    {
                        invoices = isDesc ? context.Invoices.OrderByDescending(i => i.Shipping).ToList() :
                            context.Invoices.OrderBy(i => i.Shipping).ToList();
                        break;
                    }
                case 6:
                    {
                        invoices = isDesc ? context.Invoices.OrderByDescending(i => i.InvoiceTotal).ToList() :
                            context.Invoices.OrderBy(i => i.InvoiceTotal).ToList();
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(id))
            {
                id = id.Trim().ToLower();

                invoices = invoices.Where(i =>
                    i.InvoiceID.ToString().ToLower().Contains(id) ||
                    i.Customer.Name.ToString().ToLower().Contains(id) ||
                    i.InvoiceDate.ToString().ToLower().Contains(id) ||
                    i.ProductTotal.ToString().ToLower().Contains(id) ||
                    i.SalesTax.ToString().ToLower().Contains(id) ||
                    i.Shipping.ToString().ToLower().Contains(id) ||
                    i.InvoiceTotal.ToString().ToLower().Contains(id)
                ).ToList();
            }

            invoices = invoices.Where(i => i.IsDeleted == false).ToList();

            return View(invoices);
        }

        /// <summary>
        /// Used to retrieve a invoice and view its details
        /// </summary>
        /// <param name="id">InvoiceID field</param>
        /// <returns>Invoice object or redirect to action "All" to view all invoices</returns>
        [HttpGet]
        public ActionResult Upsert(int id)
        {
            BooksEntities context = new BooksEntities();
            Invoice invoice = context.Invoices.Where(i => i.InvoiceID == id).FirstOrDefault();
            List<Customer> customer = context.Customers.Where(c => c.IsDeleted == false).ToList();
            OrderOption orderOption = context.OrderOptions.Where(o => o.IsDeleted == false).ToList()[0];

            UpsertInvoiceModel viewModel = new UpsertInvoiceModel()
            {
                Invoice = invoice,
                Customer = customer,
                OrderOption = orderOption
            };

            if ((!viewModel.IsNull() && (bool)invoice.IsDeleted) || (viewModel.IsNull() && id != 0))
            {
                return RedirectToAction("All");
            }

            return View(viewModel);
        }

        /// <summary>
        /// Used to update/insert a customer
        /// </summary>
        /// <param name="invoice">Invoice object</param>
        /// <returns>Redirect to action "All" to view all invoices</returns>
        [HttpPost]
        public ActionResult Upsert(Invoice invoice)
        {
            Invoice newInvoice = invoice;
            BooksEntities context = new BooksEntities();

            try
            {
                if (context.Invoices.Where(i => i.InvoiceID == invoice.InvoiceID).Count() > 0)
                {
                    var invoiceToSave = context.Invoices.Where(i => i.InvoiceID == invoice.InvoiceID).ToList()[0];
                    //invoiceToSave.CustomerID = newInvoice.CustomerID;
                    invoiceToSave.InvoiceDate = newInvoice.InvoiceDate;
                    //invoiceToSave.ProductTotal = newInvoice.ProductTotal;
                    //invoiceToSave.SalesTax = newInvoice.SalesTax;
                    //invoiceToSave.Shipping = newInvoice.Shipping;
                    //invoiceToSave.InvoiceTotal = invoiceToSave.ProductTotal + invoiceToSave.SalesTax + invoiceToSave.Shipping;
                }
                else
                {
                    newInvoice.CustomerID = invoice.CustomerID;
                    newInvoice.InvoiceDate = invoice.InvoiceDate;
                    newInvoice.ProductTotal = 0;
                    newInvoice.SalesTax = 0;
                    newInvoice.Shipping = 0;
                    newInvoice.InvoiceTotal = 0;

                    newInvoice.IsActive = true;
                    newInvoice.IsDeleted = false;

                    context.Invoices.Add(newInvoice);
                }

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("All");
        }

        /// <summary>
        /// Used to delete a invoice
        /// </summary>
        /// <param name="id">InvoiceID</param>
        /// <returns>Redirect to action "All" to view all invoices</returns>
        [HttpGet]
        public ActionResult Delete(int id)
        {
            BooksEntities context = new BooksEntities();
            try
            {
                Invoice invoice = context.Invoices.Where(i => i.InvoiceID == id).FirstOrDefault();
                invoice.IsDeleted = true;

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("All");
        }
    }
}