namespace PPProject.DailyMission.Model
{
    public class UserDailyMission
    {
        public int Id { get; set; }
        public long uId { get; set; }
        public int MissionId { get; set; }
        public DateOnly MissionDate { get; set; }
        public int Progress { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime CompletedTime { get; set; }
        public bool ReceiveReward { get; set; }
        public DateTime ReceiveTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
