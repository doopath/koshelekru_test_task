using api.Domain;
using api.Domain.DTOs;
using api.Domain.Models;
using api.Infrastructure.Contracts;
using Npgsql;

namespace api.Infrastructure.Repositories;

public class MessageRepository(DbSettings dbSettings) : IEntityRepository<Message, AddMessageDTO>
{
    public IEnumerable<Message> GetAll()
    {
        var query = "SELECT id, content, date FROM Messages";
        var messages = new List<Message>();
        
        return EnsureConnection<IEnumerable<Message>>((source) =>
        {
            using var command = source.CreateCommand(query);
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
                messages.Add(new Message()
                {
                    Id=reader.GetInt32(0),
                    Content=reader.GetString(1),
                    Date=reader.GetDateTime(2)
                });

            return messages;
        });
    }

    public void Add(AddMessageDTO dto)
    {
        var now = DateTime.Now;
        var content = dto.Content.Replace("'", "''");
        var query = $"INSERT INTO Messages (content, date) VALUES ('{content}', now())";

        EnsureConnection(source =>
        {
            using var command = source.CreateCommand(query);
            command.ExecuteNonQuery();
        });
    }

    private T EnsureConnection<T>(Func<NpgsqlDataSource, T> action)
    {
        using var source = NpgsqlDataSource.Create(dbSettings.ConnectionString);
        return action(source);
    }
    
    private void EnsureConnection(Action<NpgsqlDataSource> action)
    {
        using var source = NpgsqlDataSource.Create(dbSettings.ConnectionString);
        action(source);
    }
}