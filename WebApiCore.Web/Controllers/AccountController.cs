using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApiCore.DataTransferObject;
using WebApiCore.Web.Helper;

namespace WebApiCore.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly JWTSettings _options;

        public AccountController(IMediator mediator, IOptions<JWTSettings> optionsAccessor)
        {
            _mediator = mediator;
            _options = optionsAccessor.Value;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Credentials credentials)
        {
            if (ModelState.IsValid)
            {
                var command = new ApplicationAPI.APIs.Authentication.RegisterApi.Command()
                {
                    UserName = credentials.Username,
                    Email = credentials.Email,
                    Password = credentials.Password
                };

                var result = await _mediator.Send(command);

                return Ok(result);

            }

            var error = string.Join(",", ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList());

            return BadRequest(error);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(ApplicationAPI.APIs.Authentication.LoginApi.Query query)
        {
            var result = await _mediator.Send(query);

            if (result.IsSuccessful)
            {
                var credential = new Credentials()
                {
                    Id = result.Id,
                    Email = result.Email,
                    Username = query.UserName
                };

                return new JsonResult(new Dictionary<string, object>
                  {
                    { "access_token", JWTHelper.GetAccessToken(_options, credential) },
                    { "id_token", JWTHelper.GetIdToken(_options, credential) }
                  });
            }

            return Ok(string.Join(",",result.Messages));
        }
    }
}