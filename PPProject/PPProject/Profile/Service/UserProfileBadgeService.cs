using PPProject.Profile.DTO;
using PPProject.Profile.DTO.Response;
using PPProject.Profile.Infrastructure;

namespace PPProject.Profile.Service
{
    public class UserProfileBadgeService
    {
        private readonly UserProfileBadgeRepository _userProfileBadgeRepository;
        public UserProfileBadgeService(UserProfileBadgeRepository userProfileBadgeRepository)
        {
            _userProfileBadgeRepository = userProfileBadgeRepository;
        }

        public async Task<List<ProfileBadgeDTO>> GetUserProfileBadgesAsync(long uId)
        {
            var result = await _userProfileBadgeRepository.GetUserBadgesByUidAsync(uId);

            return result.Select(badge => new ProfileBadgeDTO
            {
                SlotIndex = badge.slotIndex,
                BadgeType = badge.badgeType,
                BadgeId = badge.badgeId,
                DisplayValue = badge.displayValue
            }).ToList();
        }
    }
}
