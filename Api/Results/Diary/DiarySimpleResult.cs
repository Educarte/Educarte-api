using Api.Results.Classroom;
using Api.Results.Students;
using Core;
using Core.Enums;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Results.Diary;

public class DiarySimpleResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string FileUri { get; set; }
    public string Description { get; set; }
    public bool IsDiaryForAll { get; set; }
    public DiaryType DiaryType { get; set; }
    public DateTime Time { get; set; }
}
