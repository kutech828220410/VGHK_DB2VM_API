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
using MyOffice;
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
        public class drug_class
        {
            public string 藥碼 = "";
            public string 藥名 = "";
            public string 藥師證字 = "";
            public string 管制級別 = "";
        }
        public class cdmis_class
        {
            public string 藥碼 = "";
            public string 藥名 = "";
            public string 藥局 = "";
            public string 交易時間 = "";
            public string 收支原因 = "";
            public string 收入數量 = "";
            public string 數量 = "";
            public string 收入藥品批號_效期 = "";
            public string 支出數量 = "";
            public string 結存數量 = "";
            public string 姓名 = "";
            public string 調配藥師字號 = "";
            public string 調配藥師 = "";
            public string 點斑藥師 = "";
            public string 核對藥師 = "";
            public string 病歷號 = "";
            public string 備註 = "";
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

        [Route("download_excel/{value}")]
        [HttpGet]
        public async Task<ActionResult> GET_download_excel(string value)
        {
            try
            {
                string[] str_ary = value.Split(',');
                if (str_ary.Length != 2)
                {
                    return StatusCode(500, "傳入日期格式錯誤");
                }
                if (str_ary[0].Length != 12 || str_ary[1].Length != 12)
                {
                    return StatusCode(500, "傳入日期格式錯誤");
                }
                string date1 = $"{str_ary[0].Substring(0, 4)}-{str_ary[0].Substring(4, 2)}-{str_ary[0].Substring(6, 2)} {str_ary[0].Substring(8, 2)}:{str_ary[0].Substring(10, 2)}:00";
                string date2 = $"{str_ary[1].Substring(0, 4)}-{str_ary[1].Substring(4, 2)}-{str_ary[1].Substring(6, 2)} {str_ary[1].Substring(8, 2)}:{str_ary[1].Substring(10, 2)}:00";
                if(date1.Check_Date_String() == false || date2.Check_Date_String() == false)
                {
                    return StatusCode(500, "傳入日期格式錯誤");
                }

                string connectionString = "Server=192.168.222.19;Database=CabinetDB_new;User Id=Cabinet_new;Password=Cabinet_new;";
                string query = $"SELECT * FROM TransLog WHERE (MedicalRecordNo != '取消交易' AND MedicalRecordNo != '誤開抽屜' AND MedicalRecordNo != '修改藥名' AND MedicalRecordNo != '')AND (TransTime >= '{date1}' AND TransTime <= '{date2}');";
                string query_drug = $"SELECT * FROM DrugTable";
                List<cdmis_class> cdmis_Classes = new List<cdmis_class>();
                List<drug_class> drug_Classes = new List<drug_class>();
                List<drug_class> drug_Classes_buf = new List<drug_class>();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("連接成功!");
                    using (SqlCommand command = new SqlCommand(query_drug, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                drug_class drug_Class = new drug_class();
                                drug_Class.藥碼 = reader["DrugCode"].ToString();
                                drug_Class.藥名 = reader["DrugName"].ToString();
                                drug_Class.藥師證字 = reader["NHI_Code"].ToString().Trim();
                                drug_Class.管制級別 = reader["DrugDegree"].ToString();
                                if (drug_Class.藥碼.Length < 5) drug_Class.藥碼 = "0" + drug_Class.藥碼;
                                if (drug_Class.藥碼.Length < 5) drug_Class.藥碼 = "0" + drug_Class.藥碼;
                                if (drug_Class.藥碼.Length < 5) drug_Class.藥碼 = "0" + drug_Class.藥碼;
                                if (drug_Class.藥碼.Length < 5) drug_Class.藥碼 = "0" + drug_Class.藥碼;
                                drug_Classes.Add(drug_Class);
                            }
                       
                        }
                    }
                }


                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("連接成功!");
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string 藥碼 = reader["DrugCode"].ToString();
                                if (藥碼.Length < 5) 藥碼 = "0" + 藥碼;
                                if (藥碼.Length < 5) 藥碼 = "0" + 藥碼;
                                if (藥碼.Length < 5) 藥碼 = "0" + 藥碼;
                                if (藥碼.Length < 5) 藥碼 = "0" + 藥碼;

                                string 藥名 = reader["DrugName"].ToString();
                                string 藥局 = reader["CabinetName"].ToString();
                                if (藥局 == "門診藥局管2") 藥局 = "ER";
                                if (藥局 == "門診藥局管3") 藥局 = "OPD";

                                DateTime 交易時間 = reader["TransTime"].ToDateTimeString().StringToDateTime();
                                string 收支原因 = reader["MedicalRecordNo"].ToString();
                                string 數量 = reader["TransAmount"].ToString();
                                string 結存數量 = reader["Balance"].ToString();
                                string 藥師姓名_temp = reader["UserName"].ToString();
                                string 藥師姓名 = 藥師姓名_temp;
                                string 調配藥師字號 = "";
                                string 核對藥師姓名 = reader["AuditorName"].ToString();

                                string 藥師ID = reader["UserNo"].ToString();
                                string 病歷號 = "";

                                if(收支原因 != "領取藥品" && 收支原因 != "對點作業" && 收支原因 != "存入藥品" && 收支原因 != "對點校正")
                                {
                                    病歷號 = reader["MedicalRecordNo"].ToString().Trim();
                                    收支原因 = "調劑領藥";
                                }

                                if (藥師姓名_temp.Length >= 9)
                                {                              
                                    藥師姓名 = 藥師姓名_temp.Substring(6, 3);
                                    調配藥師字號 = 藥師姓名_temp.Substring(0, 6);
                                }
                                cdmis_class cdmis_Class = new cdmis_class();
                                cdmis_Class.藥碼 = 藥碼;
                                cdmis_Class.藥名 = 藥名;
                                cdmis_Class.藥局 = 藥局;
                                cdmis_Class.交易時間 = 交易時間.ToDateTimeString();
                                cdmis_Class.收支原因 = 收支原因;
                                cdmis_Class.病歷號 = 病歷號;
                                cdmis_Class.調配藥師字號 = 調配藥師字號;
                                cdmis_Class.結存數量 = 結存數量;
                                if (收支原因 == "領取藥品")
                                {
                                    cdmis_Class.支出數量 = 數量;
                                    cdmis_Class.調配藥師 = 藥師姓名;
                                    cdmis_Class.調配藥師字號 = 調配藥師字號;
                                }
                                else if (收支原因 == "對點作業" || 收支原因 == "對點校正")
                                {
                                    cdmis_Class.點斑藥師 = 藥師姓名;
                                    cdmis_Class.核對藥師 = 核對藥師姓名;
                                }
                                else if (收支原因 == "存入藥品")
                                {
                                    cdmis_Class.收入數量 = 數量;
                                    cdmis_Class.調配藥師 = 藥師姓名;
                                    cdmis_Class.調配藥師字號 = 調配藥師字號;
                                }
                                else if (收支原因 == "調劑領藥")
                                {
                                    cdmis_Class.支出數量 = 數量;
                                    cdmis_Class.調配藥師 = 藥師姓名;
                                    cdmis_Class.調配藥師字號 = 調配藥師字號;
                                }
                                else
                                {

                                }
                                cdmis_Class.備註 = 藥局;
                                cdmis_Classes.Add(cdmis_Class);
                            }
                        }
                    }
                
                }

                string loadText = Basic.MyFileStream.LoadFileAllText(@"./cmdi_excel.txt", "utf-8");
                int row_max = 60000;
                string xlsx_command = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string xls_command = "application/vnd.ms-excel";

                List<SheetClass> sheetClasses = new List<SheetClass>();

                List<cdmis_class> cdmis_Classes_buf = new List<cdmis_class>();
                List<string> Codes = (from temp in cdmis_Classes
                                      select temp.藥碼).Distinct().ToList();
                for (int i = 0; i < Codes.Count; i++)
                {
                    cdmis_Classes_buf = (from temp in cdmis_Classes
                                         where temp.藥碼 == Codes[i]
                                         select temp).ToList();
                    drug_Classes_buf = (from temp in drug_Classes
                                        where temp.藥碼 == Codes[i]
                                        select temp).ToList();

                    SheetClass sheetClass = loadText.JsonDeserializet<SheetClass>();
                    sheetClass.Name = $"{Codes[i]}";
                    sheetClass.ReplaceCell(1, 1, $"{cdmis_Classes_buf[0].藥名}");
                    sheetClass.ReplaceCell(1, 4, $"{cdmis_Classes_buf[0].藥名}");
                    if (drug_Classes_buf.Count > 0)
                    {
                        sheetClass.ReplaceCell(1, 7, $"衛署藥製字第{drug_Classes_buf[0].藥師證字}");
                        sheetClass.ReplaceCell(4, 1, $"{drug_Classes_buf[0].管制級別}");

                    }
                    for (int k = 0; k < cdmis_Classes_buf.Count; k++)
                    {
                        sheetClass.AddNewCell_Webapi(8 + k, 0, $"{cdmis_Classes_buf[k].交易時間}", "微軟正黑體", 12, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(8 + k, 1, $"{cdmis_Classes_buf[k].收支原因}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(8 + k, 2, $"{cdmis_Classes_buf[k].收入數量}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(8 + k, 3, $"{cdmis_Classes_buf[k].收入藥品批號_效期}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(8 + k, 4, $"{cdmis_Classes_buf[k].支出數量}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(8 + k, 5, $"{cdmis_Classes_buf[k].結存數量}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(8 + k, 6, $"{cdmis_Classes_buf[k].病歷號}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(8 + k, 7, $"{cdmis_Classes_buf[k].調配藥師}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(8 + k, 8, $"{cdmis_Classes_buf[k].調配藥師字號}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(8 + k, 9, $"{cdmis_Classes_buf[k].點斑藥師}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(8 + k, 10, $"{cdmis_Classes_buf[k].核對藥師}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                        sheetClass.AddNewCell_Webapi(8 + k, 11, $"{cdmis_Classes_buf[k].備註}", "微軟正黑體", 14, false, NPOI_Color.BLACK, 430, NPOI.SS.UserModel.HorizontalAlignment.Left, NPOI.SS.UserModel.VerticalAlignment.Bottom, NPOI.SS.UserModel.BorderStyle.Thin);
                    }
                    sheetClasses.Add(sheetClass);
                }

                // 構建文件路徑
                byte[] excelData = sheetClasses.NPOI_GetBytes(Excel_Type.xls);
                Stream stream = new MemoryStream(excelData);
                return await Task.FromResult(File(stream, xls_command, $"{value}_收支結存簿冊.xls"));

            }
            catch (Exception e)
            {
                return StatusCode(500, "內部伺服器錯誤");
            }
        }
    }
}
