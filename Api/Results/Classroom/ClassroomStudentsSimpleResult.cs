﻿using Api.Results.Diary;
using Api.Results.Students;
using Api.Results.Users;
using Core.Enums;

namespace Api.Results.Classroom;

public class ClassroomStudentsSimpleResult
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public int MaxStudents { get; set; }
    public int CurrentQuantityStudents { get; set; }
    public UserSimpleResult Teacher { get; set; }

    public IList<StudentSimpleListResult> Students { get; set; }

    public Status Status { get; set; }
    public Time Time { get; set; }
    public ClassroomType ClassroomType { get; set; }
}
