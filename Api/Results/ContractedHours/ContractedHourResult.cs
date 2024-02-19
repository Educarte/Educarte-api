using Core.Enums;

namespace Api.Results.ContractedHours;

public class ContractedHourResult
{
    public Guid Id { get; set; }
    public decimal Hours { get; set; }
    public Status Status { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
