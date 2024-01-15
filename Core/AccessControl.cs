using Core.Enums;
using Core.Interfaces;

namespace Core;

public class AccessControl : IEntity, IDeletable
{
    public Guid Id { get; set; }

    public AccessControlType AccessControlType { get; set; }
    public DateTime Time { get; set; }

    public Guid StudentId { get; set; }
    public Student Student { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
