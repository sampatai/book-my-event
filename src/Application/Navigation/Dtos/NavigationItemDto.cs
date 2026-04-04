namespace Application.Navigation.Dtos
{
    public class NavigationItemDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? RequiredPermission { get; set; }
        public bool IsAllowed { get; set; } // Based on user permissions
        public List<NavigationItemDto> Items { get; set; } = new();
    }

    public class MenuResponse
    {
        public List<NavigationItemDto> MenuItems { get; set; } = new();

        public UserDetail User { get; set; } = default!;
        public Dictionary<string, bool> AvailableActions { get; set; } = new(); // For button control
    }
    public record UserDetail(string name, string email, string avatar);
}