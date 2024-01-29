using Core.Enums;
using Core.Interfaces;

namespace Api.Results.Menus;

public class MenuResult
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Uri { get; set; }
    public Status Status { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime ValidUntil { get; set; }
}
