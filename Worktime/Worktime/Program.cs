using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Worktime.Models;
using Worktime.Services;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json.Serialization;
using Worktime;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var policyName = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policyName,
                      builder =>
                      {
                          builder
                            .AllowAnyOrigin().AllowAnyMethod()// Add the methods you need
                            .AllowAnyHeader();
                      });
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    // Adding Jwt Bearer  
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
            TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
//              context.Token = context.Request.Cookies["jcp-Worktime-Access-Token"];
//              context.Token = context.Request.Cookies["b2n-Worktime-Access-Token"];
                context.Token = context.Request.Cookies["jar-Worktime-Access-Token"];

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddDbContext<WorktimeDbContext>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
builder.Services.AddSession();

var app = builder.Build();

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors(policyName);
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();