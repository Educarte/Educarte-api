﻿using Api.Results.AccessControl;
using Api.Results.Classroom;
using Api.Results.ContractedHours;
using Api.Results.Diary;
using Api.Results.EmergencyContacts;
using Api.Results.Users;
using Core.Enums;

namespace Api.Results.Students;

public class StudentSoftResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public Genre Genre { get; set; }
    public BloodType BloodType { get; set; }
    public Time Time { get; set; }
    public Status Status { get; set; }

    public string ClassroomName { get; set; }
    public ContractedHourResult ContractedHours { get; set; }
}
