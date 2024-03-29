

using WeatherWebApi.DTOs;
using WeatherWebApi.Responses;

namespace WeatherWebApi.Interfaces
{
    public interface IAccountRepository
    {
        Task<ApiResponse<string>> Register(RegisterDTO registerDTO);
        Task<ApiResponse<string>> Login(LoginDTO loginDTO);
    }
}
