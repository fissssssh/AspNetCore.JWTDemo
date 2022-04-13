using AspNetCore.JWTDemo.Configurations;
using AspNetCore.JWTDemo.EntityFrameworkCore;
using AspNetCore.JWTDemo.EntityFrameworkCore.Models;
using AspNetCore.JWTDemo.Permissions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    // ���һ����ȫ����
    opts.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
    });
    // ʹ��Swashbuckle.AspNetCore.Filters����SecurityRequirementsOperationFilterΪ����ע��[Authorize]���Ե�Action��Ӱ�ȫ����
    opts.OperationFilter<SecurityRequirementsOperationFilter>(true, JwtBearerDefaults.AuthenticationScheme);// OperationFilter<T>(params object[] arguments)�Ĳ����ᴫ�ݸ�T���͵Ĺ��캯����SecurityRequirementsOperationFilter�ĵڶ���������������ӵİ�ȫ������������ͬ
});
builder.Services.AddDbContextPool<JWTDemoDbContext>(b => b.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
// ����AddIdentityCore������AddIdentity����ΪAddIdentity�����Cookie�����֤
builder.Services.AddIdentityCore<User>(opts =>
{
    // ����ǿ������
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireDigit = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequiredLength = 6;
}).AddRoles<Role>().AddSignInManager().AddEntityFrameworkStores<JWTDemoDbContext>().AddDefaultTokenProviders();
// ���Jwt�����֤���������֤�м�����Խ���JWT Token����HttpContext.User��ֵ
var jwtSettings = new JwtBearerSettings();
var jwtSettingsSection = builder.Configuration.GetSection("JwtBearer");
jwtSettingsSection.Bind(jwtSettings);
builder.Services.Configure<JwtBearerSettings>(jwtSettingsSection);
builder.Services.AddAuthentication().AddJwtBearer(opts =>
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
        ClockSkew = TimeSpan.Zero,
    };
});
// �����Ȩ����
// ע����Ȩ�����ṩ����
builder.Services.AddSingleton<IAuthorizationPolicyProvider, RBACPolicyProvider>();
// ע����Ȩ�������
builder.Services.AddScoped<IAuthorizationHandler, OwnerOnlyAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, RBACAuthorizationHandler>();
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
