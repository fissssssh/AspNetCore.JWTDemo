using AspNetCore.JWTDemo.Configurations;
using AspNetCore.JWTDemo.EntityFrameworkCore;
using AspNetCore.JWTDemo.EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    // 添加一个安全方案
    opts.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
    });
    // 使用Swashbuckle.AspNetCore.Filters包中SecurityRequirementsOperationFilter为所有注有[Authorize]特性的Action添加安全方案
    opts.OperationFilter<SecurityRequirementsOperationFilter>(true, JwtBearerDefaults.AuthenticationScheme);// OperationFilter<T>(params object[] arguments)的参数会传递给T类型的构造函数，SecurityRequirementsOperationFilter的第二个参数必须与添加的安全方案的名称相同
});
builder.Services.AddDbContextPool<JWTDemoDbContext>(b => b.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
// 调用AddIdentityCore而不是AddIdentity，因为AddIdentity会添加Cookie身份认证
builder.Services.AddIdentityCore<User>(opts =>
{
    // 密码强度配置
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireDigit = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequiredLength = 6;
}).AddRoles<Role>().AddSignInManager().AddEntityFrameworkStores<JWTDemoDbContext>().AddDefaultTokenProviders();
// 添加J身份认证服务，身份认证中间件可以解析JWT Token并给HttpContext.User赋值
var jwtSettings = new JwtBearerSettings();
var jwtSettingsSection = builder.Configuration.GetSection("JwtBearer");
jwtSettingsSection.Bind(jwtSettings);
builder.Services.Configure<JwtBearerSettings>(jwtSettingsSection);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
{
    opts.IncludeErrorDetails = true;
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerSigningKey)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.FromSeconds(30),
    };
});
// 添加授权服务
builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
