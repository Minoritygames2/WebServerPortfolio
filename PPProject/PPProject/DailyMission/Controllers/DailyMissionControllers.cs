using Microsoft.AspNetCore.Mvc;
using PPProject.Common;
using PPProject.Common.Web;
using PPProject.DailyMission.DTO.Request;
using PPProject.DailyMission.DTO.Response;
using PPProject.DailyMission.Service;
using PPProject.Filters;

namespace PPProject.DailyMission.Controllers
{
    [ApiController]
    [Route("dailymission")]
    [ValidateParams]
    public class DailyMissionControllers : BaseController
    {
        private readonly DailyMissionService _service;
        public DailyMissionControllers(DailyMissionService service) 
        {
            _service = service;
        }

        [HttpPost("get")]
        public async Task<ApiResponse<GetDailyMissionResponse>> GetDailyMission()
        {
            try
            {
                var result = await _service.GetDailyMission(UserId);
                var response = new GetDailyMissionResponse();
                response.Infos = result;
                return ApiResponse<GetDailyMissionResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<GetDailyMissionResponse>.Error(500, ex.Message);
            }
        }

    }
}
