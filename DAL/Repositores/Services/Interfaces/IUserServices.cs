using DAL.DTO.Req;
using DAL.DTO.Res;

namespace DAL.Repositores.Services.Interfaces
{
    public interface IUserServices
    {
        Task<string> Register(ReqRegisterUserDto register);
        Task<List<ResUserDto>> GetAllUsers();// Di IUserServices
        Task<List<ResUserDto>> GetAllNonAdminUsers();
        Task<ResLoginDto> Login(ReqLoginDto reqLogin);
        Task<String> AddUser(ReqRegisterUserDto addUser);
        Task<ResUserDto> UpdateUser(string id, ReqUpdateUserDto updateDto);
        Task<bool> DeleteUser(string id);
        Task<ResGetUserById> GetUserById(string id);
        //Task<ResEditSaldo> EditSaldo(string id, ReqEditSaldo editSaldo);
    }
}
