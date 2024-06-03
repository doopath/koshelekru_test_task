using api.Domain;
using api.Domain.DTOs;
using api.Domain.Models;
using api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace api.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly MessageRepository _repository;
    
    public MessageController(IConfiguration configuration)
    {
        var dbSettings = new DbSettings(configuration);
        _repository = new(dbSettings);
    }

    [HttpGet]
    [Route("in-real-time/{port:int}")]
    public string InRealTime(int port)
    {
        var ip = HttpContext.Connection.LocalIpAddress;
        MessageNotificationServer.AddConnection(ip!.ToString(), port);
        return $"{ip}:{port}";
    }
    
    [HttpPost]
    [Route("send")]
    public async void Send(AddMessageDTO dto)
    {
        _repository.Add(dto);
        await MessageNotificationServer.NotifyClients();
    }

    [HttpGet]
    [Route("last-10-minutes")]
    public IEnumerable<Message> GetLast10Minutes()
    {
        var messages = _repository.GetAll();
        var last10Minutes = messages
            .Where(m => m.Date >= DateTime.Now.AddMinutes(-10))
            .ToList();
        
        return last10Minutes;
    }
}