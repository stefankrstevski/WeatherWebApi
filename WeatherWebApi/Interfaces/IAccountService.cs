using WeatherWebApi.DTOs;
using WeatherWebApi.Responses;

namespace WeatherWebApi.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<string>> Login(LoginDTO loginDTO);
        Task<ApiResponse<string>> Register(RegisterDTO registerDTO);
    }
}