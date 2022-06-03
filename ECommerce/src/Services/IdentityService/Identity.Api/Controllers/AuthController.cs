using Identity.Api.Application.Models;
using Identity.Api.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetUser()
        {
            return Ok("caner");
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginRequestModel requestModel)
        {
            var response = await _identityService.Login(requestModel);

            return Ok(response);
        }
    }
}
