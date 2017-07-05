using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Asp.Web.Common.Mvc.Alerts
{
    public class AlertDecoratorResult : ActionResult
    {
        public ActionResult InnerResult { get; }
        public string AlertClass { get; }
        public string Message { get; }

        public AlertDecoratorResult(
            ActionResult innerResult,
            string alertClass,
            string message)
        {
            InnerResult = innerResult;
            AlertClass = alertClass;
            Message = message;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            var factory = context.HttpContext.RequestServices
                .GetService(typeof(ITempDataDictionaryFactory)) as ITempDataDictionaryFactory;
            var tempData = factory.GetTempData(context.HttpContext);

            var alerts = tempData.GetAlerts();
            alerts.Add(new Alert(AlertClass, Message));
            tempData.SetAlerts(alerts);

            await InnerResult.ExecuteResultAsync(context);
        }
    }
}