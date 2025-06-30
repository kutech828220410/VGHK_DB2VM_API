using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS_DB_Lib;
using Basic;
using System.IO;
namespace batch_UD管123批領
{
    public class UDMORQR
    {
        public string 序號 { get; set; }
        public string 住院號 { get; set; }
        public string 庫別 { get; set; }
        public string 出生年月日 { get; set; }
        public string 病人姓名 { get; set; }
        public string 病歷號 { get; set; }
        public string 藥品名稱 { get; set; }
        public string 數量 { get; set; }
        public string 頻率 { get; set; }
        public string 給藥途徑 { get; set; }
        public string 醫令日期 { get; set; }
        public string 備註1 { get; set; }
        public string 備註2 { get; set; }
        public string 藥品碼 { get; set; }
    }

    public class UDTBDSK
    {
        /// <summary>
        /// 病房與床號
        /// </summary>
        public string 病房床號 { get; set; }

        /// <summary>
        /// 病人姓名
        /// </summary>
        public string 病人姓名 { get; set; }

        /// <summary>
        /// 病歷號
        /// </summary>
        public string 病歷號 { get; set; }

        /// <summary>
        /// 出生年月日 (格式: yyyyMMdd)
        /// </summary>
        public string 出生年月日 { get; set; }

        /// <summary>
        /// 科別代碼 (如 ESUR, PS 等)
        /// </summary>
        public string 科別代碼 { get; set; }

        /// <summary>
        /// 性別 (男 / 女)
        /// </summary>
        public string 性別 { get; set; }

        /// <summary>
        /// 就醫日期 (格式: yyyyMMdd)
        /// </summary>
        public string 就醫日期 { get; set; }

        /// <summary>
        /// 健保類別 (如 健保, 健保榮民, 民眾 等)
        /// </summary>
        public string 健保類別 { get; set; }

        /// <summary>
        /// 備註 / 其他欄位
        /// </summary>
        public string 備註 { get; set; }
    }
    class Program
    {
        static private string API_Server = "http://127.0.0.1:4433";

        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;
            Logger.Log("-------------------------住院批次檔處方-------------------------");

            // 處理 UDTBDSK 資料
            Logger.Log("開始載入 UDTBDSK 資料...");
            List<UDTBDSK> allUDTBDSK = new List<UDTBDSK>();
            allUDTBDSK.AddRange(Load_UDTBDSK("A"));
            allUDTBDSK.AddRange(Load_UDTBDSK("B"));
            allUDTBDSK.AddRange(Load_UDTBDSK("C"));
            allUDTBDSK.AddRange(Load_UDTBDSK("D"));
            allUDTBDSK.AddRange(Load_UDTBDSK("E"));

            Logger.Log($"UDTBDSK 載入完成，總筆數: {allUDTBDSK.Count}");

            var uniqueUDTBDSK = allUDTBDSK
                .GroupBy(x => x.病歷號)
                .Select(g => g.First())
                .ToList();

            Logger.Log($"UDTBDSK 去除病歷號重複後筆數: {uniqueUDTBDSK.Count}");

            // 處理 UDMORQR 資料
            Logger.Log("開始載入 UDMORQR 資料...");
            List<UDMORQR> allUDMORQR = new List<UDMORQR>();
            allUDMORQR.AddRange(Load_UDMORQR("A"));
            //allUDMORQR.AddRange(Load_UDMORQR("E"));

            Logger.Log($"UDMORQR 載入完成，總筆數: {allUDMORQR.Count}");

            string missingLogFile = $@"C:\batch\log_UDTBDSK\missing_udtb_{DateTime.Now:yyyyMMdd}.log";
            int missingCount = 0;

            Logger.Log("開始轉換資料至 OrderClass...");
            List<OrderClass> orders = new List<OrderClass>();

            int processed = 0;
            foreach (var morqr in allUDMORQR)
            {
                var matchUDTB = uniqueUDTBDSK.FirstOrDefault(x => x.病歷號 == morqr.病歷號);

                if (matchUDTB == null)
                {
                    string warnMsg = $"{DateTime.Now.Get_DateTimeTINY()}警告: 找不到對應 UDTBDSK 資料 " +
                                     $"(病歷號: {morqr.病歷號}, 病人姓名: {morqr.病人姓名}, " +
                                     $"藥品碼: {morqr.藥品碼}, 藥品名稱: {morqr.藥品名稱}, 數量: {morqr.數量})";
                    Logger.Log(warnMsg);
                    File.AppendAllText(missingLogFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {warnMsg}{Environment.NewLine}");
                    missingCount++;
                }

                string[] 病房床號 = matchUDTB?.病房床號.Split('-') ?? new string[0];
                string 病房 = "";
                string 床號 = "";
                if (病房床號.Length == 2)
                {
                    病房 = 病房床號[0].Trim();
                    床號 = 病房床號[1].Trim();
                }

                OrderClass order = new OrderClass
                {
                    GUID = Guid.NewGuid().ToString(),
                    PRI_KEY = $"{morqr.藥品碼};{morqr.數量};{morqr.病歷號};{morqr.醫令日期};{morqr.住院號}",
                    藥局代碼 = "UD",
                    藥袋類型 = "UDMORQR",
                    病歷號 = morqr.病歷號,
                    藥品碼 = morqr.藥品碼,
                    藥品名稱 = morqr.藥品名稱,
                    頻次 = morqr.頻率,
                    途徑 = morqr.給藥途徑,
                    交易量 = (morqr.數量.StringToDouble() * -1).ToString(),
                    開方日期 = morqr.醫令日期,
                    住院序號 = morqr.住院號,
                    病房 = 病房,
                    床號 = 床號,
                    病人姓名 = morqr.病人姓名,
                    科別 = matchUDTB?.科別代碼 ?? "",
                    費用別 = matchUDTB?.健保類別 ?? ""
                };

                orders.Add(order);

                processed++;
                if (processed % 1000 == 0)
                {
                    Logger.Log($"已處理 {processed}/{allUDMORQR.Count} 筆 OrderClass");
                }
            }

            Logger.Log($"缺資料筆數: {missingCount}");
            Logger.Log($"OrderClass 產生完成，共 {orders.Count} 筆");

            Logger.Log("查詢現有醫令資料...");
            var range = GetOrderClassDateTimeRange(orders);
            List<OrderClass> orderClasses = OrderClass.get_by_rx_time_st_end(API_Server, range.minDateTime.GetStartDate(), range.maxDateTime.GetEndDate());
            Logger.Log($"查詢完成，取得 {orderClasses.Count} 筆現有醫令資料");

            DateTime matchStart = DateTime.Now;
            List<OrderClass> orderClasses_add = new List<OrderClass>();
            Dictionary<string, List<OrderClass>> keyValuePairs_orders = orderClasses.CoverToDictionaryBy_PRI_KEY();

            foreach (OrderClass order in orders)
            {
                var orderClasses_buf = keyValuePairs_orders.SortDictionaryBy_PRI_KEY(order.PRI_KEY);
                if (orderClasses_buf.Count == 0)
                {
                    Logger.Log($"比對需新增醫令資料: PRI_KEY={order.PRI_KEY}");
                    orderClasses_add.Add(order);
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

            Logger.Log($"整體總耗時: {(DateTime.Now - startTime).TotalMilliseconds} ms");
        }
        static List<UDTBDSK> Load_UDTBDSK(string tpye)
        {
            DateTime startTime = DateTime.Now;
            string fileName = $"UDTBDSK{tpye}.TXT";
            string dst_filename = $@"C:\batch\log_UDTBDSK\{fileName}_{DateTime.Now.ToDateString("")}.txt";
            List<string> list_text = new List<string>();
            List<UDTBDSK> udtbdsks = new List<UDTBDSK>();

            try
            {
                Logger.Log($"複製來源檔案 Y:\\{fileName} 至 {dst_filename}");
                Basic.FileIO.CopyFile($@"Y:\{fileName}", $"{dst_filename}");
                Logger.Log($"複製完成，耗時: {(DateTime.Now - startTime).TotalMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Logger.Log($"複製檔案發生例外: {ex.Message}");
                Logger.Log($"堆疊資訊: {ex.StackTrace}");
                return udtbdsks;  // 回傳空清單
            }

            try
            {
                Logger.Log($"載入檔案 {dst_filename}");
                list_text = MyFileStream.LoadFile($"{dst_filename}");
                Logger.Log($"載入完成，共 {list_text.Count} 行，耗時: {(DateTime.Now - startTime).TotalMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Logger.Log($"載入檔案發生例外: {ex.Message}");
                Logger.Log($"堆疊資訊: {ex.StackTrace}");
                return udtbdsks;  // 回傳空清單
            }

            DateTime parseStart = DateTime.Now;
            try
            {
                for (int i = 0; i < list_text.Count; i++)
                {
                    string[] testAry = list_text[i].Split('$');
                    if (testAry.Length >= 8) // 確認欄位足夠
                    {
                        UDTBDSK udtb = new UDTBDSK()
                        {
                            病房床號 = testAry[0].Trim(),
                            病人姓名 = testAry[1].Trim(),
                            病歷號 = testAry[2].Trim(),
                            出生年月日 = testAry[3].Trim(),
                            科別代碼 = testAry[4].Trim(),
                            性別 = testAry[5].Trim(),
                            就醫日期 = testAry[6].Trim(),
                            健保類別 = testAry[7].Trim(),
                            備註 = (testAry.Length > 8) ? testAry[8].Trim() : ""
                        };
                        udtbdsks.Add(udtb);
                    }
                    else
                    {
                        Logger.Log($"第 {i + 1} 行資料欄位不足，跳過該行");
                    }
                }
                Logger.Log($"解析完成，共取得 {udtbdsks.Count} 筆資料，耗時: {(DateTime.Now - parseStart).TotalMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Logger.Log($"解析資料發生例外: {ex.Message}");
                Logger.Log($"堆疊資訊: {ex.StackTrace}");
            }

            Logger.Log($"整體流程總耗時: {(DateTime.Now - startTime).TotalMilliseconds} ms");

            return udtbdsks;
        }
        static List<UDMORQR> Load_UDMORQR(string type)
        {
            DateTime startTime = DateTime.Now;
            string fileName = $"UDMORQR{type}.TXT";
            string dst_filename = $@"C:\batch\log_UDMORQR\{fileName}_{DateTime.Now.ToDateString("")}.txt";
            List<string> list_text = new List<string>();
            List<UDMORQR> udmorqrs = new List<UDMORQR>();

            try
            {
                Logger.Log($"複製來源檔案 Y:\\{fileName} 至 {dst_filename}");
                Basic.FileIO.CopyFile($@"Y:\{fileName}", $"{dst_filename}");
                Logger.Log($"複製完成，耗時: {(DateTime.Now - startTime).TotalMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Logger.Log($"複製檔案發生例外: {ex.Message}");
                Logger.Log($"堆疊資訊: {ex.StackTrace}");
                return udmorqrs;
            }

            try
            {
                Logger.Log($"載入檔案 {dst_filename}");
                list_text = MyFileStream.LoadFile($"{dst_filename}");
                Logger.Log($"載入完成，共 {list_text.Count} 行，耗時: {(DateTime.Now - startTime).TotalMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Logger.Log($"載入檔案發生例外: {ex.Message}");
                Logger.Log($"堆疊資訊: {ex.StackTrace}");
                return udmorqrs;
            }

            DateTime parseStart = DateTime.Now;

            try
            {
                for (int i = 0; i < list_text.Count; i++)
                {
                    string[] ary = list_text[i].Split(',');
                    if (ary.Length >= 16)
                    {
                        UDMORQR obj = new UDMORQR()
                        {
                            序號 = ary[0].Trim(),
                            病歷號 = ary[11].Trim(),
                            庫別 = ary[2].Trim(),
                            出生年月日 = ary[3].Trim(),
                            病人姓名 = ary[4].Trim(),
                            藥品名稱 = ary[6].Trim(),
                            數量 = ary[10].Trim(),
                            頻率 = ary[8].Trim(),
                            給藥途徑 = ary[9].Trim(),
                            住院號 = ary[1].Trim(),
                            醫令日期 = ary[12].Trim(),
                            備註1 = ary[13].Trim(),
                            備註2 = ary[14].Trim(),
                            藥品碼 = ary[15].Trim(),
                        };
                        udmorqrs.Add(obj);
                    }
                    else
                    {
                        Logger.Log($"第 {i + 1} 行資料欄位不足（{ary.Length} 欄），已跳過");
                    }
                }

                Logger.Log($"解析完成，共取得 {udmorqrs.Count} 筆資料，耗時: {(DateTime.Now - parseStart).TotalMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Logger.Log($"解析資料發生例外: {ex.Message}");
                Logger.Log($"堆疊資訊: {ex.StackTrace}");
            }

            Logger.Log($"整體流程總耗時: {(DateTime.Now - startTime).TotalMilliseconds} ms");
            return udmorqrs;
        }


        static public (DateTime minDateTime, DateTime maxDateTime) GetOrderClassDateTimeRange(List<OrderClass> list)
        {
            var dateList = list
                .Select(x => DateTime.TryParse(x.開方日期, out var dt) ? dt : (DateTime?)null)
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
