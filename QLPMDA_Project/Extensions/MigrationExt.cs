using Microsoft.EntityFrameworkCore;
using QLPMDA_Project.Models;

namespace QLPMDA_Project.Extensions
{
    public static class MigrationExt
    {
        public static void MigrateDatabase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<QLPMDAContext>();
            context.Database.Migrate();
        }
    }
}
