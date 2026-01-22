using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LaStoreAPI.Data;
using LaStoreAPI.Entities;
using LaStoreAPI.DTOs;

namespace LaStoreAPI.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _Hasher = new();

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task Register(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new Exception("User with this email already exists.");

            var user = new User
            {
                Email = request.Email,
                Name = request.Name
            };

            user.PasswordHash = _Hasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> Login(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email)
                ?? throw new Exception("Invalid email or password.");

            var result = _Hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Invalid email or password.");

            return user;
        }
    }
}
