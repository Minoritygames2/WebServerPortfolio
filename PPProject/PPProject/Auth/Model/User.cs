namespace PPProject.Auth.Model
{
    public class User
    {
        public long uId { get; set; }
        public int platformCode { get; set; }
        public string platformUserId { get; set; } = string.Empty;
        public int Status { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime updateTime { get; set; }
    }
}
