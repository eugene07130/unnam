// 어셈블리 호출
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Microsoft.ML.OnnxRuntime;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using UnnamHS_App_Backend.Repositories;
using UnnamHS_App_Backend.Data;
using UnnamHS_App_Backend.Services;
using UnnamHS_App_Backend.Models;
using UnnamHS_App_Backend.Interceptors;

// ──────────────────────────────────────────────────────────────
var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// JWT 키 검증
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
    throw new Exception("Jwt:Key is missing in appsettings.json!");

// AuthN/AuthZ
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer   = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        RoleClaimType   = "role",
        ClockSkew       = TimeSpan.FromMinutes(1)
    };
});
builder.Services.AddAuthorization();

// DI
builder.Services.AddScoped<IAuthService,          AuthService>();
builder.Services.AddScoped<IRegistrationService,  RegistrationService>();
builder.Services.AddScoped<IVerifyStudentService, VerifyStudentService>();
builder.Services.AddScoped<IPointsService, PointsService>();
builder.Services.AddScoped<IBorrowService, BorrowService>();
builder.Services.AddScoped<IBookmarkService, BookmarkService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IJwtTokenFactory, JwtTokenFactory>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPointHistoryRepository, PointHistoryRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();


// --- SQLite 경로/연결문자열: AddDbContext보다 "먼저" 선언 ---
var contentRoot = builder.Environment.ContentRootPath;
var dbDir  = Path.Combine(contentRoot, "AppData");
Directory.CreateDirectory(dbDir);
var dbPath = Path.Combine(dbDir, "app.db");
var connString = $"Data Source={dbPath}";

// FK 인터셉터 등록 → 컨텍스트에 주입
builder.Services.AddScoped<DbConnectionInterceptor, SqliteFkInterceptor>();

// DbContext: ★ 이 블록 "한 번만" 존재해야 함
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options
        .UseSqlite(connString, o =>
            o.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
        .AddInterceptors(sp.GetRequiredService<DbConnectionInterceptor>());
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "UnnamHS API",
        Version     = "v1",
        Description = "UnnamHS Application Backend API",
    });

    var xmlFile = $"{builder.Environment.ApplicationName}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        opt.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    opt.DocInclusionPredicate((docName, apiDesc) =>
        apiDesc.GroupName == null || apiDesc.GroupName == docName);
    opt.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"]! });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name        = "Authorization",
        In          = ParameterLocation.Header,
        Type        = SecuritySchemeType.ApiKey,
        Scheme      = "Bearer",
        Description = "Bearer {token} 형식으로 입력"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ONNX
builder.Services.AddSingleton(new InferenceSession(
    builder.Configuration["OnnxModel:Path"]
    ?? throw new InvalidOperationException("OnnxModel:Path is not configured.")));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// ──────────────────────────────────────────────────────────────
var app = builder.Build();

app.UseCors("AllowAll");
app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        opt.SwaggerEndpoint("/swagger/v1/swagger.json", "UnnamHS API v1");
        // opt.RoutePrefix = string.Empty;
    });
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
