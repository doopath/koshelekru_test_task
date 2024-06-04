using api.Domain;
using api.Domain.DTOs;
using api.Domain.Entities;
using api.Infrastructure.Contracts;
using NLog;
using Npgsql;

namespace api.Infrastructure.Repositories;

/// <summary>
/// Represents a repository for managing messages.
/// </summary>
public class MessageRepository : IEntityRepository<Message, AddMessageDTO>
{
    private readonly DbSettings _dbSettings;
    private readonly Logger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageRepository"/> class.
    /// </summary>
    /// <param name="dbSettings">The database settings.</param>
    public MessageRepository(DbSettings dbSettings)
    {
        _dbSettings = dbSettings;
        _logger = LogManager.GetCurrentClassLogger();
    }

    /// <summary>
    /// Retrieves all messages from the database.
    /// </summary>
    /// <returns>An enumerable collection of messages.</returns>
    public IEnumerable<Message> GetAll()
    {
        var query = "SELECT id, content, date FROM Messages";
        var messages = new List<Message>();

        var result = EnsureConnection<IEnumerable<Message>>((source) =>
        {
            using var command = source.CreateCommand(query);
            using var reader = command.ExecuteReader();

            while (reader.Read())
                messages.Add(new Message()
                {
                    Id = reader.GetInt32(0),
                    Content = reader.GetString(1),
                    Date = reader.GetDateTime(2)
                });

            return messages;
        });
        
        _logger.Info($"Retrieved {messages.Count} messages from the database.");
        
        return result;
    }

    /// <summary>
    /// Adds a new message to the database.
    /// </summary>
    /// <param name="dto">The data transfer object containing the message content.</param>
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

        _logger.Info("Added a new message to the database.");
    }

    /// <summary>
    /// Ensures a connection to the database and executes the provided action.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="action">The action to be executed.</param>
    /// <returns>The result of the action.</returns>
    private T EnsureConnection<T>(Func<NpgsqlDataSource, T> action)
    {
        using var source = NpgsqlDataSource.Create(_dbSettings.ConnectionString);
        _logger.Info("Connecting to the database.");
        
        return action(source);
    }

    /// <summary>
    /// Ensures a connection to the database and executes the provided action.
    /// </summary>
    /// <param name="action">The action to be executed.</param>
    private void EnsureConnection(Action<NpgsqlDataSource> action)
    {
        using var source = NpgsqlDataSource.Create(_dbSettings.ConnectionString);
        _logger.Info("Connected to the database.");
        
        action(source);
    }
}