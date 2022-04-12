using AspNetCore.JWTDemo.EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AspNetCore.JWTDemo.Permissions
{
    public class SelfOnlyAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, User>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, User resource)
        {
            if (context.User.Identity?.Name == resource.UserName)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
