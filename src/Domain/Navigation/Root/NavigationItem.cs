using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Navigation.Root
{
    public class NavigationItem : Entity, IAggregateRoot
    {
        public string Title { get; private set; }
        public string Url { get; private set; }
        public string? Icon { get; private set; }

        // The link to our Identity RoleClaim Value
        public string? RequiredPermission { get; private set; }

        public int? ParentId { get; private set; }

        // Use a backing field for encapsulation
        private readonly List<NavigationItem> _children = new();
        public IReadOnlyCollection<NavigationItem> Children => _children.AsReadOnly();

        // Required for EF Core
        protected NavigationItem() { }

        public NavigationItem(string title, string url, string? requiredPermission = null, string? icon = null)
        {
            UpdateDetails(title, url, requiredPermission, icon);
        }

        public void UpdateDetails(string title, string url, string? requiredPermission, string? icon)
        {
            Guard.Against.NullOrEmpty(title, nameof(title));
            Title = title;
            Url = url;
            RequiredPermission = requiredPermission;
            Icon = icon;
        }

        public void AddChild(string title, string url, string? requiredPermission = null)
        {
            // Business Rule: A menu item can only be nested 2 levels deep

            if (ParentId.HasValue)
                throw new InvalidOperationException("Cannot nest items more than two levels deep.");

            var child = new NavigationItem(title, url, requiredPermission);
            _children.Add(child);
        }
    }
}
