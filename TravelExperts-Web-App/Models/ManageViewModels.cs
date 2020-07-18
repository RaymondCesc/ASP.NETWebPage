using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace TravelExperts_Web_App.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Prov { get; set; }
        public string Postal { get; set; }
        public string Country { get; set; }
        [Display(Name = "Home Number")]
        public string HomePhone { get; set; }
        [Display(Name = "Business Number")]
        public string BusPhone { get; set; }
        public string Email { get; set; }
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        public List<Booking> Bookings { get; set; }
    }
    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }

    public class FactorViewModel
    {
        public string Purpose { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }

    /// <summary>
    /// Model for editing user name
    /// </summary>
    /// @author - Harry
    public class ChangeUserNameViewModel
    {
        [Required(ErrorMessage = "New user name required.")]
        [StringLength(256, ErrorMessage = "Invalid new user name.")]
        [Display(Name = "New User Name")]
        public string NewUserName { get; set; }
    }

    /// <summary>
    /// Model for editing home phone number
    /// </summary>
    /// @author - Harry
    public class ChangeHomePhoneViewModel
    {
        [StringLength(20, ErrorMessage = "Phone number too long.")]
        [Display(Name = "New Home Phone Number")]
        public string NewHomePhoneNumber { get; set; }

        /// <summary>
        /// Update cusomter with new home phone number
        /// </summary>
        /// <returns>true on success, false otherwise</returns>
        /// <param name="customer">updated customer</param>
        public bool Update(Customer customer)
        {
            using (TravelExpertsEntities db = new TravelExpertsEntities())
            {
                // get customer from Customer table by phone number
                var cust = db.Customers.SingleOrDefault(c => c.CustBusPhone == customer.CustBusPhone);
                if (cust != null) // found customer
                {
                    cust.CustHomePhone = customer.CustHomePhone;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }
    }

    /// <summary>
    /// Model for editing business phone number
    /// </summary>
    /// @author - Harry
    public class ChangeBusPhoneViewModel
    {


        [Required(ErrorMessage = "New phone number required.")]
        [StringLength(20, ErrorMessage = "New phone number too long.")]
        [Display(Name = "New Business Phone Number")]
        public string NewBusPhoneNumber { get; set; }

        /// <summary>
        /// Update cusomter with new business phone number
        /// </summary>
        /// <param name="customer">updated customer</param>
        /// <returns>True on success, false otherwise</returns>
        public bool Update(Customer customer)
        {
            bool flag = false;
            // update in customers table
            using (TravelExpertsEntities db = new TravelExpertsEntities())
            {
                // get customer from Customer table by email
                var cust = db.Customers.SingleOrDefault(c => c.CustEmail == customer.CustEmail);
                if (cust != null) // found customer
                {
                    cust.CustBusPhone = customer.CustBusPhone;
                    db.SaveChanges();
                    flag = true;
                }
            }

            // update in accounts table
            using (AccountEntities db = new AccountEntities()) 
            {
                // get account 
                var account = db.AspNetUsers.SingleOrDefault(accnt => accnt.Email == customer.CustEmail);
                if (account != null) // found account
                {
                    if (flag) // make sure customers table update succesfully
                    {
                        account.PhoneNumber = customer.CustBusPhone;
                        db.SaveChanges();
                        return true;
                    }
                }
                return false; // one or both failed
            }
        }
    }

    /// <summary>
    /// Model for editing address
    /// </summary>
    /// @author - Harry
    public class ChangeAddressViewModel
    {
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(75, ErrorMessage = "Address too long.")]
        [Display(Name = "Address")]
        public string NewAddress { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(75, ErrorMessage = "City too long.")]
        [Display(Name = "City")]
        public string NewCity { get; set; }

        [Required(ErrorMessage = "Province is required.")]
        [StringLength(2, ErrorMessage = "Province too long. Use province abbreviation.")]
        [Display(Name = "Province")]
        public string NewProv { get; set; }

        [Required(ErrorMessage = "Postal code required.")]
        [StringLength(7, ErrorMessage = "Postal code too long.")]
        [Display(Name = "Postal Code")]
        public string NewPostal { get; set; }

        [StringLength(25, ErrorMessage = "Country too long.")]
        [Display(Name = "Country")]
        public string NewCountry { get; set; }

        /// <summary>
        /// Update customer with new address
        /// </summary>
        /// <param name="customer">updated customer</param>
        /// <returns>True on success, false otherwise</returns>
        public bool Update(Customer customer)
        {
            using (TravelExpertsEntities db = new TravelExpertsEntities())
            {
                // get customer from Customer table by phone number
                var cust = db.Customers.SingleOrDefault(c => c.CustBusPhone == customer.CustBusPhone);
                if (cust != null) // found customer
                {
                    cust.CustAddress = customer.CustAddress;
                    cust.CustCity = customer.CustCity;
                    cust.CustProv = customer.CustProv;
                    cust.CustPostal = customer.CustPostal;
                    cust.CustCountry = customer.CustCountry;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
        }
    }

    /// <summary>
    /// Model for editing email
    /// </summary>
    /// @author - Harry
    public class ChangeEmailViewModel
    {
        [Required(ErrorMessage = "New email is required.")]
        [StringLength(50, ErrorMessage = "New email too long.")]
        [Display(Name = "New Email")]
        public string NewEmail { get; set; }

        /// <summary>
        /// Update customer with new email
        /// </summary>
        /// <param name="customer">updated customer</param>
        /// <returns>True on success, false otherwise</returns>
        public bool Update(Customer customer) 
        {
            bool flag = false;
            // update in customers table
            using (TravelExpertsEntities db = new TravelExpertsEntities())
            {
                // get customer from Customer table by phone number
                var cust = db.Customers.SingleOrDefault(c => c.CustBusPhone == customer.CustBusPhone);
                if (cust != null) // found customer
                {
                    cust.CustEmail = customer.CustEmail;
                    db.SaveChanges();
                    flag = true;
                }
            }

            // update in accounts table
            using (AccountEntities db = new AccountEntities())
            {
                // get account 
                var account = db.AspNetUsers.SingleOrDefault(accnt => accnt.PhoneNumber == customer.CustBusPhone);
                if (account != null) // found account
                {
                    if (flag) // make sure customers table update succesfully
                    {
                        account.Email = customer.CustEmail;
                        db.SaveChanges();
                        return true;
                    }
                }
                return false; // one or both failed
            }
        }
    }

    /// <summary>
    /// Model for showing booking detials including fees
    /// </summary>
    /// @author - Harry
    public class BookingDetailsViewModel
    {
        public List<BookingDetail> Details { get; set; }
        public List<Fee> Fees { get; set; }
        [DisplayFormat(DataFormatString = "{0:c2}")]
        public decimal? TotalOwing { get; set; }
    }
}
