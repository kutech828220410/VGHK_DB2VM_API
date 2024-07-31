using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace batch_TohsCopy
{
    class Program
    {
        static public string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static private string MyConfigFileName = $"{currentDirectory}//MyConfig.txt";
        public class MyConfigClass
        {
            private string _improtFilename = "D:\\Tohs\\Import\\TOHS\\importcabinet.csv";
            private string _exprotFilename = "D:\\Tohs\\Import\\TOHS\\exportcabinet.csv";


            private bool creat_importcabinet = false;
            private bool creat_exportcabinet = false;

            public bool Creat_importcabinet { get => creat_importcabinet; set => creat_importcabinet = value; }
            public bool Creat_exportcabinet { get => creat_exportcabinet; set => creat_exportcabinet = value; }
            public string ImprotFilename { get => _improtFilename; set => _improtFilename = value; }
            public string ExprotFilename { get => _exprotFilename; set => _exprotFilename = value; }
        }
        static MyConfigClass myConfigClass = new MyConfigClass();
        static public bool LoadDBConfig()
        {
            string jsonstr = MyFileStream.LoadFileAllText($"{MyConfigFileName}");
            Console.WriteLine($"路徑 : {MyConfigFileName} 開始讀取....");
            if (jsonstr.StringIsEmpty())
            {

                jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(new MyConfigClass(), true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($"{MyConfigFileName}", list_jsonstring))
                {
                    Console.WriteLine($"建立{MyConfigFileName}檔案失敗!");
                    return false;
                }
                Console.WriteLine($"未建立參數文件!請至子目錄設定{MyConfigFileName}");
                return false;
            }
            else
            {
                myConfigClass = Basic.Net.JsonDeserializet<MyConfigClass>(jsonstr);

                jsonstr = Basic.Net.JsonSerializationt<MyConfigClass>(myConfigClass, true);
                List<string> list_jsonstring = new List<string>();
                list_jsonstring.Add(jsonstr);
                if (!MyFileStream.SaveFile($"{MyConfigFileName}", list_jsonstring))
                {
                    Console.WriteLine($"建立{MyConfigFileName}檔案失敗!");
                    return false;
                }

            }
            return true;

        }
        async static Task Main(string[] args)
        {
            try
            {
                if (LoadDBConfig() == false) return;
                if(myConfigClass.Creat_importcabinet)
                {
                    await DownloadFileAsync("http://10.18.1.146:4435/Tohs/download/ITUD0031", $@"{myConfigClass.ImprotFilename}");
                    Logger.Log($"[DownloadFileAsync] http://10.18.1.146:4435/Tohs/download/ITUD0031    {myConfigClass.ImprotFilename}  ....ok");
                }
                if (myConfigClass.Creat_exportcabinet)
                {
                    await DownloadFileAsync("http://10.18.1.146:4435/Tohs/download/UDMORQRE", $@"{myConfigClass.ExprotFilename}");
                    Logger.Log($"[DownloadFileAsync] http://10.18.1.146:4435/Tohs/download/UDMORQRE    {myConfigClass.ExprotFilename}  ....ok");
                }
                 
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
