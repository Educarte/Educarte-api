using Core;
using Core.Enums;

namespace Api.Results.Students;

public class StudentResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Status Status { get; set; }

    public Guid LegarGuardianId { get; set; }
    public User LegalGuardian { get; set; }

    public Guid ClassroomId { get; set; }
    public Classroom Classroom { get; set; }

    public IList<AccessControl> AccessControls { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
