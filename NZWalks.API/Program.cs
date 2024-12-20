using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Mappings;
using NZWalks.API.Repositories;
using NZWalks.API.WebSocket;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Add Windows Service support
//builder.Host.UseWindowsService();
builder.Host.UseWindowsService()
       .ConfigureLogging(logging =>
       {
           logging.AddEventLog(); // Log to Windows Event Viewer
       });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<NZWalksDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksConnectionString")));
builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:8080") // Change this to specific origins in production
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Allows credentials like cookies or tokens
    });
});

var app = builder.Build();

// Use CORS in the pipeline
app.UseCors();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
