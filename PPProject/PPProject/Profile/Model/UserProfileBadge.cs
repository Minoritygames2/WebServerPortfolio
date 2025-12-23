namespace PPProject.Profile.Model
{
    public class UserProfileBadge
    {
        public long uId { get; set; }
        public int slotIndex { get; set; }
        public int badgeType { get; set; }
        public int badgeId { get; set; }
        public string displayValue { get; set; } = string.Empty;
        public DateTime createdTime { get; set; }
        public DateTime updateTime { get; set; }
    }
}
