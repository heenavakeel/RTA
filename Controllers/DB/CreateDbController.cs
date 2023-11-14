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

namespace Rta.Controllers.DB
{
    [Route("api/db/[controller]")]
    [ApiController]
    public class CreateDbController : ControllerBase
    {
        private IConfiguration Configurations;
        private string DBConnStr;
        private CommonAuth commonAuth;
        private IActionResult objAction;
        private loginDBContextFactory LoginDB;

        public CreateDbController(IConfiguration config)
        {
            Configurations = config;
            DBConnStr = RTAAPI.DB.GetDBCred(Configurations);
            commonAuth = new CommonAuth(DBConnStr);
        }
        
        [HttpGet("CreateLoginDB")]
        [Produces("application/json")]
        public IActionResult CreateLoginDB()
        {
            try
            {
                new loginDBContextFactory().MigrateDbContext(DBConnStr);
                return Content("{ \"Message\":\"db created successfully\" }", "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                throw new Exception("Error message", ex);
                
            }
          

        }






    }

    
}