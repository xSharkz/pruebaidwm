using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pruebaidwm.Data;
using pruebaidwm.Models;
using pruebaidwm.Interfaces;

namespace pruebaidwm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // POST /api/user
        [HttpPost]
        public async Task<IActionResult> CreateUser(User user)
        {
            if (await _userRepository.UserExistsByRut(user.Rut))
            {
                return Conflict("El RUT ya existe.");
            }

            bool hasFormatError = false;

            if (string.IsNullOrWhiteSpace(user.Name) || user.Name.Length < 3 || user.Name.Length > 100)
            {
                hasFormatError = true;
            }

            if (!IsValidEmail(user.Email))
            {
                hasFormatError = true;
            }

            var validGenders = new[] { "masculino", "femenino", "otro", "prefiero no decirlo" };
            if (!validGenders.Contains(user.Gender.ToLower()))
            {
                hasFormatError = true;
            }

            if (user.BirthDate >= DateTime.Now)
            {
                hasFormatError = true;
            }

            if (hasFormatError)
            {
                return BadRequest("Alguna validación no fue cumplida.");
            }

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new 
            {
                Message = "Usuario creado exitosamente.",
                User = user
            });
        }

        // GET /api/user
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] string sort = null, [FromQuery] string gender = null)
        {

            var validGenders = new[] { "masculino", "femenino", "otro", "prefiero no decirlo" };

            if (gender != null && !validGenders.Contains(gender.ToLower()))
            {
                return BadRequest("Algún filtro es inválido.");
            }

            if (sort != null && sort.ToLower() != "asc" && sort.ToLower() != "desc")
            {
                return BadRequest("Algún filtro es inválido.");
            }

            var users = await _userRepository.GetUsersAsync();

            if (!string.IsNullOrEmpty(gender))
            {
                users = users.Where(u => u.Gender.ToLower() == gender.ToLower());
            }

            if (!string.IsNullOrEmpty(sort))
            {
                users = sort.ToLower() == "asc" ? users.OrderBy(u => u.Name) : users.OrderByDescending(u => u.Name);
            }

            return Ok(new 
            {
                Message = "Usuarios obtenidos exitosamente.",
                Users = users.ToList()
            });
        }

        // GET /api/user/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // PUT /api/user/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User user)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();
            return Ok(user);
        }

        // DELETE /api/user/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            await _userRepository.DeleteUserAsync(id);
            await _userRepository.SaveChangesAsync();

            return Ok("Usuario eliminado exitosamente.");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mail = new System.Net.Mail.MailAddress(email);
                return mail.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}