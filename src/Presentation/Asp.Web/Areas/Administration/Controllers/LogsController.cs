using System;
using System.Linq;
using System.Threading.Tasks;
using Asp.Core;
using Asp.Core.Data;
using Asp.Core.Extensions;
using Asp.Repositories.Logging;
using Asp.Web.Common.Models.LogViewModels;
using AutoMapper;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Asp.Web.Areas.Administration.Controllers
{
    [Area(Constants.Areas.Administration)]
    [Authorize(Policy = Constants.RoleNames.Administrator)]
    public class LogsController : Controller
    {
        private readonly ILogRepository _logRepository;
        private readonly IMapper _mapper;

        public LogsController(
            ILogRepository logRepository,
            IMapper mapper)
        {
            _logRepository = logRepository;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public async Task<ActionResult> List()
        {
            var availableLevels = (await _logRepository.GetLevels())
                .Select(l => new SelectListItem {Text = l, Value = l})
                .ToList();
            availableLevels.Insert(0, new SelectListItem {Text = "All", Value = ""});
            var model = new LogSearchViewModel {AvailableLevels = availableLevels};
            return View("List", model);
        }

        [HttpPost]
        public async Task<ActionResult> List([DataSourceRequest] DataSourceRequest request, LogSearchViewModel model)
        {
            var dataRequest = ParsePagedDataRequest(request, model);
            var entities = await _logRepository.GetLogs(dataRequest);
            var models = entities.Select(l => _mapper.Map<Core.Domains.Log, LogViewModel>(l)).ToList();
            var result = new DataSourceResult {Data = models.ToList(), Total = entities.TotalCount};
            return Json(result);
        }

        private LogPagedDataRequest ParsePagedDataRequest(DataSourceRequest request, LogSearchViewModel model)
        {
            var dataRequest = new LogPagedDataRequest
            {
                Level = model.SelectedLevel,
                Message = model.Message,
                Logger = model.Logger,
                Callsite = model.Callsite,
                Exception = model.Exception,
                PageIndex = request.Page - 1,
                PageSize = request.PageSize
            };

            if (model.FromDate.HasValue)
                dataRequest.FromDate = model.FromDate.Value;

            if (model.ToDate.HasValue)
                dataRequest.ToDate = model.ToDate.Value.ToEndOfDay();

            SortDescriptor sort = request.Sorts.FirstOrDefault();
            if (sort != null)
            {
                LogSortField sortField;
                if (!Enum.TryParse(sort.Member, out sortField))
                    sortField = LogSortField.Logged;
                dataRequest.SortField = sortField;
                dataRequest.SortOrder = sort.SortDirection == ListSortDirection.Ascending ? SortOrder.Ascending : SortOrder.Descending;
            }
            return dataRequest;
        }
    }
}