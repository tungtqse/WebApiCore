using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiCore.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] ApplicationAPI.APIs.Authentication.RegisterApi.Command command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(ApplicationAPI.APIs.Authentication.LoginApi.Query query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}