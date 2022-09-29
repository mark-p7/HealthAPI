using HealthAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var host = builder.Configuration["DBHOST"] ?? "localhost";
var port = builder.Configuration["DBPORT"] ?? "3333";
var password = builder.Configuration["DBPASSWORD"] ?? "secret";
var db = builder.Configuration["DBNAME"] ?? "HealthDB";

string connectionString = $"server={host}; userid=root; pwd={password};"
        + $"port={port}; database={db};SslMode=none;allowpublickeyretrieval=True;";

var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));

builder.Services.AddDbContext<HealthContext>(options => options.UseMySql(connectionString, serverVersion));
builder.Services.AddControllers().AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// builder.Services.AddDbContext<HealthContext>(options => options.UseSqlServer(connectionString));
// builder.Services.AddControllers().AddNewtonsoftJson(options =>
// options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
// );
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddPolicy("HealthPolicy", builder =>
{
    builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// app.UseAuthentication();

app.UseRouting();
app.UseCors();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<HealthContext>();
    context.Database.Migrate();
}

app.Run();
