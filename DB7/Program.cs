using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        using (AppDbContext db = new AppDbContext())
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var user1 = new User { Name = "John", Email = "john@example.com" };
            var user2 = new User { Name = "Jane", Email = "jane@example.com" };
            var user3 = new User { Name = "Alice", Email = "alice@example.com" };

            var userSettings1 = new UserSettings { UserId = 1, Language = "English", Theme = "Dark" };
            var userSettings2 = new UserSettings { UserId = 2, Language = "Spanish", Theme = "Light" };
            var userSettings3 = new UserSettings { UserId = 3, Language = "French", Theme = "Dark" };

            user1.UserSettings = userSettings1;
            user2.UserSettings = userSettings2;
            user3.UserSettings = userSettings3;

            db.Users.AddRange(user1, user2, user3);
            db.UserSettings.AddRange(userSettings1, userSettings2, userSettings3);
            db.SaveChanges();

            Console.WriteLine("Users and their settings:");
            foreach (var user in db.Users.Include(u => u.UserSettings).ToList())
            {
                Console.WriteLine($"User: {user.Name}, Email: {user.Email}, Language: {user.UserSettings.Language}, Theme: {user.UserSettings.Theme}");
            }

            var userWithSettings = db.Users.Include(u => u.UserSettings).FirstOrDefault(u => u.Id == 2);
            if (userWithSettings != null)
            {
                Console.WriteLine($"\nUser with Id = 2: {userWithSettings.Name}, Settings: Language - {userWithSettings.UserSettings.Language}, Theme - {userWithSettings.UserSettings.Theme}");
            }

            var userToDelete = db.Users.Include(u => u.UserSettings).FirstOrDefault(u => u.Id == 3);
            if (userToDelete != null)
            {
                db.Users.Remove(userToDelete);
                db.SaveChanges();
                Console.WriteLine("\nUser with Id = 3 and their settings have been deleted.");
            }
        }
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserSettings UserSettings { get; set; }
}

public class UserSettings
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Language { get; set; }
    public string Theme { get; set; }
    public User User { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=testdb;Trusted_Connection=True;");
    }
}
