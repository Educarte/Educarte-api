using Core.Enums;
using Core.Interfaces;

namespace Core;

public class Student : IEntity, IDeletable
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public Status Status { get; set; }

    public Guid LegalGuardianId { get; set; }
    public User LegalGuardian { get; set; }

    public Guid ClassroomId { get; set; }
    public Classroom Classroom { get; set; }

    public Guid? MenuPackId { get; set; }
    public MenuPack MenuPack { get; set; }

    public IList<AccessControl> AccessControls { get; set; }
    public IList<Diary> Diaries { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
