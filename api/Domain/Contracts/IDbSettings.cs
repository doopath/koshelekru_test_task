namespace api.Domain.Contracts;

public interface IDbSettings
{
    public string ConnectionString { get; }
}