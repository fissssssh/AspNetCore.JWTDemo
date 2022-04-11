using AspNetCore.JWTDemo.EntityFrameworkCore;
using AspNetCore.JWTDemo.EntityFrameworkCore.Models;
using AspNetCore.JWTDemo.Migrator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var app = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) =>
    {
        services.AddDbContextPool<JWTDemoDbContext>(b => b.UseSqlServer(context.Configuration.GetConnectionString("Default")));
        // 调用AddIdentityCore而不是AddIdentity，因为AddIdentity会添加Cookie身份认证
        services.AddIdentityCore<User>(opts =>
        {
            // 密码强度配置
            opts.Password.RequireNonAlphanumeric = false;
            opts.Password.RequireDigit = false;
            opts.Password.RequireLowercase = false;
            opts.Password.RequireUppercase = false;
            opts.Password.RequiredLength = 6;
        }).AddRoles<Role>().AddEntityFrameworkStores<JWTDemoDbContext>();
        services.AddScoped<DbInitializer>();
    }).Build();

using var scope = app.Services.CreateScope();
var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
await initializer.InitAsync();

