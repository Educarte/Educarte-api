using Core.Enums;
using Core.Interfaces;

namespace Core;

public class Menu : IEntity, IDeletable
{
    public Guid Id { get; set; }

    public string Description { get; set; }
    public DaysOfTheWeek DayOfTheWeek { get; set; }


    public Guid MenuPackId { get; set; }
    public MenuPack MenuPack { get; set; }

    public DateTime ValidUntil { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
