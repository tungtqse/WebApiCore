using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiCore.DataAccess.UnitOfWork;
using WebApiCore.DataModel.Models;
using System.Linq.Expressions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace WebApiCore.Web.Controllers
{
    [Authorize]
    //[Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
    //[Authorize("Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Get(string name = "", int skip = 0, int take = 10)
        {
            var query = new ApplicationAPI.APIs.CategoryAPI.SearchApi.Query()
            {
                Name = name,
                Skip = skip,
                Take = take
            };

            var result = await _mediator.Send(query);

            result.SearchResultItems.Add(new ApplicationAPI.APIs.CategoryAPI.SearchApi.NestedModel.CategoryModel()
            {
                Name = "Test"
            });

            return Ok(result);
        }
    }
}