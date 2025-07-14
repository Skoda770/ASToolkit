using System;
using ASToolkit.Auth.DAL;
using ASToolkit.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace ASToolkit.AuthTests;

public static class TestDbContextFactory
{
    public static IdentityDbContext CreateDbInMemory()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new IdentityDbContext(options);
        
        return context;
    }

    public static void SeedUsers(this IdentityDbContext context)
    {
        context.Users.Add(new ApplicationUser()
        {
            Id = Guid.NewGuid(),
            UserName = "initial_user",
            Email = "initial_user@test.com"
        });
        context.SaveChanges();
    }
}