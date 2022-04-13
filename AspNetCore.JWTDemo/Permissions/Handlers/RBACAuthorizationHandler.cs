using AspNetCore.JWTDemo.EntityFrameworkCore;
using AspNetCore.JWTDemo.EntityFrameworkCore.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.JWTDemo.Permissions
{
    public class RBACAuthorizationHandler : AuthorizationHandler<RBACAuthorizationRequirement>
    {
        private readonly JWTDemoDbContext _context;

        public RBACAuthorizationHandler(JWTDemoDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RBACAuthorizationRequirement requirement)
        {
            var permissionClaim = await (from u in _context.Users.AsNoTracking()
                                         join ur in _context.UserRoles.AsNoTracking() on u.Id equals ur.UserId
                                         join r in _context.Roles.AsNoTracking() on ur.RoleId equals r.Id
                                         join rc in _context.RoleClaims.AsNoTracking() on r.Id equals rc.RoleId
                                         where u.UserName == context.User.Identity!.Name && rc.ClaimType.StartsWith(EntityFrameworkCore.Permissions.PolicyDefinitions.RBAC + "." + requirement.Resource.ToString())
                                         select new
                                         {
                                             rc.ClaimType,
                                             rc.ClaimValue
                                         }).FirstOrDefaultAsync();
            if (permissionClaim != null)
            {
                if (int.TryParse(permissionClaim.ClaimValue, out var operationValue))
                {
                    var operation = (Operation)operationValue;
                    if (operation.HasFlag(requirement.Operation))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }
    }
}
