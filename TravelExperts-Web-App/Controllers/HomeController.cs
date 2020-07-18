using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelExperts_Web_App.Models;

namespace TravelExperts_Web_App.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // get current customer by email
                Customer curr = GetCurrentCustomer();
                ViewBag.Message = "Welcome " + curr.CustFirstName + "!";
            }
            else
                ViewBag.Message = "Login or Register to access your vacation packages";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "About Us";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Us: ";

            return View();
        }

        #region helpers
        /// <summary>
        /// get current customer
        /// </summary>
        /// @author Harry
        private Customer GetCurrentCustomer()
        {
            var userId = User.Identity.GetUserId();

            // get current customer email
            string email = TravelExpertsData.GetEmailInAccount(userId);

            if (email == null)
                return null;

            // get current customer by email
            return TravelExpertsData.GetCustomer(email);
        }
        #endregion
    }
}