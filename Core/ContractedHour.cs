using Core.Enums;
using Core.Interfaces;

namespace Core;

public class ContractedHour : IEntity, IDeletable
{
    public Guid Id { get; set; }

    public decimal Hours { get; set; }
    public Status Status { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public Guid StudentId { get; set; }
    public Student Student { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
