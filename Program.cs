using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("default")
    .AddCookie("default",
    o =>
    {
        o.Cookie.Name = "mycookie";
        // o.Cookie.Path = "/test";
        // o.Cookie.HttpOnly= false;
        // o.Cookie.SecurePolicy = CookieSecurePolicy.Always; // None : send cookie over http
        // o.Cookie.SameSite = SameSiteMode.Lax;
        // o.Cookie.Expiration = 
        o.ExpireTimeSpan = TimeSpan.FromSeconds(10);
        o.SlidingExpiration = true;
    });

builder.Services.AddControllers();
builder.Services.AddAuthorization(builder =>
{
    builder.AddPolicy("mypolicy", pb => pb.RequireAuthenticatedUser()
    .RequireClaim("abc", "xyz"));
});

var app = builder.Build();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World").RequireAuthorization("mypolicy");

app.MapGet("/test", () => "Hello World").RequireAuthorization();

app.MapGet("/test22", async (HttpContext ctx) =>
{
    await ctx.ChallengeAsync("default",
           new AuthenticationProperties()
           {
               RedirectUri = "/anything "
           });

    return "Ok";
}).RequireAuthorization();



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
               ),
               new AuthenticationProperties()
               {
                   IsPersistent = true,
               });

    return "Ok";
});

app.MapGet("/signout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync("default", 
               new AuthenticationProperties()
               {
                   IsPersistent = true,
               });

    return "Ok";
});

app.MapDefaultControllerRoute();

app.Run();