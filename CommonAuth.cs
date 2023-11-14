using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DB.Login;
using DB.Login.Tables;
using RTA.Common;
using RTA.Common.Models;

namespace RTAAPI
{
    public class CommonAuth
    {

        IConfiguration Configurations;
        string DBConnStr;
        private loginDBContext LoginDB;
        public CommonAuth(string ConnStr)
        {
            DBConnStr = ConnStr;
        }

        //private string userRights;

        public ModelAuth Login_Auth(long Token_ID, string Token_Data)
        {
            try
            {
                AuthTokenBLL authTokenBLL = new AuthTokenBLL(DBConnStr);
            ModelAuthToken token = new ModelAuthToken(){
                token_id=Token_ID, 
                token_data=Token_Data};

            ModelAuth modelAuth  = authTokenBLL.VerifyToken(token);
            return modelAuth;
            }
            catch (Exception ex)
            {
                ModelAuth modelAuth = new ModelAuth(){
                status=false, 
                message=ex.Message};
                return modelAuth;

            }
            
        }
        
        public bool VerifyFormRights(ModelAuth modelAuth, long FormId, string CheckRight)
        {
            //VerificationBLL verificationBLL = new VerificationBLL(DBConnStr);
            //var usrFrmRights;
            bool Res = false;

            if(modelAuth.auth_type=="profile_type")
            {
            User_Profile_Master usrProfileRights= modelAuth.UserProfileRights;
            Console.WriteLine("USER RIGHT" + usrProfileRights.report_yes_no);

            switch(CheckRight)
            {
                case "SAVE" :
                    if (usrProfileRights.save_yes_no.ToUpper()=="YES") {Res=true;}
                    break;
                case "MODIFY":
                    if (usrProfileRights.modify_yes_no.ToUpper()=="YES") {Res=true;}
                    break;
                case "DELETE":
                    if (usrProfileRights.modify_yes_no.ToUpper()=="YES") {Res=true;}
                    break;          
                case "PRINT":
                    if (usrProfileRights.print_only.ToUpper()=="YES") {Res=true;}
                    break;
                case "REPORT":
                    if (usrProfileRights.report_yes_no.ToUpper()=="YES") {Res=true;}
                    break;
            }
            return Res;

            }
            if(modelAuth.auth_type=="user_type")
            {
            List<Forms_Trx_Master> usrFrmRights= modelAuth.UserRights.Where(
                x=>x.form_master_id == FormId
            ).ToList();

            switch(CheckRight)
            {
                case "SAVE" :
                    if (usrFrmRights.Where(x=>x.save_yes_no.ToUpper()=="YES").Count()>0) {Res=true;}
                    break;
                case "MODIFY":
                    if (usrFrmRights.Where(x=>x.modify_yes_no.ToUpper()=="YES").Count()>0) {Res=true;}
                    break;
                case "DELETE":
                    if (usrFrmRights.Where(x=>x.modify_yes_no.ToUpper()=="YES").Count()>0) {Res=true;}
                    break;          
                case "PRINT":
                    if (usrFrmRights.Where(x=>x.print_only.ToUpper()=="YES").Count()>0) {Res=true;}
                    break;
                case "REPORT":
                    if (usrFrmRights.Where(x=>x.report_yes_no.ToUpper()=="YES").Count()>0) {Res=true;}
                    break;
            }
            return Res;
            }
            
            //Forms_Trx_Master fm  = verificationBLL.VerifyUserRights(FormId,user.user_code);
            return Res;


            
        }
    }
}