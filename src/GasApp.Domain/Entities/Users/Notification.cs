namespace GasApp.Domain.Entities.Users;

public class Notification
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public string Type { get; private set; } = null!;
    public Guid? ReferenceId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Notification() { }

    public static Notification Create(Guid userId, string title, string body, string type, Guid? referenceId = null)
        => new()
        {
            UserId = userId,
            Title = title,
            Body = body,
            Type = type,
            ReferenceId = referenceId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

    public void MarkAsRead() => IsRead = true;
}
