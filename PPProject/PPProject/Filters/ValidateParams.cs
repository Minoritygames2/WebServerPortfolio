using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PPProject.Common;

namespace PPProject.Filters
{
    public class ValidateParams : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if(!context.ModelState.IsValid)
            {
                //비어져있는 값들 필터링
                var errors = context.ModelState
                    .Where(e => e.Value != null && e.Value.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                //응답값을 Error로 처리
                var response = ApiResponse<object>.Error(
                    ErrorCode.PARAMETER_VALIDATION_FAILED, 
                    "Parameter validation failed",
                    errors
                );

                context.Result = new BadRequestObjectResult(response);
            }
            base.OnActionExecuting(context);
        }
    }
}
