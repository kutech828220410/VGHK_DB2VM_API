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

            MyTimerBasic myTimer = new MyTimerBasic(50000);
            //String MyDb2ConnectionString = "server=192.168.51.102:50000;database=DBDSNP;uid=APUD07;pwd=UD07AP;";
            string ODBC_string = "driver={IBM DB2 ODBC DRIVER};Database=DBDSNP;Hostname=192.168.51.102;Protocol=TCPIP;Port=50000;Uid=APUD07;Pwd=UD07AP;";

            try
            {

                System.Data.Odbc.OdbcConnection odbcConnection = new OdbcConnection(ODBC_string);
                odbcConnection.Open();


                //System.Data.Odbc.OdbcCommand MyDB2Command = odbcConnection.CreateCommand();
                //MyDB2Command.CommandText = "SELECT * FROM UD.UDGETLOG WHERE HID='1A0'";

                //var reader = MyDB2Command.ExecuteReader();
                odbcConnection.Close();
            }
            catch(Exception e)
            {
                return $"{ODBC_string}\n{e.Message}";
            }

            return $"DB2 Connecting sucess! , {ODBC_string}";


        }


    }
}
