using Microsoft.Extensions.Configuration;
using RTA.Masters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTAAPI
{
    public class DB
    {
        public static string GetDBCred(IConfiguration Configurations)
        {
            string dbName = Configurations["DB_Settings:db"].ToString();
            string UID = Configurations["DB_Settings:userID"].ToString();
            string password = Configurations["DB_Settings:password"].ToString();
            string Server = Configurations["DB_Settings:db_server"].ToString();
            string DBConnString = "SERVER=" + Server +
                ";PORT=3306;DATABASE=" + dbName +
                ";USER ID=" + UID +
                ";PASSWORD=" + password + ";";
            return DBConnString;
        }

        public static ModelCompDBParas GetNewCompDBCred(IConfiguration Configurations)
        {
            return new ModelCompDBParas()
            {
                userId = Configurations["Comp_DB_Setting:userID"].ToString(),
                password = Configurations["Comp_DB_Setting:password"].ToString(),
                server_ip = Configurations["Comp_DB_Setting:db_server"].ToString(),
            };
   
           
        }

    
    }
}
