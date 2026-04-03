# Navigation Data Seeding - Next Steps

## Add Navigation Seed Method to DbSeeder

To complete the Navigation implementation, add this method to `src/Auth/Infrastructure/Data/DbSeeder.cs`:

```csharp
public static async Task SeedNavigationMenuAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
{
    using var scope = serviceProvider.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Only seed if not already seeded
    if (await db.NavigationItems.AnyAsync(cancellationToken))
        return;

    var navigationItems = new List<Domain.Navigation.Root.NavigationItem>
    {
        // Root menu items
        new("Dashboard", "/dashboard", null, "fa-home"),
        new("Users", "/users", "users.view", "fa-users"),
        new("Pandit", "/pandit", "pandit.view", "fa-person"),
        new("Devotee", "/devotee", "devotee.view", "fa-heart"),
        new("Bookings", "/bookings", "bookings.view", "fa-calendar"),
    };

    // Add Users sub-menu
    var usersMenu = navigationItems.First(n => n.Title == "Users");
    usersMenu.AddChild("List Users", "/users/list", "users.list", "fa-list");
    usersMenu.AddChild("Create User", "/users/create", "users.create", "fa-plus");
    usersMenu.AddChild("Edit User", "/users/edit", "users.edit", "fa-edit");
    usersMenu.AddChild("Delete User", "/users/delete", "users.delete", "fa-trash");

    // Add Pandit sub-menu
    var panditMenu = navigationItems.First(n => n.Title == "Pandit");
    panditMenu.AddChild("List Pandits", "/pandit/list", "pandit.list", "fa-list");
    panditMenu.AddChild("Create Pandit", "/pandit/create", "pandit.create", "fa-plus");
    panditMenu.AddChild("Edit Pandit", "/pandit/edit", "pandit.edit", "fa-edit");
    panditMenu.AddChild("Delete Pandit", "/pandit/delete", "pandit.delete", "fa-trash");

    // Add Devotee sub-menu
    var devoteeMenu = navigationItems.First(n => n.Title == "Devotee");
    devoteeMenu.AddChild("List Devotees", "/devotee/list", "devotee.list", "fa-list");
    devoteeMenu.AddChild("Create Devotee", "/devotee/create", "devotee.create", "fa-plus");
    devoteeMenu.AddChild("Edit Devotee", "/devotee/edit", "devotee.edit", "fa-edit");
    devoteeMenu.AddChild("Delete Devotee", "/devotee/delete", "devotee.delete", "fa-trash");

    // Add Bookings sub-menu
    var bookingsMenu = navigationItems.First(n => n.Title == "Bookings");
    bookingsMenu.AddChild("List Bookings", "/bookings/list", "bookings.list", "fa-list");
    bookingsMenu.AddChild("Create Booking", "/bookings/create", "bookings.create", "fa-plus");
    bookingsMenu.AddChild("Edit Booking", "/bookings/edit", "bookings.edit", "fa-edit");
    bookingsMenu.AddChild("Delete Booking", "/bookings/delete", "bookings.delete", "fa-trash");

    db.NavigationItems.AddRange(navigationItems);
    await db.SaveChangesAsync(cancellationToken);
}
```

## Enable the Seeder in Program.cs

Once the method is added to DbSeeder, uncomment this section in `src/Web.Api/Program.cs`:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithOAuth(app.Configuration);
    app.UseDeveloperExceptionPage();

    // Uncomment to seed navigation data on startup
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await DbSeeder.SeedNavigationMenuAsync(services, CancellationToken.None);
    }
}
```

## Verify Installation Checklist

- [ ] Navigation repository interface (`INavigationRepository`) exists
- [ ] Navigation repository implementation registered in DependencyInjection
- [ ] DbContext includes `DbSet<NavigationItem> NavigationItems`
- [ ] Database migration created for NavigationItems table
- [ ] DbSeeder method `SeedNavigationMenuAsync()` is implemented
- [ ] All 6 Navigation endpoints are working:
  - [ ] `POST /api/navigation` (Create)
  - [ ] `PUT /api/navigation/{id}` (Update)
  - [ ] `DELETE /api/navigation/{id}` (Delete)
  - [ ] `GET /api/navigation/{id}` (GetById)
  - [ ] `GET /api/navigation` (List)
  - [ ] `GET /api/navigation/user/menu` (GetUserMenu)

## API Documentation

### Permissions Required

Each endpoint requires specific permissions:

| Endpoint | Permission | Method |
|----------|-----------|--------|
| Create Navigation | `navigation.create` | POST |
| Update Navigation | `navigation.edit` | PUT |
| Delete Navigation | `navigation.delete` | DELETE |
| Get Navigation | `navigation.list` | GET |
| Get User Menu | Authenticated user | GET |

### Sample Request/Response

#### Get User Navigation Menu
```http
GET /api/navigation/user/menu HTTP/1.1
Authorization: Bearer {access_token}
```

**Response:**
```json
{
  "menuItems": [
    {
      "id": 1,
      "title": "Dashboard",
      "url": "/dashboard",
      "icon": "fa-home",
      "requiredPermission": null,
      "isAllowed": true,
      "children": []
    },
    {
      "id": 2,
      "title": "Users",
      "url": "/users",
      "icon": "fa-users",
      "requiredPermission": "users.view",
      "isAllowed": true,
      "children": [
        {
          "id": 6,
          "title": "List Users",
          "url": "/users/list",
          "icon": "fa-list",
          "requiredPermission": "users.list",
          "isAllowed": true,
          "children": []
        }
        // ... more children
      ]
    }
    // ... more items
  ],
  "availableActions": {
    "canView": true,
    "canCreate": true,
    "canEdit": true,
    "canDelete": true,
    "canList": true
  }
}
```

## Testing Commands

### Using PowerShell with curl

```powershell
# Get the menu (requires authentication)
$headers = @{
    "Authorization" = "Bearer YOUR_TOKEN_HERE"
    "Content-Type" = "application/json"
}

curl -Headers $headers "https://localhost:5001/api/navigation/user/menu"

# Create navigation item
$body = @{
    title = "Settings"
    url = "/settings"
    icon = "fa-cog"
    requiredPermission = "settings.view"
} | ConvertTo-Json

curl -Method POST -Headers $headers -Body $body "https://localhost:5001/api/navigation"
```

---

**All Navigation handlers are now error-free and ready for testing!** ✅
