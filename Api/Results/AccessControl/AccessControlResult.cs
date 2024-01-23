using Core.Enums;

namespace Api.Results.AccessControl;

public class AccessControlResult
{
    public Guid Id { get; set; }
    public AccessControlType AccessControlType { get; set; }
    public DateTime Time { get; set; }
}
