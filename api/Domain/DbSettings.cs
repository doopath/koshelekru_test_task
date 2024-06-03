using api.Domain.Contracts;

namespace api.Domain;

public class DbSettings(IConfiguration configuration) : IDbSettings
{
    public string ConnectionString { get; } = configuration.GetConnectionString("DefaultConnection")!;
}