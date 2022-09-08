using Microsoft.AspNetCore.Mvc.Formatters;
using AdventureWorksNS.Data;
using static System.Console;
using AdventureWorksAPI.Repositories;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace AdventureWorksAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //builder.WebHost.UseUrls();

            // Agregar el contexto de la base de datos de AdventureWorks
            builder.Services.AdventureWorksDBContext();

            // Add services to the container.
            // Agregar y visualizar los formatos soportados
            builder.Services.AddControllers(options =>
                {
                    WriteLine("Formatos por omision:");
                    foreach (IOutputFormatter formatter in options.OutputFormatters)
                    {
                        OutputFormatter? mediaFormatter = formatter as OutputFormatter;
                        if (mediaFormatter == null)
                        {
                            WriteLine($" {formatter.GetType().Name}");
                        }
                        else
                        {
                            WriteLine($" {mediaFormatter.GetType().Name}, " +
                                $"Media types: {string.Join(", ", mediaFormatter.SupportedMediaTypes)}");
                        }
                    }
                })
                .AddXmlDataContractSerializerFormatters()
                .AddXmlSerializerFormatters()
            .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();

            /// agregar cors-origins 
            builder.Services.AddCors();


            builder.Services.AddHealthChecks()                .AddDbContextCheck<AdventureWorksDB>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();



            app.UseCors(configurePolicy: Options =>
            {
                Options.WithMethods("GET", "POST", "PUT", "DELETE");                Options.WithOrigins("https://localhost:7260/");

            });



            app.MapControllers();

            app.UseHealthChecks(path: "/estado");

            app.UseMiddleware<SecuityHealter>();

            app.Run();
        }
    }
}