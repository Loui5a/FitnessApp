
using FitnessApp.Components;
using FitnessApp.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;

namespace FitnessApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FitnessContext>(options =>
                options.UseNpgsql("Host=db;Username=lmb;Password=YVFt4huiVSI8ja;Database=FitnessappDB")
            );

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddDataGridEntityFrameworkAdapter();
            builder.Services.AddFluentUIComponents(options =>
            {
                options.ValidateClassNames = false;
            });
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<FitnessContext>();
                 db.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly)
                .AddInteractiveServerRenderMode();
            
            // Update Database to newest version

            app.Run();
        }
    }
}
