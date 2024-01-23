using Api.Results.Classroom;
using Api.Results.Students;
using Core;
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
    public string Description { get; set; }
    public DateTime Time { get; set; }

    public IList<StudentResult> Students { get; set; }
    public IList<ClassroomResult> Classrooms { get; set; }
}
