using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_One_to_Many
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; } // сотрудники компании
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }  // компания пользователя
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public ApplicationContext()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=relationsdb;Trusted_Connection=True;");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                // создание и добавление моделей
                Company microsoft = new Company { Name = "Microsoft" };
                Company google = new Company { Name = "Google" };
                db.Companies.Add(microsoft);
                db.Companies.Add(google);
                db.SaveChanges();

                User tom = new User { Name = "Tom", Company = microsoft };
                User bob = new User { Name = "Bob", Company = microsoft };
                User alice = new User { Name = "Alice", Company = google };
                db.Users.AddRange(tom, bob, alice);
                db.SaveChanges();

                // вывод пользователей
                var users = db.Users.Include(u => u.Company).ToList();
                foreach (User user in users)
                    Console.WriteLine($"{user.Name} - {user.Company?.Name}");

                // вывод компаний
                var companies = db.Companies.Include(c => c.Users).ToList();
                foreach (Company comp in companies)
                {
                    Console.WriteLine($"\n Компания: {comp.Name}");
                    foreach (User user in comp.Users)
                    {
                        Console.WriteLine($"{user.Name}");
                    }
                }

                // Редактирование
                User user1 = db.Users.FirstOrDefault(p => p.Name == "Tom");
                if (user1 != null)
                {
                    user1.Name = "Tomek";
                    db.SaveChanges();
                }

                Company FirstComp = db.Companies.FirstOrDefault(p => p.Name == "Google");
                if (FirstComp != null)
                {
                    FirstComp.Name = "Alphabet";
                    db.SaveChanges();
                }

                // смена компании сотрудника
                User user2 = db.Users.FirstOrDefault(p => p.Name == "Bob");
                if (user2 != null && FirstComp != null)
                {
                    user2.Company = FirstComp;
                    db.SaveChanges();
                }

                // вывод пользователей
                var usersAfterEdit = db.Users.Include(u => u.Company).ToList();
                foreach (User user in usersAfterEdit)
                    Console.WriteLine($"{user.Name} - {user.Company?.Name}");

                // вывод компаний
                var companiesAfterEdit = db.Companies.Include(c => c.Users).ToList();
                foreach (Company comp in companiesAfterEdit)
                {
                    Console.WriteLine($"\n Компания: {comp.Name}");
                    foreach (User user in comp.Users)
                    {
                        Console.WriteLine($"{user.Name}");
                    }
                }

                ////Удаление
                //User user1 = db.Users.FirstOrDefault(p => p.Name == "Bob");
                //if (user1 != null)
                //{
                //    db.Users.Remove(user1);
                //    db.SaveChanges();
                //}

                //Company comp = db.Companies.FirstOrDefault();
                //if (comp != null)
                //{
                //    db.Companies.Remove(comp);
                //    db.SaveChanges();
                //}
            }
        }
    }
}
