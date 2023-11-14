//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DB.Login;
using DB.Login.Tables;
using RTA.Admin;
using RTA.Admin.Models;
using RTAAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RTA.Common.Models;

namespace Rta.Controllers.Admin
{
    //[Authorize]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class User_DetailController : ControllerBase
    {
        IConfiguration Configurations;
        string DBConnStr;
        private loginDBContext LoginDB;
        private CommonAuth commonAuth;
        private IActionResult objAction;

        public User_DetailController(IConfiguration config)
        {
            Configurations = config;
            DBConnStr = RTAAPI.DB.GetDBCred(Configurations);
            commonAuth = new CommonAuth(DBConnStr);
        }

        //GET ALL RECORDS
        [HttpGet("GetUsers")]
        [Produces("application/json", Type = typeof(List<User_Detail>))]
        public IActionResult GetTblRecords()
        {
            try
            {
                RTA.Admin.UsersMstBLL um = new RTA.Admin.UsersMstBLL(DBConnStr);
                var Res = um.GetUsers();
                IActionResult objAction = CreatedAtAction("GetTblRecords", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error : " + ex.Message);
            }
        }


        //Login
        [HttpPost("Login")]
        [Produces("application/json", Type = typeof(ModelLoginResp))]
        [Consumes("application/json")]
        public IActionResult Login([FromBody] ModelLoginReq LoginReq)
        {
            try
            {
                RTA.Admin.UsersMstBLL um = new RTA.Admin.UsersMstBLL(DBConnStr);
                var Res = um.Login(LoginReq);
                IActionResult objAction = CreatedAtAction("Login", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                ModelLoginResp Res = new ModelLoginResp()
                {
                    status = false,
                    ErrorMessage = ex.Message
                };
                IActionResult objAction = CreatedAtAction("Login", Res);
                return objAction;
            }
        }


        [HttpPost("Save_User")]
        [Produces("application/json", Type = typeof(ModelUserResp))]
        [Consumes("application/json")]
        public IActionResult Save_User([FromBody] ModelUserReq User_Detail)
        {
            try
            {
                RTA.Admin.UsersMstBLL um = new RTA.Admin.UsersMstBLL(DBConnStr);
                um.New_User(User_Detail);
                IActionResult objAction = CreatedAtAction("Save_User", "success");
                return objAction;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelUserResp data = new ModelUserResp()
                {
                    status = false,
                    Message = ex.InnerException.Message
                };
                IActionResult objAction = CreatedAtAction("Save_User", data);
                return objAction;
            }

        }

        [HttpGet("GetUserById")]
        [Produces("application/json", Type = typeof(ModelUserResp))]
        [Consumes("application/json")]
        public IActionResult GetUserById(
            [FromQuery] long Id,
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data
            )
        {
            try
            {
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);
                ModelUserResp Res = new ModelUserResp();

                if (commonAuth.VerifyFormRights(modelAuth, 1, "REPORT"))
                {

                    RTA.Admin.UsersMstBLL um = new RTA.Admin.UsersMstBLL(DBConnStr);
                    Res = um.GetUserById(Id);
                    objAction = CreatedAtAction("GetUserById", Res);
                    return objAction;
                }
                Res = new ModelUserResp()
                {
                    status = false,
                    Message = modelAuth.message
                };
                objAction = CreatedAtAction("GetUserById", Res);
                return objAction;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelUserResp data = new ModelUserResp()
                {
                    status = false,
                    Message = ex.InnerException.Message
                };
                IActionResult objAction = CreatedAtAction("GetUserById", data);
                return objAction;
            }

        }

        [HttpPost("Edit_User")]
        [Produces("application/json", Type = typeof(ModelUserResp))]
        [Consumes("application/json")]
        public IActionResult Edit_User(
            [FromHeader] long Id,
            [FromHeader] long Token_ID,
            [FromHeader] string Token_Data,
            [FromBody] ModelUserReq UserReq
            )
        {
            try
            {
                ModelAuth modelAuth = commonAuth.Login_Auth(Token_ID, Token_Data);

                RTA.Admin.UsersMstBLL um = new RTA.Admin.UsersMstBLL(DBConnStr);
                um.Edit_User(Id, UserReq);
                IActionResult objAction = CreatedAtAction("Edit_User", "success");
                return objAction;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelUserResp data = new ModelUserResp()
                {
                    status = false,
                    Message = ex.InnerException.Message
                };
                IActionResult objAction = CreatedAtAction("Edit_User", data);
                return objAction;
            }

        }

        [HttpPost("Save_User_Pofile")]
        [Produces("application/json", Type = typeof(ModelUserResp))]
        [Consumes("application/json")]
        public IActionResult Save_User_Pofile([FromBody] User_Profile_Master User_Profile_Master)
        {
            try
            {
                RTA.Admin.UsersMstBLL um = new RTA.Admin.UsersMstBLL(DBConnStr);
                um.New_User_Profile_Role(User_Profile_Master);
                IActionResult objAction = CreatedAtAction("Save_User_Pofile", "success");
                return objAction;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelUserResp data = new ModelUserResp()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("Save_User_Pofile", data);
                return objAction;
            }

        }

        [HttpPost("InsertRegistration")]
        [Produces("application/json", Type = typeof(ModelRegistrationResp))]
        [Consumes("application/json")]
        public IActionResult InsertRegistration([FromBody] ModelRegistrationReq res)
        {
            try
            {
                RTA.Admin.UsersMstBLL um = new RTA.Admin.UsersMstBLL(DBConnStr);
                var Registration = um.InsertRegistration(res);
                IActionResult objAction = CreatedAtAction("InsertRegistration", Registration);
                return objAction;
            }
            catch (Exception ex)
            {
                ModelRegistrationResp Res = new ModelRegistrationResp()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("InsertRegistration", Res);
                return objAction;
            }
        }

        [HttpGet("ValidateOTP")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult ValidateOTP(long mobile, string otp)
        {
            try
            {
                RTA.Admin.UsersMstBLL um = new RTA.Admin.UsersMstBLL(DBConnStr);
                var Registration = um.ValidateOTP(mobile, otp);
                IActionResult objAction = CreatedAtAction("ValidateOTP", Registration);
                return objAction;
            }
            catch (Exception ex)
            {
                ModelRegistrationResp res = new ModelRegistrationResp()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("ValidateOTP", res);
                return objAction;
            }
        }

        [HttpGet("GenerateOTP")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult GenerateOTP(string email, long mobile)
        {
            try
            {
                RTA.Admin.UsersMstBLL um = new RTA.Admin.UsersMstBLL(DBConnStr);
                var Registration = um.GenerateOTP(email, mobile);
                IActionResult objAction = CreatedAtAction("GenerateOTP", Registration);
                return objAction;
            }
            catch (Exception ex)
            {
                ModelRegistrationResp res = new ModelRegistrationResp()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("GenerateOTP", res);
                return objAction;
            }
        }

        [HttpGet("ValidateEmail")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult ValidateEmail(string email)
        {
            try
            {
                RTA.Admin.UsersMstBLL um = new RTA.Admin.UsersMstBLL(DBConnStr);
                var valemail = um.ValidateEmail(email);
                IActionResult objAction = CreatedAtAction("ValidateEmail", valemail);
                return objAction;
            }
            catch (Exception ex)
            {
                ModelRegistrationResp res = new ModelRegistrationResp()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("ValidateEmail", res);
                return objAction;
            }
        }

        [HttpGet("VerifyEmailotp")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult VerifyEmailotp(string email, string otp, long id)
        {
            try
            {
                RTA.Admin.UsersMstBLL um = new RTA.Admin.UsersMstBLL(DBConnStr);
                var valemail = um.VerifyEmailotp(email, otp, id);
                IActionResult objAction = CreatedAtAction("VerifyEmailotp", valemail);
                return objAction;
            }
            catch (Exception ex)
            {
                ModelRegistrationResp res = new ModelRegistrationResp()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("VerifyEmailotp", res);
                return objAction;
            }
        }

        [HttpGet("CreatePassword")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult CreatePassword(string password, string email)
        {
            try
            {
                RTA.Admin.UsersMstBLL um = new RTA.Admin.UsersMstBLL(DBConnStr);
                var valemail = um.CreatePassword(password, email);
                IActionResult objAction = CreatedAtAction("CreatePassword", valemail);
                return objAction;
            }
            catch (Exception ex)
            {
                ModelRegistrationResp res = new ModelRegistrationResp()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("CreatePassword", res);
                return objAction;
            }
        }

        [HttpGet("ChangePassword")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult ChangePassword(string password, string newPassword)
        {
            try
            {
                RTA.Admin.UsersMstBLL um = new RTA.Admin.UsersMstBLL(DBConnStr);
                var valemail = um.ChangePassword(password, newPassword);
                IActionResult objAction = CreatedAtAction("ChangePassword", valemail);
                return objAction;
            }
            catch (Exception ex)
            {
                ModelRegistrationResp res = new ModelRegistrationResp()
                {
                    status = false,
                    Message = ex.Message
                };
                IActionResult objAction = CreatedAtAction("ChangePassword", res);
                return objAction;
            }
        }
    }
}

