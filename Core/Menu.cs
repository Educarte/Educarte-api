using Core.Enums;
using Core.Interfaces;

namespace Core;

public class Menu : IEntity, IDeletable
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Uri { get; set; }
    public Status Status { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime ValidUntil { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
