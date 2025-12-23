namespace PPProject.Profile.DTO.Response
{
    public class GetUserProfileResponse
    {
        public string Nickname { get; set; }
        public int CharId { get; set; }
        public int LabelId { get; set; }
        public string Message { get; set; }

        public List<ProfileBadgeDTO> Badges { get; set; } = new ();
    }

    public class ProfileBadgeDTO
    {
        public int SlotIndex { get; set; }
        public int BadgeType { get; set; }
        public int BadgeId { get; set; }
        public string DisplayValue { get; set; }
    }
}
