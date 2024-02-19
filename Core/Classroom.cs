using Core.Enums;
using Core.Interfaces;

namespace Core;

public class Classroom : IEntity, IDeletable
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public int MaxStudents { get; set; }

    public Status Status { get; set; }
    public Time Time { get; set; }
    public ClassroomType ClassroomType { get; set; }

    public List<User> Teachers { get; set; }
    public List<Student> Students { get; set; }
    public IList<Diary> Diaries { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
