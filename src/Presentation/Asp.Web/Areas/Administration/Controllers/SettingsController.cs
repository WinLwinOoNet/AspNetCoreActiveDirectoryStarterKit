using System.Linq;
using System.Threading.Tasks;
using Asp.Core;
using Asp.Core.Domains;
using Asp.Repositories.Settings;
using Asp.Web.Common.Models.SettingViewModels;
using Asp.Web.Common.Mvc.Alerts;
using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp.Web.Areas.Administration.Controllers
{
    [Area(Constants.Areas.Administration)]
    [Authorize(Policy = Constants.RoleNames.Administrator)]
    public class SettingsController : Controller
    {
        private readonly ISettingRepository _settingRepository;
        private readonly IMapper _mapper;

        public SettingsController(
            ISettingRepository settingRepository, 
            IMapper mapper)
        {
            _settingRepository = settingRepository;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpPost]
        public JsonResult List([DataSourceRequest] DataSourceRequest request)
        {
            var result = _settingRepository.GetAllSettings()
                .Select(e => _mapper.Map<Setting, SettingViewModel>(e))
                .ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult Edit(int id)
        {
            var setting = _settingRepository.GetSettingById(id);

            if (setting == null)
                return RedirectToAction("List");

            var model = _mapper.Map<Setting, SettingViewModel>(setting);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SettingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _settingRepository.GetSettingById(model.Id);

                if (entity == null)
                    return RedirectToAction("List");

                entity = _mapper.Map(model, entity);
                await _settingRepository.UpdateSettingAsync(entity);
                return RedirectToAction("List").WithSuccess($"{entity.Name} setting was updated successfully.");
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult ClearCache()
        {
            _settingRepository.ClearCache();
            return Json(null);
        }
    }
}