using AuthPlaypen.Api.Data;
using Microsoft.EntityFrameworkCore;
using AuthPlaypen.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AuthPlaypenDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IScopeService, ScopeService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthPlaypenDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
