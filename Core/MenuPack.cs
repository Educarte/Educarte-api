using Core.Enums;
using Core.Interfaces;

namespace Core;

public class MenuPack : IEntity, IDeletable
{
    public Guid Id { get; set; }

    public string NutritionistName { get; set; }
    public bool RestrictiveMenu { get; set; }

    public IList<Menu> Menu { get; set; }
    public IList<Student> Students { get; set; }

    public DateTime ValidUntil { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
