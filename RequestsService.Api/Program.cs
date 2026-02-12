using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using RequestsService.Application.Common.Interfaces;
using RequestsService.Domain.Repositories;
using RequestsService.Infrastructure.Messaging;
using RequestsService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

#region 1. CONFIGURACIÓN DE SERVICIOS (DI Container)

// ===== SWAGGER + API VERSIONING =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "RequestsService API", Version = "v1" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.EnableAnnotations();
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ===== CLEAN ARCHITECTURE DI (Application + Infrastructure) =====
builder.Services.AddMediatR(cfg =>
{
    // Ensamblado de Application, donde están los handlers
    cfg.RegisterServicesFromAssembly(
        typeof(RequestsService.Application.Features.Solicitudes.Create.CreateSolicitudHandler).Assembly
    );
});

// InMemory repo con Singleton 
builder.Services.AddSingleton<ISolicitudRepository, InMemorySolicitudRepository>();

// Publisher de eventos a Azure Queue Storage
builder.Services.AddScoped<IRequestCreatedPublisher, AzureQueueStoragePublisher>();

// ===== VALIDATION (FluentValidation) =====
builder.Services.AddValidatorsFromAssembly(
    typeof(RequestsService.Application.Features.Solicitudes.Create.CreateSolicitudValidator).Assembly
);
builder.Services.AddFluentValidationAutoValidation();

// ===== CORS POLICIES =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("RequestsService.Cors.Development", policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)   // Dev: permite cualquier origen
            .AllowAnyMethod()
            .AllowAnyHeader();
    });

    options.AddPolicy("RequestsService.Cors.Production", policy =>
    {
        policy
            .WithOrigins("https://mi-frontend.com", "https://otro-dominio.com")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ===== Enable  Controllers =====
builder.Services.AddControllers();

#endregion

var app = builder.Build();

#region 2. PIPELINE ESTÁNDAR WEB API CONTROLLERS

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Manejo de errores y seguridad extra en producción
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// CORS según entorno
app.UseCors(app.Environment.IsDevelopment()
    ? "RequestsService.Cors.Development"
    : "RequestsService.Cors.Production");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
