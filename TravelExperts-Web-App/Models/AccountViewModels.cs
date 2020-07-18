using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TravelExperts_Web_App.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [DisplayName("First Name")]
        [StringLength(25)]
        public string CustFirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        [StringLength(25)]
        public string CustLastName { get; set; }

        [Required]
        [DisplayName("Address")]
        [StringLength(75)]
        public string CustAddress { get; set; }

        [Required]
        [DisplayName("City")]
        [StringLength(50)]
        public string CustCity { get; set; }

        [Required]
        [DisplayName("Province")]
        [StringLength(2)]
        public string CustProv { get; set; }

        [Required]
        [DisplayName("Postal Code")]
        [StringLength(7)]
        [DataType(DataType.PostalCode)]
        public string CustPostal { get; set; }

        [DisplayName("Country (optional)")]
        [StringLength(25)]
        public string CustCountry { get; set; }

        [DisplayName("Home Number (optional)")]
        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        public string CustHomePhone { get; set; }

        [Required]
        [DisplayName("Business Number")]
        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        public string CustBusPhone { get; set; }

        [Required]
        [DisplayName("Email")]
        [StringLength(50)]
        public string CustEmail { get; set; }

        [DisplayName("Employee Number (optional)")]
        public Nullable<int> AgentId { get; set; }

        [Required]
        [Display(Name = "User Name")]
        [StringLength(256)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
