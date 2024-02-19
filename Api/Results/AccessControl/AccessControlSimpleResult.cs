using Core.Enums;

namespace Api.Results.AccessControl;

public class AccessControlSimpleResult
{
    public Guid Id { get; set; }
    public AccessControlType AccessControlType { get; set; }
    public DateTime Time { get; set; }
}
