using Microsoft.AspNetCore.Http;
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
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {

        private IConfiguration Configurations;
        private string DBConnStr;
        private CommonAuth commonAuth;

        private loginDBContext LoginDB;
        public SampleController(IConfiguration config)
        {
            Configurations = config;
            DBConnStr = RTAAPI.DB.GetDBCred(Configurations);
            commonAuth = new CommonAuth(DBConnStr);
        }

        //GET ALL RECORDS
        [HttpGet("GetSampleRecords")]
        [Produces("application/json", Type = typeof(ModelSampleData))]
        public IActionResult GetSampleRecords(
              [FromHeader] long Token_ID,
              [FromHeader] string Token_Data)
        {
            try
            {
                ModelAuth modelAuth= commonAuth.Login_Auth(Token_ID, Token_Data);
                SampleMastBLL sample = new SampleMastBLL(DBConnStr);

                var Res = sample.GetSampleData();

                IActionResult objAction = CreatedAtAction("GetSampleRecords", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ModelSampleData data = new ModelSampleData()
                {
                    status = false,
                    ErrorMessage = ex.Message
                };
                IActionResult objAction = CreatedAtAction("GetInwardRecords", data);
                return objAction;
            }
        }


        [HttpGet("GetSampleRecordByID")]
        [Produces("application/json", Type = typeof(Sample_Detail))]
        public IActionResult GetSampleRecordByID(int id,
             [FromHeader] long Token_ID,
             [FromHeader] string Token_Data)
        {
            try
            {
                ModelAuth modelAuth= commonAuth.Login_Auth(Token_ID, Token_Data);

                RTA.Masters.SampleMastBLL sample = new SampleMastBLL(DBConnStr);
                var Res = sample.GetSampleDataByID(id);
                IActionResult objAction = CreatedAtAction("GetSampleRecordByID", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error : " + ex.Message);
            }
        }

        [HttpPost("AddSampleMaster")]
        [Produces("application/json", Type = typeof(Sample_Detail))]
        public IActionResult AddSampleMaster(Sample_Detail samp)
        {
            try
            {
                

                RTA.Masters.SampleMastBLL sample = new SampleMastBLL(DBConnStr);
                var Res = sample.InsertSample(samp);
                IActionResult objAction = CreatedAtAction("AddSampleMaster", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error : " + ex.Message);
            }
        }

        [HttpPut("UpdateSampleMaster")]
        [Produces("application/json", Type = typeof(Sample_Detail))]
        public IActionResult UpdateSampleMaster(int id, Sample_Detail samp,
            [FromHeader] long Token_ID,
             [FromHeader] string Token_Data)
        {
            try
            {
                ModelAuth modelAuth= commonAuth.Login_Auth(Token_ID, Token_Data);

                RTA.Masters.SampleMastBLL sample = new SampleMastBLL(DBConnStr);
                var Res = sample.UpdateSample(id, samp);
                IActionResult objAction = CreatedAtAction("UpdateSampleMaster", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error : " + ex.Message);
            }
        }

        [HttpDelete("DeleteSampleMaster")]
        [Produces("application/json", Type = typeof(Sample_Detail))]
        public IActionResult DeleteSampleMaster(int id,
             [FromHeader] long Token_ID,
             [FromHeader] string Token_Data)
        {
            try
            {
                ModelAuth modelAuth= commonAuth.Login_Auth(Token_ID, Token_Data);
                RTA.Masters.SampleMastBLL inwaard = new SampleMastBLL(DBConnStr);
                var Res = inwaard.DeleteSample(id);
                IActionResult objAction = CreatedAtAction("DeleteSampleMaster", Res);
                return objAction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Error : " + ex.Message);
            }
        }
    }
}
