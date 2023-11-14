using System.Security.AccessControl;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DB.Login;
using DB.Login.Tables;
using RTA.Masters;
using RTAAPI;
using RTA.Masters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RTA.Common.Models;

namespace Rta.Controllers.Masters
{
    [Route("api/master/[controller]")]
    [ApiController]
    public class InwardController : ControllerBase
    {

        private IConfiguration Configurations;
        private string DBConnStr;
        private CommonAuth commonAuth;
        private IActionResult objAction;
        private loginDBContext LoginDB;
        public InwardController(IConfiguration config)
        {
            Configurations = config;
            DBConnStr = RTAAPI.DB.GetDBCred(Configurations);
            commonAuth = new CommonAuth(DBConnStr);
        }

        //GET ALL RECORDS
        [HttpGet("GetInwardRecords")]
        [Produces("application/json", Type = typeof(ModelInwardResp))]
        public IActionResult GetInwardRecords(
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data
        )
        {
            try
            {
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);
                if (commonAuth.VerifyFormRights(modelAuth, 1, "REPORT"))
                {
                    InwardMstBLL Inward = new InwardMstBLL(DBConnStr);
                    ModelInwardResp Res = Inward.GetInwardData();

                    objAction = CreatedAtAction("GetInwardRecords", Res);
                    return objAction;
                }
                throw new Exception("User doesnot have rights");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelInwardResp data = new ModelInwardResp()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("GetInwardRecords", data);
                return objAction;
            }
        }


        //GET data for live search
        [HttpGet("InwardLiveSearch")]
        [Produces("application/json", Type = typeof(ModelInwardResp))]
        public IActionResult InwardLiveSearch(
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data,
            [FromHeader] string Search_String
        )
        {
            try
            {
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);
                if (commonAuth.VerifyFormRights(modelAuth, 1, "REPORT"))
                {
                    InwardMstBLL Inward = new InwardMstBLL(DBConnStr);
                    ModelInwardResp Res = Inward.Search_Data(Search_String);
                    objAction = CreatedAtAction("InwardLiveSearch", Res);
                    return objAction;
                }
                throw new Exception("User doesnot have rights");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelInwardResp data = new ModelInwardResp()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("InwardLiveSearch", data);
                return objAction;
            }
        }

        //GET data for live search
        [HttpGet("InwardFeildset")]
        [Produces("application/json", Type = typeof(ModelInwardResp))]
        public IActionResult InwardFeildset(
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data
        )
        {
            try
            {
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);
                if (commonAuth.VerifyFormRights(modelAuth, 1, "REPORT"))
                {
                    InwardMstBLL Inward = new InwardMstBLL(DBConnStr);
                    ModelInwardResp Res = Inward.Column_name();
                    objAction = CreatedAtAction("InwardFeildset", Res);
                    return objAction;
                }
                throw new Exception("User doesnot have rights");
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
                ModelInwardResp data = new ModelInwardResp()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("InwardFeildset", data);
                return objAction;
            }
        }


        [HttpPost("Insert_Inward_Records")]
        [Produces("application/json", Type = typeof(ModelInwardResp))]
        [Consumes("application/json")]
        public IActionResult Insert_Inward_Records(
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data,
            [FromBody] ModelInwardReq InwardReq
        )
        {
            try
            {
                //attention c_contact_phn no should be unique 
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);
                ModelInwardResp Res = new ModelInwardResp();
                if (commonAuth.VerifyFormRights(modelAuth, 1, "REPORT"))
                {

                    string username = modelAuth.User.user_name;

                    InwardMstBLL um = new InwardMstBLL(DBConnStr);

                    Res = um.InsertSample(InwardReq);
                    objAction = CreatedAtAction("Insert_Inward_Records", Res);
                    return objAction;
                }
                Res = new ModelInwardResp()
                {
                    status = false,
                    Message = modelAuth.message
                };
                objAction = CreatedAtAction("Insert_Inward_Records", Res);
                return objAction;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
                ModelInwardResp data = new ModelInwardResp()
                {
                    status = false,
                    Message = ex.InnerException.Message
                };
                IActionResult objAction = CreatedAtAction("Insert_Inward_Records", data);
                return objAction;
            }

        }


        [HttpPost("Update_Inward_Records")]
        [Produces("application/json", Type = typeof(ModelInwardResp))]
        [Consumes("application/json")]
        public IActionResult Update_Inward_Records(
            [FromHeader] long ID,
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data,
            [FromBody] ModelInwardReq InwardReq
        )
        {
            try
            {
                //attention c_contact_phn no should be unique 
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);
                ModelInwardResp Res = new ModelInwardResp();
                if (commonAuth.VerifyFormRights(modelAuth, 1, "REPORT"))
                {

                    string username = modelAuth.User.user_name;

                    InwardMstBLL um = new InwardMstBLL(DBConnStr);

                    Res = um.UpdateSample(ID, InwardReq);
                    objAction = CreatedAtAction("Update_Inward_Records", Res);
                    return objAction;
                }
                Res = new ModelInwardResp()
                {
                    status = false,
                    Message = modelAuth.message
                };
                objAction = CreatedAtAction("Update_Inward_Records", Res);
                return objAction;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
                ModelInwardResp data = new ModelInwardResp()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("Update_Inward_Records", data);
                return objAction;
            }

        }


    }
}