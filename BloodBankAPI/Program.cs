
using Amazon.DynamoDBv2;
using BloodBank;
using BloodBank.Interfaces;
using BloodBank.Repositories;

namespace BloodBankAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();

            // Register configuration for AWS DynamoDB
            builder.Services.AddSingleton<IAmazonDynamoDB>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                return DynamoDbClientHelper.CreateClient(configuration);
            });

            // Register your repository
            builder.Services.AddScoped<IDonationCenterRepository, DonationCenterRepository>();


            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
}
