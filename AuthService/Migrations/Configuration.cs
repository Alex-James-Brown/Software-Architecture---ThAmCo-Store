using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using AuthService.Infrastructure;

internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
{
    public Configuration()
    {
        CommandTimeout = Int32.MaxValue;
        AutomaticMigrationsEnabled = false;
    }

    protected override void Seed(ApplicationDbContext context)
    {
        //  This method will be called after migrating to the latest version.

        var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

        var user = new ApplicationUser()
        {
            UserName = "SuperUser",
            Email = "superuser@ThAmCo.com",
            EmailConfirmed = true,
            FirstName = "Super",
            LastName = "User",
            Level = 1,
            JoinDate = DateTime.Now
        };

        manager.Create(user, "SuperP@ss");

        if (roleManager.Roles.Count() == 0)
        {
            roleManager.Create(new IdentityRole { Name = "SuperAdmin" });
            roleManager.Create(new IdentityRole { Name = "Admin" });
            roleManager.Create(new IdentityRole { Name = "User" });
        }

        var adminUser = manager.FindByName("SuperUser");

        manager.AddToRoles(adminUser.Id, new string[] { "SuperAdmin", "Admin" });
    }
}