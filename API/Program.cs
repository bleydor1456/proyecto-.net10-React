using CrudNet10.Data;
using CrudNet10.Helpers;
using Microsoft.AspNetCore.Mvc;
using CrudNet10.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CrudNet10.Services;
using CrudNet10.Services.Interfaces;
using Scalar.AspNetCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
   options.AddPolicy("AllowReactApp", policy =>
   {
       policy.WithOrigins("http://localhost:5173")
             .AllowAnyHeader()
             .AllowAnyMethod();
   });
});

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
        .Where(e => e.Value?.Errors.Count > 0)
        .SelectMany(e => e.Value!.Errors)
        .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
            ? "Error de validación en el campo."
            : e.ErrorMessage)
        .ToList();

        var response = new ValidationErrorResponse
        {
            Success = false,
            Message = "Errores de validación",
            Errors = errors,
            StatusCode = StatusCodes.Status400BadRequest
        };

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        // 1. En .NET 10 ya NO existe la propiedad "Reference" aquí adentro
        var scheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };

        // 2. Inicializamos la colección de componentes
        document.Components ??= new OpenApiComponents();
        
        // 3. Usamos el nuevo método de .NET 10 para inyectar el esquema
        document.AddComponent("Bearer", scheme);

        // 4. La propiedad ahora se llama estrictamente "Security"
        document.Security ??= new List<OpenApiSecurityRequirement>();
        
        // 5. Ahora se EXIGE el uso de OpenApiSecuritySchemeReference para anclarlo
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });

        return Task.CompletedTask;
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=crudnet10.db"));

var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors("AllowReactApp");

await DbInitializer.SeedAdminAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("API REST - Documentación")
                .WithTheme(ScalarTheme.Moon);
    });
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();