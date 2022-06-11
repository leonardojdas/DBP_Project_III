using deAndrade_Project_III.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace deAndrade_Project_III.Controllers
{
    /// <summary>
    /// state controller
    /// </summary>
    public class StateController : Controller
    {
        /// <summary>
        /// Display all state without any filtration
        /// </summary>
        /// <param name="id">string field to search</param>
        /// <param name="sortBy">int field to identify which column will order</param>
        /// <param name="isDesc">bool field that indicates the order is ascending or descending</param>
        /// <returns>List of customer object</returns>
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<State> states;

            switch (sortBy)
            {
                case 0:
                default:
                    {
                        states = isDesc ? context.States.OrderByDescending(s => s.StateCode).ToList() :
                            context.States.OrderBy(s => s.StateCode).ToList();
                        break;
                    }
                case 1:
                    {
                        states = isDesc ? context.States.OrderByDescending(s => s.StateName).ToList() :
                            context.States.OrderBy(s => s.StateName).ToList();
                        break;
                    }
                case 2:
                    {
                        states = isDesc ? context.States.OrderByDescending(s => s.Country).ToList() :
                            context.States.OrderBy(s => s.Country).ToList();
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(id))
            {
                id = id.Trim().ToLower();

                states = states.Where(s =>
                    s.StateCode.ToString().ToLower().Contains(id) ||
                    s.StateName.ToString().ToLower().Contains(id) ||
                    s.Country.ToString().ToLower().Contains(id)
                ).ToList();
            }

            states = states.Where(s => s.IsDeleted == false).ToList();

            return View(states);
        }

        /// <summary>
        /// Used to retrieve a state and view its details
        /// </summary>
        /// <param name="id">StateOD field</param>
        /// <returns>Customer object or redirect to action "All" to view all states</returns>
        [HttpGet]
        public ActionResult Upsert(string id)
        {
            BooksEntities context = new BooksEntities();
            State state = context.States.Where(s => s.StateCode == id).FirstOrDefault();

            UpsertStateModel viewModel = new UpsertStateModel()
            {
                State = state
            };


            if ((!viewModel.IsNull() && (bool) state.IsDeleted) || (viewModel.IsNull() && id != "0"))
            {
                return RedirectToAction("All");
            }

            return View(viewModel);
        }

        /// <summary>
        /// Used to update/insert a state
        /// </summary>
        /// <param name="state">state object</param>
        /// <returns>Redirect to action "All" to view all states</returns>
        [HttpPost]
        public ActionResult Upsert(State state)
        {
            State newState = state;
            BooksEntities context = new BooksEntities();

            try
            {
                if (context.States.Where(s => s.StateCode.Trim() == state.StateCode.Trim()).Count() > 0)
                {
                    var stateToSave = context.States.Where(s => s.StateCode == state.StateCode).ToList()[0];
                    stateToSave.StateName = newState.StateName;
                    stateToSave.Country = newState.Country;
                }
                else
                {
                    newState.IsActive = true;
                    newState.IsDeleted = false;

                    context.States.Add(newState);
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
        /// Used to delete a state
        /// </summary>
        /// <param name="id">stateID</param>
        /// <returns>Redirect to action "All" to view all states</returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {
            BooksEntities context = new BooksEntities();

            try
            {
                State state = context.States.Where(s => s.StateCode == id).FirstOrDefault();
                state.IsDeleted = true;

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