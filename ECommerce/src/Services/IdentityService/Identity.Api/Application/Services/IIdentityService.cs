using Identity.Api.Application.Models;

namespace Identity.Api.Application.Services
{
    public interface IIdentityService
    {
        Task<LoginResponseModel> Login(LoginRequestModel requestModel);
    }
}
