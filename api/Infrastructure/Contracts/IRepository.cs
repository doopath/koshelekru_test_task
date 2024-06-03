using api.Domain.Contracts;

namespace api.Infrastructure.Contracts;

public interface IEntityRepository<T, D> where T : IEntity where D : IEntityDTO
{
    public IEnumerable<T> GetAll();
    public void Add(D dto);
}