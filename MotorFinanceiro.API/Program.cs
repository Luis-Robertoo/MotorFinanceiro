using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseRequestLocalization(new RequestLocalizationOptions
{
DefaultRequestCulture = new RequestCulture(new CultureInfo("pt-BR")),
SupportedCultures = new List<CultureInfo>
    {
        new CultureInfo("pt-BR")
    },
SupportedUICultures = new List<CultureInfo>
    {
        new CultureInfo("pt-BR")
    }
});



// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Console.WriteLine(app.Environment.ToString());

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();

app.MapControllers();

app.Run();
