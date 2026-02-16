using RequestsService.Domain.Common;
using RequestsService.Domain.Enums;

namespace RequestsService.Domain.Entities;

public class Solicitud
{
    // Properties are private set or init-only to enforce immutability/encapsulation
    public Guid Id { get; init; }
    public string Name { get; private set; }
    public string Payload { get; private set; }
    public SolicitudStatus Status { get; private set; }
    public DateTime CreatedAt { get; init; }

    // Private constructor for EF Core or serialization if needed, though mostly for strict creation via factory
    private Solicitud() 
    { 
        Name = null!;
        Payload = null!;
    }

    public static Result<Solicitud> Create(string name, string payload)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Solicitud>.Failure("Name cannot be null or empty.");
        
        if (string.IsNullOrWhiteSpace(payload))
            return Result<Solicitud>.Failure("Payload cannot be null or empty.");

        var solicitud = new Solicitud
        {
            Id = Guid.NewGuid(),
            Name = name,
            Payload = payload,
            Status = SolicitudStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        return Result<Solicitud>.Success(solicitud);
    }

    public void MarkAsProcessed()
    {
        Status = SolicitudStatus.Processed;
    }

    public void MarkAsFailed()
    {
        Status = SolicitudStatus.Failed;
    }
}
