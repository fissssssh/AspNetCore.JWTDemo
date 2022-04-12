using AspNetCore.JWTDemo.EntityFrameworkCore;
using AspNetCore.JWTDemo.EntityFrameworkCore.Extensions;
using AspNetCore.JWTDemo.EntityFrameworkCore.Models;
using AspNetCore.JWTDemo.EntityFrameworkCore.Permissions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AspNetCore.JWTDemo.Migrator
{
    public class DbInitializer
    {
        private readonly JWTDemoDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(JWTDemoDbContext db, UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<DbInitializer> logger)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitAsync()
        {
            await _db.Database.EnsureCreatedAsync();
            await _db.Database.MigrateAsync();

            var adminUser = await _userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                _logger.LogInformation("User “admin” not exists.");
                _logger.LogInformation("Creating user “admin”.");
                adminUser = new User { UserName = "admin", Email = "admin@admin.com", EmailConfirmed = true };
                await _userManager.CreateAsync(adminUser, "123qwe*");
                _logger.LogInformation("Created user “admin”.");
            }
            var adminRole = await _roleManager.FindByNameAsync("admin");
            if (adminRole == null)
            {
                _logger.LogInformation("Role “admin” not exists.");
                _logger.LogInformation("Creating role “admin”.");
                adminRole = new Role { Name = "admin" };
                await _roleManager.CreateAsync(adminRole);
                _logger.LogInformation("Created role “admin”.");
            }
            if (!await _userManager.IsInRoleAsync(adminUser, adminRole.Name))
            {
                _logger.LogInformation("Add user “admin” into role “admin”.");
                await _userManager.AddToRoleAsync(adminUser, adminRole.Name);
            }
            var adminPermissions = await _roleManager.GetClaimsAsync(adminRole);
            var permissions = Enum.GetValues(typeof(Resource)).Cast<Resource>().Where(r => !adminPermissions.Any(p => p.Type == $"{AuthorizationPolicyDefinition.RBAC}.{r}")).ToDictionary(x => x, x => Operation.ReadWrite);
            await _roleManager.AddPermissions(adminRole, permissions);
            await _db.SaveChangesAsync();
        }
    }
}
