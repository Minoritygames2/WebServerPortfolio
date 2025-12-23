using Microsoft.AspNetCore.Mvc;
using PPProject.Common;
using PPProject.Common.Web;
using PPProject.Filters;
using PPProject.Profile.Config;
using PPProject.Profile.DTO.Request;
using PPProject.Profile.DTO.Response;
using PPProject.Profile.Service;
using Sprache;

namespace PPProject.Profile.Controllers
{
    [ApiController]
    [Route("userProfile")]
    [ValidateParams]
    public class UserProfileController : BaseController
    {
        private readonly UserProfileService _userProfileService;
        private readonly UserProfileBadgeService _userProfileBadgeService;
        public UserProfileController(UserProfileService userProfileService, UserProfileBadgeService userProfileBadgeService)
        {
            _userProfileService = userProfileService;
            _userProfileBadgeService = userProfileBadgeService;
        }

        [HttpPost("GetUserProfile")]
        public async Task<ActionResult<ApiResponse<GetUserProfileResponse>>> GetUserProfile()
        {
            try
            {
                var profile = await _userProfileService.GetUserProfileAsync(UserId);
                var badges = await _userProfileBadgeService.GetUserProfileBadgesAsync(UserId);
                var response = new GetUserProfileResponse()
                {
                    Nickname = profile.Nickname,
                    CharId = profile.CharId,
                    LabelId = profile.LabelId,
                    Message = profile.Message,
                    Badges = badges
                };

                return ApiResponse<GetUserProfileResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return ApiResponse<GetUserProfileResponse>.Error(500, ex.Message);
            }
        }

        [HttpPost("ChangeUserProfile")]
        public async Task<ApiResponse<bool>> ChangeUserProfile(ChangeUserProfileRequest request)
        {
            try
            {
                int labelId = request.LabelId ?? 0;
                var result = await _userProfileService.ChangeUserProfileAsync(
                    UserId,
                    request.IsChangeNickname,
                    request.Nickname,
                    request.IsChangeLabel,
                    labelId,
                    request.IsChangeMessage,
                    request.Message);
                return ApiResponse<bool>.Success(result);
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Error(ErrorCode.DB_ERROR, ex.Message);
            }
        }
    }
}
