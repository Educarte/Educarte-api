using Api.Results.Classroom;
using Api.Results.Students;
using Api.Results.Users;
using Core;
using Core.Enums;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Results.Diary;

public class DiaryResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string FileUri { get; set; }
    public string Description { get; set; }
    public bool IsDiaryForAll { get; set; }

    public Status Status { get; set; }

    public DateTime Time { get; set; }
    public DateTime CreatedAt { get; set; }

    public IList<StudentBasicResult> Students { get; set; }
    public IList<ClassroomSimpleResult> Classrooms { get; set; }
}
