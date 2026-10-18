using PlanningPoker.Api.Hubs;
using PlanningPoker.Api.Realtime;
using PlanningPoker.Application;
using PlanningPoker.Core.Interfaces;
using PlanningPoker.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// API
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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
