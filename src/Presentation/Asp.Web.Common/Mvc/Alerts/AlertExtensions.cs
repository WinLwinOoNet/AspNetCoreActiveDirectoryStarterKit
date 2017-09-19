using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Asp.Web.Common.Mvc.Alerts
{
    /// <summary>
    /// Alert JavaScript code displayed at client-side is located inside App.js.
    /// </summary>
    public static class AlertExtensions
    {
        private const string AlertsKey = "alerts";

        public static IList<Alert> GetAlerts(this ITempDataDictionary tempData)
        {
            if (!tempData.ContainsKey(AlertsKey))
            {
                tempData.Set(AlertsKey, new List<Alert>());
            }
            return tempData.Get<List<Alert>>(AlertsKey);
        }

        public static void SetAlerts(this ITempDataDictionary tempData, IList<Alert> alerts)
        {
            tempData.Set(AlertsKey, alerts);
        }

        public static IActionResult WithSuccess(this IActionResult result, string message)
        {
            return new AlertDecoratorResult(result, "alert-success", message);
        }

        public static IActionResult WithInfo(this IActionResult result, string message)
        {
            return new AlertDecoratorResult(result, "alert-info", message);
        }

        public static IActionResult WithWarning(this IActionResult result, string message)
        {
            return new AlertDecoratorResult(result, "alert-warning", message);
        }

        public static IActionResult WithError(this IActionResult result, string message)
        {
            return new AlertDecoratorResult(result, "alert-danger", message);
        }
    }
}