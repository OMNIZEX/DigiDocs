
using Microsoft.EntityFrameworkCore;

namespace DigiDocs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<Models.DigidocsContext>(Digi => Digi.UseSqlServer(builder.Configuration.GetConnectionString("Digidocs")));
            // Add services to the container.
            

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI(
                c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = string.Empty; // Makes Swagger UI available at http://localhost:5000/
                });

            app.MapControllers();

            app.Run();
        }
    }
}
