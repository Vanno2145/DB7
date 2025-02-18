using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PerfumeStoreLibrary
{
    public class PerfumeStoreContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=PerfumeStoreDB;Trusted_Connection=True;");
        }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PurchasePrice { get; set; }
        public int RetailPrice { get; set; }
        public int Quantity { get; set; }
        public string Manufacturer { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }

    public class Delivery
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PaymentType { get; set; }
        public string Status { get; set; }
        public string Details { get; set; }
        public DateTime? DispatchDate { get; set; }
        public DateTime? ReceiptDate { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string PaymentType { get; set; }
        public string Status { get; set; }
        public string Details { get; set; }
        public DateTime? DispatchDate { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T Get(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        private PerfumeStoreContext context;
        private DbSet<T> dbSet;

        public Repository(PerfumeStoreContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        public IEnumerable<T> GetAll() => dbSet.ToList();
        public T Get(int id) => dbSet.Find(id);
        public void Add(T entity)
        {
            dbSet.Add(entity);
            context.SaveChanges();
        }
        public void Update(T entity)
        {
            dbSet.Update(entity);
            context.SaveChanges();
        }
        public void Delete(int id)
        {
            var entity = dbSet.Find(id);
            if (entity != null)
            {
                dbSet.Remove(entity);
                context.SaveChanges();
            }
        }
    }

    public class PerfumeStoreTest
    {
        public static void RunTests()
        {
            using var context = new PerfumeStoreContext();
            var categoryRepo = new Repository<Category>(context);
            var productRepo = new Repository<Product>(context);

            var category = new Category { Name = "Цветочные", Description = "Парфюмерия на основе цветов" };
            categoryRepo.Add(category);

            var product = new Product
            {
                Name = "Розовый аромат",
                Description = "Нежный аромат розы",
                PurchasePrice = 10,
                RetailPrice = 25,
                Quantity = 100,
                Manufacturer = "ПарфюмКО",
                ExpirationDate = DateTime.Now.AddYears(1),
                CategoryId = category.Id
            };
            productRepo.Add(product);

            var products = productRepo.GetAll();
            foreach (var p in products)
            {
                Console.WriteLine($"Товар: {p.Name}, Цена: {p.RetailPrice}");
            }
        }
    }
}