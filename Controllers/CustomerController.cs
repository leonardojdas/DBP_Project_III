using deAndrade_Project_III.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deAndrade_Project_III.Controllers
{
    /// <summary>
    /// Customer Controller
    /// </summary>
    public class CustomerController : Controller
    {
        /// <summary>
        /// Display all customers without any filtration
        /// </summary>
        /// <param name="id">string field to search</param>
        /// <param name="sortBy">int field to identify which column will order</param>
        /// <param name="isDesc">bool field that indicates the order is ascending or descending</param>
        /// <returns>List of customer object</returns>
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Customer> customers;

            switch (sortBy)
            {
                case 0:
                default:
                    {
                        customers = isDesc ? context.Customers.OrderByDescending(c => c.CustomerID).ToList() :
                            context.Customers.OrderBy(c => c.CustomerID).ToList();
                        break;
                    }
                case 1:
                    {
                        customers = isDesc ? context.Customers.OrderByDescending(c => c.Name).ToList() :
                            context.Customers.OrderBy(c => c.Name).ToList();
                        break;
                    }
                case 2:
                    {
                        customers = isDesc ? context.Customers.OrderByDescending(c => c.Email).ToList() :
                            context.Customers.OrderBy(c => c.Email).ToList();
                        break;
                    }
                case 3:
                    {
                        customers = isDesc ? context.Customers.OrderByDescending(c => c.Address).ToList() :
                            context.Customers.OrderBy(c => c.Address).ToList();
                        break;
                    }
                case 4:
                    {
                        customers = isDesc ? context.Customers.OrderByDescending(c => c.City).ToList() :
                            context.Customers.OrderBy(c => c.City).ToList();
                        break;
                    }
                case 5:
                    {
                        customers = isDesc ? context.Customers.OrderByDescending(c => c.State).ToList() :
                            context.Customers.OrderBy(c => c.State).ToList();
                        break;
                    }
                case 6:
                    {
                        customers = isDesc ? context.Customers.OrderByDescending(c => c.ZipCode).ToList() :
                            context.Customers.OrderBy(c => c.ZipCode).ToList();
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(id))
            {
                id = id.Trim().ToLower();

                customers = customers.Where(c =>
                    c.CustomerID.ToString().Contains(id) ||
                    c.Name.ToLower().Contains(id) ||
                    c.Address.ToLower().Contains(id) ||
                    c.City.ToLower().Contains(id) ||
                    c.State.ToLower().Contains(id) ||
                    c.ZipCode.ToLower().Contains(id) ||
                    c.Email.ToLower().Contains(id)
                ).ToList();
            }

            customers = customers.Where(c => c.IsDeleted == false).ToList();

            return View(customers);
        }

        /// <summary>
        /// Used to retrieve a customer and view its details
        /// </summary>
        /// <param name="id">CustomerID field</param>
        /// <returns>Customer object or redirect to action "All" to view all customers</returns>
        [HttpGet]
        public ActionResult Upsert(int id)
        {
            BooksEntities context = new BooksEntities();
            Customer customer = context.Customers.Where(c => c.CustomerID == id).FirstOrDefault();

            UpsertCustomerModel viewModel = new UpsertCustomerModel()
            {
                Customer = customer
            };

            if ((!viewModel.IsNull() && (bool)customer.IsDeleted) || (viewModel.IsNull() && id != 0))
            {
                return RedirectToAction("All");
            }

            return View(viewModel);
        }

        /// <summary>
        /// Used to update/insert a customer
        /// </summary>
        /// <param name="customer">Customer object</param>
        /// <returns>Redirect to action "All" to view all customers</returns>
        [HttpPost]
        public ActionResult Upsert(Customer customer)
        {
            Customer newCustomer = customer;
            BooksEntities context = new BooksEntities();

            try
            {
                if (context.Customers.Where(c => c.CustomerID == customer.CustomerID).Count() > 0)
                {
                    var customerToSave = context.Customers.Where(c => c.CustomerID == customer.CustomerID).ToList()[0];
                    customerToSave.Name = newCustomer.Name;
                    customerToSave.Email = newCustomer.Email;
                    customerToSave.Address = newCustomer.Address;
                    customerToSave.City = newCustomer.City;
                    customerToSave.State = newCustomer.State;
                    customerToSave.ZipCode = newCustomer.ZipCode;
                }
                else
                {
                    newCustomer.IsActive = true;
                    newCustomer.IsDeleted = false;

                    context.Customers.Add(newCustomer);
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
        /// Used to delete a customer
        /// </summary>
        /// <param name="id">CustomerID</param>
        /// <returns>Redirect to action "All" to view all customers</returns>
        [HttpGet]
        public ActionResult Delete(int id)
        {
            BooksEntities context = new BooksEntities();

            try
            {
                Customer customer = context.Customers.Where(c => c.CustomerID == id).FirstOrDefault();
                customer.IsDeleted = true;

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