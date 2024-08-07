﻿using Api.Results.AccessControl;
using Api.Results.Classroom;
using Api.Results.ContractedHours;
using Api.Results.Diary;
using Api.Results.EmergencyContacts;
using Api.Results.Menus;
using Api.Results.Users;
using Core.Enums;

namespace Api.Results.Students;

public class StudentResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Naturalness { get; set; }
    public string HealthProblem { get; set; }
    public string AllergicFood { get; set; }
    public string AllergicMedicine { get; set; }
    public bool Epilepsy { get; set; }
    public string AllergicBugBite { get; set; }
    public string SpecialChild { get; set; }
    public bool SpecialChildHasReport { get; set; }

    public Genre Genre { get; set; }
    public BloodType BloodType { get; set; }
    public Time Time { get; set; }
    public Status Status { get; set; }

    public DateTime BirthDate { get; set; }

    public MenuResult CurrentMenu { get; set; }

    public ClassroomSimpleResult Classroom { get; set; }

    public UserSimpleResult LegalGuardian { get; set; }
    public IList<AccessControlSimpleResult> AccessControls { get; set; }
    public List<DiarySimpleResult> Diaries { get; set; }
    public IList<ContractedHourResult> ContractedHours { get; set; }
    public IList<EmergencyContactResult> EmergencyContacts { get; set; }
}
