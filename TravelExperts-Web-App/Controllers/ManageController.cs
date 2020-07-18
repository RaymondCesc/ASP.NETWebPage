using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TravelExperts_Web_App.Models;

namespace TravelExperts_Web_App.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public ActionResult Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two-factor authentication provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred. Unable to perform change."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.EditPhoneSuccess ? "Phone number changed."
                : message == ManageMessageId.EditAddressSuccess ? "Address changed."
                : message == ManageMessageId.EditUserNameSuccess ? "User name changed."
                : message == ManageMessageId.EditEmailSuccess ? "Email changed."
                : message == ManageMessageId.EditAddressSuccess ? "Address changed."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            // get current customer by email
            Customer curr = GetCurrentCustomer();

            if (curr == null) // couldn't find account or user
            {
                ViewBag.ErrorMsg = "We're sorry, an error has occured while trying to get your information.";
                return View();
            }

            // get customer bookings
            List<Booking> bookings = TravelExpertsData.GetBookings(curr);

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                UserName = curr.UserName,
                FirstName = curr.CustFirstName,
                LastName = curr.CustLastName,
                Address = curr.CustAddress,
                City = curr.CustCity,
                Prov = curr.CustProv,
                Postal = curr.CustPostal,
                Country = curr.CustCountry,
                HomePhone = curr.CustHomePhone,
                BusPhone = curr.CustBusPhone,
                Email = curr.CustEmail,
                Bookings = bookings
            };
            return View(model);
        }

        // GET: /Manage/BookingDetails
        /// <summary>
        /// Serve booking details page
        /// </summary>
        /// @author - Harry
        public ActionResult BookingDetails(string bookingNo)
        {
            // make sure we have a booking number
            if (bookingNo == null)
                return RedirectToAction("Index");

            // put booking number in bag
            ViewBag.BookingNo = bookingNo;
            // get customer
            Customer curr = GetCurrentCustomer();

            List<BookingDetail> details = TravelExpertsData.GetBookingDetails(curr, bookingNo);
            List<Fee> fees = new List<Fee>();

            // get each booking details fee
            foreach (BookingDetail detail in details)
            {
                fees.Add(TravelExpertsData.GetFee(curr, detail.BookingDetailId));
            }

            // set up model
            var model = new BookingDetailsViewModel
            {
                Details = details,
                Fees = fees
            };
            return View(model);
        }

        //
        // GET: /Manage/ChangeUserName
        /// <summary>
        /// Serve change user name page
        /// </summary>
        /// @author - Harry
        public ActionResult ChangeUserName()
        {
            return View();
        }

        /// <summary>
        /// Serve change user name post
        /// </summary>
        /// <param name="model">form data</param>
        /// @author - Harry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeUserName(ChangeUserNameViewModel model)
        {
            if (TravelExpertsData.IsUniqueUserName(model.NewUserName))
            {
                Customer curr = GetCurrentCustomer();

                // make sure we found customer
                if (curr == null)
                {
                    ModelState.AddModelError(String.Empty, "Sorry an error occured while trying to find you. Please try log in again.");
                    return View();
                }
                curr.UserName = model.NewUserName;
                if (TravelExpertsData.UpdateCustomerUserName(curr) && TravelExpertsData.UpdateAccountUserName(curr))
                    return RedirectToAction("Index", new { Message = ManageMessageId.EditUserNameSuccess });

                // something went wrong
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            ModelState.AddModelError(String.Empty, "Account is already linked to this user name.");
            return View();
        }

        //
        // GET: /Manage/ChangeHomePhoneNumber
        /// <summary>
        /// Serve change home phone number page
        /// </summary>
        /// @author - Harry
        public ActionResult ChangeHomePhone()
        {
            return View();
        }

        /// <summary>
        /// Serve change Home phone number post
        /// </summary>
        /// <param name="model">form data</param>
        /// @author - Harry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeHomePhone(ChangeHomePhoneViewModel model)
        {
            if (string.IsNullOrEmpty(model.NewHomePhoneNumber) ||
                    Validator.IsCanadianPhoneNumber(model.NewHomePhoneNumber, out _))
            {
                Customer curr = GetCurrentCustomer();

                // make sure we found customer
                if (curr == null) 
                {
                    ModelState.AddModelError(String.Empty, "Sorry an error occured while trying to find you. Please try log in again.");
                    return View();
                }
                curr.CustHomePhone = model.NewHomePhoneNumber;
                if (model.Update(curr))
                    return RedirectToAction("Index", new { Message = ManageMessageId.EditPhoneSuccess });
                // something went wrong
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            ModelState.AddModelError(String.Empty, "Invalid phone number.");
            return View();
        }

        // GET: /Manage/ChangeBusPhone
        /// <summary>
        /// Serve change business phone number page
        /// </summary>
        /// @author - Harry
        public ActionResult ChangeBusPhone()
        {
            return View();
        }

        /// <summary>
        /// Serve change business phone number post
        /// </summary>
        /// <param name="model">form data</param>
        /// @author - Harry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeBusPhone(ChangeBusPhoneViewModel model)
        {
            string notUnique = "";
            if (Validator.IsCanadianPhoneNumber(model.NewBusPhoneNumber, out string invalid) &&
                    TravelExpertsData.IsUniquePhone(model.NewBusPhoneNumber, out notUnique))
            {
                Customer curr = GetCurrentCustomer();

                // make sure we found customer
                if (curr == null)
                {
                    ModelState.AddModelError(String.Empty, "Sorry an error occured while trying to find you. Please try log in again.");
                    return View();
                }
                curr.CustBusPhone = model.NewBusPhoneNumber;
                if (model.Update(curr))
                    return RedirectToAction("Index", new { Message = ManageMessageId.EditPhoneSuccess });
                // something went wrong
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }

            // made it here, something went wrong
            if (!string.IsNullOrEmpty(invalid))
                ModelState.AddModelError(String.Empty, "Invalid phone number.");
            if (!string.IsNullOrEmpty(notUnique))
                ModelState.AddModelError(String.Empty, "An account is already linked to this number.");

            return View();
        }

        // GET: /Manage/ChangeAddress
        /// <summary>
        /// Serve change address page
        /// </summary>
        /// @author - Harry
        public ActionResult ChangeAddress()
        {
            return View();
        }

        /// <summary>
        /// Serve change address post
        /// </summary>
        /// <param name="model">form data</param>
        /// @author - Harry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeAddress(ChangeAddressViewModel model)
        {
            if (Validator.IsCanadianPostal(model.NewPostal, out string invalid)) 
            { 
                Customer curr = GetCurrentCustomer();

                // make sure we found customer
                if (curr == null)
                {
                    ModelState.AddModelError(String.Empty, "Sorry an error occured while trying to find you. Please try log in again.");
                    return View();
                }
                curr.CustAddress = model.NewAddress;
                curr.CustCity = model.NewCity;
                curr.CustProv = model.NewProv;
                curr.CustCity = model.NewCity;
                curr.CustPostal = model.NewPostal;
                curr.CustCountry = model.NewCountry;
                if (model.Update(curr))
                    return RedirectToAction("Index", new { Message = ManageMessageId.EditAddressSuccess });
                // something went wrong
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }

            // made it here, something went wrong with data
            if (!string.IsNullOrEmpty(invalid))
                ModelState.AddModelError(String.Empty, "Invalid postal code.");

            return View();
        }

        // GET: /Manage/ChangeEmail
        /// <summary>
        /// Serve change email page
        /// </summary>
        /// @author - Harry
        public ActionResult ChangeEmail()
        {
            return View();
        }

        /// <summary>
        /// Serve change email post
        /// </summary>
        /// <param name="model">form data</param>
        /// @author - Harry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEmail(ChangeEmailViewModel model)
        {
            if (TravelExpertsData.IsUniqueEmail(model.NewEmail, out string error)) 
            {
                Customer curr = GetCurrentCustomer();

                // make sure we found customer
                if (curr == null)
                {
                    ModelState.AddModelError(String.Empty, "Sorry an error occured while trying to find you. Please try log in again.");
                    return View();
                }

                curr.CustEmail = model.NewEmail;
                if(model.Update(curr))
                    return RedirectToAction("Index", new { Message = ManageMessageId.EditEmailSuccess });

                // something went wrong
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            // an account is already linked
            if (!string.IsNullOrEmpty(error))
                ModelState.AddModelError(string.Empty, "An account is already linked to this email.");

            return View();
        }


        // auto-generated
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //auto-generated
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers
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

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error,
            EditPhoneSuccess,
            EditAddressSuccess,
            EditUserNameSuccess,
            EditEmailSuccess
        }

#endregion
    }
}