using AspNetCore.JWTDemo.EntityFrameworkCore.Permissions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;

namespace AspNetCore.JWTDemo.Permissions
{
    public class RBACPolicyProvider : IAuthorizationPolicyProvider
    {
        private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; }
        public RBACPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            // ASP.NET Core only uses one authorization policy provider, so if the custom implementation
            // doesn't handle all policies it should fall back to an alternate provider.
            BackupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy?>(null);
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(AuthorizationPolicyDefinition.PermissionPrefix))
            {
                var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser();
                if (policyName.StartsWith(AuthorizationPolicyDefinition.RBAC, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (AuthorizationPolicyHelper.TryParseRBAC(policyName, out var resource, out var operation))
                    {
                        policy.AddRequirements(new RBACAuthorizationRequirement(resource, operation));
                        return Task.FromResult<AuthorizationPolicy?>(policy.Build());
                    }
                }
                else if (policyName == AuthorizationPolicyDefinition.SELF_ONLY)
                {
                    policy.Requirements.Add(new OperationAuthorizationRequirement());
                    return Task.FromResult<AuthorizationPolicy?>(policy.Build());
                }
                else
                {
                    return BackupPolicyProvider.GetPolicyAsync(policyName);
                }
            }
            return BackupPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
