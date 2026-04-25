using MockAPIs.BLL.DTOs;

namespace MockAPIs.BLL.Interfaces
{
    public interface IAuthServices
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto);
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);
        Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken);
    }
}