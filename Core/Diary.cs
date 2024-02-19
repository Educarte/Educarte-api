using Core.Enums;
using Core.Interfaces;

namespace Core;

public class Diary : IEntity, IDeletable
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string FileUri { get; set; }
    public bool IsDiaryForAll { get; set; }
    public DateTime Time { get; set; }

    public Status Status { get; set; }
    public DiaryType DiaryType { get; set; }

    public List<Student> Students { get; set; }
    public List<Classroom> Classrooms { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get ; set ; }
}
