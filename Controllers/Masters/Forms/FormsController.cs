using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DB.Login;
using DB.Login.Tables;
using RTA.Masters;
using RTAAPI;
using RTA.Common;
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
    public class FormsController : ControllerBase
    {
        private IConfiguration Configurations;
        private string DBConnStr;
        private CommonAuth commonAuth;
        private IActionResult objAction;

        private loginDBContext LoginDB;
        public FormsController(IConfiguration config)
        {
            Configurations = config;
            DBConnStr = RTAAPI.DB.GetDBCred(Configurations);
            commonAuth = new CommonAuth(DBConnStr);
        }

          //GET ALL RECORDS
        [HttpGet("GetAll")]
        [Produces("application/json", Type = typeof(ModelFormResp))]
        public IActionResult GetAll(
            [FromHeader] long Token_ID, 
            [FromHeader] string Token_Data
        )
        {
            try
            {
                ModelAuth modelAuth= commonAuth.Login_Auth(Token_ID, Token_Data);
                
                FormMstBLL Form = new FormMstBLL(DBConnStr);
                ModelFormResp Res = Form.GetData();

                objAction = CreatedAtAction("GetAll", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelFormResp data = new ModelFormResp(){
                    status=false,
                    Message=ex.Message
                };
                objAction = CreatedAtAction("GetAll", data);
                return objAction;
            }
        }


        [HttpGet("GetNavbarData")]
        [Produces("application/json")]
        public IActionResult GetNavbarData(
            [FromHeader] long Token_ID, 
            [FromHeader] string Token_Data
        )
        {
            try
            {
                ModelAuth modelAuth= commonAuth.Login_Auth(Token_ID, Token_Data);
                
                FormMstBLL Form = new FormMstBLL(DBConnStr);
                var Res = Form.GetNavbarData(modelAuth.User.user_code);

                objAction = CreatedAtAction("GetNavbarData", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelFormResp data = new ModelFormResp(){
                    status=false,
                    Message=ex.Message
                };
                objAction = CreatedAtAction("GetNavbarData", data);
                return objAction;
            }
        }



        [HttpPost("CreateForm")]
        [Produces("application/json",Type = typeof(ModelFormResp))]
        [Consumes("application/json")]
        public IActionResult CreateForm(
            [FromHeader] long Token_ID, 
            [FromHeader] string Token_Data,
            [FromBody] ModelFormReq FormReq
        )
        {
            try
            {
                ModelAuth modelAuth= commonAuth.Login_Auth(Token_ID, Token_Data);

                string user_profile = modelAuth.User.user_profile;

                //if(user_profile=="admin")
                //{
                FormMstBLL um = new FormMstBLL(DBConnStr);
                
                ModelFormResp Res = um.CreateData(FormReq);
                objAction = CreatedAtAction("CreateForm", Res);
                return objAction;

                //}
                //throw new Exception ( "Only Admin have the rights");
              
            }
            catch (Exception ex)    
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException.Message);
                ModelFormResp data = new ModelFormResp(){
                    status=false,
                    Message=ex.InnerException.Message 
                };
                objAction = CreatedAtAction("CreateForm", data);
                return objAction;
            }

        }



        
    }
}