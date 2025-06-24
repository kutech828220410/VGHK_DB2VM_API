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
using SQLUI;
namespace DB2VM.Controller
{
   

    [Route("dbvm/[controller]")]
    [ApiController]
    public class BBCMController : ControllerBase
    {
        static string API_Server = "http://127.0.0.1:4433";
        private SQLControl sQLControl_UDSDBBCM = new SQLControl(MySQL_server, MySQL_database, "medicine_page_cloud", MySQL_userid, MySQL_password, (uint)MySQL_port.StringToInt32(), MySql.Data.MySqlClient.MySqlSslMode.None);

        static string MySQL_server = $"{ConfigurationManager.AppSettings["MySQL_server"]}";
        static string MySQL_database = $"{ConfigurationManager.AppSettings["MySQL_database"]}";
        static string MySQL_userid = $"{ConfigurationManager.AppSettings["MySQL_user"]}";
        static string MySQL_password = $"{ConfigurationManager.AppSettings["MySQL_password"]}";
        static string MySQL_port = $"{ConfigurationManager.AppSettings["MySQL_port"]}";

        [Route("")]
        [HttpGet]
        public string Get(string? Code)
        {
            MyTimerBasic myTimer = new MyTimerBasic(50000);
            string ODBC_string = "driver={IBM DB2 ODBC DRIVER - DB2COPY1};Database=DBDSNP;Hostname=192.168.51.102;Protocol=TCPIP;Port=50000;Uid=APUD07;Pwd=UD07AP;";

            // Use the dsn alias that you defined in db2dsdriver.cfg with the db2cli writecfg command in step 1.
     

            returnData returnData = new returnData();
            try
            {

                System.Data.Odbc.OdbcConnection odbcConnection = new OdbcConnection(ODBC_string);
                odbcConnection.Open();
                var MyDB2Command = odbcConnection.CreateCommand();

                //IBM.Data.DB2.DB2Connection odbcConnection = new IBM.Data.DB2.DB2Connection(MyDb2ConnectionString);
                //odbcConnection.Open();

                MyDB2Command = odbcConnection.CreateCommand();
                MyDB2Command.CommandText = "SELECT * FROM UD.UDDRGVWA A LEFT OUTER JOIN UD.UDPRDPF B ON A.UDDRGNO = B.UDDRGNO AND A.HID = B.HID WHERE A.HID = '1A0' WITH UR";

                var reader = MyDB2Command.ExecuteReader();
                medClass medClass = new medClass();
                List<medClass> medClasses = new List<medClass>();

                while (reader.Read())
                {
                    medClass = new medClass();
                    medClass.料號 = reader["UDSTOKNO"].ToString().Trim();

                    if (medClass.料號.Length >= 5)
                    {
                        medClass.料號 = medClass.料號.Substring(medClass.料號.Length - 5);
                    }
                    else
                    {
                        medClass.料號 = "";
                    }
                    medClass.藥品碼 = reader["UDDRGNO"].ToString().Trim();
                    if (medClass.藥品碼.StringIsEmpty() == false)
                    {
                        medClass.藥品名稱 = reader["UDRPNAME"].ToString().Trim();
                        medClass.中文名稱 = reader["UDCHTNAM"].ToString().Trim();
                        medClass.藥品學名 = reader["UDHIMDPN"].ToString().Trim();
                        medClass.包裝單位 = reader["UDUNFORM"].ToString().Trim();
                        medClass.包裝數量 = reader["UDCONVER"].ToString().Trim();
                  
                        string UDSTOCK = reader["UDSTOCK"].ToString().Trim();

                        if (UDSTOCK == "1" || UDSTOCK == "2" || UDSTOCK == "3" || UDSTOCK == "4")
                        {
                            medClass.管制級別 = UDSTOCK;
                        }
               
                        else
                        {
                            medClass.管制級別 = "N";
                        }
                        if (UDSTOCK == "H")
                        {
                            medClass.高價藥品 = "True";
                        }
                        else
                        {
                            medClass.高價藥品 = "False";
                        }
                    }
                    medClasses.Add(medClass);
                }
                odbcConnection.Close();
                List<medClass> medClasses_cloud = medClass.get_med_cloud(API_Server);
                List<medClass> medClasses_cloud_buf = new List<medClass>();
                List<medClass> medClasses_cloud_add = new List<medClass>();
                List<medClass> medClasses_cloud_replace = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_med_cloud = medClasses_cloud.CoverToDictionaryByCode();

                for (int i = 0; i < medClasses.Count; i++)
                {
                    string code = medClasses[i].藥品碼;

                    medClasses_cloud_buf = keyValuePairs_med_cloud.SortDictionaryByCode(code);
                    if(medClasses_cloud_buf.Count > 0)
                    {
                        medClasses_cloud_add.Add(medClasses[i]);
                    }
                    else
                    {
                        medClasses[i].GUID = Guid.NewGuid().ToString();
                        medClasses_cloud_add.Add(medClasses[i]);
                    }
                }



                returnData returnData_med_cloud = medClass.add_med_clouds(API_Server, medClasses_cloud_add);


                returnData.Code = 200;
                returnData.Data = medClasses;
                returnData.Result = $"{returnData_med_cloud.Result}";
                returnData.TimeTaken = myTimer.ToString();
                return returnData.JsonSerializationt(true);

            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }

     
        }
    }
}
