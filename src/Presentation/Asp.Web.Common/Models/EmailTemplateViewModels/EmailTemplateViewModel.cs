using System;
using System.ComponentModel.DataAnnotations;

namespace Asp.Web.Common.Models.EmailTemplateViewModels
{
    public class EmailTemplateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Template Name")]
        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(50, ErrorMessage = "{0} must not exceed {1} characters.")]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(200, ErrorMessage = "{0} must not exceed {1} characters.")]
        public string Description { get; set; }

        [Display(Name = "Subject")]
        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(200, ErrorMessage = "{0} must not exceed {1} characters.")]
        public string Subject { get; set; }

        //[AllowHtml]
        [Display(Name = "body")]
        [Required(ErrorMessage = "Please enter body.")]
        public string Body { get; set; }

        //[AllowHtml]
        [Display(Name = "Instruction")]
        public string Instruction { get; set; }
        
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
