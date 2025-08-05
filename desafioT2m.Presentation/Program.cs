using desafioT2m.Service;
using desafioT2m.Infraestructure.RabbitMQ;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IRabbitMQProducer, RabbitMQProducer>();
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddSingleton<RabbitMQConnection>();
        builder.Services.AddScoped<RabbitMQProducer>();
        builder.Services.AddHostedService<RabbitMqConsumer>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy => policy
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        var app = builder.Build();
        app.UseCors("AllowFrontend");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
