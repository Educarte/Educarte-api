using Core.Interfaces;

namespace Core;

public class Diary : IEntity, IDeletable
{
    public Guid Id { get; set; }

    public string Description { get; set; }
    public DateTime Time { get; set; }

    public Guid? UserId { get; set; }
    public User User { get; set; }

    public IList<Student> Students { get; set; }
    public IList<Classroom> Classrooms { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get ; set ; }
}
