using System.Collections.Generic;
using System.Linq;

namespace WebApiTestHarness.GraphQL
{
    public class Query
    {
        public List<User> GetUsers(int limit = 10) => new List<User>
        {
            new User { Id = "user-1", Name = "John Doe", Email = "john@example.com" }
        }.Take(limit).ToList();

        public List<Post> GetPosts(string userId) => new List<Post>
        {
            new Post { Id = "post-1", Title = "Hello World", Content = "My first post", UserId = userId }
        };

        public List<Item> GetItems(ItemFilter? filter = null) => new List<Item>
        {
            new Item { Id = "item-1", Name = "Gadget", Status = "active" }
        }.Where(i => filter == null || i.Status == filter.Status).ToList();
    }

    public class User { public string Id { get; set; } = ""; public string Name { get; set; } = ""; public string Email { get; set; } = ""; }
    public class Post { public string Id { get; set; } = ""; public string Title { get; set; } = ""; public string Content { get; set; } = ""; public string UserId { get; set; } = ""; }
    public class Item { public string Id { get; set; } = ""; public string Name { get; set; } = ""; public string Status { get; set; } = ""; }
    public class ItemFilter { public string? Status { get; set; } }
}
