using KargoTakip.Server.Application;
using KargoTakip.Server.Infrastructure;
using KargoTakip.Server.WebAPI;
using KargoTakip.Server.WebAPI.Controllers;
using KargoTakip.Server.WebAPI.Modules;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCompression(opt =>
{
    opt.EnableForHttps = true;
});

builder.AddServiceDefaults();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCors();


builder.Services.AddOpenApi();
builder.Services
    .AddControllers()
    .AddOData(opt =>
        opt
        .Select()
        .Filter()
        .Count()
        .Expand()
        .OrderBy()
        .SetMaxTop(null)
        .AddRouteComponents("odata", AppODataController.GetEdmModel())
    )
    ;
builder.Services.AddRateLimiter(x =>
x.AddFixedWindowLimiter("fixed", cfg =>
{
    cfg.QueueLimit = 100;
    cfg.Window = TimeSpan.FromSeconds(1);
    cfg.PermitLimit = 100;
    cfg.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
}));
builder.Services.AddExceptionHandler<ExceptionHandler>().AddProblemDetails();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseCors(x => x
.AllowAnyHeader()
.AllowCredentials()
.AllowAnyMethod()
.SetIsOriginAllowed(t => true)
.SetPreflightMaxAge(TimeSpan.FromMinutes(10)));

app.RegisterRoutes();

app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCompression();

app.UseExceptionHandler();

app.MapControllers().RequireRateLimiting("fixed").RequireAuthorization();

ExtensionsMiddleware.CreateFirstUser(app);

app.Run();


//using KargoTakip.Server.WebAPI;
//using KargoTakip.Server.WebAPI.Controllers;
//using KargoTakip.Server.WebAPI.Installers;
//using KargoTakip.Server.WebAPI.Modules;
//using Microsoft.AspNetCore.OData;
//using Scalar.AspNetCore;

//var builder = WebApplication.CreateBuilder(args);

//builder.AddServiceDefaults();
//builder.Services.AddHttpContextAccessor();

//builder.Services.AddInternalServices(builder.Configuration);

//builder.Services.AddExternalServices();

////builder.Services.AddCors();
//// Örnek: ASP.NET Core'da CORS ayarlarý
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAllOrigins",
//        builder =>
//        {
//            builder.AllowAnyOrigin()
//                   .AllowAnyHeader()
//                   .AllowAnyMethod();
//        });
//});

//builder.Services
//    .AddControllers()
//    .AddOData(opt =>
//        opt
//        .Select()
//        .Filter()
//        .Count()
//        .Expand()
//        .OrderBy()
//        .SetMaxTop(null)
//        .AddRouteComponents("odata", AppODataController.GetEdmModel())
//    )
//    ;

////builder.Services.AddAuthentication(options =>
////{
////    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
////    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
////}).AddJwtBearer();

//builder.Services.AddAuthorization();

//var app = builder.Build();

//app.MapOpenApi();

//app.MapScalarApiReference();

//app.MapDefaultEndpoints();

//app.AddMiddlewares();

//app.MapControllers().RequireRateLimiting("fixed").RequireAuthorization();

//ExtensionsMiddleware.CreateFirstUser(app);

//app.UseCors(x => x
//.AllowAnyHeader()
//.AllowCredentials()
//.AllowAnyMethod()
//.SetIsOriginAllowed(t => true)
//.SetPreflightMaxAge(TimeSpan.FromMinutes(10)));

//app.RegisterRoutes();

//app.UseAuthentication();
//app.UseAuthorization();

//app.Run();