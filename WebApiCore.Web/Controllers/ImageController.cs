using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiCore.Utility;

namespace WebApiCore.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ImageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Upload()
        {
            var result = new DataTransferObject.ErrorDetails();

            try
            {
                var fileCount = Request.Form.Files.Count;
                byte[] fileArray;

                if (fileCount == 0)
                {
                    result.Code = 500;
                    result.State = "Internal Server Error";
                    result.Messages.Add("Please select an file.");
                }
                else
                {
                    var file = Request.Form.Files[0];
                    
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        fileArray = memoryStream.ToArray();
                    }

                    var extension = FileHelper.GetExtension(fileArray);

                    if(extension == ".jpg" || extension == ".png")
                    {
                        var command = new ApplicationAPI.APIs.Images.UploadImageApi.Command()
                        {
                            FileName = file.FileName,
                            FileType = file.ContentType,
                            FileData = fileArray,
                            FileExtension = extension,
                            //FileSize = int.Parse(file.Length)
                        };

                        await _mediator.Send(command);

                        result.Code = 200;
                        result.IsSuccessful = true;
                        result.State = "Success";
                        result.Messages.Add("File Uploaded");
                    }
                    else
                    {
                        result.Code = 500;
                        result.State = "Internal Server Error";
                        result.Messages.Add("Only accept Image file (.jpg or .png)");                       
                    }
                    
                }
            }
            catch(Exception ex)
            {
                result.Code = 500;
                result.State = "Internal Server Error";
                result.Messages.Add(ex.Message);             
            }

            return Ok(result);
        }
    }
}