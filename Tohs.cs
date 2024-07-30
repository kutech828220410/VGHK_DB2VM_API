using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SQLUI;
using Basic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using HIS_DB_Lib;
namespace DB2VM_API
{
    [Route("[controller]")]
    [ApiController]
    public class Tohs : ControllerBase
    {
        [Route("download/ITUD0031")]
        [HttpGet]
        public async Task<ActionResult> GET_download_ITUD0031()
        {
            try
            {
                // 構建文件路徑
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"C:\東軒data\importcabinet.csv");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("文件未找到");
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0; // 將內存流的位置重置為開頭


                return File(memory, "application/octet-stream", "importcabinet.csv");
            }
            catch (Exception e)
            {
                return StatusCode(500, "內部伺服器錯誤");
            }
        }
        [Route("download/UDMORQRE")]
        [HttpGet]
        public async Task<ActionResult> GET_download_UDMORQRE()
        {
            try
            {
                // 構建文件路徑
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"C:\東軒data\cabinet.csv");

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("文件未找到");
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0; // 將內存流的位置重置為開頭


                return File(memory, "application/octet-stream", "cabinet.csv");
            }
            catch (Exception e)
            {
                return StatusCode(500, "內部伺服器錯誤");
            }
        }
    }
}
