using Microsoft.AspNetCore.Mvc;
using PPProject.Common;
using PPProject.Common.Web;
using PPProject.Profile.DTO.Response;
using PPProject.Resource.Service;

namespace PPProject.Resource.Controller
{
    public class ResourceTest
    {
        public int ResourceId { get; set; }
        public int Amount { get; set; }
    }
    [ApiController]
    [Route("resourcetest")]
    
    public class ResourceTestController: BaseController
    {
        private ResourceService _resourceService;
        public ResourceTestController(ResourceService resourceService)
        {  
            _resourceService = resourceService;
        }

        [HttpPost("Add")]
        public async Task<ActionResult<ApiResponse<string>>> AddResource(ResourceTest request)
        {
            try
            {
                var result = await _resourceService.AddResourceProcess(
                UserId,
                request.ResourceId,
                request.Amount,
                ResourceReasonCode.EARN
                );

                return ApiResponse<string>.Success(result.ToString());
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Error(500, ex.Message);
            }
        }

        [HttpPost("Consume")]
        public async Task<ActionResult<ApiResponse<string>>> ConsumeResource(ResourceTest request)
        {
            try
            {
                var result = await _resourceService.ConsumeResourceProcess(
                UserId,
                request.ResourceId,
                request.Amount,
                ResourceReasonCode.BUY
                );
                return ApiResponse<string>.Success(result.ToString());
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Error(500, ex.Message);
            }
        }
    }
}
