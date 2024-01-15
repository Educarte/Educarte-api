using Core.Interfaces;

namespace Core;

public class DiaryClassroom : IEntity, IDeletable
{
    public Guid Id { get; set; }

    public string Description { get; set; }
    public DateTime Time { get; set; }

    public Guid ClassroomId { get; set; }
    public Classroom Classroom { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get ; set ; }
}
