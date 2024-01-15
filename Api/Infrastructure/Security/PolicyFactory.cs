using Core.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Infrastructure.Security
{
    internal static class PolicyFactory
    {
        internal static void AddPolicies(AuthorizationOptions op)
        {
            Enum.GetValues<Profile>()
                .Cast<Profile>()
                .ToList()
                .ForEach(_role =>
                {
                    op.AddPolicy(_role.ToString(), policy => policy.AddAuthenticationSchemes("Bearer")
                                                                   .RequireAuthenticatedUser()
                                                                   .RequireClaim(ClaimTypes.Role, _role.ToString())
                                                                   .Build());
                });
        }
    }
}
