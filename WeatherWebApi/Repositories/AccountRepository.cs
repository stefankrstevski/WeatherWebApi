using WeatherWebApi.Data.Domain;
using WeatherWebApi.DTOs;
using WeatherWebApi.Interfaces;
using WeatherWebApi.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WeatherWebApi.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AccountRepository> _logger;


        public AccountRepository(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<JwtSettings> jwtSettings,
            ILogger<AccountRepository> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> Login(LoginDTO loginDTO)
        {
            _logger.LogInformation("Login attempt for {Email}", loginDTO.Email);
            if (loginDTO == null)
            {
                _logger.LogWarning("Login attempt with null loginDTO");
                return new ApiResponse<string>(new List<string> { "Login parameters are null" }, "Login parameters are null");
            }

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed for {Email}: user not found", loginDTO.Email);
                return new ApiResponse<string>(new List<string> { "User not found" }, "User not found");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Invalid password for {Email}", loginDTO.Email);
                return new ApiResponse<string>(new List<string> { "Invalid email or password" }, "Invalid email or password");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateToken(user, roles);
            _logger.LogInformation("Login successful for {Email}", loginDTO.Email);
            return new ApiResponse<string>(token, "Login successful");
        }

        public async Task<ApiResponse<string>> Register(RegisterDTO registerDTO)
        {
            _logger.LogInformation("Registration attempt for {Email}", registerDTO.Email);
            if (registerDTO == null)
            {
                _logger.LogWarning("Registration attempt with null registerDTO");
                return new ApiResponse<string>(new List<string> { "Registration data is missing" }, "Registration data is missing");
            }

            var userExists = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (userExists != null)
            {
                _logger.LogWarning("User {Email} already registered", registerDTO.Email);
                return new ApiResponse<string>(new List<string> { "Registration failed" }, "Registration failed");
            }

            var user = new User
            {
                UserName = registerDTO.Email,
                Email = registerDTO.Email,
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }
                _logger.LogWarning("User registration failed for {Email}", registerDTO.Email);
                return new ApiResponse<string>(errors, "User registration failed");
            }

            await EnsureRoleExists("User");
            await _userManager.AddToRoleAsync(user, "User");
            _logger.LogInformation("User {Email} registered successfully", registerDTO.Email);
            return new ApiResponse<string>("Account created successfully.", "Registration successful");
        }

        private async Task EnsureRoleExists(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private string GenerateToken(User user, IList<string> roles)
        {
            _logger.LogDebug("Generating JWT for {Email}", user.Email);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(_jwtSettings.ExpireHours),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
