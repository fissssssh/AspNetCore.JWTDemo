using AspNetCore.JWTDemo.EntityFrameworkCore.Permissions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.JWTDemo.EntityFrameworkCore.Extensions
{
    public static class RoleManagerExtensions
    {
        public static Task AddPermission<TRole>(this RoleManager<TRole> roleManager, TRole role, Resource resource, Operation operation) where TRole : IdentityRole
        {
            return roleManager.AddClaimAsync(role, new Claim($"{PolicyDefinitions.RBAC}.{resource}", ((int)operation).ToString()));
        }
        public static async Task AddPermissions<TRole>(this RoleManager<TRole> roleManager, TRole role, IEnumerable<KeyValuePair<Resource, Operation>> permissions) where TRole : IdentityRole
        {
            foreach (var permission in permissions)
            {
                await roleManager.AddPermission(role, permission.Key, permission.Value);
            }
        }
    }
}
