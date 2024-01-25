using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS_DB_Lib;
using Basic;
using SQLUI;
namespace UDFTPSTK4ConsoleApp
{
    class Program
    {
        public enum enum_UDFTPSTK4
        {
            GUID,
            藥碼,
            藥名,
            病房,
            床號,
            病歷號,
            病人姓名,
            交易量,
            日期,
            加入時間,
        }
        static private string server = "127.0.0.1";
        static private string dbName = "dbvm_vghks_ud";
        static private string tableName = "batch_med_list";
        static private string user = "user";
        static private string pwd = "66437068";
        static private uint port = 3306;
        static void Main(string[] args)
        {
            Console.WriteLine("-------------------------住院批次檔處方-------------------------");
            string dst_path = @"C:\Users\hsonds01\Desktop\UDFTPSTK4\";
            string dst_filename = $@"UDFTPSTK4.TXT";
            string new_dst_filename = $@"UDFTPSTK4_{DateTime.Now.ToDateString("_")}.TXT";
            Basic.FileIO.CopyFile(@"Z:\UDFTPSTK4.TXT", $"{dst_path}{new_dst_filename}");
            List<string> list_text = MyFileStream.LoadFile($"{dst_path}{new_dst_filename}");
            SQLUI.SQLControl sQLControl = new SQLUI.SQLControl(server, dbName, tableName, user, pwd, port, MySql.Data.MySqlClient.MySqlSslMode.None);
            Table table = new Table("");
            table.TableName = tableName;
            table.AddColumnList("GUID", Table.StringType.CHAR, 50, Table.IndexType.PRIMARY);
            table.AddColumnList("藥碼", Table.StringType.CHAR, 50, Table.IndexType.INDEX);
            table.AddColumnList("藥名", Table.StringType.CHAR, 200, Table.IndexType.None);
            table.AddColumnList("病房", Table.StringType.CHAR, 50, Table.IndexType.None);
            table.AddColumnList("床號", Table.StringType.CHAR, 50, Table.IndexType.None);
            table.AddColumnList("病歷號", Table.StringType.CHAR, 50, Table.IndexType.None);
            table.AddColumnList("病人姓名", Table.StringType.CHAR, 100, Table.IndexType.None);
            table.AddColumnList("交易量", Table.StringType.CHAR, 50, Table.IndexType.None);
            table.AddColumnList("日期", Table.DateType.DATETIME, 50, Table.IndexType.INDEX);
            table.AddColumnList("加入時間", Table.DateType.DATETIME, 50, Table.IndexType.INDEX);
            if (sQLControl.IsTableCreat() == false)
            {                
                sQLControl.CreatTable(table);
            }
            else
            {
                sQLControl.CheckAllColumnName(table, true);
            }

            List<object[]> list_UDFTPSTK4 = new List<object[]>();
            List<object[]> list_DB_UDFTPSTK4 = new List<object[]>();
            List<object[]> list_DB_UDFTPSTK4_buf = new List<object[]>();
            List<object[]> list_DB_UDFTPSTK4_add = new List<object[]>();

            for (int i = 0; i < list_text.Count; i++)
            {
                string[] testAry = list_text[i].Split('\t');
                if(testAry.Length >= 7)
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
                    object[] value = new object[new enum_UDFTPSTK4().GetLength()];
                    value[(int)enum_UDFTPSTK4.藥碼] = 藥碼;
                    value[(int)enum_UDFTPSTK4.藥名] = 藥名;
                    value[(int)enum_UDFTPSTK4.病歷號] = 病歷號;
                    value[(int)enum_UDFTPSTK4.病人姓名] = 病人姓名;
                    value[(int)enum_UDFTPSTK4.交易量] = 交易量;
                    value[(int)enum_UDFTPSTK4.日期] = 日期;
                    value[(int)enum_UDFTPSTK4.病房] = 病房;
                    value[(int)enum_UDFTPSTK4.床號] = 床號;

                    list_UDFTPSTK4.Add(value);
                }

            }
            List<string> dates = (from temp in list_UDFTPSTK4
                            select temp[(int)enum_UDFTPSTK4.日期].ObjectToString().StringToDateTime().ToDateString()).Distinct().ToList();
            for (int i = 0; i < dates.Count; i++)
            {
                List<object[]> list_temp = sQLControl.GetRowsByDefult(null, (int)enum_UDFTPSTK4.日期, dates[i]);
                list_DB_UDFTPSTK4.LockAdd(list_temp);
            }
            for(int i = 0; i< list_UDFTPSTK4.Count; i++)
            {
                string 藥碼 = list_UDFTPSTK4[i][(int)enum_UDFTPSTK4.藥碼].ObjectToString();
                string 病歷號 = list_UDFTPSTK4[i][(int)enum_UDFTPSTK4.病歷號].ObjectToString();
                string 交易量 = list_UDFTPSTK4[i][(int)enum_UDFTPSTK4.交易量].ObjectToString();
                string 日期 = list_UDFTPSTK4[i][(int)enum_UDFTPSTK4.日期].ObjectToString();
                list_DB_UDFTPSTK4_buf = (from temp in list_DB_UDFTPSTK4
                                         where temp[(int)enum_UDFTPSTK4.藥碼].ObjectToString() == 藥碼
                                         where temp[(int)enum_UDFTPSTK4.病歷號].ObjectToString() == 病歷號
                                         where temp[(int)enum_UDFTPSTK4.交易量].ObjectToString() == 交易量
                                         where temp[(int)enum_UDFTPSTK4.日期].ToDateString("-") == 日期
                                         select temp).ToList();
                if(list_DB_UDFTPSTK4_buf.Count == 0)
                {
                    list_UDFTPSTK4[i][(int)enum_UDFTPSTK4.GUID] = Guid.NewGuid().ToString();
                    list_UDFTPSTK4[i][(int)enum_UDFTPSTK4.加入時間] = DateTime.Now.ToDateTimeString_6();
                    list_DB_UDFTPSTK4_add.Add(list_UDFTPSTK4[i]);
                }
            }
            sQLControl.AddRows(null, list_DB_UDFTPSTK4_add);
            Console.WriteLine($"共新增<{list_DB_UDFTPSTK4_add.Count}>筆資料!");
            System.Threading.Thread.Sleep(2000);
            //System.IO.File.Move($"{dst_path}{dst_filename}", $"{dst_path}{new_dst_filename}");
        }
    }
}
