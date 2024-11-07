using inmobiliaria_AT.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration["ConnectionStrings:DefaultConnection"], 
                     ServerVersion.AutoDetect(builder.Configuration["ConnectionStrings:DefaultConnection"]))
           .EnableSensitiveDataLogging());

builder.Services.AddScoped<IPropietarioService, PropietarioService>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully.");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            if (context.AuthenticateFailure != null) 
            {
                context.Response.StatusCode = 401; 
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new { error = "Token no válido o expirado." });
                return context.Response.WriteAsync(result);
            }
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
			{
				var accessToken = context.Request.Query["access_token"];
				var path = context.HttpContext.Request.Path;
				if (!string.IsNullOrEmpty(accessToken) &&
					(path.StartsWithSegments("/api/auth/reset") ||
					path.StartsWithSegments("/api/auth/token")||
                    path.StartsWithSegments("/api/auth/email")))
				{
					context.Token = accessToken;
				}
				return Task.CompletedTask;
			}
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["TokenAuthentication:Issuer"],
        ValidAudience = builder.Configuration["TokenAuthentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenAuthentication:SecretKey"]))
        ,RequireExpirationTime = true 
    };
    
});



builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inmobiliaria API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// habilitar Swagger 
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inmobiliaria API V1");
    c.RoutePrefix = string.Empty; // swagger 
});


app.UseRouting();
app.UseCors("AllowAllOrigins");
//app.UseHttpsRedirection();
app.UseAuthentication(); 
app.UseAuthorization(); 

app.Urls.Add("http://192.168.100.18:5181");
app.Urls.Add("https://192.168.100.18:5180");

app.MapControllers(); 

app.Run();
