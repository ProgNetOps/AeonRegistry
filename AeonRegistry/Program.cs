using AeonRegistry.Endpoints.CustomIdentityEndpoints;
using AeonRegistry.Endpoints.Home;
using AeonRegistry.Endpoints.Sites;
using AeonRegistry.Models;
using AeonRegistry.Services;
using AeonRegistry.Services.Interfaces;
using AeonRegistryAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddCustomSwagger();

//get a connection to the database
var connectionString = DataUtility.GetConnectionString(
    builder.Configuration);

//configure the database context for postgresql
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});


#region Add Identity Endpoints
builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
}).AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

#region Add Admin Policy
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
#endregion

#region Enable Validation for Minimal API Models
builder.Services.AddValidation();
#endregion

#endregion

//Email Sender Service
builder.Services.AddTransient<IEmailSender, ConsoleEmailService>();

#region Register Custom Services
builder.Services.AddScoped<ISiteService, SiteService>();

#endregion


var app = builder.Build(); 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Checks if the data is empty or if there are pending migrations and acts as appropriate
using (var scope = app.Services.CreateScope())
{
    await DataSeed.ManageDataAsync(scope.ServiceProvider);
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

//Block certain endpoints scaffolded from Identity and return status code 404
app.UseMiddleware<BlockIdentityEndpoints>();

var authRouteGroup = app.MapGroup("/api/auth")
    .WithTags("Admin");
authRouteGroup.MapIdentityApi<ApplicationUser>();

#region Register Endpoints
app.MapCustomIdentityEndpoints();
app.MapSiteEndpoints();
app.MapHomeEndpoints();

#endregion

app.Run();