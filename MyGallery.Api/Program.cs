using Microsoft.EntityFrameworkCore;
using MyGallery.Data.Repositories;
using MyGallery.Services;
using MyGallery.Services.Interfaces;
using MyGallery.Data;
using System.Text.Json.Serialization;
using MyGallery.Application.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bağlantı dizesini ayarlayın
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddDbContext<MyGalleryContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 40))));

// Repository ve Service bağımlılıklarını ekleyin
builder.Services.AddScoped<IProjectsRepository, ProjectsRepository>();
builder.Services.AddScoped<IProjectsService, ProjectsService>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IMinioFileService, MinioFileService>();
builder.Services.AddScoped<IContantMeRepository, ContantMeRepository>();
builder.Services.AddScoped<IContantMeService, ContantMeService>();

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
             .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Geliştirme ortamında herhangi bir origin'e izin verebilirsiniz.
    });
});

// builder.Services.AddControllers().AddJsonOptions(options =>
// {
//     options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(System.IO.File.ReadAllText("wwwroot/index.html"));
    });
});


app.UseHttpsRedirection();
// API'yi yapılandırın
app.UseCors();
// Middleware ayarları
app.UseAuthorization();
app.MapControllers();
app.Run();

