using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Asp.Web.Common.Models.LogViewModels
{
    public class LogSearchViewModel
    {
        [Display(Name = "From Date Time")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date Time")]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Level")]
        public string SelectedLevel { get; set; }

        public string Message { get; set; }

        public string Logger { get; set; }

        public string Callsite { get; set; }

        public string Exception { get; set; }

        public IList<SelectListItem> AvailableLevels { get; set; }

        public LogSearchViewModel()
        {
            AvailableLevels = new List<SelectListItem>();
        }
    }
}