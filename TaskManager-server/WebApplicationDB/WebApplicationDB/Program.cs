using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WebApplicationDB.Data;
using WebApplicationDB.Models.Service;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<TaskManagerService, TaskManagerServiceImpl>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => {

    options.AddPolicy("AllowOrigin", builder => {

        builder.WithOrigins("http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("X-Total-Count");
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {

//    app.UseSwagger();
//    app.UseSwaggerUI();
    app.UseCors("AllowAnyOrigin");
}

app.UseHttpsRedirection();
app.UseCors("AllowOrigin");
app.UseAuthorization();

app.MapControllers();

app.Run();
