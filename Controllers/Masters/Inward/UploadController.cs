using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DB.Login;
using DB.Login.Tables;
using RTA.Masters;
using RTAAPI;
using RTA.FileMaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RTA.Common.Models;
using RTA.FileMaster;

namespace Rta.Controllers.Masters.Inward
{
    [Route("api/master/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private IConfiguration Configurations;
        private string DBConnStr;
        private CommonAuth commonAuth;
        private IActionResult objAction;
        private loginDBContext LoginDB;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpClientFactory _httpClientFactory;

        public UploadController(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            Configurations = config;
            _httpClientFactory = httpClientFactory;
            DBConnStr = RTAAPI.DB.GetDBCred(Configurations);
            commonAuth = new CommonAuth(DBConnStr);
        }

        [HttpPost("Inward_Upload")]
        //[Produces("application/json",Type = typeof(ModelInwardFileResp))]
        //[Consumes("application/json")]
        public IActionResult Inward_Upload([FromForm] ModelInwardFileReq fileReq, long Inward_id)
        {
            try
            {
                // var baseUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
                // Console.WriteLine("baseurl "+baseUrl);
                string BaseDir = Configurations["DataFilesBasePath"].ToString();
                //string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploaded-Files");

                ModelInwardFileResp Res = new ModelInwardFileResp();
                InwardFileBLL InFile = new InwardFileBLL(DBConnStr);
                Res = InFile.UploadFile(fileReq, BaseDir, Inward_id);

                objAction = CreatedAtAction("Inward_Upload", Res);
                return objAction;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelInwardFileResp data = new ModelInwardFileResp()
                {
                    status = false,
                    Message = ex.InnerException.Message
                };
                IActionResult objAction = CreatedAtAction("Inward_Upload", data);
                return objAction;
            }

        }



        [HttpGet("Inward_Download")]
        //[Produces("application/json",Type = typeof())]
        [Consumes("application/json")]
        public IActionResult Download([FromHeader] long Inward_Id, long Doc_Id)
        {
            InwardFileBLL InFile = new InwardFileBLL(DBConnStr);

            Images_Store_Master_In_Ward file_data = InFile.GetFilePath(Inward_Id, Doc_Id);

            var stream = System.IO.File.OpenRead(file_data.store_path);

            return File(stream, "application/octet-stream", file_data.store_file_name);

        }


        [HttpGet("Inward_FileUrl")]
        //[Produces("application/json",Type = typeof())]
        [Consumes("application/json")]
        public async Task<IActionResult> FileUrl([FromHeader] long Inward_Id, long Doc_Id)
        {
            InwardFileBLL InFile = new InwardFileBLL(DBConnStr);

            Images_Store_Master_In_Ward file_data = InFile.GetFilePath(Inward_Id, Doc_Id);
            string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
            var file_stream = Directory.GetFiles(file_data.store_path);
            //string filePath = Path.Combine(directoryPath, file_stream.store_file_name);

            // file_stream.CopyTo(directoryPath);

            // using(var stream = new FileStream(filePath, FileMode.Create))
            // {
            //     filePath.CopyTo(file_stream);
            // }

            //var stream = new FileStream(filePath, FileMode.Create);
            //file_data.store_path.CopyTo(stream);
            string url = file_data.store_path;
            var httpClient = _httpClientFactory.CreateClient("PdfDomain");
            var response = await httpClient.GetAsync(url);
            MemoryStream ms = new MemoryStream(await response.Content.ReadAsByteArrayAsync());

            return new FileStreamResult(ms, "application/octet-stream");

        }



    }
}