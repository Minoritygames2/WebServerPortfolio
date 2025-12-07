using IdGen;

namespace PPProject.Infrastructure
{
    public static class SnowflakeExtensions
    {
        public static IServiceCollection AddSnowflake(this IServiceCollection service)
        {
            //기준 시간
            var epoch = new DateTime(2025, 12, 6, 0, 0, 0, DateTimeKind.Utc);
            //비트 합계가 63이 되어야함
            var structure = new IdStructure(45, 2, 16);
            var options = new IdGeneratorOptions(structure, new DefaultTimeSource(epoch));

            service.AddSingleton<IdGenerator>(_ => new IdGenerator(1, options));
            return service;
        }
    }
}
