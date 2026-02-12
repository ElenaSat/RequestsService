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

    public static Solicitud Create(string name, string payload)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Name cannot be null or empty.");
        
        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentNullException(nameof(payload), "Payload cannot be null or empty.");

        return new Solicitud
        {
            Id = Guid.NewGuid(),
            Name = name,
            Payload = payload,
            Status = SolicitudStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
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
