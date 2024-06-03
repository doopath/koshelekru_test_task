using api.Domain;
using api.Domain.DTOs;
using api.Domain.Entities;
using api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace api.Application.Controllers;

/// <summary>
/// Controller for handling messages.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly MessageRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageController"/> class.
    /// </summary>
    /// <param name="configuration">The configuration for database settings.</param>
    public MessageController(IConfiguration configuration)
    {
        var dbSettings = new DbSettings(configuration);
        _repository = new(dbSettings);
    }

    /// <summary>
    /// Registers a new connection for the notification service.
    /// </summary>
    /// <param name="port">The port number for the connection.</param>
    /// <returns>The IP address and port number of the new connection.</returns>
    [HttpGet]
    [Route("in-real-time/{port:int}")]
    public string InRealTime(int port)
    {
        var ip = HttpContext.Connection.LocalIpAddress;
        MessageNotificationServer.AddConnection(ip!.ToString(), port);
        return $"{ip}:{port}";
    }

    /// <summary>
    /// Adds a new message to the repository and notifies connected clients.
    /// </summary>
    /// <param name="dto">The data transfer object containing the message details.</param>
    [HttpPost]
    [Route("send")]
    public async void Send(AddMessageDTO dto)
    {
        _repository.Add(dto);
        await MessageNotificationServer.NotifyClients();
    }

    /// <summary>
    /// Retrieves messages from the last specified number of minutes.
    /// </summary>
    /// <param name="timestamp">The number of minutes to consider.</param>
    /// <returns>An enumerable collection of messages from the last specified number of minutes.</returns>
    [HttpGet]
    [Route("last-minutes/{timestamp:int}")]
    public IEnumerable<Message> GetLast10Minutes(int timestamp)
    {
        var messages = _repository.GetAll();
        var lastMinutes = messages
           .Where(m => m.Date >= DateTime.Now.AddMinutes(-timestamp))
           .ToList();

        return lastMinutes;
    }
}