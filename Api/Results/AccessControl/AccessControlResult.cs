using Api.Results.Classroom;
using Api.Results.ContractedHours;
using Api.Results.Students;
using Api.Results.Users;
using Core.Enums;

namespace Api.Results.AccessControl;

public class AccessControlResult
{
    public StudentBasicResult Student { get; set; }
    public ClassroomBasicResult Classroom { get; set; }
    public List<AccessControl> AccessControlsByDate { get; set; }
    public List<UserSimpleResult> LegalGuardians { get; set; }
    public TimeSpan Summary { get; set; }
}

public class AccessControl
{
    public DateTime Date { get; set; }

    public List<AccessControlSimpleResult> AccessControls { get; set; }
    public ContractedHourResult? ContractedHour { get; set; }
    public TimeSpan? DailySummary { get; set; }
}