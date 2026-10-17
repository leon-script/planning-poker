using Microsoft.EntityFrameworkCore;
using PlanningPoker.Api.Hubs;
using PlanningPoker.Api.Realtime;
using PlanningPoker.Application.Interfaces;
using PlanningPoker.Application.Services;
using PlanningPoker.Core.Interfaces;
using PlanningPoker.Infrastructure.Persistence;
using PlanningPoker.Infrastructure.Stores;

var builder = WebApplication.CreateBuilder(args);

// API
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database
var connectionString = builder.Configuration.GetConnectionString("PlanningPoker")
    ?? throw new InvalidOperationException("Connection string 'PlanningPoker' not found.");

builder.Services.AddDbContext<PlanningPokerDbContext>(
    options => options.UseSqlServer(connectionString));

// Application services
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<IEstimationService, EstimationService>();
builder.Services.AddScoped<IVoteService, VoteService>();

// Infrastructure stores
builder.Services.AddScoped<IRoomStore, RoomStore>();
builder.Services.AddScoped<IParticipantStore, ParticipantStore>();
builder.Services.AddScoped<IEstimationStore, EstimationStore>();
builder.Services.AddScoped<IVoteStore, VoteStore>();

// SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<IRoomEventPublisher, SignalRRoomEventPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.MapHub<RoomHub>("/hubs/rooms");
app.Run();
