using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using System.Data.SqlClient;
using System.IO;
namespace batch_Tohs_SQL_cmdi
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string datetime = DateTime.Now.ToString("yyyyMMddHHmm");
                string str = Basic.Net.WEBApiGet($"http://10.18.1.146:4435/tohs/DGOPDPPL/{datetime}");
                Logger.Log($"{str}");
                string dst_path = @"W:\";
                string dst_filename = $@"DGOPDPPL_{DateTime.Now.ToDateTinyString()}.TXT";
                WriteStringToFile($"C:\\DG\\{dst_filename}", str, Encoding.UTF8);
                //FileIO.CopyToServer($"C:\\", $"{dst_filename}", @"\\192.168.222.19\DG", dst_filename, "phruser", "PHRUSER@76122");
                Logger.Log($"[目的檔案] {dst_path}{dst_filename}");
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception : {ex.Message}");
            }
            finally
            {

            }


        }

        static void WriteStringToFile(string path, string content, Encoding encoding)
        {
            try
            {
                // 使用 StreamWriter 來寫入檔案，並確保使用 Windows 的 CRLF 行結尾
                using (StreamWriter writer = new StreamWriter(path, false, encoding))
                {
                    // 手動替換行結尾為 CRLF
                    string formattedContent = content.Replace("\n", "\r\n");
                    writer.Write(formattedContent);
                }
            }
            catch (Exception ex)
            {
                // 處理可能的例外情況
                Console.WriteLine($"寫入檔案時發生錯誤: {ex.Message}");
            }
        }
    }

}
