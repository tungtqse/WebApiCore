﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiCore.DataAccess.UnitOfWork;
using WebApiCore.DataModel.Models;
using System.Linq.Expressions;
using MediatR;

namespace WebApiCore.Web.Controllers
{
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

            return Ok(result);
        }
    }
}