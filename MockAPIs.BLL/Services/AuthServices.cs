using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MockAPIs.BLL.DTOs;
using MockAPIs.BLL.Interfaces;
using MockAPIs.DAL.Data;
using MockAPIs.DAL.Models;

namespace MockAPIs.BLL.Services
{
    public class AuthService : IAuthServices
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        // ─── Register ─────────────────────────────────────────────
        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            // 1. Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email!);
            if (existingUser != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var existingByName = await _userManager.FindByNameAsync(dto.UserName!);
            if (existingByName != null)
                throw new InvalidOperationException("A user with this username already exists.");

            // 2. Create the AppUser entity
            var user = new AppUser
            {
                FullName = dto.FullName!,
                UserName = dto.UserName,
                Email = dto.Email,
            };

            // 3. Create user via Identity (handles password hashing)
            var result = await _userManager.CreateAsync(user, dto.Password!);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Registration failed: {errors}");
            }

            // 4. Assign default role
            await _userManager.AddToRoleAsync(user, "User");

            // 5. Generate tokens & return response
            return await BuildAuthResponse(user);
        }

        // ─── Login ────────────────────────────────────────────────
        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            // 1. Find user by username
            var user = await _userManager.FindByNameAsync(dto.UserName!);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password.");

            // 2. Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password!, lockoutOnFailure: false);
            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid username or password.");

            // 3. Generate tokens & return response
            return await BuildAuthResponse(user);
        }

        // ─── Refresh Token ───────────────────────────────────────
        public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            // 1. Find the refresh token in DB
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken == null)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            if (!storedToken.IsActive)
                throw new UnauthorizedAccessException("Refresh token has expired or been revoked.");

            // 2. Revoke the old refresh token (rotation)
            storedToken.RevokedAt = DateTime.UtcNow;

            // 3. Generate new tokens for the same user
            var response = await BuildAuthResponse(storedToken.User);
            await _context.SaveChangesAsync();

            return response;
        }

        // ─── Private Helpers ─────────────────────────────────────

        /// <summary>
        /// Builds the full AuthResponseDTO: JWT + new refresh token + user info.
        /// </summary>
        private async Task<AuthResponseDTO> BuildAuthResponse(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var jwtToken = CreateJwtToken(user, roles);
            var refreshToken = await CreateRefreshToken(user.Id);

            return new AuthResponseDTO
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["JWT:ExpirationInMinutes"] ?? "60")),
                User = new UserInfoDTO
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    Role = roles.FirstOrDefault() ?? "User",
                    CreatedAt = user.CreatedAt,
                }
            };
        }

        /// <summary>
        /// Creates a JWT access token with user claims.
        /// </summary>
        private string CreateJwtToken(AppUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("fullName", user.FullName),
            };

            // Add role claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expirationMinutes = double.Parse(
                _configuration["JWT:ExpirationInMinutes"] ?? "60");

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Creates a random refresh token and stores it in the database.
        /// </summary>
        private async Task<RefreshToken> CreateRefreshToken(Guid userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }
    }
}