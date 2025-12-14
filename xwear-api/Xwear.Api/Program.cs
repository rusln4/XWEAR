var builder = WebApplication.CreateBuilder(args);

// Контроллеры
builder.Services.AddControllers();

// База данных
builder.Services.AddDbContext<Xwear.Api.Models.DbExwearContext>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
