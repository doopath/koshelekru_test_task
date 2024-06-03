namespace api.Infrastructure.Exceptions;

public class EntityNotFoundException(string message) : BaseInfrastructureException(message);