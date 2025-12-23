namespace PPProject.Profile.DTO.Request
{
    public class ChangeUserProfileRequest
    {
        public bool IsChangeNickname { get; set; }
        public string? Nickname { get; set; }
        public bool IsChangeLabel { get; set; }
        public int? LabelId { get; set; }
        public bool IsChangeMessage { get; set; }
        public string? Message { get; set; }
    }
}
