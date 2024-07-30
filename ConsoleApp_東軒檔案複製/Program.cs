using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Basic;
using SQLUI;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
namespace ConsoleApp_東軒檔案複製
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CopyITUD0031();
                CopyUDMORQRE();
            }
            catch(Exception ex)
            {

                Logger.Log($"Exception : {ex.Message}");
            }
            finally
            {

            }
      
            
        }
        static void CopyITUD0031()
        {

            string src_path = @"C:\\";
            string stc_filename = @"ITUD0031.TXT";

            string dst_path = @"C:\data_temp\";
            string dst_filename = $@"ITUD0031_{DateTime.Now.ToDateTinyString()}.TXT";

            string dst_pc_path = @"C:\東軒data\";
            string dst_pc_filename = $"importcabinet.csv";
            Logger.Log($"[來源檔案] {src_path}{stc_filename}");
            Logger.Log($"[目的檔案] {dst_path}{dst_filename}");
            Logger.Log($"[目的檔案] {dst_pc_path}{dst_pc_filename}");

            Basic.FileIO.CopyFile($@"{src_path}{stc_filename}", $"{dst_path}{dst_filename}");
            Basic.FileIO.CopyFile($@"{src_path}{stc_filename}", $"{dst_pc_path}{dst_pc_filename}");

            Logger.Log($"複製完成");
        }
        static void CopyUDMORQRE()
        {
            string src_path = @"Z:\\";
            string stc_filename = @"UDMORQRE.TXT";

            string dst_path = @"C:\data_temp\";
            string dst_filename = $@"UDMORQRE_{DateTime.Now.ToDateTinyString()}.TXT";

            string dst_pc_path = @"C:\東軒data\";
            string dst_pc_filename = $"cabinet.csv";
            Logger.Log($"[來源檔案] {src_path}{stc_filename}");
            Logger.Log($"[目的檔案] {dst_path}{dst_filename}");
            Logger.Log($"[目的檔案] {dst_pc_path}{dst_pc_filename}");

            Basic.FileIO.CopyFile($@"{src_path}{stc_filename}", $"{dst_path}{dst_filename}");
            Basic.FileIO.CopyFile($@"{src_path}{stc_filename}", $"{dst_pc_path}{dst_pc_filename}");

            Logger.Log($"複製完成");
        }

  
    }
}
