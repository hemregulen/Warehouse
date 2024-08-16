using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;
using System.Text;
using Warehouse.Contact.Model;

var builder = WebApplication.CreateBuilder(args);

// appsettings.json dosyas�ndan ayarlar� y�kleme
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Yap�land�rma ayarlar�n� DI konteynerine ekleme
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/swagger"))
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Swagger\"";
            context.Response.StatusCode = 401;
            return;
        }

        var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
        var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
        var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
        var username = credentials[0];
        var password = credentials[1];

        // appsettings.json'dan kullan�c� ad� ve �ifreyi al
        var configUsername = app.Configuration["SwaggerAuth:Username"];
        var configPassword = app.Configuration["SwaggerAuth:Password"];

        // Kullan�c� ad� ve �ifreyi do�rula
        if (!(username.Equals(configUsername, StringComparison.InvariantCultureIgnoreCase) &&
              password.Equals(configPassword)))
        {
            context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Swagger\"";
            context.Response.StatusCode = 401;
            return;
        }
    }

    await next();
});

// Swagger'� ekle
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();