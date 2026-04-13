using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Domain.Navigation.Root;
using Domain.Users.Root;
using Domain.ValueObjects;
using Infrastructure.Database;
using Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using SharedKernel.Model;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Infrastructure
{
    public static class Seeder
    {
        // Add this method to DbSeeder class
        public static async Task SeedOpenIddictClientsAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();
            var _configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var servicesOptions = new ServicesOptions();
            _configuration.GetSection("Services").Bind(servicesOptions);   
            // Seed ServiceEntity
            if (!await db.Users.AnyAsync(cancellationToken: cancellationToken))
            {
                // Example Address creation (adjust as needed)
                // The Address constructor requires a timezone parameter (last argument).
                // Use positional arguments in the constructor in the order: street, state, postalCode, country, city, addressLine1, addressLine2, timeZone
                var address = new Address(
                    "123 Main St",
                    "StateName",
                    "12345",
                    "CountryName",
                    "CityName",
                    "Apt 4B",
                    null,
                    "UTC"
                );

                // Example ServiceEntity creation
              

                // Seed User linked to ServiceEntity
                var user = new User(

                   email: "admin@example.com",
                   phoneNumber: "+1234567890"
                );


                user.EmailConfirmed = true;


                // Optionally set the ServiceEntityId if you have one
                var result = await userManager.CreateAsync(user, "Admin@1234");
                if (result.Succeeded)
                {
                    // Ensure the Administrator role exists
                    const string adminRole = "Administrator";
                    if (!await roleManager.RoleExistsAsync(adminRole))
                    {
                        await roleManager.CreateAsync(new IdentityRole<long>(adminRole));
                    }

                    var role = await roleManager.FindByNameAsync(adminRole);
                    if (role is not null)
                    {
                        var existingRoleClaims = await roleManager.GetClaimsAsync(role);
                        if (!existingRoleClaims.Any(c => c.Type == "permission" && c.Value == "admin"))
                        {
                            await roleManager.AddClaimAsync(role, new Claim("permission", "admin"));
                        }
                    }

                    // Assign user to Administrator role
                    await userManager.AddToRoleAsync(user, adminRole);

                    // Add custom and global claims
                    var claims = new List<Claim>
                    {
                        new Claim("custom_claim", "custom_value"),
                        new Claim("globaluserclaim", "global_value"),

                    };
                    await userManager.AddClaimsAsync(user, claims);
                }

            }
        }

        public static async Task SeedNavigationMenuAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (await db.NavigationItems.AnyAsync(cancellationToken))
                return;

            var navigationItems = new List<NavigationItem>
                {
                    // Main menu items
                    new NavigationItem("Dashboard", "/dashboard", null, "home"),
                    new NavigationItem("Users", "/users", "users.view", "users"),
                    new NavigationItem("Pandit", "/pandit", "pandit.view", "user"),
                    new NavigationItem("Devotee", "/devotee", "devotee.view", "heart"),
                    new NavigationItem("Bookings", "/bookings", "bookings.view", "calendar"),
                };

            // Add sub-menu items
            var usersMenu = navigationItems.First(n => n.Title == "Users");
            usersMenu.AddChild("List Users", "/users/list", "users.list", "list");
            usersMenu.AddChild("Create User", "/users/create", "users.create", "plus");
            usersMenu.AddChild("Edit User", "/users/edit", "users.edit", "pencil");
            usersMenu.AddChild("Delete User", "/users/delete", "users.delete", "trash-2");

            var panditMenu = navigationItems.First(n => n.Title == "Pandit");
            panditMenu.AddChild("List Pandits", "/pandit/list", "pandit.list", "list");
            panditMenu.AddChild("Create Pandit", "/pandit/create", "pandit.create", "plus");
            panditMenu.AddChild("Edit Pandit", "/pandit/edit", "pandit.edit", "pencil");
            panditMenu.AddChild("Delete Pandit", "/pandit/delete", "pandit.delete", "trash-2");

            var devoteeMenu = navigationItems.First(n => n.Title == "Devotee");
            devoteeMenu.AddChild("List Devotees", "/devotee/list", "devotee.list", "list");
            devoteeMenu.AddChild("Create Devotee", "/devotee/create", "devotee.create", "plus");
            devoteeMenu.AddChild("Edit Devotee", "/devotee/edit", "devotee.edit", "pencil");
            devoteeMenu.AddChild("Delete Devotee", "/devotee/delete", "devotee.delete", "trash-2");

            var bookingsMenu = navigationItems.First(n => n.Title == "Bookings");
            bookingsMenu.AddChild("List Bookings", "/bookings/list", "bookings.list", "list");
            bookingsMenu.AddChild("Create Booking", "/bookings/create", "bookings.create", "plus");
            bookingsMenu.AddChild("Edit Booking", "/bookings/edit", "bookings.edit", "pencil");
            bookingsMenu.AddChild("Delete Booking", "/bookings/delete", "bookings.delete", "trash-2");

            db.NavigationItems.AddRange(navigationItems);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}