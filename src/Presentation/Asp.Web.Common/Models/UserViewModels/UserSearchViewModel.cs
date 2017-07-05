using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Asp.Web.Common.Models.UserViewModels
{
    public class UserSearchViewModel
    {
        [Display(Name = "Rolename")]
        public int? SelectedRoleId { get; set; }

        public IList<SelectListItem> AvailableRoles { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }
        
        public IList<SelectListItem> AvailableStatus { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public UserSearchViewModel()
        {
            AvailableRoles = new List<SelectListItem>();
            AvailableStatus = new List<SelectListItem>();
        }
    }
}
