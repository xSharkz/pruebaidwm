using Bogus;
using Microsoft.EntityFrameworkCore;
using pruebaidwm.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pruebaidwm.Data
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;

        public DataSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedDataAsync()
        {
            if (await _context.Users.AnyAsync()) return;

            var faker = new Faker<User>()
                .RuleFor(u => u.Rut, f => f.Random.Replace("########-#"))
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Gender, f => f.PickRandom(new[] { "masculino", "femenino", "otro", "prefiero no decirlo" }))
                .RuleFor(u => u.BirthDate, f => f.Date.Past(30, DateTime.Today.AddYears(-18)));

            var users = faker.Generate(10);

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();
        }
    }
}