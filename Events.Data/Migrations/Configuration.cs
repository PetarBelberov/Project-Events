namespace Events.Data.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Events.Data.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
        }

        //  This method will be called after migrating to the latest version.
        protected override void Seed(Events.Data.ApplicationDbContext context)
        {

            if (!context.Roles.Any())
            {
                this.CreateRole(context, "Admin");
                this.CreateRole(context, "User");

               
            }

            //  Set initial data only if database is empty
            if (!context.Users.Any())
            {
                this.CreateAdminUser(context, "admin@admin.com", "Admin", "System Administrator", "admin");
                this.SetRoleToUser(context, "admin@admin.com", "Admin");
            }
        }

        private void SetRoleToUser(ApplicationDbContext context, string email, string role)
        {
            var userManagaer = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            var user = context.Users.Where(u => u.Email == email).First();

            var result = userManagaer.AddToRole(user.Id, role);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors));
            }

        }

        private void CreateRole(ApplicationDbContext context, string roleName)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            var result = roleManager.Create(new IdentityRole(roleName));

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors));
            }
        }

        private void CreateSeveralEvents(ApplicationDbContext context)
        {

            context.Events.Add(new Event()
            {
                Title = "Party @ NBU",
                StartDateTime = DateTime.Now.Date.AddDays(5).AddHours(21).AddMinutes(30),
                Author = context.Users.First()
            });

            context.Events.Add(new Event()
            {
                Title = "Upcoming Event",
                StartDateTime = DateTime.Now.Date.AddDays(+5).AddHours(21).AddMinutes(30),
                Author = context.Users.First()
            });

            context.Events.Add(new Event()
            {
                Title = "Party @ NBU",
                StartDateTime = DateTime.Now.Date.AddDays(5).AddHours(21).AddMinutes(30),
                Author = context.Users.First()
            });

            context.Events.Add(new Event()
            {
                Title = "Passed Event <Anonymous>",
                StartDateTime = DateTime.Now.Date.AddDays(-2).AddHours(21).AddMinutes(30),
                Duration = TimeSpan.FromHours(1.5),
                Description = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum"
            });
        }

        private void CreateAdminUser(ApplicationDbContext context, string adminEmail, string adminUserName, string adminFullName, string adminPassword)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 1,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            // Create the "admin" user
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                FullName = adminFullName,
                Email = adminEmail
            };


           // var userStore = new UserStore<ApplicationUser>(context);
            

           

            var result = userManager.Create(admin, adminPassword);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join("; ", result.Errors));
            }
        }
    }
}
