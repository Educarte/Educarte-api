using Core.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Api.Infrastructure.Security
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthorizeByProfileAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profiles"></param>
        public AuthorizeByProfileAttribute(params Profile[] profiles) : base()
        {
            Policy = profiles.Select(d => d.ToString())
                             .Aggregate((d1, d2) => $"{d1},{d2}");
        }
    }
}
