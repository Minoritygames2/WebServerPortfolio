namespace PPProject.Usecase
{
    public static class UsecaseExtentions
    {
        public static IServiceCollection AddUsecases(this IServiceCollection services)
        {
            services.AddScoped<CreateUser.CreateUserUseCase>();
            return services;
        }
    }
}
