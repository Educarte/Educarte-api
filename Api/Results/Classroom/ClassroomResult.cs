using Api.Results.Diary;
using Api.Results.Students;
using Api.Results.Users;
using Core.Enums;

namespace Api.Results.Classroom;

public class ClassroomResult
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public int MaxStudents { get; set; }
    public int CurrentQuantityStudents { get; set; }

    public Status Status { get; set; }
    public Time Time { get; set; }
    public ClassroomType ClassroomType { get; set; }

    public IList<UserSimpleResult> Teachers { get; set; }
    public IList<StudentBasicResult> Students { get; set; }
    public IList<DiarySimpleResult> Diaries { get; set; }
}
