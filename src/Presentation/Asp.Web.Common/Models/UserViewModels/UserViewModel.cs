using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Asp.Web.Common.Models.UserViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; }

        [Display(Name = "Last Login Date")]
        public DateTime LastLoginDate { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        [Display(Name = "Last Updated Date")]
        public DateTime ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        [Display(Name = "Authorized Roles")]
        public string AuthorizedRoleNames { get; set; }

        public IList<int> AuthorizedRoleIds { get; set; }

        public UserViewModel()
        {
            AuthorizedRoleIds = new List<int>();
        }
    }
}
