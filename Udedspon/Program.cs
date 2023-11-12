using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Configuration;
using Basic;
using SQLUI;
using System.Text.Json.Serialization;
using HIS_DB_Lib;

namespace Udedspon
{
    class Program
    {
        public class UdedsponClass
        {
            [JsonPropertyName("ZUDEDSPO")]
            public string 資料異動時戳 { get; set; }
            [JsonPropertyName("HCASETYP")]
            public string 就醫類別 { get; set; }
            [JsonPropertyName("HCASENO")]
            public string 就醫序號 { get; set; }
            [JsonPropertyName("UDORDSEQ")]
            public string 住院序號 { get; set; }
            [JsonPropertyName("HHISNUM")]
            public string 病歷號 { get; set; }
            [JsonPropertyName("HNAMEC")]
            public string 病患姓名 { get; set; }
            [JsonPropertyName("DSPQTY")]
            public string 配藥量 { get; set; }
            [JsonPropertyName("DSPDATE")]
            public string 日期 { get; set; }
            [JsonPropertyName("DSPTIME")]
            public string 時間 { get; set; }
            [JsonPropertyName("HNURSTA")]
            public string 病房 { get; set; }
            [JsonPropertyName("HBED")]
            public string 床號 { get; set; }
            [JsonPropertyName("ORDTYPE")]
            public string 資料種類 { get; set; }
            [JsonPropertyName("UDDRGNO")]
            public string 藥品碼 { get; set; }
            [JsonPropertyName("UDRPNAME")]
            public string 藥品名稱 { get; set; }
            [JsonPropertyName("DSPUNI")]
            public string 包裝單位 { get; set; }
            [JsonPropertyName("HID")]
            public string 醫院別 { get; set; }
            [JsonPropertyName("TAKEPCSN")]
            public string 勤務領藥號 { get; set; }
            [JsonPropertyName("UDFREQN")]
            public string 頻次 { get; set; }
            
        }
        private static System.Threading.Mutex mutex;
        static private string Api_server = "http://127.0.0.1:4433";
        static private string DiviceName = "protal";
        static void Main(string[] args)
        {
            mutex = new System.Threading.Mutex(true, "OnlyRun");
            if (mutex.WaitOne(0, false))
            {
               
            }
            else
            {
                return;
            }
            MyTimerBasic myTimer = new MyTimerBasic();
            while (true)
            {
                try
                {
                    string json_med_Refresh = Net.WEBApiGet("http://192.168.110.73:443/dbvm/BBCM");

                    List<HIS_DB_Lib.ServerSettingClass> serverSettingClasses = ServerSettingClassMethod.WebApiGet($"{Api_server}/api/ServerSetting");
                    ServerSettingClass serverSettingClass = serverSettingClasses.MyFind(DiviceName, enum_ServerSetting_Type.傳送櫃, "醫囑資料");
                    if (serverSettingClass == null) break;

                    SQLControl sQLControl_醫囑資料 = new SQLControl(serverSettingClass.Server, serverSettingClass.DBName, "order_list", serverSettingClass.User, serverSettingClass.Password, serverSettingClass.Port.StringToUInt32(), MySql.Data.MySqlClient.MySqlSslMode.None);
                    sQLControl_醫囑資料 = new SQLControl("127.0.0.1", "protal", "order_list", "user", "66437068", 3306, MySql.Data.MySqlClient.MySqlSslMode.None);
                    string url_read = "https://zwmc01p.vghks.gov.tw:4436/UDTKService/ud/udtk/ReadUdedspon";
                    string url_write = " https://zwmc01p.vghks.gov.tw:4436/UDTKService/ud/udtk/WriteUdedspon";
                    myTimer.TickStop();
                    myTimer.StartTickTime(50000);

                    UdedsponClass udedsponClass = new UdedsponClass();
                    udedsponClass.醫院別 = "1A0";
                    string json_in = udedsponClass.JsonSerializationt();
                    string json = Basic.Net.WEBApiPostJson(url_read, json_in);
                    List<UdedsponClass> udedsponClasses = json.JsonDeserializet<List<UdedsponClass>>();

                    List<OrderClass> orderClasses = new List<OrderClass>();
                    for (int i = 0; i < udedsponClasses.Count; i++)
                    {
                        OrderClass orderClass = new OrderClass();
                        orderClass.GUID = $"{udedsponClasses[i].資料異動時戳};{udedsponClasses[i].病歷號};{udedsponClasses[i].資料種類};{udedsponClasses[i].藥品碼};{udedsponClasses[i].頻次}";
                        orderClass.PRI_KEY = $"{udedsponClasses[i].病歷號};{udedsponClasses[i].就醫類別};{udedsponClasses[i].就醫序號};{udedsponClasses[i].住院序號};{udedsponClasses[i].資料異動時戳}";
                        orderClass.藥局代碼 = "PHER";
                        orderClass.藥品碼 = udedsponClasses[i].藥品碼.Trim();
                        orderClass.藥品名稱 = udedsponClasses[i].藥品名稱.Trim();
                        orderClass.劑量單位 = udedsponClasses[i].包裝單位.Trim();
                        orderClass.交易量 = (udedsponClasses[i].配藥量.Trim().StringToInt32() * -1).ToString();
                        orderClass.病人姓名 = udedsponClasses[i].病患姓名.Trim();
                        orderClass.開方日期 = $"{udedsponClasses[i].日期} {udedsponClasses[i].時間}";
                        orderClass.領藥號 = udedsponClasses[i].勤務領藥號.Trim();
                        orderClass.病房 = udedsponClasses[i].病房.Trim();
                        orderClass.床號 = udedsponClasses[i].床號.Trim();
                        orderClass.頻次 = udedsponClasses[i].頻次.Trim();
                        orderClass.病歷號 = udedsponClasses[i].病歷號.Trim();
                        orderClass.藥袋類型 = udedsponClasses[i].資料種類.Trim();
                        orderClass.產出時間 = DateTime.Now.ToDateTimeString_6();
                        orderClass.狀態 = "未調劑";

                        if (orderClass.床號 == "999" && (orderClass.病房.ToUpper().Contains("PER") == false))
                        {
                            orderClass.病房 = "999";
                        }
                        List<object[]> temp = sQLControl_醫囑資料.GetRowsByDefult(null, (int)enum_醫囑資料.GUID, orderClass.GUID);

                        orderClasses.Add(orderClass);
                    }
                    Console.WriteLine("----------------------------------------------------------------------------------------------------------------------");
                    Console.WriteLine($"Udedspon API 讀取共<{udedsponClasses.Count}>筆資料 ,耗時 {myTimer.ToString()} {DateTime.Now.ToDateTimeString()}");
                    myTimer.TickStop();
                    myTimer.StartTickTime(50000);

                    List<object[]> list_醫囑資料 = orderClasses.ClassToSQL<OrderClass, enum_醫囑資料>();
                    List<object[]> list_醫囑資料_add = new List<object[]>();
                    List<object[]> list_醫囑資料_replace = new List<object[]>();
                    for (int i = 0; i < list_醫囑資料.Count; i++)
                    {
                        string GUID = list_醫囑資料[i][(int)enum_醫囑資料.GUID].ObjectToString();
                        List<object[]> temp = sQLControl_醫囑資料.GetRowsByDefult(null, (int)enum_醫囑資料.GUID, GUID);
                        if(temp.Count == 0)
                        {
                            list_醫囑資料_add.Add(list_醫囑資料[i]);
                        }
                        else
                        {
                            list_醫囑資料_replace.Add(list_醫囑資料[i]);
                        }
               
                    }


                    sQLControl_醫囑資料.AddRows(null, list_醫囑資料_add);
                    sQLControl_醫囑資料.UpdateByDefulteExtra(null, list_醫囑資料_replace);
                    Console.WriteLine($"SQL 醫令資料 新增共<{list_醫囑資料_add.Count}>筆資料 修改共<{list_醫囑資料_replace.Count}>筆資料 ,耗時 {myTimer.ToString()} {DateTime.Now.ToDateTimeString()}");

                    myTimer.TickStop();
                    myTimer.StartTickTime(50000);
                    Basic.Net.WEBApiPostJson(url_write, json);
                    Console.WriteLine($"Udedspon API 回寫共<{udedsponClasses.Count}>筆資料 ,耗時 {myTimer.ToString()} {DateTime.Now.ToDateTimeString()}");

                    System.Threading.Thread.Sleep(5000);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                    break;
                }
               
            }
         
        }
    }
}
