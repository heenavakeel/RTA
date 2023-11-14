using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DB.Login;
using DB.Login.Tables;
using RTA.Common;
using RTAAPI;
using RTA.Common.Models;
using RTA.Masters.Models;
using DB.RTAComp.Tables;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace Rta.Controllers.Files
{
    [Route("api/master/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private IConfiguration Configurations;
        private string DBConnStr;
        private CommonAuth commonAuth;
        private IActionResult objAction;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private loginDBContext LoginDB;

        public UploadController(IConfiguration config, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            Configurations = config;
            DBConnStr = RTAAPI.DB.GetDBCred(Configurations);
            commonAuth = new CommonAuth(DBConnStr);
        }

        [HttpPost("Common_Upload")]
        //[Produces("application/json",Type = typeof(ModelInwardFileResp))]
        //[Consumes("application/json")]
        public IActionResult Common_Upload(
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data,
            [FromForm] ModelFileReq fileReq)
        {
            try
            {
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);

                CommonMstBLL cm = new CommonMstBLL(DBConnStr);
                var Comp_DB_Cred = cm.GetComp_DB_ConnStr(fileReq.company_id);

                long user_id = modelAuth.User.id;
                string BaseDir = Configurations["DataFilesBasePath"].ToString();

                ModelFileResp Res = new ModelFileResp();
                FilesMstBLL InFile = new FilesMstBLL(DBConnStr, Comp_DB_Cred);
                Res = InFile.UploadFile(fileReq, user_id, BaseDir);

                objAction = CreatedAtAction("Common_Upload", Res);
                return objAction;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelFileResp data = new ModelFileResp()
                {
                    status = false,
                    Message = ex.InnerException.Message
                };
                IActionResult objAction = CreatedAtAction("Common_Upload", data);
                return objAction;
            }

        }

        [HttpGet("Common_Download")]
        //[Produces("application/json",Type = typeof())]
        [Consumes("application/json")]
        public IActionResult Common_Download(
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data,
            [FromHeader] long Comp_id,
            [FromHeader] long Doc_Id,
            [FromHeader] string FilterType
            )
        {
            try
            {
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);
                ModelFileResp Res = new ModelFileResp();
                if (commonAuth.VerifyFormRights(modelAuth, 1, "REPORT"))
                {

                    CommonMstBLL cm = new CommonMstBLL(DBConnStr);
                    var Comp_DB_Cred = cm.GetComp_DB_ConnStr(Comp_id);

                    FilesMstBLL InFile = new FilesMstBLL(DBConnStr, Comp_DB_Cred);

                    ModelFileResp Fd = InFile.GetFilePath(Doc_Id, FilterType);

                    if (Fd.fileData.form_name == "benpos" && FilterType != "all")
                    {
                        Console.WriteLine("check 1");
                        var result = WriteCsvToMemory(Fd.benposData);
                        var memoryStream = new MemoryStream(result);
                        return File(memoryStream, "text/csv", "EXPORT.csv");
                    }

                    var stream = System.IO.File.OpenRead(Fd.fileData.file_path);
                    return File(stream, "application/octet-stream", Fd.fileData.file_name);

                }
                Res = new ModelFileResp()
                {
                    status = false,
                    Message = modelAuth.message
                };
                IActionResult objAction = CreatedAtAction("Common_Download", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelFileResp data = new ModelFileResp()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("Common_Download", data);
                return objAction;
            }


        }

        private byte[] WriteCsvToMemory(List<ModelBenposReader> records)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }


        [HttpGet("CSVReader")]
        //[Produces("application/json",Type = typeof())]
        [Consumes("application/json")]
        public IActionResult CSVReader(
            [FromHeader] long Comp_id,
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data,
            [FromHeader] long Doc_Id)
        {
            try
            {
                //ModelAuth modelAuth= commonAuth.Login_Auth(Token_ID, Token_Data);

                CommonMstBLL cm = new CommonMstBLL(DBConnStr);
                var Comp_DB_Cred = cm.GetComp_DB_ConnStr(Comp_id);

                FilesMstBLL FileMst = new FilesMstBLL(DBConnStr, Comp_DB_Cred);

                List<ModelBenposReader> benpos = FileMst.CSVFileReader("F:/juni_docs/DETAIL-HPL_217-03-02-2023.csv", "All");

                var result = WriteCsvToMemory(benpos);
                var memoryStream = new MemoryStream(result);
                return File(memoryStream, "text/csv", "EXPORT.csv");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelFileResp data = new ModelFileResp()
                {
                    status = false,
                    Message = ex.InnerException.Message
                };
                IActionResult objAction = CreatedAtAction("CSVReader", data);
                return objAction;
            }


        }



        [HttpPost("UploadFile")]
        public IActionResult Upload(List<IFormFile> files)
        {
            if (files.Count == 0)
            {
                throw new Exception("NO FILES SELECTED");
            }

            string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploaded-Files");

            foreach (var file in files)
            {
                //string FileName = file.Name + "_" +DateTime.Now.ToString();
                string filePath = Path.Combine(directoryPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                Console.WriteLine("FileName : " + file.FileName);

            }
            Console.WriteLine("DIRECTORY : " + directoryPath);


            return Ok("Upload Successful");

        }



        //GET TRX DATE RECORDS
        [HttpGet("GetTrxDate")]
        [Produces("application/json", Type = typeof(ModelFileResp))]
        public IActionResult GetTrxDate(
            [FromHeader] long Comp_id,
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data
        )
        {
            try
            {
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);
                ModelFileResp Res = new ModelFileResp();
                if (commonAuth.VerifyFormRights(modelAuth, 1, "REPORT"))
                {

                    CommonMstBLL cm = new CommonMstBLL(DBConnStr);
                    var Comp_DB_Cred = cm.GetComp_DB_ConnStr(Comp_id);

                    FilesMstBLL fm = new FilesMstBLL(DBConnStr, Comp_DB_Cred);
                    Res = fm.GetTrxDate();

                    objAction = CreatedAtAction("GetTrxDate", Res);
                    return objAction;

                }
                Res = new ModelFileResp()
                {
                    status = false,
                    Message = modelAuth.message
                };

                objAction = CreatedAtAction("GetTrxDate", Res);
                return objAction;
                throw new Exception("User doesnot have rights");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelFileResp data = new ModelFileResp()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("GetTrxDate", data);
                return objAction;
            }
        }


    }
}