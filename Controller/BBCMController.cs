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
            //String MyDb2ConnectionString = "server=192.168.51.102:50000;database=DBDSNP;uid=APUD07;pwd=UD07AP;";
            string ODBC_string = "driver={IBM DB2 ODBC DRIVER};Database=DBDSNP;Hostname=192.168.51.102;Protocol=TCPIP;Port=50000;Uid=APUD07;Pwd=UD07AP;";
            returnData returnData = new returnData();
            try
            {

                System.Data.Odbc.OdbcConnection odbcConnection = new OdbcConnection(ODBC_string);
                odbcConnection.Open();
                System.Data.Odbc.OdbcCommand MyDB2Command = odbcConnection.CreateCommand();
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
                        //medClass.廠牌 = reader["UDMFTCOD"].ToString().Trim();
                        if (medClass.藥品碼 == "06008")
                        { 
                        }
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

                List<object[]> list_BBCM = sQLControl_UDSDBBCM.GetAllRows(null);
                List<object[]> list_BBCM_buf = new List<object[]>();
                List<object[]> list_BBCM_add = new List<object[]>();
                List<object[]> list_BBCM_replace = new List<object[]>();
                for (int i = 0; i < medClasses.Count; i++)
                {
                    string 藥品碼 = medClasses[i].藥品碼;
                    list_BBCM_buf = list_BBCM.GetRows((int)enum_雲端藥檔.藥品碼, 藥品碼);
                    if(list_BBCM_buf.Count > 0)
                    {
                        bool flag_replace = false;
                        string 中文名稱 = medClasses[i].中文名稱;
                        string 藥品學名 = medClasses[i].藥品碼;
                        string 包裝單位 = medClasses[i].藥品碼;
                        string 包裝數量 = medClasses[i].包裝數量;
                        string 管制級別 = medClasses[i].管制級別;
                        string 高價藥品 = medClasses[i].高價藥品;
                        if (中文名稱 != list_BBCM_buf[0][(int)enum_雲端藥檔.中文名稱].ObjectToString()) flag_replace = true;
                        if (藥品學名 != list_BBCM_buf[0][(int)enum_雲端藥檔.藥品學名].ObjectToString()) flag_replace = true;
                        if (包裝單位 != list_BBCM_buf[0][(int)enum_雲端藥檔.包裝單位].ObjectToString()) flag_replace = true;
                        if (包裝數量 != list_BBCM_buf[0][(int)enum_雲端藥檔.包裝數量].ObjectToString()) flag_replace = true;
                        if (管制級別 != list_BBCM_buf[0][(int)enum_雲端藥檔.管制級別].ObjectToString()) flag_replace = true;
                        if (高價藥品 != list_BBCM_buf[0][(int)enum_雲端藥檔.高價藥品].ObjectToString()) flag_replace = true;
                        if (flag_replace)
                        {
                            list_BBCM_buf[0][(int)enum_雲端藥檔.中文名稱] = 中文名稱;
                            list_BBCM_buf[0][(int)enum_雲端藥檔.藥品學名] = 藥品學名;
                            list_BBCM_buf[0][(int)enum_雲端藥檔.包裝單位] = 包裝單位;
                            list_BBCM_buf[0][(int)enum_雲端藥檔.包裝數量] = 包裝數量;
                            list_BBCM_buf[0][(int)enum_雲端藥檔.管制級別] = 管制級別;
                            list_BBCM_buf[0][(int)enum_雲端藥檔.高價藥品] = 高價藥品;
                            list_BBCM_replace.Add(list_BBCM_buf[0]);
                        }
                     
                    }
                    else
                    {
                        medClasses[i].GUID = Guid.NewGuid().ToString();
                        list_BBCM_add.Add(medClasses[i].ClassToSQL<medClass, enum_雲端藥檔>());
                    }

                }
                if (list_BBCM_add.Count > 0) sQLControl_UDSDBBCM.AddRows(null, list_BBCM_add);
                if (list_BBCM_replace.Count > 0) sQLControl_UDSDBBCM.UpdateByDefulteExtra(null, list_BBCM_replace);

                returnData.Code = 200;
                returnData.Data = medClasses;
                returnData.Result = $"取得藥檔成功!,新增<{list_BBCM_add.Count}>筆 修改<{list_BBCM_replace.Count}>筆";
                returnData.TimeTaken = myTimer.ToString();
                return returnData.JsonSerializationt(true);

            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{ODBC_string}\n{e.Message}";
                return returnData.JsonSerializationt(true);
            }

            //List<object[]> list_BBCM = new List<object[]>();
            //if(Code.StringIsEmpty())
            //{
            //    list_BBCM = sQLControl_UDSDBBCM.GetAllRows(null);
            //}
            //else
            //{
            //    list_BBCM = sQLControl_UDSDBBCM.GetRowsByDefult(null, (int)enum_雲端藥檔.藥品碼, Code);
            //}

            //List<object[]> list_BBCM_buf = new List<object[]>();
            //List<object[]> list_BBCM_Add = new List<object[]>();
            //List<object[]> list_BBCM_Replace = new List<object[]>();
            //List<medClass> medClasses = list_BBCM.SQLToClass<medClass, enum_雲端藥檔>();


            //return medClasses.JsonSerializationt(); 
        }
    }
}
