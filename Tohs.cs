using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SQLUI;
using Basic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using HIS_DB_Lib;
using System.Data.SqlClient;
namespace DB2VM_API
{
    [Route("[controller]")]
    [ApiController]
    public class Tohs : ControllerBase
    {
        public class DGOPDPPL_Class
        {
            public string 藥碼 = "";
            public string 藥局 = "";
            public string 交易時間 = "";
            public string 盤點量 = "";
            public string 盤點人帳號 = "";
            public string 盤點人姓名 = "";
            public override string ToString()
            {
                if (藥碼.Length < 5) 藥碼 = "0" + 藥碼;
                if (藥碼.Length < 5) 藥碼 = "0" + 藥碼;
                if (藥碼.Length < 5) 藥碼 = "0" + 藥碼;
                if (藥碼.Length < 5) 藥碼 = "0" + 藥碼;
                if (盤點量.Length < 7) 盤點量 = " " + 盤點量;
                if (盤點量.Length < 7) 盤點量 = " " + 盤點量;
                if (盤點量.Length < 7) 盤點量 = " " + 盤點量;
                if (盤點量.Length < 7) 盤點量 = " " + 盤點量;
                if (盤點量.Length < 7) 盤點量 = " " + 盤點量;
                if (盤點量.Length < 7) 盤點量 = " " + 盤點量;
                if (盤點量.Length < 7) 盤點量 = " " + 盤點量;
                if (盤點量.Length < 7) 盤點量 = " " + 盤點量;



                return 藥碼.StringLength(5) + 藥局.StringLength(4) + 交易時間.StringLength(12) + 盤點量 + 盤點人帳號.StringLength(8) + 盤點人姓名.StringLength(20);
            }

        }
        [Route("download/ITUD0031")]
        [HttpGet]
        public async Task<ActionResult> GET_download_ITUD0031()
        {
            try
            {
                // 構建文件路徑
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"C:\東軒data\importcabinet.csv");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("文件未找到");
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0; // 將內存流的位置重置為開頭


                return File(memory, "application/octet-stream", "importcabinet.csv");
            }
            catch (Exception e)
            {
                return StatusCode(500, "內部伺服器錯誤");
            }
        }
        [Route("download/UDMORQRE")]
        [HttpGet]
        public async Task<ActionResult> GET_download_UDMORQRE()
        {
            try
            {
                // 構建文件路徑
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"C:\東軒data\cabinet.csv");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("文件未找到");
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0; // 將內存流的位置重置為開頭


                return File(memory, "application/octet-stream", "cabinet.csv");
            }
            catch (Exception e)
            {
                return StatusCode(500, "內部伺服器錯誤");
            }
        }

        [Route("DGOPDPPL/{datetime}")]
        [HttpGet]
        public string DGOPDPPL(string datetime)
        {
            string str = "";
            try
            {
                string connectionString = "Server=192.168.222.19;Database=CabinetDB_new;User Id=Cabinet_new;Password=Cabinet_new;";

                // 建立SQL連接
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        //if (datetime.Check_Date_String() == false)
                        //{
                        //    return "請輸入合法日期格式";
                        //}
                        if (datetime.Length != 12)
                        {
                            return "請輸入合法日期格式";
                        }
                        string date_str = $"{datetime.Substring(0, 4)}-{datetime.Substring(4, 2)}-{datetime.Substring(6, 2)} {datetime.Substring(8, 2)}:{datetime.Substring(10, 2)}";
                        DateTime dateTime = date_str.StringToDateTime();
                        DateTime dateTime_start = new DateTime();
                        DateTime dateTime_end = new DateTime();
                        int hour = dateTime.Hour;
                        if (hour >= 0 && hour <= 07)
                        {
                            dateTime = dateTime.AddDays(-1);
                            dateTime_start = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 21, 00, 00);
                            dateTime_end = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
                        }
                        else if (hour >= 08 && hour <= 15)
                        {
                            dateTime_start = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 08, 00, 00);
                            dateTime_end = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 15, 59, 59);
                        }
                        else if (hour >= 16 && hour <= 23)
                        {
                            dateTime_start = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 16, 00, 00);
                            dateTime_end = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 22, 59, 59);
                        }
                        // 開啟連接
                        connection.Open();
                        Console.WriteLine("連接成功!");
                        string date_st = dateTime_start.ToDateTimeString();
                        string date_end = dateTime_end.ToDateTimeString();
                        // SQL查詢語句
                        string query = $"SELECT * FROM TransLog WHERE MedicalRecordNo = '對點作業' AND (TransTime >= '{date_st}' AND TransTime <= '{date_end}')";
                        List<DGOPDPPL_Class> dGOPDPPL_Classes = new List<DGOPDPPL_Class>();
                        // 建立SQL命令
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            // 執行命令並讀取資料
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    DGOPDPPL_Class dGOPDPPL = new DGOPDPPL_Class();
                                    string 動作 = reader["MedicalRecordNo"].ToString();
                                    string 藥碼 = reader["DrugCode"].ToString();
                                    string 藥局 = reader["CabinetName"].ToString();
                                    DateTime 交易時間 = reader["TransTime"].ToDateTimeString().StringToDateTime();
                                    string 盤點量 = reader["Balance"].ToString();
                                    string 盤點人帳號 = reader["UserNo"].ToString();
                                    string 盤點人姓名 = reader["UserName"].ToString();
                                    dGOPDPPL.藥碼 = 藥碼;
                                    if (藥局 == "門診藥局管2") 藥局 = "ER";
                                    if (藥局 == "門診藥局管3") 藥局 = "OPD";
                                    dGOPDPPL.藥局 = 藥局;

                                    dGOPDPPL.交易時間 = $"{交易時間.ToString("yyyyMMddHHmm")}";
                                    dGOPDPPL.盤點量 = 盤點量;
                                    dGOPDPPL.盤點人帳號 = 盤點人帳號.Substring(盤點人帳號.Length - 4 , 4);
                                    dGOPDPPL.盤點人姓名 = 盤點人姓名;
                                    dGOPDPPL_Classes.Add(dGOPDPPL);

                                }
                            }
                        }
                        
                        for (int i = 0; i < dGOPDPPL_Classes.Count; i++)
                        {
                            str += $"{dGOPDPPL_Classes[i]}";
                            if (i != dGOPDPPL_Classes.Count - 1) str += "\n";
                        }
                    }
                    catch (Exception ex)
                    {
                        // 處理例外情況
                        Console.WriteLine("連接失敗: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return str;
        }
    }
}
