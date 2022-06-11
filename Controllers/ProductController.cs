using deAndrade_Project_III.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deAndrade_Project_III.Controllers
{
    /// <summary>
    /// product controller
    /// </summary>
    public class ProductController : Controller
    {
        /// <summary>
        /// Display all products without any filtration
        /// </summary>
        /// <param name="id">string field to search</param>
        /// <param name="sortBy">int field to identify which column will order</param>
        /// <param name="isDesc">bool field that indicates the order is ascending or descending</param>
        /// <returns>List of products object</returns>
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<Product> products;

            switch (sortBy)
            {
                case 0:
                default:
                    {
                        products = isDesc ? context.Products.OrderByDescending(p => p.ProductCode).ToList() :
                            context.Products.OrderBy(p => p.ProductCode).ToList();
                        break;
                    }
                case 1:
                    {
                        products = isDesc ? context.Products.OrderByDescending(p => p.Description).ToList() :
                            context.Products.OrderBy(p => p.Description).ToList();
                        break;
                    }
                case 2:
                    {
                        products = isDesc ? context.Products.OrderByDescending(p => p.UnitPrice).ToList() :
                            context.Products.OrderBy(p => p.UnitPrice).ToList();
                        break;
                    }
                case 3:
                    {
                        products = isDesc ? context.Products.OrderByDescending(p => p.OnHandQuantity).ToList() :
                            context.Products.OrderBy(p => p.OnHandQuantity).ToList();
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(id)) {
                id = id.Trim().ToLower();

                products = products.Where(p =>
                    p.ProductCode.ToString().ToLower().Contains(id) ||
                    p.Description.ToString().ToLower().Contains(id) ||
                    p.UnitPrice.ToString().Contains(id) ||
                    p.OnHandQuantity.ToString().Contains(id)
                ).ToList();
            }

            products = products.Where(s => s.IsDeleted == false).ToList();

            return View(products);
        }

        /// <summary>
        /// Used to retrieve a product and view its details
        /// </summary>
        /// <param name="id">ProducID field</param>
        /// <returns>Customer object or redirect to action "All" to view all products</returns>
        [HttpGet]
        public ActionResult Upsert(string id) {
            BooksEntities context = new BooksEntities();
            Product product = context.Products.Where(p => p.ProductCode == id).FirstOrDefault();

            UpsertProductModel viewModel = new UpsertProductModel()
            {
                Product = product
            };

            return View(viewModel);
        }

        /// <summary>
        /// Used to update/insert a product
        /// </summary>
        /// <param name="product">Product object</param>
        /// <returns>Redirect to action "All" to view all products</returns>
        [HttpPost]
        public ActionResult Upsert(Product product) {
            Product newProduct = product;
            BooksEntities context = new BooksEntities();

            try
            {
                if (context.Products.Where(p => p.ProductCode == product.ProductCode).Count() > 0)
                {
                    var productToSave = context.Products.Where(p => p.ProductCode == product.ProductCode).ToList()[0];
                    productToSave.Description = product.Description;
                    productToSave.UnitPrice = product.UnitPrice;
                    productToSave.OnHandQuantity = product.OnHandQuantity;
                }
                else
                {
                    newProduct.IsActive = true;
                    newProduct.IsDeleted = false;

                    context.Products.Add(newProduct);
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
        /// Used to delete a product
        /// </summary>
        /// <param name="id">productID</param>
        /// <returns>Redirect to action "All" to view all products</returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {
            BooksEntities context = new BooksEntities();

            try
            {
                Product product = context.Products.Where(p => p.ProductCode == id).FirstOrDefault();
                product.IsDeleted = true;

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