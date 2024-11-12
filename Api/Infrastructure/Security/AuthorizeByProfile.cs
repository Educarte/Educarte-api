using Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Infrastructure.Security;

public class AuthorizeByProfile : TypeFilterAttribute
{
    public AuthorizeByProfile(params Profile[] controllerRoles) : base(typeof(AuthorizeByClaimFilter))
    {
        Arguments = new object[] { controllerRoles };
    }
}

public class AuthorizeByClaimFilter : IAuthorizationFilter
{
    private readonly Profile[] controllerRoles;

    public AuthorizeByClaimFilter(Profile[] controllerRoles)
    {
        this.controllerRoles = controllerRoles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.User.Claims.Any())
        {
            context.Result = new ForbidResult();
            return;
        }

        var userRole = Enum.Parse<Profile>(context.HttpContext.User.Claims
                .FirstOrDefault(d => d.Type == nameof(Profile).ToLower()).Value);

        if (!controllerRoles.Any(cr => cr == userRole))
            context.Result = new ForbidResult();
    }
}