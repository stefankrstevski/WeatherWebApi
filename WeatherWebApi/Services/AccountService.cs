using WeatherWebApi.DTOs;
using WeatherWebApi.Interfaces;
using WeatherWebApi.Responses;

namespace WeatherWebApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IAccountRepository accountRepository, ILogger<AccountService> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<string>> Login(LoginDTO loginDTO)
        {
            try
            {
                return await _accountRepository.Login(loginDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return new ApiResponse<string>(new List<string> { "Invalid login attempt." }, "Login failed");
            }
        }

        public async Task<ApiResponse<string>> Register(RegisterDTO registerDTO)
        {
            try
            {
               return await _accountRepository.Register(registerDTO);
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return new ApiResponse<string>(new List<string> { "Unable to complete registration." }, "Registration failed");
            }
        }
    }


}
