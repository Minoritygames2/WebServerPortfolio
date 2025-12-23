namespace PPProject.Profile.Model
{
    public class UserProfile
    {
        public long uId { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public int CharId { get; set; }
        public int LabelId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
