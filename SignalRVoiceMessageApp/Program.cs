using SignalRVoiceMessageApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add SignalR
builder.Services.AddSignalR();

var app = builder.Build();


app.UseHttpsRedirection();

app.MapControllers();
app.UseRouting();
app.UseCors(policy =>
    policy.AllowAnyOrigin()  // Allowing any origin for development
        .AllowAnyMethod()
        .AllowAnyHeader());

// Map SignalR Hub to a route
app.MapHub<VoiceMessageHub>("/hub");

app.Run();