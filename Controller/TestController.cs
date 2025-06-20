using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using Basic;
using Oracle.ManagedDataAccess.Client;
using System.Data.Odbc;
using HIS_DB_Lib;
namespace DB2VM
{
    [Route("dbvm/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
   
        
        // GET api/values
        [HttpGet]
        public string Get()
        {
            var found = System.IO.File.Exists(@"C:\Program Files\IBM\SQLLIB\BIN\db2cli.dll");
            try
            {
             

                string ODBC_string = "driver={IBM DB2 ODBC DRIVER};Database=DBDSNP;Hostname=192.168.51.102;Protocol=TCPIP;Port=50000;Uid=APUD07;Pwd=UD07AP;";
                System.Data.Odbc.OdbcConnection odbcConnection = new OdbcConnection(ODBC_string);
                odbcConnection.Open();
                var MyDB2Command = odbcConnection.CreateCommand();
                odbcConnection.Close();
            }
            catch(Exception ex)
            {
                return $"Exception : {ex.Message} {System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString()} found:{found} {Environment.GetEnvironmentVariable("PATH")}";
            }
         
            return $"API Connecting sucess! {System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString()} found:{found} {Environment.GetEnvironmentVariable("PATH")}";


        }


    }
}
