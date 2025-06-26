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

namespace batch_UD醫令及出入庫
{
    class Program
    {
        public class UDPHRLOGClass
        {
            [JsonPropertyName("UDPHRID")]
            public string 藥局代碼 { get; set; }

            [JsonPropertyName("UDSTAMP")]
            public DateTime? 時戳 { get; set; }

            [JsonPropertyName("HID")]
            public string 醫院代碼 { get; set; }

            [JsonPropertyName("UDFROM")]
            public string 來源代碼 { get; set; }

            [JsonPropertyName("UDOPID")]
            public string 操作者ID { get; set; }

            [JsonPropertyName("UDWARD")]
            public string 病房代號 { get; set; }

            [JsonPropertyName("UDDRGNO")]
            public string 藥碼 { get; set; }

            [JsonPropertyName("UDRPNAME")]
            public string 學名 { get; set; }

            [JsonPropertyName("UDSTOCK")]
            public string 藥類 { get; set; }

            [JsonPropertyName("UDLDATE")]
            public DateTime? 進日誌日期 { get; set; }

            [JsonPropertyName("UDLTIME")]
            public TimeSpan? 進日誌時間 { get; set; }

            [JsonPropertyName("UDSTATCH")]
            public string 進日誌狀態 { get; set; }

            [JsonPropertyName("UDLQNTY")]
            public int? 藥量 { get; set; }

            [JsonPropertyName("UDBAHNUM")]
            public string 批號 { get; set; }

            [JsonPropertyName("UDBAHDAT")]
            public DateTime? 效期 { get; set; }

            [JsonPropertyName("HHISNUM")]
            public string 病歷號 { get; set; }

            [JsonPropertyName("HCASETYP")]
            public string 就醫類別 { get; set; }

            [JsonPropertyName("HCASENO")]
            public string 就醫序號 { get; set; }

            [JsonPropertyName("UDORDSEQ")]
            public string 處方序號 { get; set; }

            [JsonPropertyName("PROCFLAG")]
            public string 已接收註記 { get; set; }

            [JsonPropertyName("PROCSTMP")]
            public DateTime? 廠商回寫時戳 { get; set; }

            [JsonPropertyName("DISPFLAG")]
            public string 已調劑註記 { get; set; }

            [JsonPropertyName("DISPCARD")]
            public string 調劑藥師卡號 { get; set; }

            [JsonPropertyName("DISPNAME")]
            public string 調劑藥師姓名 { get; set; }

            [JsonPropertyName("ZUDPHRLO")]
            public DateTime? 異動時戳 { get; set; }
        }

        private static System.Threading.Mutex mutex;
        static private string Api_server = "http://127.0.0.1:4433";
        static void Main(string[] args)
        {
            Console.Title = "batch_UD醫令及出入庫";
            mutex = new System.Threading.Mutex(true, "batch_UD醫令及出入庫");
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
                    List<HIS_DB_Lib.sys_serverSettingClass> sys_serverSettingClasses = sys_serverSettingClassMethod.WebApiGet($"{Api_server}/api/serverSetting");
                    sys_serverSettingClass sys_serverSettingClass = sys_serverSettingClasses.myFind("Main", "網頁", "VM端");

                    string url_read = "https://zwmc01p.vghks.gov.tw:4436/UDTKService/ud/udtk/ReadUdedspon";
                    string url_write = " https://zwmc01p.vghks.gov.tw:4436/UDTKService/ud/udtk/WriteUdedspon";

                    string json = Basic.Net.WEBApiPostJson(url_read, "{\"udphrid\":\"PHR\",\"hid\":\"1A0\"}");
                }
                catch(Exception ex)
                {

                }
            }
        }
    }
}
