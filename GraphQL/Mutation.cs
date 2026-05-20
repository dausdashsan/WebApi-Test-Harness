using System.Collections.Generic;

namespace WebApiTestHarness.GraphQL
{
    public class Mutation
    {
        public User CreateUser(UserInput input) => new User { Id = "user-new", Name = input.Name, Email = input.Email };
        public User UpdateUser(string id, UserInput input) => new User { Id = id, Name = input.Name, Email = input.Email };
        public bool DeleteUser(string id) => true;
        public List<Item> BatchCreate(List<ItemInput> items) => new List<Item>();
    }

    public class UserInput { public string Name { get; set; } = ""; public string Email { get; set; } = ""; }
    public class ItemInput { public string Name { get; set; } = ""; public string Status { get; set; } = ""; }
}
