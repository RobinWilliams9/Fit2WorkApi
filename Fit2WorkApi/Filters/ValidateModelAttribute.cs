using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AnvilGroup.Services.Fit2WorkApi.Filters {
    public class ValidateModelAttribute : ActionFilterAttribute {
        public override void OnActionExecuting(HttpActionContext actionContext) {
            if (actionContext.ModelState.IsValid == false) {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
    }
}