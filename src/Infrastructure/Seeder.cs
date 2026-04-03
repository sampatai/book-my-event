using System;
using System.Collections.Generic;
using System.Text;
using Domain.Navigation.Root;
using Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class Seeder
    {
        // Add this method to DbSeeder class

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