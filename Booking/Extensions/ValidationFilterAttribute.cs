using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Booking.Helpers;

namespace Booking.Extensions
{
    public class ValidationFilterAttribute : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.ModelState.IsValid)
            {
                var actionReporter = ActionReporterProvider.Set("Missing required fields.", StatusCodes.Status400BadRequest, new Dictionary<object, object>());

                foreach (var error in filterContext.ModelState)
                {
                    var valuesError = new List<string>();

                    foreach (var value in error.Value.Errors)
                    {
                        valuesError.Add(value.ErrorMessage);
                    }

                    actionReporter.Details.Add(error.Key, valuesError);
                }

                filterContext.Result = new BadRequestObjectResult(actionReporter);
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext) { }
    }
}
