using System.Text;
using AutoMapper;
using CDF_Core.Models.Auth;
using CDF_WebApi.Middlewares;
using CDF_Core.Models.Auth;
using CDF_Services.SignalrHub;
using CDF_Infrastructure.Persistence.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CDF_Services.Mapping;
using Swashbuckle.AspNetCore.SwaggerUI;
using CDF_Services.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("V1", new OpenApiInfo
    {
        Version = builder.Configuration["app:version"],
        Title = builder.Configuration["app:name"],
        Description = builder.Configuration["app:desc"]
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
            new string[] { }
        }
        });
    c.EnableAnnotations();
});

builder.Services.AddTransient<GlobalExeptionHandlingMiddleware>();

builder.Services.ImplementPersistence(builder.Configuration);


builder.Services.AddIdentity<ApplicationUser, ApplicationRoles>().AddEntityFrameworkStores<ApplicationDBContext>();
builder.Services.AddDbContext<ApplicationDBContext>(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString("myConnection")));

//builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//                .AddJwtBearer(o =>
//                {
//                    o.RequireHttpsMetadata = false;
//                    o.SaveToken = false;
//                    o.TokenValidationParameters = new TokenValidationParameters
//                    {
//                        ValidateIssuerSigningKey = true,
//                        ValidateIssuer = true,
//                        ValidateAudience = true,
//                        ValidateLifetime = true,
//                        ValidIssuer = builder.Configuration["JWT:Issuer"],
//                        ValidAudience = builder.Configuration["JWT:Audience"],
//                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
//                    };
//                });
//.AddFacebook(options =>
//{
//    options.AppId = builder.Configuration["Auth:Facebook:AppId"];
//    options.AppSecret = builder.Configuration["Auth:Facebook:AppSecret"];
//}).AddInstagram(options =>
//{
//    options.ClientId = builder.Configuration["Auth:Instagram:ClientId"];
//    options.ClientSecret = builder.Configuration["Auth:Instagram:ClientSecret"];
//}).AddSnapchat(options =>
//{
//    options.ClientId = builder.Configuration["Auth:Snapchat:ClientId"];
//    options.ClientSecret = builder.Configuration["Auth:Snapchat:ClientSecret"];
//}).AddGoogle(googleOptions =>
//{
//    googleOptions.ClientId = builder.Configuration["Auth:Gmail:client_id"];
//    googleOptions.ClientSecret = builder.Configuration["Auth:Gmail:client_secret"];
//});

// Auto Mapper Configurations

var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddCors(option => option.AddPolicy("CorsPolicy", build =>
{
    build.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(origin => true).AllowCredentials(); ;
}));

builder.Services.AddHttpClient();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    //options.RoutePrefix = string.Empty;
    options.SwaggerEndpoint("/swagger/V1/swagger.json", builder.Configuration["app:name"] + ": " + app.Environment.EnvironmentName);
    options.DocExpansion(DocExpansion.None);//This will not expand all the API's.
});

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
    {
        context.Request.Path = "/index.html";
        await next();
    }
});

app.UseDefaultFiles();
app.UseStaticFiles();


app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<MessageHub>("/responses");
    //endpoints.MapHub<AdminRequestsHub>("/NumberOfAdminRequestsNotification");
    //endpoints.MapHub<OrganizerRequestsHub>("/NumberOfOrganizerRequestsNotification");
    //endpoints.MapHub<SupplierRequestsHub>("/NumberOfSupplierRequestsNotification");
    endpoints.MapHub<MessageHub>("/RequestCurrentLocationToUser");
});
app.UseMiddleware<GlobalExeptionHandlingMiddleware>();

app.MapControllers();

app.Run();
