namespace PPProject.DailyMission.DTO.Request
{
    public class SuccessDailyMissionRequest
    {
        public List<DailyMissionProgress> MissionProgresses { get; set; }
    }

    public class DailyMissionProgress
    {
        public int MissionId { get; set; }
        public int Progress { get; set; }
    }
}
