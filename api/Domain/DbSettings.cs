namespace api.Domain;

using api.Domain.Contracts;

public class DbSettings(IConfiguration configuration) : IDbSettings
{
    public string ConnectionString { get; } = configuration.GetConnectionString("DefaultConnection")!;
}