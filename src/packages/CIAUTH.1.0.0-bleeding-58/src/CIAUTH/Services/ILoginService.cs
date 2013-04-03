using CIAPI.DTO;

namespace CIAUTH.Code
{
    public interface ILoginService
    {
        ApiChangePasswordResponseDTO ChangePassword(string username, string password, string newPassword);
        ApiLogOnResponseDTO Login(string username, string password);
        bool Logout(string username, string session);
    }
}