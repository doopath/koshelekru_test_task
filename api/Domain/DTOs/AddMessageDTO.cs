using api.Domain.Contracts;

namespace api.Domain.DTOs;

public class AddMessageDTO : IEntityDTO
{
    public required string Content { get; init; }
}