using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication()
    .AddCookie("default",
    o =>
    {
        o.Cookie.Name = "mycookie";
    });

builder.Services.AddControllers();

var app = builder.Build();

app.UseStaticFiles();
app.UseAuthentication();

app.MapGet("/", () => "Hello World");

app.MapPost("/login", async (HttpContext ctx) =>
{
    await ctx.SignInAsync("default", new ClaimsPrincipal(
               new ClaimsIdentity(
                   new Claim[]
                   {
                        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                   },
                   "default"
                   )
               ));

    return "Ok";
});

app.MapDefaultControllerRoute();

app.Run();
