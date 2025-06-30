using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS_DB_Lib;
using Basic;
namespace batch_UD管4批領
{
    class Program
    {
        public class UDFTPSTK4
        {
            /// <summary>
            /// 唯一識別碼
            /// </summary>
            public string GUID { get; set; }

            /// <summary>
            /// 藥品代碼
            /// </summary>
            public string 藥碼 { get; set; }

            /// <summary>
            /// 藥品名稱
            /// </summary>
            public string 藥名 { get; set; }

            /// <summary>
            /// 病房名稱
            /// </summary>
            public string 病房 { get; set; }

            /// <summary>
            /// 床號
            /// </summary>
            public string 床號 { get; set; }

            /// <summary>
            /// 病歷號碼
            /// </summary>
            public string 病歷號 { get; set; }

            /// <summary>
            /// 病人姓名
            /// </summary>
            public string 病人姓名 { get; set; }

            /// <summary>
            /// 交易量
            /// </summary>
            public string 交易量 { get; set; }

            /// <summary>
            /// 日期 (格式: yyyy-MM-dd)
            /// </summary>
            public string 日期 { get; set; }

            /// <summary>
            /// 加入時間 (格式: yyyy-MM-dd HH:mm:ss)
            /// </summary>
            public string 加入時間 { get; set; }
        }
        static private string API_Server = "http://127.0.0.1:4433";

        static void Main(string[] args)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                Logger.Log("-------------------------住院批次檔處方-------------------------");

                string dst_filename = $@"C:\batch\log_UDFTPSTK4\UDFTPSTK4_{DateTime.Now.ToDateString("")}.txt";
                Logger.Log($"複製來源檔案 Y:\\UDFTPSTK4.TXT 至 {dst_filename}");
                Basic.FileIO.CopyFile(@"Y:\UDFTPSTK4.TXT", $"{dst_filename}");
                Logger.Log($"複製完成，耗時: {(DateTime.Now - startTime).TotalMilliseconds} ms");

                Logger.Log($"載入檔案 {dst_filename}");
                List<string> list_text = MyFileStream.LoadFile($"{dst_filename}");
                Logger.Log($"載入完成，共 {list_text.Count} 行，耗時: {(DateTime.Now - startTime).TotalMilliseconds} ms");

                List<UDFTPSTK4> uDFTPSTK4s = new List<UDFTPSTK4>();
                Logger.Log($"開始解析檔案");
                DateTime parseStart = DateTime.Now;

                for (int i = 0; i < list_text.Count; i++)
                {
                    string[] testAry = list_text[i].Split('\t');
                    if (testAry.Length >= 7)
                    {
                        string 藥名 = testAry[0].Trim();
                        string 藥碼 = testAry[1].Trim();
                        string 病房_床號 = testAry[2].Trim();
                        string 病歷號 = testAry[3].Trim();
                        string 病人姓名 = testAry[4].Trim();
                        string 交易量 = testAry[5].Trim();
                        string 日期 = testAry[6].Trim();
                        string 病房 = "";
                        string 床號 = "";

                        string[] temp_Ary = 病房_床號.Split('-');
                        if (temp_Ary.Length == 2)
                        {
                            病房 = temp_Ary[0];
                            床號 = temp_Ary[1];
                        }

                        if (交易量.StringIsDouble() == false) 交易量 = "0";
                        else 交易量 = (交易量.StringToDouble() * -1).ToString();

                        UDFTPSTK4 uDFTPSTK4 = new UDFTPSTK4()
                        {
                            藥碼 = 藥碼,
                            藥名 = 藥名,
                            病歷號 = 病歷號,
                            病人姓名 = 病人姓名,
                            交易量 = 交易量,
                            日期 = 日期,
                            病房 = 病房,
                            床號 = 床號
                        };

                        uDFTPSTK4s.Add(uDFTPSTK4);
                    }
                }
                Logger.Log($"解析完成，共取得 {uDFTPSTK4s.Count} 筆資料，耗時: {(DateTime.Now - parseStart).TotalMilliseconds} ms");

                DateTime queryStart = DateTime.Now;
                var range_date = GetUDFTPSTK4DateTimeRange(uDFTPSTK4s);
                Logger.Log($"取得日期範圍: {range_date.minDateTime.ToString("yyyy-MM-dd HH:mm:ss")} ~ {range_date.maxDateTime.ToString("yyyy-MM-dd HH:mm:ss")}");

                Logger.Log("查詢現有醫令資料...");
                List<OrderClass> orderClasses = OrderClass.get_by_rx_time_st_end(API_Server, range_date.minDateTime, range_date.maxDateTime);
                Logger.Log($"現有醫令資料筆數: {orderClasses.Count}，耗時: {(DateTime.Now - queryStart).TotalMilliseconds} ms");

                DateTime matchStart = DateTime.Now;
                List<OrderClass> orderClasses_add = new List<OrderClass>();
                Dictionary<string, List<OrderClass>> keyValuePairs_orders = orderClasses.CoverToDictionaryBy_PRI_KEY();

                Logger.Log("開始比對與新增醫令資料...");
                for (int i = 0; i < uDFTPSTK4s.Count; i++)
                {
                    UDFTPSTK4 uDFTPSTK4 = uDFTPSTK4s[i];
                    string pri_key = $"{uDFTPSTK4.藥碼};{uDFTPSTK4.交易量};{uDFTPSTK4.病歷號};{uDFTPSTK4.日期}";

                    var orderClasses_buf = keyValuePairs_orders.SortDictionaryBy_PRI_KEY(pri_key);
                    if (orderClasses_buf.Count == 0)
                    {
                        OrderClass orderClass = new OrderClass
                        {
                            GUID = Guid.NewGuid().ToString(),
                            PRI_KEY = pri_key,
                            藥品碼 = uDFTPSTK4.藥碼,
                            藥品名稱 = uDFTPSTK4.藥名,
                            交易量 = uDFTPSTK4.交易量,
                            病歷號 = uDFTPSTK4.病歷號,
                            開方日期 = uDFTPSTK4.日期,
                            病房 = uDFTPSTK4.病房,
                            床號 = uDFTPSTK4.床號,
                            病人姓名 = uDFTPSTK4.病人姓名,
                            藥袋類型 = "UDFTPSTK4",
                            藥局代碼 = "UD",
                        };
                        orderClasses_add.Add(orderClass);
                        Logger.Log($"新增醫令資料: PRI_KEY={pri_key}");
                    }
                }
                Logger.Log($"比對完成，需新增醫令資料: {orderClasses_add.Count} 筆，耗時: {(DateTime.Now - matchStart).TotalMilliseconds} ms");

                if (orderClasses_add.Count > 0)
                {
                    DateTime addStart = DateTime.Now;
                    Logger.Log($"即將新增 {orderClasses_add.Count} 筆醫令資料");
                    OrderClass.add(API_Server, orderClasses_add);
                    Logger.Log($"醫令資料新增完成，耗時: {(DateTime.Now - addStart).TotalMilliseconds} ms");
                }
                else
                {
                    Logger.Log("無需新增醫令資料");
                }

                Logger.Log($"整體流程完成，總耗時: {(DateTime.Now - startTime).TotalMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Logger.Log($"發生例外: {ex.Message}");
                Logger.Log($"堆疊資訊: {ex.StackTrace}");
            }


        }
        static public (DateTime minDateTime, DateTime maxDateTime) GetUDFTPSTK4DateTimeRange(List<UDFTPSTK4> list)
        {
            var dateList = list
                .Select(x => DateTime.TryParse(x.日期, out var dt) ? dt : (DateTime?)null)
                .Where(dt => dt.HasValue)
                .Select(dt => dt.Value.Date)  // 只取日期部分
                .ToList();

            if (dateList.Count == 0)
                return (DateTime.MinValue, DateTime.MinValue);

            DateTime minDate = dateList.Min().Date.AddHours(0).AddMinutes(0).AddSeconds(0);
            DateTime maxDate = dateList.Max().Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            return (minDate, maxDate);
        }
    }
}
