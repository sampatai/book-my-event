using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace Domain.Navigation.Root
{
    public class NavigationItem : Entity, IAggregateRoot
    {
        public string Title { get; private set; }
        public string Url { get; private set; }
        public string? Icon { get; private set; }

        // The link to our Identity RoleClaim Value
        public string? RequiredPermission { get; private set; }

        public long? ParentId { get; private set; }

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

        public void AddChild(string title, string url, string? requiredPermission = null, string? icon = null)
        {
            // Business Rule: A menu item can only be nested 2 levels deep
            if (ParentId.HasValue)
                throw new InvalidOperationException("Cannot nest items more than two levels deep.");

            var child = new NavigationItem(title, url, requiredPermission, icon);
            _children.Add(child);
        }

        public void RemoveChild(long childId)
        {
            var child = _children.FirstOrDefault(c => c.Id == childId);
            if (child != null)
            {
                _children.Remove(child);
            }
        }

        public bool RequiresPermission(string permission)
        {
            return !string.IsNullOrEmpty(RequiredPermission) && RequiredPermission == permission;
        }
    }
}
