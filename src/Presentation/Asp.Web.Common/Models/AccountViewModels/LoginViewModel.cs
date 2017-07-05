using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Asp.Web.Common.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Domain")]
        [Required(ErrorMessage = "Please enter your domain.")]
        public string Domain { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Please enter your username.")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter your password.")]
        public string Password { get; set; }
        
        public IList<SelectListItem> AvailableDomains { get; set; }

        public LoginViewModel()
        {
            AvailableDomains = new List<SelectListItem>();
        }
    }
}
