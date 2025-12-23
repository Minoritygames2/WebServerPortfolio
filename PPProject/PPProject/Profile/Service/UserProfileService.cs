using PPProject.Profile.Config;
using PPProject.Profile.Infrastructure;
using PPProject.Profile.Model;

namespace PPProject.Profile.Service
{
    public class UserProfileService
    {
        private readonly UserProfileRepository _userProfileRepository;
        public UserProfileService(UserProfileRepository userProfileRepository) 
        { 
            _userProfileRepository = userProfileRepository;
        }
        public async Task<UserProfile> CreateDefaultProfile(long uId)
        {
            //기본 프로필 생성
            await _userProfileRepository.CreateProfileAsync(
                uId,
                UserProfileConfigs.DEFAULT_NICKNAME,
                UserProfileConfigs.DEFAULT_CHAR_ID,
                UserProfileConfigs.DEFAULT_LABEL_ID,
                UserProfileConfigs.DEFAULT_MESSAGE
                );

            return new UserProfile
            {
                uId = uId,
                Nickname = UserProfileConfigs.DEFAULT_NICKNAME,
                CharId = UserProfileConfigs.DEFAULT_CHAR_ID,
                LabelId = UserProfileConfigs.DEFAULT_LABEL_ID,
                Message = UserProfileConfigs.DEFAULT_MESSAGE,
                CreatedTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
        }

        public async Task<UserProfile?> GetUserProfileAsync(long uId)
        {
            return await _userProfileRepository.GetByUidAsync(uId);
        }

        public async Task<bool> ChangeUserProfileAsync(
            long uId,
            bool isChangeNickName,
            string nickName,
            bool isChangeLabel,
            int labelId,
            bool isChangeMessage,
            string message
            )
        {
            return await _userProfileRepository.UpdateUserProfileAsync(
                uId,
                isChangeNickName,
                nickName,
                isChangeLabel,
                labelId,
                isChangeMessage,
                message
                );
        }
    } 
}
