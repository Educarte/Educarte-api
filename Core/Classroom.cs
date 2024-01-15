using Core.Interfaces;

namespace Core;

public class Classroom : IEntity, IDeletable
{
    public Guid Id { get; set; }

    public IList<User> Employee { get; set; }
    public IList<Student> Students { get; set; }
    public IList<DiaryClassroom> DiaryClassrooms { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
