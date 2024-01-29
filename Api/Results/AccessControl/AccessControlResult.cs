using Api.Results.ContractedHours;
using Api.Results.Students;
using Core.Enums;

namespace Api.Results.AccessControl;

public class AccessControlResult
{
    public Guid Id { get; set; }
    public AccessControlType AccessControlType { get; set; }
    public DateTime Time { get; set; }

    public StudentResult Student { get; set; }
    public ContractedHourResult ContractedHour { get; set; }
}
