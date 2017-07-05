using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Asp.Core;
using Asp.Core.Domains;
using Asp.Repositories.Messages;
using Asp.Web.Common;
using Asp.Web.Common.Models.EmailTemplateViewModels;
using Asp.Web.Common.Mvc.Alerts;
using AutoMapper;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asp.Web.Areas.Administration.Controllers
{
    [Area(Constants.Areas.Administration)]
    [Authorize(Policy = Constants.RoleNames.Administrator)]
    public class EmailTemplatesController : Controller
    {
        private readonly IDateTime _dateTime;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IMapper _mapper;
        private readonly IUserSession _userSession;

        public EmailTemplatesController(
            IDateTime dateTime,
            IEmailTemplateRepository emailTemplateRepository,
            IMapper mapper,
            IUserSession userSession)
        {
            _dateTime = dateTime;
            _emailTemplateRepository = emailTemplateRepository;
            _mapper = mapper;
            _userSession = userSession;
        }

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            return View("List");
        }

        [HttpPost]
        public async Task<ActionResult> List([DataSourceRequest] DataSourceRequest request)
        {
            var entities = await _emailTemplateRepository.GetAllEmailTemplates();
            var models = entities.Select(e => _mapper.Map<EmailTemplate, EmailTemplateViewModel>(e)).ToList();
            var result = new DataSourceResult {Data = models, Total = models.Count};
            return Json(result);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var emailTemplate = await _emailTemplateRepository.GetEmailTemplateById(id);

            if (emailTemplate == null)
                return RedirectToAction("List");

            var model = _mapper.Map<EmailTemplate, EmailTemplateViewModel>(emailTemplate);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EmailTemplateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var emailTemplate = await _emailTemplateRepository.GetEmailTemplateById(model.Id);

                if (emailTemplate == null)
                    return RedirectToAction("List");

                model.Body = WebUtility.HtmlDecode(model.Body);
                model.Instruction = WebUtility.HtmlDecode(model.Instruction);
                emailTemplate = _mapper.Map<EmailTemplateViewModel, EmailTemplate>(model);
                emailTemplate.ModifiedBy = _userSession.UserName;
                emailTemplate.ModifiedOn = _dateTime.Now;
                await _emailTemplateRepository.UpdateEmailTemplate(emailTemplate);
                return RedirectToAction("List").WithSuccess($"{emailTemplate.Name} template was updated successfully.");
            }
            return View(model);
        }
    }
}