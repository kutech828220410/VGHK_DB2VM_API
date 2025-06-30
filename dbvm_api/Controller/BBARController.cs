using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using Basic;
using SQLUI;
using System.Text.Json.Serialization;
using Oracle.ManagedDataAccess.Client;
using HIS_DB_Lib;
using System.Data.Odbc;

namespace DB2VM
{
    [Route("dbvm/[controller]")]
    [ApiController]
    public class BBARController : ControllerBase
    {
    
        private enum enum_藥袋17_1
        {
            病房 = 1,
            藥品名稱 = 2,
            中文名稱 = 3,
            商品名稱 = 4,
            包裝單位 = 5,
            總量 = 7,
            頻次 = 8,
            病歷號 = 10,
            病人姓名 = 11,
            藥品碼 = 12,
            開方日期 = 15,
            開方時間 = 16,
        }
        private enum enum_藥袋18_1
        {
            領藥號 = 1,
            病房 = 2,
            藥品名稱 = 3,
            中文名稱 = 4,
            商品名稱 = 5,
            包裝單位 = 6,
            總量 = 8,
            頻次 = 9,
            病歷號 = 11,
            病人姓名 = 12,
            藥品碼 = 13,
            開方日期 = 16,
            開方時間 = 17,
        }
        private enum enum_藥袋19_1
        {
            領藥號 = 1,
            病房 = 2,
            藥品名稱 = 4,
            中文名稱 = 5,
            商品名稱 = 6,
            包裝單位 = 7,
            總量 = 9,
            頻次 = 10,
            病歷號 = 12,
            病人姓名 = 13,
            藥品碼 = 14,
            開方日期 = 17,
            開方時間 = 18,
        }
        private enum enum_藥袋12_1
        {
            領藥號 = 1,
            病房 = 2,
            藥品名稱 = 3,
            包裝單位 = 4,
            總量 = 5,
            頻次 = 6,
            病歷號 = 8,
            藥品碼 = 9,
            開方日期 = 10,
            開方時間 = 11,
        }
        private enum enum_藥袋13_1
        {
            領藥號 = 1,
            病房 = 2,
            病床 = 3,
            藥品名稱 = 4,
            總量 = 6,
            頻次 = 7,
            病歷號 = 9,
            藥品碼 = 10,
            開方日期 = 11,
            開方時間 = 12,
        }
     
        public enum enum_出院帶藥
        {
            藥袋類型,       // [0]
            身分證號碼,     // [1]
            空欄位_2,       // [2] 無欄位名稱，先以佔位命名
            出生日期,       // [3]
            病人姓名,       // [4]
            空欄位_5,       // [5] 無欄位名稱，先以佔位命名
            單次劑量,       // [6]
            頻次,           // [7]
            途徑,           // [8]
            總量,         // [9]
            病歷號,       // [10]
            開方日期,       // [11]
            空欄位_12,      // [12]
            開方時間,      // [13]
            藥品碼,         // [14]
            領藥號,         // [15]
            備註            // [16]
        }
        public enum enum_管制藥品退藥
        {
            藥袋類型,       // [0]
            空欄位_1,     // [1]
            空欄位_2,       // [2] 無欄位名稱，先以佔位命名
            空欄位_3,      // [3]
            病房號,       // [4]
            空欄位_5,       // [5] 無欄位名稱，先以佔位命名
            空欄位_6,       // [6]
            空欄位_7,           // [7]
            空欄位_8,           // [8]
            總量,         // [9]
            空欄位_10,       // [10]
            開方日期,       // [11]
            空欄位_12,      // [12]
            空欄位_13,      // [13]
            藥品碼,         // [14]
            操作代碼,         // [15]
            備註            // [16]
        }
        public enum enum_管制藥品撥發
        {
            藥袋類型,       // [0]
            空欄位_1,     // [1]
            空欄位_2,       // [2] 無欄位名稱，先以佔位命名
            空欄位_3,      // [3]
            病房號,       // [4]
            空欄位_5,       // [5] 無欄位名稱，先以佔位命名
            空欄位_6,       // [6]
            空欄位_7,           // [7]
            空欄位_8,           // [8]
            總量,         // [9]
            空欄位_10,       // [10]
            開方日期,       // [11]
            空欄位_12,      // [12]
            空欄位_13,      // [13]
            藥品碼,         // [14]
            操作代碼,         // [15]
            備註            // [16]
        }

        static string MySQL_server = $"{ConfigurationManager.AppSettings["MySQL_server"]}";
        static string MySQL_database = $"{ConfigurationManager.AppSettings["MySQL_database"]}";
        static string MySQL_userid = $"{ConfigurationManager.AppSettings["MySQL_user"]}";
        static string MySQL_password = $"{ConfigurationManager.AppSettings["MySQL_password"]}";
        static string MySQL_port = $"{ConfigurationManager.AppSettings["MySQL_port"]}";

        private SQLControl sQLControl_UDSDBBCM = new SQLControl(MySQL_server, MySQL_database, "UDSDBBCM", MySQL_userid, MySQL_password, (uint)MySQL_port.StringToInt32(), MySql.Data.MySqlClient.MySqlSslMode.None);
        private string API_Server = "http://127.0.0.1:4433";
        [HttpGet]
        public string GET(string? barcode)
        {
            MyTimerBasic myTimer = new MyTimerBasic();
            myTimer.StartTickTime(50000);
            returnData returnData = new returnData();
            returnData.Method = "get_order";
            try
            {
                OrderClass orderClass = ParseBarcodeToOrderClass(barcode, out returnData);
                if(returnData.Code != 200)
                {
                    return returnData.JsonSerializationt(true);
                }
               

                List<OrderClass> orderClasses = OrderClass.get_by_barcode(API_Server, orderClass.藥袋條碼);
                List<OrderClass> orderClasses_add = new List<OrderClass>();
                List<OrderClass> orderClasses_update = new List<OrderClass>();
                if (orderClasses.Count == 0)
                {
                    orderClass.狀態 = "未過帳";
                    orderClasses_add.Add(orderClass);
                    orderClasses = OrderClass.add(API_Server, orderClasses_add);
                }
                returnData.TimeTaken = myTimer.ToString();
                returnData.Code = 200;
                returnData.Value = barcode;
                returnData.Data = orderClasses;
                returnData.Result = $"取得醫令成功,新增<{orderClasses_add.Count}>筆,修改<{orderClasses_update.Count}>筆,共<{orderClasses.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        [Route("opd_order")]
        [HttpGet]
        public string GET_opd_order(string? barcode)
        {
            MyTimerBasic myTimer = new MyTimerBasic();
            myTimer.StartTickTime(50000);
            returnData returnData = new returnData();
            returnData.Method = "opd_order";
            try
            {
                OrderClass orderClass = ParseBarcodeToOrderClass(barcode, out returnData);
                if (returnData.Code != 200)
                {
                    return returnData.JsonSerializationt(true);
                }
                int type = 0;
                List<OrderClass> orderClasses = new List<OrderClass>();
                List<OrderClass> orderClasses_add = new List<OrderClass>();
                orderClasses = OrderClass.get_by_barcode(API_Server, barcode);
                if (orderClasses.Count > 0)
                {
                    returnData.TimeTaken = myTimer.ToString();
                    returnData.Code = 200;
                    returnData.Value = barcode;
                    returnData.Data = orderClasses;
                    returnData.Result = $"取得醫令成功,共<{orderClasses.Count}>筆";
                    return returnData.JsonSerializationt(true);
                }

                orderClass.GUID = Guid.NewGuid().ToString();
                orderClass.藥局代碼 = "OPD";
                orderClasses_add.Add(orderClass);
                OrderClass.add(API_Server, orderClasses_add);

                returnData.TimeTaken = myTimer.ToString();
                returnData.Code = 200;
                returnData.Value = barcode;
                returnData.Data = orderClasses_add;
                returnData.Result = $"取得醫令成功!共<{orderClasses_add.Count}>筆,新增<{orderClasses_add.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        [Route("ud_order")]
        [HttpGet]
        public string GET_ud_order(string? barcode)
        {
            MyTimerBasic myTimer = new MyTimerBasic();
            myTimer.StartTickTime(50000);
            returnData returnData = new returnData();
            returnData.Method = "ud_order";
            try
            {
                OrderClass orderClass = ParseBarcodeToOrderClass(barcode, out returnData);
                if (returnData.Code != 200)
                {
                    return returnData.JsonSerializationt(true);
                }
                int type = 0;
                List<OrderClass> orderClasses = new List<OrderClass>();
                List<OrderClass> orderClasses_add = new List<OrderClass>();
                orderClasses = OrderClass.get_by_barcode(API_Server, barcode);
                if (orderClasses.Count > 0)
                {
                    returnData.TimeTaken = myTimer.ToString();
                    returnData.Code = 200;
                    returnData.Value = barcode;
                    returnData.Data = orderClasses;
                    returnData.Result = $"取得醫令成功,共<{orderClasses.Count}>筆";
                    return returnData.JsonSerializationt(true);
                }

                orderClass.GUID = Guid.NewGuid().ToString();
                orderClass.藥局代碼 = "UD";
                orderClasses_add.Add(orderClass);
                OrderClass.add(API_Server, orderClasses_add);

                returnData.TimeTaken = myTimer.ToString();
                returnData.Code = 200;
                returnData.Value = barcode;
                returnData.Data = orderClasses_add;
                returnData.Result = $"取得醫令成功!共<{orderClasses_add.Count}>筆,新增<{orderClasses_add.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }
        [Route("pher_order")]
        [HttpGet]
        public string GET_pher_order(string? barcode)
        {
            MyTimerBasic myTimer = new MyTimerBasic();
            myTimer.StartTickTime(50000);
            returnData returnData = new returnData();
            returnData.Method = "pher_order";
            try
            {
                OrderClass orderClass = ParseBarcodeToOrderClass(barcode, out returnData);
                if (returnData.Code != 200)
                {
                    return returnData.JsonSerializationt(true);
                }
                int type = 0;
                List<OrderClass> orderClasses = new List<OrderClass>();
                List<OrderClass> orderClasses_add = new List<OrderClass>();
                orderClasses = OrderClass.get_by_barcode(API_Server, barcode);
                if (orderClasses.Count > 0)
                {
                    returnData.TimeTaken = myTimer.ToString();
                    returnData.Code = 200;
                    returnData.Value = barcode;
                    returnData.Data = orderClasses;
                    returnData.Result = $"取得醫令成功,共<{orderClasses.Count}>筆";
                    return returnData.JsonSerializationt(true);
                }

                orderClass.GUID = Guid.NewGuid().ToString();
                orderClass.藥局代碼 = "PHER";
                orderClasses_add.Add(orderClass);
                OrderClass.add(API_Server, orderClasses_add);

                returnData.TimeTaken = myTimer.ToString();
                returnData.Code = 200;
                returnData.Value = barcode;
                returnData.Data = orderClasses_add;
                returnData.Result = $"取得醫令成功!共<{orderClasses_add.Count}>筆,新增<{orderClasses_add.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }

        //淘汰
        [Route("order_pher")]
        [HttpGet]
        public string GET_order_pher(string? barcode)
        {
            MyTimerBasic myTimer = new MyTimerBasic();
            myTimer.StartTickTime(50000);
            returnData returnData = new returnData();
            returnData.Method = "order_pher";
            try
            {
                List<OrderClass> orderClasses_pher = OrderClass.get_by_barcode(API_Server, barcode);
                if (orderClasses_pher.Count > 0)
                {

                    returnData.TimeTaken = myTimer.ToString();
                    returnData.Code = 200;
                    returnData.Value = barcode;
                    returnData.Data = orderClasses_pher;
                    returnData.Result = $"取得醫令成功!共<{orderClasses_pher.Count}>筆";
                    return returnData.JsonSerializationt(true);
                }

                SQLControl sQLControl_醫囑資料_protal = new SQLControl("127.0.0.1", "protal", "order_list", "user", "66437068", 3306, MySql.Data.MySqlClient.MySqlSslMode.None);
                List<OrderClass> orderClasses = new List<OrderClass>();
                OrderClass orderClass = ParseBarcodeToOrderClass(barcode, out returnData);
                if (returnData.Code != 200)
                {
                    return returnData.JsonSerializationt(true);
                }
                List<object[]> list_醫令資料 = new List<object[]>();
                if (orderClass.領藥號.StringIsEmpty())
                {
                    string[] serch_colName = { "藥品碼", "病歷號", "病房", "交易量", "頻次", "開方日期" };
                    string[] serch_colValue = { orderClass.藥品碼, orderClass.病歷號, orderClass.病房, orderClass.交易量, orderClass.頻次, orderClass.開方日期 };
                    list_醫令資料 = sQLControl_醫囑資料_protal.GetRowsByDefult(null, serch_colName, serch_colValue);
                }
                else
                {
                    string[] serch_colName = { "領藥號", "藥品碼" };
                    string[] serch_colValue = { orderClass.領藥號, orderClass.藥品碼 };
                    DateTime dateTime = orderClass.開方日期.StringToDateTime();
                    list_醫令資料 = sQLControl_醫囑資料_protal.GetRowsByDefult(null, serch_colName, serch_colValue);
                    list_醫令資料 = list_醫令資料.GetRowsInDate((int)enum_醫囑資料.開方日期, dateTime);
                }
                if (list_醫令資料.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"未搜尋到醫令資料!";
                    return returnData.JsonSerializationt(true);
                }
                object[] order_ary = orderClass.ClassToSQL<OrderClass, enum_醫囑資料>();
                if (order_ary.Length == 19)
                {
                    list_醫令資料[0][(int)enum_醫囑資料.病房] = orderClass.病房;
                    sQLControl_醫囑資料_protal.UpdateByDefulteExtra(null, list_醫令資料);
                }
                if (order_ary.Length == 13)
                {
                    list_醫令資料[0][(int)enum_醫囑資料.病房] = orderClass.病房;
                    sQLControl_醫囑資料_protal.UpdateByDefulteExtra(null, list_醫令資料);
                }
                orderClasses = list_醫令資料.SQLToClass<OrderClass, enum_醫囑資料>();

            
                for (int i = 0; i < orderClasses.Count; i++)
                {
                    string pri_key = orderClasses[i].PRI_KEY;
                    string[] datas = pri_key.Split(";");
                    orderClasses[i].藥袋條碼 = barcode;
                    if (datas.Length >= 5)
                    {
                        orderClasses[i].就醫類別 = datas[1];
                        orderClasses[i].就醫序號 = datas[2];
                        orderClasses[i].住院序號 = datas[3];
                    }
                    orderClasses[i].狀態 = "未過帳";
                }
                OrderClass.add(API_Server, orderClasses);

                returnData.TimeTaken = myTimer.ToString();
                returnData.Code = 200;
                returnData.Value = barcode;
                returnData.Data = orderClasses;
                returnData.Result = $"[初次]取得醫令成功!共<{orderClasses.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }




        [Route("protal")]
        [HttpGet]
        public string GET_protal(string? barcode)
        {
            MyTimerBasic myTimer = new MyTimerBasic();
            myTimer.StartTickTime(50000);
            returnData returnData = new returnData();
            returnData.Method = "get_order";
            try
            {
                int type = -1;
                SQLControl sQLControl_醫囑資料 = new SQLControl("127.0.0.1", "protal", "order_list", "user", "66437068", 3306, MySql.Data.MySqlClient.MySqlSslMode.None);
                List<OrderClass> orderClasses = new List<OrderClass>();
                OrderClass orderClass = new OrderClass();
                string[] order_ary = barcode.Split('~');
                type = 1;
                if (order_ary.Length <= 2)
                {
                    order_ary = barcode.Split(';');
                    type = 2;
                }
                for (int i = 0; i < order_ary.Length; i++)
                {
                    order_ary[i] = order_ary[i].Trim();
                  
                }
                if(type == 1)
                {
                    if (order_ary.Length == 17)
                    {

                        orderClass.GUID = Guid.NewGuid().ToString();
                        orderClass.藥袋條碼 = barcode;
                        orderClass.藥品碼 = order_ary[(int)enum_藥袋17_1.藥品碼].ObjectToString();
                        orderClass.藥品名稱 = order_ary[(int)enum_藥袋17_1.藥品名稱].ObjectToString();
                        orderClass.劑量單位 = order_ary[(int)enum_藥袋17_1.包裝單位].ObjectToString();
                        orderClass.交易量 = (order_ary[(int)enum_藥袋17_1.總量].ObjectToString().Trim().StringToInt32() * -1).ToString();
                        orderClass.頻次 = order_ary[(int)enum_藥袋17_1.頻次].ObjectToString();
                        orderClass.病歷號 = order_ary[(int)enum_藥袋17_1.病歷號].ObjectToString();
                        orderClass.病人姓名 = order_ary[(int)enum_藥袋17_1.病人姓名].ObjectToString();
                        orderClass.病房 = order_ary[(int)enum_藥袋17_1.病房].ObjectToString();
                        orderClass.病房 = orderClass.病房.Split('-')[0].Trim();
                        string 日期 = order_ary[(int)enum_藥袋17_1.開方日期].ObjectToString();
                        string 時間 = order_ary[(int)enum_藥袋17_1.開方時間].ObjectToString();
                        string date_str = $"{日期} {時間}";
                        orderClass.開方日期 = date_str;
                    }
                    if (order_ary.Length == 18)
                    {

                        orderClass.GUID = Guid.NewGuid().ToString();
                        orderClass.藥袋條碼 = barcode;
                        orderClass.藥品碼 = order_ary[(int)enum_藥袋18_1.藥品碼].ObjectToString();
                        orderClass.領藥號 = order_ary[(int)enum_藥袋18_1.領藥號].ObjectToString();
                        orderClass.藥品名稱 = order_ary[(int)enum_藥袋18_1.藥品名稱].ObjectToString();
                        orderClass.劑量單位 = order_ary[(int)enum_藥袋18_1.包裝單位].ObjectToString();
                        orderClass.交易量 = (order_ary[(int)enum_藥袋18_1.總量].ObjectToString().Trim().StringToInt32() * -1).ToString();
                        orderClass.頻次 = order_ary[(int)enum_藥袋18_1.頻次].ObjectToString();
                        orderClass.病歷號 = order_ary[(int)enum_藥袋18_1.病歷號].ObjectToString();
                        orderClass.病人姓名 = order_ary[(int)enum_藥袋18_1.病人姓名].ObjectToString();
                        orderClass.病房 = order_ary[(int)enum_藥袋18_1.病房].ObjectToString();
                        orderClass.病房 = orderClass.病房.Split('-')[0].Trim();
                        string 日期 = order_ary[(int)enum_藥袋18_1.開方日期].ObjectToString();
                        string 時間 = order_ary[(int)enum_藥袋18_1.開方時間].ObjectToString();
                        string date_str = $"{日期} {時間}";
                        orderClass.開方日期 = date_str;
                    }
                    if (order_ary.Length == 19)
                    {

                        orderClass.GUID = Guid.NewGuid().ToString();
                        orderClass.藥袋條碼 = barcode;
                        orderClass.藥品碼 = order_ary[(int)enum_藥袋19_1.藥品碼].ObjectToString();
                        orderClass.領藥號 = order_ary[(int)enum_藥袋19_1.領藥號].ObjectToString();
                        orderClass.藥品名稱 = order_ary[(int)enum_藥袋19_1.藥品名稱].ObjectToString();
                        orderClass.劑量單位 = order_ary[(int)enum_藥袋19_1.包裝單位].ObjectToString();
                        orderClass.交易量 = (order_ary[(int)enum_藥袋19_1.總量].ObjectToString().Trim().StringToInt32() * -1).ToString();
                        orderClass.頻次 = order_ary[(int)enum_藥袋19_1.頻次].ObjectToString();
                        orderClass.病歷號 = order_ary[(int)enum_藥袋19_1.病歷號].ObjectToString();
                        orderClass.病人姓名 = order_ary[(int)enum_藥袋19_1.病人姓名].ObjectToString();
                        orderClass.病房 = order_ary[(int)enum_藥袋19_1.病房].ObjectToString();
                        orderClass.病房 = orderClass.病房.Split('-')[0].Trim();
                        string 日期 = order_ary[(int)enum_藥袋19_1.開方日期].ObjectToString();
                        string 時間 = order_ary[(int)enum_藥袋19_1.開方時間].ObjectToString();
                        string date_str = $"{日期} {時間}";
                        orderClass.開方日期 = date_str;
                    }
                    if (order_ary.Length == 12)
                    {
                        orderClass.GUID = Guid.NewGuid().ToString();
                        orderClass.藥袋條碼 = barcode;
                        orderClass.領藥號 = order_ary[(int)enum_藥袋12_1.領藥號].ObjectToString();
                        orderClass.藥品碼 = order_ary[(int)enum_藥袋12_1.藥品碼].ObjectToString();
                        orderClass.藥品名稱 = order_ary[(int)enum_藥袋12_1.藥品名稱].ObjectToString();
                        orderClass.劑量單位 = order_ary[(int)enum_藥袋12_1.包裝單位].ObjectToString();
                        orderClass.交易量 = (order_ary[(int)enum_藥袋12_1.總量].ObjectToString().Trim().StringToInt32() * -1).ToString();
                        orderClass.頻次 = order_ary[(int)enum_藥袋12_1.頻次].ObjectToString();
                        orderClass.病歷號 = order_ary[(int)enum_藥袋12_1.病歷號].ObjectToString();
                        orderClass.病房 = order_ary[(int)enum_藥袋12_1.病房].ObjectToString();
                        orderClass.病房 = orderClass.病房.Split('-')[0].Trim();
                        string 日期 = order_ary[(int)enum_藥袋12_1.開方日期].ObjectToString();
                        string 時間 = order_ary[(int)enum_藥袋12_1.開方時間].ObjectToString();
                        string date_str = $"{日期} {時間}";
                        orderClass.開方日期 = date_str;
                    }
                    if (order_ary.Length == 13)
                    {
                        orderClass.GUID = Guid.NewGuid().ToString();
                        orderClass.藥袋條碼 = barcode;
                        orderClass.領藥號 = order_ary[(int)enum_藥袋13_1.領藥號].ObjectToString();
                        orderClass.藥品碼 = order_ary[(int)enum_藥袋13_1.藥品碼].ObjectToString();
                        orderClass.藥品名稱 = order_ary[(int)enum_藥袋13_1.藥品名稱].ObjectToString();
                        orderClass.交易量 = (order_ary[(int)enum_藥袋13_1.總量].ObjectToString().Trim().StringToInt32() * -1).ToString();
                        orderClass.頻次 = order_ary[(int)enum_藥袋13_1.頻次].ObjectToString();
                        orderClass.病歷號 = order_ary[(int)enum_藥袋13_1.病歷號].ObjectToString();
                        orderClass.病房 = order_ary[(int)enum_藥袋13_1.病房].ObjectToString();
                        orderClass.病房 = orderClass.病房.Split('-')[0].Trim();
                        string 日期 = order_ary[(int)enum_藥袋13_1.開方日期].ObjectToString();
                        string 時間 = order_ary[(int)enum_藥袋13_1.開方時間].ObjectToString();
                        string date_str = $"{日期} {時間}";
                        orderClass.開方日期 = date_str;
                    }
                }
                else if (type == 2)
                {
                  
                    returnData.Code = -200;
                    returnData.Result = $"Barcode格式異常!";
                    return returnData.JsonSerializationt(true);
                }
                else
                {
                    returnData.Code = -200;
                    returnData.Result = $"Barcode格式異常!";
                    return returnData.JsonSerializationt(true);
                }
                List<object[]> list_醫令資料 = new List<object[]>();
                if (orderClass.領藥號.StringIsEmpty())
                {
                    string[] serch_colName = { "藥品碼", "病歷號", "病房", "交易量", "頻次", "開方日期" };
                    string[] serch_colValue = { orderClass.藥品碼, orderClass.病歷號, orderClass.病房, orderClass.交易量, orderClass.頻次, orderClass.開方日期 };
                    list_醫令資料 = sQLControl_醫囑資料.GetRowsByDefult(null, serch_colName, serch_colValue);
                }
                else
                {
                    string[] serch_colName = { "領藥號", "藥品碼" };
                    string[] serch_colValue = { orderClass.領藥號 , orderClass.藥品碼 };
                    DateTime dateTime = orderClass.開方日期.StringToDateTime();
                    list_醫令資料 = sQLControl_醫囑資料.GetRowsByDefult(null, serch_colName, serch_colValue);
                    list_醫令資料 = list_醫令資料.GetRowsInDate((int)enum_醫囑資料.開方日期, dateTime);
                }
                if(list_醫令資料.Count == 0)
                {
                    returnData.Code = -200;
                    returnData.Result = $"未搜尋到醫令資料!";
                    return returnData.JsonSerializationt(true);
                }
                if (order_ary.Length == 19)
                {
                    list_醫令資料[0][(int)enum_醫囑資料.病房] = orderClass.病房;
                    sQLControl_醫囑資料.UpdateByDefulteExtra(null, list_醫令資料);
                }
                if (order_ary.Length == 13)
                {
                    list_醫令資料[0][(int)enum_醫囑資料.病房] = orderClass.病房;
                    sQLControl_醫囑資料.UpdateByDefulteExtra(null, list_醫令資料);
                }
                orderClasses = list_醫令資料.SQLToClass<OrderClass,enum_醫囑資料>();

                for (int i = 0; i < orderClasses.Count; i++)
                {
                    string pri_key = orderClasses[i].PRI_KEY;
                    string[] datas = pri_key.Split(";");
                    if (datas.Length >= 5)
                    {
                        orderClasses[i].就醫類別 = datas[1];
                        orderClasses[i].就醫序號 = datas[2];
                        orderClasses[i].住院序號 = datas[3];
                    }
                }
                returnData.TimeTaken = myTimer.ToString();
                returnData.Code = 200;
                returnData.Value = barcode;
                returnData.Data = orderClasses;
                returnData.Result = $"取得醫令成功!共<{orderClasses.Count}>筆";
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Code = -200;
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }

        [Route("order_checkin")]
        [HttpPost]
        public string POST_order_checkin(returnData returnData)
        {
            MyTimerBasic myTimer = new MyTimerBasic();
            myTimer.StartTickTime(50000);
            returnData.Method = "POST_order_checkin";
            string ODBC_string = "driver={IBM DB2 ODBC DRIVER};Database=DBDSNP;Hostname=192.168.51.101;Protocol=TCPIP;Port=50000;Uid=APUD07;Pwd=UD07AP;";
            string[] datas = returnData.Value.Split(';');
            try
            {
                OrderClass orderClass = returnData.Data.ObjToClass<OrderClass>();
                if (orderClass == null)
                {
                    returnData.Result = $"傳入資料錯誤!";
                    return returnData.JsonSerializationt(true);
                }
                System.Data.Odbc.OdbcConnection odbcConnection = new OdbcConnection(ODBC_string);
                odbcConnection.Open();
                System.Data.Odbc.OdbcCommand MyDB2Command = odbcConnection.CreateCommand();
                string HHISNUM = orderClass.病歷號;
                string HCASETYP = orderClass.就醫類別;
                string HCASENO = orderClass.就醫序號;
                string UDORDSEQ = orderClass.住院序號;
                string ZUDGETLO = DateTime.Now.ToDateTimeString_6().Replace('/','-');
                DateTime datetime_temp = DateTime.Now.StringToDateTime();
                string UDDSDATE = $"{datetime_temp.Year.ToString("0000")}-{datetime_temp.Month.ToString("00")}-{datetime_temp.Day.ToString("00")}";//藥師刷入日期
                string UDDSTIME = $"{datetime_temp.Hour.ToString("00")}:{datetime_temp.Minute.ToString("00")}:{datetime_temp.Second.ToString("00")}";//藥師刷入時間
                string UDDSCARD = $"{orderClass.藥師ID}";//藥師ID
                if (UDDSCARD.Length > 4) UDDSCARD = "";
                string UDGDATE = $"0001-01-01";//勤務領藥日期
                string UDGTIME = $"00:00:00";//勤務領藥時間
                string UDCARDNO = $"";//勤務領藥ID
                if (UDCARDNO.Length > 4) UDCARDNO = "";
                string UDUSRNAM =$"";//勤務領藥姓名

                MyDB2Command.CommandText = $"insert into UD.UDGETLOG (HHISNUM, HCASETYP, HCASENO, UDORDSEQ, ZUDGETLO, UDDSDATE, UDDSTIME, UDDSCARD, UDGDATE, UDGTIME, UDGLOC, UDCARDNO, UDUSRNAM, HID) " +
                    $"values ('{HHISNUM}','{HCASETYP}','{HCASENO}','{UDORDSEQ}','{ZUDGETLO}','{UDDSDATE}','{UDDSTIME}','{UDDSCARD}','{UDGDATE}','{UDGTIME}','PHRE','{UDCARDNO}','{UDUSRNAM}','1A0')";
                MyDB2Command.ExecuteNonQuery();
                returnData.Result = "藥局刷入HIS成功!";

                odbcConnection.Close();
                return returnData.JsonSerializationt(true);
            }
            catch(Exception e)
            {
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }

        [Route("order_takeout")]
        [HttpPost]
        public string POST_order_takeout(returnData returnData)
        {
            MyTimerBasic myTimer = new MyTimerBasic();
            myTimer.StartTickTime(50000);
            returnData.Method = "POST_order_takeout";
            string ODBC_string = "driver={IBM DB2 ODBC DRIVER};Database=DBDSNP;Hostname=192.168.51.101;Protocol=TCPIP;Port=50000;Uid=APUD07;Pwd=UD07AP;";
            string[] datas = returnData.Value.Split(';');
            try
            {
                OrderClass orderClass = returnData.Data.ObjToClass<OrderClass>();
                if (orderClass == null)
                {
                    returnData.Result = $"傳入資料錯誤!";
                    return returnData.JsonSerializationt(true);
                }
                System.Data.Odbc.OdbcConnection odbcConnection = new OdbcConnection(ODBC_string);
                odbcConnection.Open();
                System.Data.Odbc.OdbcCommand MyDB2Command = odbcConnection.CreateCommand();
                string HHISNUM = orderClass.病歷號;
                string HCASETYP = orderClass.就醫類別;
                string HCASENO = orderClass.就醫序號;
                string UDORDSEQ = orderClass.住院序號;
                string ZUDGETLO = DateTime.Now.ToDateTimeString_6().Replace('/', '-');
                DateTime datetime_temp = DateTime.Now.StringToDateTime();
                string UDDSDATE = $"0001-01-01";//藥師刷入日期
                string UDDSTIME = $"00:00:00";//藥師刷入時間
                string UDDSCARD = $"";//藥師ID
                if (UDDSCARD.Length > 4) UDDSCARD = "";
                string UDGDATE = $"{datetime_temp.Year.ToString("0000")}-{datetime_temp.Month.ToString("00")}-{datetime_temp.Day.ToString("00")}";//勤務領藥日期
                string UDGTIME = $"{datetime_temp.Hour.ToString("00")}:{datetime_temp.Minute.ToString("00")}:{datetime_temp.Second.ToString("00")}";//勤務領藥時間
                string UDCARDNO = $"{orderClass.領藥ID}";//勤務領藥ID
                if (UDCARDNO.Length > 4) UDCARDNO = "";
                string UDUSRNAM = $"{orderClass.領藥姓名}";//勤務領藥姓名

                MyDB2Command.CommandText = $"insert into UD.UDGETLOG (HHISNUM, HCASETYP, HCASENO, UDORDSEQ, ZUDGETLO, UDDSDATE, UDDSTIME, UDDSCARD, UDGDATE, UDGTIME, UDGLOC, UDCARDNO, UDUSRNAM, HID) " +
                    $"values ('{HHISNUM}','{HCASETYP}','{HCASENO}','{UDORDSEQ}','{ZUDGETLO}','{UDDSDATE}','{UDDSTIME}','{UDDSCARD}','{UDGDATE}','{UDGTIME}','PHRE','{UDCARDNO}','{UDUSRNAM}','1A0')";
                MyDB2Command.ExecuteNonQuery();
                returnData.Result = "勤務刷入HIS成功!";

                odbcConnection.Close();
                return returnData.JsonSerializationt(true);
            }
            catch (Exception e)
            {
                returnData.Result = $"{e.Message}";
                return returnData.JsonSerializationt(true);
            }
        }


        public OrderClass ParseBarcodeToOrderClass(string barcode, out returnData returnData)
        {
            returnData = new returnData();
            string[] order_ary = barcode.Split('~');
            int type = 1;

            if (order_ary.Length <= 2)
            {
                order_ary = barcode.Split(';');
                type = 2;
            }

            for (int i = 0; i < order_ary.Length; i++)
            {
                order_ary[i] = order_ary[i].Trim();
            }

            OrderClass orderClass = new OrderClass();

            try
            {
                if (type == 1)
                {
                    switch (order_ary.Length)
                    {
                        case 17:
                            orderClass.GUID = Guid.NewGuid().ToString();
                            orderClass.藥袋條碼 = barcode;
                            orderClass.藥品碼 = order_ary[(int)enum_藥袋17_1.藥品碼].ObjectToString();
                            orderClass.藥品名稱 = order_ary[(int)enum_藥袋17_1.藥品名稱].ObjectToString();
                            orderClass.劑量單位 = order_ary[(int)enum_藥袋17_1.包裝單位].ObjectToString();
                            orderClass.交易量 = (order_ary[(int)enum_藥袋17_1.總量].ObjectToString().Trim().StringToInt32() * -1).ToString();
                            orderClass.頻次 = order_ary[(int)enum_藥袋17_1.頻次].ObjectToString();
                            orderClass.病歷號 = order_ary[(int)enum_藥袋17_1.病歷號].ObjectToString();
                            orderClass.病人姓名 = order_ary[(int)enum_藥袋17_1.病人姓名].ObjectToString();
                            orderClass.病房 = order_ary[(int)enum_藥袋17_1.病房].ObjectToString().Split('-')[0].Trim();
                            orderClass.開方日期 = $"{order_ary[(int)enum_藥袋17_1.開方日期]} {order_ary[(int)enum_藥袋17_1.開方時間]}";
                            break;

                        case 18:
                            orderClass.GUID = Guid.NewGuid().ToString();
                            orderClass.藥袋條碼 = barcode;
                            orderClass.藥品碼 = order_ary[(int)enum_藥袋18_1.藥品碼].ObjectToString();
                            orderClass.領藥號 = order_ary[(int)enum_藥袋18_1.領藥號].ObjectToString();
                            orderClass.藥品名稱 = order_ary[(int)enum_藥袋18_1.藥品名稱].ObjectToString();
                            orderClass.劑量單位 = order_ary[(int)enum_藥袋18_1.包裝單位].ObjectToString();
                            orderClass.交易量 = (order_ary[(int)enum_藥袋18_1.總量].ObjectToString().Trim().StringToInt32() * -1).ToString();
                            orderClass.頻次 = order_ary[(int)enum_藥袋18_1.頻次].ObjectToString();
                            orderClass.病歷號 = order_ary[(int)enum_藥袋18_1.病歷號].ObjectToString();
                            orderClass.病人姓名 = order_ary[(int)enum_藥袋18_1.病人姓名].ObjectToString();
                            orderClass.病房 = order_ary[(int)enum_藥袋18_1.病房].ObjectToString().Split('-')[0].Trim();
                            orderClass.開方日期 = $"{order_ary[(int)enum_藥袋18_1.開方日期]} {order_ary[(int)enum_藥袋18_1.開方時間]}";
                            break;

                        case 19:
                            orderClass.GUID = Guid.NewGuid().ToString();
                            orderClass.藥袋條碼 = barcode;
                            orderClass.藥品碼 = order_ary[(int)enum_藥袋19_1.藥品碼].ObjectToString();
                            orderClass.領藥號 = order_ary[(int)enum_藥袋19_1.領藥號].ObjectToString();
                            orderClass.藥品名稱 = order_ary[(int)enum_藥袋19_1.藥品名稱].ObjectToString();
                            orderClass.劑量單位 = order_ary[(int)enum_藥袋19_1.包裝單位].ObjectToString();
                            orderClass.交易量 = (order_ary[(int)enum_藥袋19_1.總量].ObjectToString().Trim().StringToInt32() * -1).ToString();
                            orderClass.頻次 = order_ary[(int)enum_藥袋19_1.頻次].ObjectToString();
                            orderClass.病歷號 = order_ary[(int)enum_藥袋19_1.病歷號].ObjectToString();
                            orderClass.病人姓名 = order_ary[(int)enum_藥袋19_1.病人姓名].ObjectToString();
                            orderClass.病房 = order_ary[(int)enum_藥袋19_1.病房].ObjectToString().Split('-')[0].Trim();
                            orderClass.開方日期 = $"{order_ary[(int)enum_藥袋19_1.開方日期]} {order_ary[(int)enum_藥袋19_1.開方時間]}";
                            break;

                        case 12:
                            orderClass.GUID = Guid.NewGuid().ToString();
                            orderClass.藥袋條碼 = barcode;
                            orderClass.領藥號 = order_ary[(int)enum_藥袋12_1.領藥號].ObjectToString();
                            orderClass.藥品碼 = order_ary[(int)enum_藥袋12_1.藥品碼].ObjectToString();
                            orderClass.藥品名稱 = order_ary[(int)enum_藥袋12_1.藥品名稱].ObjectToString();
                            orderClass.劑量單位 = order_ary[(int)enum_藥袋12_1.包裝單位].ObjectToString();
                            orderClass.交易量 = (order_ary[(int)enum_藥袋12_1.總量].ObjectToString().Trim().StringToInt32() * -1).ToString();
                            orderClass.頻次 = order_ary[(int)enum_藥袋12_1.頻次].ObjectToString();
                            orderClass.病歷號 = order_ary[(int)enum_藥袋12_1.病歷號].ObjectToString();
                            orderClass.病房 = order_ary[(int)enum_藥袋12_1.病房].ObjectToString().Split('-')[0].Trim();
                            orderClass.開方日期 = $"{order_ary[(int)enum_藥袋12_1.開方日期]} {order_ary[(int)enum_藥袋12_1.開方時間]}";
                            break;

                        case 13:
                            orderClass.GUID = Guid.NewGuid().ToString();
                            orderClass.藥袋條碼 = barcode;
                            orderClass.領藥號 = order_ary[(int)enum_藥袋13_1.領藥號].ObjectToString();
                            orderClass.藥品碼 = order_ary[(int)enum_藥袋13_1.藥品碼].ObjectToString();
                            orderClass.藥品名稱 = order_ary[(int)enum_藥袋13_1.藥品名稱].ObjectToString();
                            orderClass.交易量 = (order_ary[(int)enum_藥袋13_1.總量].ObjectToString().Trim().StringToInt32() * -1).ToString();
                            orderClass.頻次 = order_ary[(int)enum_藥袋13_1.頻次].ObjectToString();
                            orderClass.病歷號 = order_ary[(int)enum_藥袋13_1.病歷號].ObjectToString();
                            orderClass.病房 = order_ary[(int)enum_藥袋13_1.病房].ObjectToString().Split('-')[0].Trim();
                            orderClass.開方日期 = $"{order_ary[(int)enum_藥袋13_1.開方日期]} {order_ary[(int)enum_藥袋13_1.開方時間]}";
                            break;

                        default:
                            returnData.Code = -200;
                            returnData.Result = $"Barcode 欄位數不符：{order_ary.Length}";
                            return null;
                    }
                }
                else if(type == 2)
                {
                    switch (order_ary.Length)
                    {
                        case 17:
                            if (order_ary[15].ObjectToString() == "A001")
                            {               
                                if (order_ary[(int)enum_管制藥品撥發.藥袋類型].ObjectToString() == "1")
                                {
                                    orderClass.藥袋類型 = "管藥撥發";
                                }
                                orderClass.GUID = Guid.NewGuid().ToString();
                                orderClass.藥品碼 = order_ary[(int)enum_管制藥品撥發.藥品碼].ObjectToString();
                                orderClass.交易量 = (order_ary[(int)enum_管制藥品撥發.總量].ObjectToString().Trim().StringToDouble() * -1).ToString();
                                orderClass.病房 = order_ary[(int)enum_管制藥品撥發.病房號].ObjectToString().Trim();
                                string 日期 = order_ary[(int)enum_管制藥品撥發.開方日期].ObjectToString();
                                string 時間 = "00:00:00";
                                string date_str = $"{日期} {時間}";
                                orderClass.開方日期 = date_str;
                                orderClass.狀態 = "未過帳";
                            }
                            else if (order_ary[15].ObjectToString() == "B001")
                            {
        
                                if (order_ary[(int)enum_管制藥品退藥.藥袋類型].ObjectToString() == "1")
                                {
                                    orderClass.藥袋類型 = "管藥退藥";
                                }
                                orderClass.GUID = Guid.NewGuid().ToString();
                                orderClass.藥品碼 = order_ary[(int)enum_管制藥品退藥.藥品碼].ObjectToString();
                                orderClass.交易量 = (order_ary[(int)enum_管制藥品退藥.總量].ObjectToString().Trim().StringToDouble() * 1).ToString();
                                orderClass.病房 = order_ary[(int)enum_管制藥品退藥.病房號].ObjectToString().Trim();
                                string 日期 = order_ary[(int)enum_管制藥品退藥.開方日期].ObjectToString();
                                string 時間 = "00:00:00";
                                string date_str = $"{日期} {時間}";
                                orderClass.開方日期 = date_str;
                                orderClass.狀態 = "未過帳";
                            }
                            else
                            {
                                orderClass.藥袋類型 = "出院帶藥";
                                orderClass.GUID = Guid.NewGuid().ToString();
                                orderClass.藥袋條碼 = barcode;
                                orderClass.藥袋類型 = order_ary[(int)enum_出院帶藥.藥袋類型].ObjectToString();
                                orderClass.藥品碼 = order_ary[(int)enum_出院帶藥.藥品碼].ObjectToString();
                                orderClass.交易量 = (order_ary[(int)enum_出院帶藥.總量].ObjectToString().Trim().StringToDouble() * -1).ToString();
                                orderClass.頻次 = order_ary[(int)enum_出院帶藥.頻次].ObjectToString();
                                orderClass.途徑 = order_ary[(int)enum_出院帶藥.途徑].ObjectToString();
                                orderClass.病歷號 = order_ary[(int)enum_出院帶藥.病歷號].ObjectToString();
                                orderClass.病人姓名 = order_ary[(int)enum_出院帶藥.病人姓名].ObjectToString();
                                string date = order_ary[(int)enum_出院帶藥.開方日期].ObjectToString();
                                string timeRaw = order_ary[(int)enum_出院帶藥.開方時間].ObjectToString();

                                string timeFormatted = "";
                                if (timeRaw.Length == 4 && timeRaw.All(char.IsDigit))
                                {
                                    string hour = timeRaw.Substring(0, 2);
                                    string minute = timeRaw.Substring(2, 2);
                                    if (int.TryParse(hour, out int h) && int.TryParse(minute, out int m))
                                    {
                                        if (h >= 0 && h <= 23 && m >= 0 && m <= 59)
                                        {
                                            timeFormatted = $"{hour}:{minute}:00";
                                        }
                                    }
                                }

                                // 組成開方日期
                                if (!string.IsNullOrEmpty(timeFormatted))
                                {
                                    orderClass.開方日期 = $"{date} {timeFormatted}";
                                }
                                else
                                {
                                    orderClass.開方日期 = date;
                                }
                            }
                          
                            break;

                        default:
                            returnData.Code = -200;
                            returnData.Result = $"Barcode 欄位數不符：{order_ary.Length}";
                            return null;
                    }
                }
                else
                {
                    returnData.Code = -200;
                    returnData.Result = $"Barcode 格式異常 (分號型)！";
                    return null;
                }
            }
            catch (Exception ex)
            {
                returnData.Code = -200;
                returnData.Result = $"解析失敗: {ex.Message}";
                return null;
            }

            orderClass.PRI_KEY = orderClass.藥袋條碼;
            medClass _medClass = medClass.get_med_clouds_by_code("http://127.0.0.1:4433", orderClass.藥品碼);
            if (_medClass != null)
            {
                orderClass.藥品名稱 = _medClass.藥品名稱;
                orderClass.劑量單位 = _medClass.包裝單位;
            }      

            returnData.Code = 200;
            returnData.Result = "解析成功";
            return orderClass;
        }

    }
}
