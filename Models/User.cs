using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pruebaidwm.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Rut { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public DateTime BirthDate { get; set; }
    }
}