using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DB.Login;
using DB.RTAComp;
using DB.Login.Tables;
using DB.RTAComp.Tables;
using RTA.Masters;
using RTA.Common;
using RTA.Common.Models;
using RTAAPI;
using RTA.Masters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rta.Controllers.Masters
{
    [Route("api/master/[controller]")]
    [ApiController]
    public class CDSLController : ControllerBase
    {
        private IConfiguration Configurations;
        private string DBConnStr;
        private CommonAuth commonAuth;

        private loginDBContext LoginDB;
        public CDSLController(IConfiguration config)
        {
            Configurations = config;
            DBConnStr = RTAAPI.DB.GetDBCred(Configurations);
            commonAuth = new CommonAuth(DBConnStr);
        }

        //GET ALL RECORDS
        [HttpGet("GetCDSLRecords")]
        [Produces("application/json", Type = typeof(ModelCdslDataResp))]
        public IActionResult GetCDSLRecords(
            [FromHeader] long Comp_id, 
            [FromHeader] long Token_ID, 
            [FromHeader] string Token_Data
        )
        {
            try
            {
                ModelAuth modelAuth= commonAuth.Login_Auth(Token_ID, Token_Data);

                CommonMstBLL cm = new CommonMstBLL(DBConnStr);
                var Comp_DB_Cred = cm.GetComp_DB_ConnStr(Comp_id);

                CDSLMstBLL cdsl = new CDSLMstBLL(Comp_DB_Cred);
                var Res = cdsl.GetCDSLData();

                IActionResult objAction = CreatedAtAction("GetCDSLRecords", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelCdslDataResp data = new ModelCdslDataResp(){
                    status=false,
                    Message=ex.Message
                };
                IActionResult objAction = CreatedAtAction("GetCDSLRecords", data);
                return objAction;
            }
        }

        
    }
}