using System.ComponentModel.DataAnnotations;
using api.Domain.Contracts;

namespace api.Domain.Entities;

public record Message() : IEntity
{
    public int Id { get; init; }
    [MaxLength(128)] public required string Content { get; set; }
    public DateTime Date { get; init; }
}