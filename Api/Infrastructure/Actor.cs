using Core.Enums;
using System.Security.Claims;

namespace Api.Infrastructure;

/// <summary>
/// 
/// </summary>
public interface IActor
{
    /// <summary>
    /// 
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// 
    /// </summary>
    public Profile Profile { get; }

    /// <summary>
    /// 
    /// </summary>
    public IEnumerable<Claim> Claims { get; }
}

/// <summary>
/// 
/// </summary>
public class Actor : IActor
{
    /// <summary>
    /// 
    /// </summary>
    public virtual Guid UserId { get; }

    /// <summary>
    /// 
    /// </summary>
    public virtual Profile Profile { get; }

    /// <summary>
    /// 
    /// </summary>
    public virtual IEnumerable<Claim> Claims { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accessor"></param>
    public Actor(IHttpContextAccessor accessor)
    {
        if (accessor.HttpContext != null)
        {
            var sub = accessor.HttpContext.User.Claims.FirstOrDefault(d => d.Type == ClaimTypes.NameIdentifier);
            if (sub != null)
                UserId = Guid.Parse(sub.Value);

            var profile = accessor.HttpContext.User.Claims.FirstOrDefault(d => d.Type == "role" || d.Type == "profile");
            if (profile != null)
                Profile = (Profile)Enum.Parse(typeof(Profile), profile.Value);

            Claims = accessor.HttpContext.User.Claims.ToArray();
        }
    }
}
