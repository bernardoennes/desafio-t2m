using desafio_t2m.Service;
using desafio_t2m.Utils;
using desafio_t2m.Messaging;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddSingleton<RabbitMQConnection>();
        builder.Services.AddScoped<RabbitMQProducer>();
        builder.Services.AddHostedService<RabbitMqConsumer>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

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
