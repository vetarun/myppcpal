using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mpp.WEB.Models
{
        public class LoginViewModel
        {
            [Required(ErrorMessage = "Email must be entered")]
            [Display(Name = "Email")]
            [EmailAddress(ErrorMessage = "Email address isn't valid")]
            [MaxLength(50)]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password must be entered")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            //[Display(Name = "Remember me?")]
            //public bool RememberMe { get; set; }
        }

        public class RegisterViewModel
        {
            [Required(ErrorMessage = "First Name must be entered")]
            [RegularExpression("[a-zA-Z ]*$", ErrorMessage = "Name accepts only characters")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last Name must be entered")]
            [RegularExpression("[a-zA-Z ]*$", ErrorMessage = "Name accepts only characters")]
            public string LasttName { get; set; }

            [Required(ErrorMessage = "Email must be entered")]
            [EmailAddress(ErrorMessage = "Email address isn't valid")]
            [MaxLength(50)]
            [System.Web.Mvc.Remote("CheckEmailId", "UserAccount", ErrorMessage = "Email address is already registered! Please contact administration")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password must be entered")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [RegularExpression("^(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).*$", ErrorMessage = "Password must contain at least one letter, one number and one special character(#?!@$%^&*-)")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }
            public string result { get; set; }
        }

        public class ResetPasswordViewModel
        {
             [Required]
            public String UserId { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [RegularExpression("^(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).*$", ErrorMessage = "Password must contain at least one letter, one number and one special character(#?!@$%^&*-)")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public class EmailViewModel
        {
            [Required]
            [EmailAddress]
            [MaxLength(50)]
            [Display(Name = "Email")]
            public string Email { get; set; }
        }

    public class UserProfile
    {
        public string profileId { get; set; }
        public string countryCode { get; set; }
        public string timeZone { get; set; }
        public string currencyCode { get; set; }
        public double dailyBudget { get; set; }
        public accountInfo accountinfo { get; set; }

    }
    public class accountInfo
    {
        public string marketplaceStringId { get; set; }
        public string sellerStringId { get; set; }
    }
}