using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DB.Login;
using DB.RTAComp;
using DB.Login.Tables;
using RTA.Masters;
using RTA.Common;
using RTA.Common.Models;
using RTAAPI;
using RTA.Masters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Dapper;
using System.Data;
using static Humanizer.In;

namespace Rta.Controllers.Masters
{
    [Route("api/master/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {

        private IConfiguration Configurations;
        private IActionResult objAction;
        private string DBConnStr;
        private CommonAuth commonAuth;

        private loginDBContext LoginDB;
        public CompanyController(IConfiguration config)
        {
            Configurations = config;
            DBConnStr = RTAAPI.DB.GetDBCred(Configurations);
            commonAuth = new CommonAuth(DBConnStr);
        }

        //GET ALL RECORDS
        [HttpGet("CompLiveSearch")]
        [Produces("application/json", Type = typeof(ModelCompData))]
        public IActionResult CompLiveSearch(
            [FromHeader] string searchString,
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data
        )
        {
            try
            {
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);
                ModelCompData Res = new ModelCompData();
                if (commonAuth.VerifyFormRights(modelAuth, 1, "REPORT"))
                {
                    CompMstBLL Comp = new CompMstBLL(DBConnStr);
                    Res = Comp.CompLiveSearch(searchString);
                    objAction = CreatedAtAction("CompLiveSearch", Res);
                    return objAction;
                }
                Res = new ModelCompData()
                {
                    status = false,
                    Message = modelAuth.message
                };
                objAction = CreatedAtAction("CompLiveSearch", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelCompData data = new ModelCompData()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("CompLiveSearch", data);
                return objAction;
            }
        }

        //GET top 1 RECORDS
        [HttpGet("GetCompRecords")]
        [Produces("application/json", Type = typeof(ModelCompData))]
        public IActionResult GetCompRecords(
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data
        )
        {
            try
            {
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);
                ModelCompData Res = new ModelCompData();
                if (commonAuth.VerifyFormRights(modelAuth, 1, "REPORT"))
                {
                    CompMstBLL Comp = new CompMstBLL(DBConnStr);
                    Res = Comp.GetCompData();
                    objAction = CreatedAtAction("GetCompRecords", Res);
                    return objAction;
                }
                Res = new ModelCompData()
                {
                    status = false,
                    Message = modelAuth.message
                };
                objAction = CreatedAtAction("GetCompRecords", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelCompData data = new ModelCompData()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("GetCompRecords", data);
                return objAction;
            }
        }

        //GET 1 RECORD acc to id
        [HttpGet("SearchCompName")]
        [Produces("application/json", Type = typeof(ModelCompData))]
        public IActionResult SearchCompName(
            [FromHeader] string Company_Name,
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data
        )
        {
            try
            {
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);

                CompMstBLL cm = new CompMstBLL(DBConnStr);
                var Res = cm.SearchCompName(Company_Name);

                IActionResult objAction = CreatedAtAction("SearchCompName", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelCompData data = new ModelCompData()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("SearchCompName", data);
                return objAction;
            }
        }



        //GET 1 RECORD acc to id
        [HttpGet("GetCompDBCreds")]
        [Produces("application/json", Type = typeof(ModelCompData))]
        public IActionResult GetCompDBCreds(
            long id,
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data
        )
        {
            try
            {
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);

                CommonMstBLL cm = new CommonMstBLL(DBConnStr);
                var Res = cm.GetComp_DB_ConnStr(id);

                IActionResult objAction = CreatedAtAction("GetCompDBCreds", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelCompData data = new ModelCompData()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("GetCompDBCreds", data);
                return objAction;
            }
        }


        [HttpPost("CreateComp")]
        [Produces("application/json", Type = typeof(ModelCompData))]
        [Consumes("application/json")]
        public IActionResult CreateComp(
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data,
            [FromBody] ModelGenCompReq CompReq
        )
        {
            try
            {
                //attention c_contact_phn no should be unique 
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);
                ModelCompData Res = new ModelCompData();
                if (commonAuth.VerifyFormRights(modelAuth, 1, "REPORT"))
                {
                    string username = modelAuth.User.user_name;

                    RTA.Masters.CompMstBLL um = new RTA.Masters.CompMstBLL(DBConnStr);

                    Res = um.NewComp(CompReq, RTAAPI.DB.GetNewCompDBCred(Configurations), username);
                    objAction = CreatedAtAction("CreateComp", Res);
                    return objAction;

                }
                Res = new ModelCompData()
                {
                    status = false,
                    Message = modelAuth.message
                };
                objAction = CreatedAtAction("CreateComp", Res);
                return objAction;

                throw new Exception("User doesnot have rights");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
                ModelCompData data = new ModelCompData()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("CreateComp", data);
                return objAction;
            }

        }

        [HttpPost("AddMultiAddress")]
        [Produces("application/json", Type = typeof(ModelMultiAddResp))]
        public IActionResult AddMultiAddress([FromBody] ModelMultiAddReq samp)

        {
            try
            {
                RTA.Masters.CompMstBLL sample = new CompMstBLL(DBConnStr);
                var Res = sample.InsertMultiAddress(samp);
                objAction = CreatedAtAction("AddMultiAddress", Res);
                return objAction;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelMultiAddResp data = new ModelMultiAddResp()
                {
                    status = false,
                    Message = ex.Message
                };
                objAction = CreatedAtAction("AddMultiAddress", data);
                return objAction;
            }
        }

        [HttpGet("GetMultiAddress")]
        [Produces("application/json", Type = typeof(ModelMultiAddResp))]
        public IActionResult GetMultiAddress(
   )
        {
            try
            {
                CompMstBLL sample = new CompMstBLL(DBConnStr);

                var Res = sample.GetMultiAddress();

                objAction = CreatedAtAction("GetMultiAddress", Res);
                return objAction;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelMultiAddResp data = new ModelMultiAddResp()
                {
                    status = false,
                    Message = ex.Message
                };
                objAction = CreatedAtAction("GetMultiAddress", data);
                return objAction;
            }
        }

        [HttpPost("AddMultiContact")]
        [Produces("application/json", Type = typeof(ModelMultiContactReq))]
        public IActionResult AddMultiContact([FromBody] ModelMultiContactReq samp)

        {
            try
            {
                RTA.Masters.CompMstBLL sample = new CompMstBLL(DBConnStr);
                var Res = sample.InsertMultiContact(samp);
                objAction = CreatedAtAction("AddMultiContact", Res);
                return objAction;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelMultiContactResp data = new ModelMultiContactResp()
                {
                    status = false,
                    Message = ex.Message
                };
                objAction = CreatedAtAction("AddMultiContact", data);
                return objAction;
            }
        }

        [HttpGet("GetMultiContact")]
        [Produces("application/json", Type = typeof(ModelMultiContactResp))]
        public IActionResult GetMultiContact(
   )
        {
            try
            {
                CompMstBLL sample = new CompMstBLL(DBConnStr);

                var Res = sample.GetMultiContact();

                objAction = CreatedAtAction("GetMultiContact", Res);
                return objAction;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelMultiContactResp data = new ModelMultiContactResp()
                {
                    status = false,
                    Message = ex.Message
                };
                objAction = CreatedAtAction("GetMultiContact", data);
                return objAction;
            }
        }


        [HttpGet("GetMultiContactByID")]
        [Produces("application/json", Type = typeof(ModelMultiContactResp))]
        public IActionResult GetMultiContactByID(int id

        )
        {
            try
            {
                RTA.Masters.CompMstBLL sample = new CompMstBLL(DBConnStr);
                var Res = sample.GetMultiContactByID(id);
                objAction = CreatedAtAction("GetMultiContactByID", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelMultiContactResp data = new ModelMultiContactResp()
                {
                    status = false,
                    Message = ex.Message
                };
                objAction = CreatedAtAction("GetMultiContactByID", data);
                return objAction;
            }
        }

        [HttpPut("UpdateMultiContact")]
        [Produces("application/json", Type = typeof(ModelMultiContactResp))]
        public IActionResult UpdateMultiContact(int id,
       [FromBody] ModelMultiContactReq samp
       )
        {
            try
            {

                RTA.Masters.CompMstBLL sample = new CompMstBLL(DBConnStr);
                var Res = sample.UpdateMultiContact(id, samp);
                objAction = CreatedAtAction("UpdateMultiContact", Res);
                return objAction;
                //}
                //throw new Exception ( "User doesnot have rights");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelMultiContactResp data = new ModelMultiContactResp()
                {
                    status = false,
                    Message = ex.Message
                };
                objAction = CreatedAtAction("UpdateMultiContact", data);
                return objAction;
            }
        }

        [HttpDelete("DeletMulContact")]
        [Produces("application/json", Type = typeof(ModelMultiContactResp))]
        public IActionResult DeletMulContact(int id
        )
        {
            try
            {
                RTA.Masters.CompMstBLL inwaard = new CompMstBLL(DBConnStr);
                var Res = inwaard.DeletMulContact(id);
                objAction = CreatedAtAction("DeletMulContact", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelMultiContactResp data = new ModelMultiContactResp()
                {
                    status = false,
                    Message = ex.Message
                };
                objAction = CreatedAtAction("DeletMulContact", data);
                return objAction;
            }
        }

        [HttpGet("GetGeneral_Procedure")]
        [Produces("application/json", Type = typeof(ModelMultiContactResp))]

        public IActionResult GetGeneral_Procedure(string tableName, string columnName, string search

       )
        {
            try
            {

                RTA.Masters.CompMstBLL sample = new CompMstBLL(DBConnStr);
                var Res = sample.GetGeneral_Procedure(tableName, columnName, search);
                objAction = CreatedAtAction("GetGeneral_Procedure", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelMultiContactResp data = new ModelMultiContactResp()
                {
                    status = false,
                    Message = ex.Message
                };
                objAction = CreatedAtAction("GetGeneral_Procedure", data);
                return objAction;
            }
        }
    }
}