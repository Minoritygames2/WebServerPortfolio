using System.Text.Json.Serialization;

namespace PPProject.DailyMission.DTO
{
    public class DailyMissionInfo
    {
        public int MissionId { get; set; }
        public int MissionType { get; set; }
        public int SuccessValue { get; set; }
        public bool IsSuccess { get; set; }
        public bool ReceivedReward { get; set; }
    }

    public class DailyMissionCache
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("mission_type")]
        public int MissionType { get; set; }
        [JsonPropertyName("success_value")]
        public int SuccessValue { get; set; }
        [JsonPropertyName("reward_id")]
        public int RewardId { get; set; }
    }
}
