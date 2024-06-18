using Api.Results.Classroom;
using Api.Results.Students;
using Core.Enums;
using Core.Interfaces;

namespace Api.Results.Diary;

public class DiaryResult : IMobileListResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string FileUri { get; set; }
    public string Description { get; set; }
    public bool IsDiaryForAll { get; set; }

    public Status Status { get; set; }
    public DiaryType DiaryType { get; set; }

    public DateTime Time { get; set; }
    public DateTime CreatedAt { get; set; }

    public IList<StudentBasicResult> Students { get; set; }
    public IList<ClassroomSimpleResult> Classrooms { get; set; }
}
