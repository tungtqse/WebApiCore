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
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Get(ApplicationAPI.APIs.CategoryAPI.GetDetailApi.Query query)
        {
            var result = await _mediator.Send(query);           

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Search(ApplicationAPI.APIs.CategoryAPI.SearchApi.Query query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationAPI.APIs.CategoryAPI.CreateApi.Command command)
        {            
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ApplicationAPI.APIs.CategoryAPI.UpdateApi.Command command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }
    }
}