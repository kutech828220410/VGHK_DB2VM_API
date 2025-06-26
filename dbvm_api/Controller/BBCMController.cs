using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Text;
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
            Stopwatch sw_total = Stopwatch.StartNew();
            Stopwatch sw_step = new Stopwatch();
            StringBuilder sb = new StringBuilder();

            string ODBC_string = "driver={IBM DB2 ODBC DRIVER - DB2COPY1};Database=DBDSNP;Hostname=192.168.51.102;Protocol=TCPIP;Port=50000;Uid=APUD07;Pwd=UD07AP;";
            returnData returnData = new returnData();

            try
            {
                sw_step.Restart();
                System.Data.Odbc.OdbcConnection odbcConnection = new OdbcConnection(ODBC_string);
                odbcConnection.Open();
                sb.AppendLine($"[1] 連線資料庫: {sw_step.ElapsedMilliseconds}ms");

                sw_step.Restart();
                var MyDB2Command = odbcConnection.CreateCommand();
                MyDB2Command.CommandText = "SELECT * FROM UD.UDDRGVWA A LEFT OUTER JOIN UD.UDPRDPF B ON A.UDDRGNO = B.UDDRGNO AND A.HID = B.HID WHERE A.HID = '1A0' WITH UR";
                var reader = MyDB2Command.ExecuteReader();
                sb.AppendLine($"[2] 執行查詢並取得資料: {sw_step.ElapsedMilliseconds}ms");

                sw_step.Restart();
                medClass medClass = new medClass();
                List<medClass> medClasses = new List<medClass>();

                while (reader.Read())
                {
                    medClass = new medClass();
                    medClass.料號 = reader["UDSTOKNO"].ToString().Trim();
                    if (medClass.料號.Length >= 5)
                        medClass.料號 = medClass.料號.Substring(medClass.料號.Length - 5);
                    else
                        medClass.料號 = "";

                    medClass.藥品碼 = reader["UDDRGNO"].ToString().Trim();
                    if (!medClass.藥品碼.StringIsEmpty())
                    {
                        medClass.藥品名稱 = reader["UDRPNAME"].ToString().Trim();
                        medClass.中文名稱 = reader["UDCHTNAM"].ToString().Trim();
                        medClass.藥品學名 = reader["UDHIMDPN"].ToString().Trim();
                        medClass.包裝單位 = reader["UDUNFORM"].ToString().Trim();
                        medClass.包裝數量 = reader["UDCONVER"].ToString().Trim();
                        medClass.ATC = reader["UDATCCOD"].ToString().Trim();
                        medClass.中西藥 = "西藥";

                        string UDSTOCK = reader["UDSTOCK"].ToString().Trim();
                        medClass.管制級別 = (new[] { "1", "2", "3", "4" }.Contains(UDSTOCK)) ? UDSTOCK : "N";
                        medClass.高價藥品 = (UDSTOCK == "H") ? "True" : "False";

                        if (reader["UDABSCTL"].ToString().Trim() == "Y" && reader["AROUTFLA"].ToString().Trim() == "Y")
                            medClass.開檔狀態 = "關檔中";
                        else
                            medClass.開檔狀態 = "開檔中";
                    }

                    medClasses.Add(medClass);
                }
                odbcConnection.Close();
                sb.AppendLine($"[3] 資料轉換/封裝完成: {sw_step.ElapsedMilliseconds}ms");

                sw_step.Restart();
                List<medClass> medClasses_cloud = medClass.get_med_cloud(API_Server);
                sb.AppendLine($"[4] 取得雲端藥品資料: {sw_step.ElapsedMilliseconds}ms");

                sw_step.Restart();
                List<medClass> medClasses_cloud_buf = new List<medClass>();
                List<medClass> medClasses_cloud_add = new List<medClass>();
                Dictionary<string, List<medClass>> keyValuePairs_med_cloud = medClasses_cloud.CoverToDictionaryByCode();

                for (int i = 0; i < medClasses.Count; i++)
                {
                    string code = medClasses[i].藥品碼;
                    medClasses_cloud_buf = keyValuePairs_med_cloud.SortDictionaryByCode(code);
                    if (medClasses_cloud_buf.Count > 0)
                    {
                        medClasses_cloud_add.Add(medClasses[i]);
                    }
                    else
                    {
                        medClasses[i].GUID = Guid.NewGuid().ToString();
                        medClasses_cloud_add.Add(medClasses[i]);
                    }
                }
                sb.AppendLine($"[5] 比對資料差異: {sw_step.ElapsedMilliseconds}ms");

                sw_step.Restart();
                returnData returnData_med_cloud = medClass.add_med_clouds(API_Server, medClasses_cloud_add);
                sb.AppendLine($"[6] 上傳雲端新增/更新: {sw_step.ElapsedMilliseconds}ms");

                returnData.Code = 200;
                returnData.Data = medClasses;
                sb.AppendLine($"[7] 總耗時: {sw_total.ElapsedMilliseconds}ms");
                sb.AppendLine($"[8] add_med_cloud 回傳: {returnData_med_cloud.Result}");

                returnData.Result = sb.ToString();
                returnData.TimeTaken = myTimer.ToString();

                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"錯誤: {e.Message}";
                return returnData.JsonSerializationt(true);
            }



        }
    }
}
