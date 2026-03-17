using Aplicacion;
using Infraestructura.ContextoBD;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddDbContext<AplicacionBDContexto>();
builder.Services.AddInversionOfControl();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

// NOTE: UseHttpsRedirection is intentionally omitted.
// Azure Container Apps handles SSL termination at the ingress level;
// the container only receives HTTP on port 8080. Adding HTTPS redirection
// here causes an infinite redirect loop.

app.UseAuthorization();

app.MapControllers();

app.Run();
