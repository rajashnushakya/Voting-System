using Microsoft.AspNetCore.Identity;
using VotingAPI.Model;
using VotingAPI.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddIdentity<AppUser, Role>()
    .AddDefaultTokenProviders();
builder.Services.AddTransient<IUserStore<AppUser>, AppUserStore>();
builder.Services.AddTransient<IRoleStore<Role>, AppRoleStore>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
