using PPProject.DailyMission.DTO;
using StackExchange.Redis;
using System.Text.Json;

namespace PPProject.DailyMission.Infrastructure.Redis
{
    public class DailyMissionStore
    {
        private readonly IDatabase _db;
        private const string Key = "daily_mission";

        public DailyMissionStore(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<List<DailyMissionCache>> GetStoreDailyMissionsAsync()
        {
            var result = await _db.HashGetAllAsync(Key);
            if (result.Length == 0)
                return new List<DailyMissionCache>();

            return result
                .Select(x => JsonSerializer.Deserialize<DailyMissionCache>(x.Value.ToString()!)!)
                .ToList();
        }
    }
}
