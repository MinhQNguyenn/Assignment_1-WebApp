using Assignment_1_API.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;

namespace Assignment_1_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //var conf = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json")
            //    .Build();
            // Add services to the container.

            ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<Category>("Categories");
            modelBuilder.EntitySet<Product>("Products");
            modelBuilder.EntitySet<Staff>("Staffs");
            modelBuilder.EntitySet<Order>("Orders");
            modelBuilder.EntitySet<OrderDetail>("OrderDetails");

            //confige Odata
            builder.Services.AddControllers().AddOData(option =>
                option.Filter().Select().OrderBy().Count().Expand().SetMaxTop(100).
                AddRouteComponents("odata", modelBuilder.GetEdmModel()));


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<MyStoreContext>();// (opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MyStore")));
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
