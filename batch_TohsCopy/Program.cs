using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
namespace batch_TohsCopy
{
    class Program
    {
        async static Task Main(string[] args)
        {
            try
            {
                await DownloadFileAsync("http://10.18.1.146:4435/Tohs/download/ITUD0031", @"C:\\data\\importcabinet.csv");
                Logger.Log($"[DownloadFileAsync] http://10.18.1.146:4435/Tohs/download/ITUD0031    C:\\data\\importcabinet.csv  ....ok");
                await DownloadFileAsync("http://10.18.1.146:4435/Tohs/download/UDMORQRE", @"C:\\data\\cabinet.csv");
                Logger.Log($"[DownloadFileAsync] http://10.18.1.146:4435/Tohs/download/UDMORQRE    C:\\data\\cabinet.csv  ....ok");

            }
            catch (Exception ex)
            {

                Logger.Log($"Exception : {ex.Message}");
            }
            finally
            {

            }
        }
        public static async Task DownloadFileAsync(string fileUrl, string destinationPath)
        {
            using (HttpClient client = new HttpClient())
            {
                // 發送 HTTP GET 請求並接收文件數據
                HttpResponseMessage response = await client.GetAsync(fileUrl);

                // 確保請求成功
                response.EnsureSuccessStatusCode();

                // 讀取文件內容到流中
                using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
            }
        }
    }
}
