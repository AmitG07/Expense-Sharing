using BusinessObjectLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

public static class DataSeeder
{
    public static void SeedData(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Users.Any())
            {
                var users = new List<User>
                {
                    new User
                    {
                        Name = "admin1",
                        EmailId = "admin1@expense.com",
                        Password = "Admin",
                        AvailableBalance = 50000,
                        IsAdmin = true
                    },
                    new User
                    {
                        Name = "Aman",
                        EmailId = "aman@expense.com",
                        Password = "Aman",
                        AvailableBalance = 50000,
                        IsAdmin = false
                    },
                    new User
                    {
                        Name = "Amit",
                        EmailId = "amit@expense.com",
                        Password = "Amit",
                        AvailableBalance = 50000,
                        IsAdmin = false
                    },
                    new User
                    {
                        Name = "Akash",
                        EmailId = "akash@expense.com",
                        Password = "Akash",
                        AvailableBalance = 50000,
                        IsAdmin = false
                    },
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }
    }
}
