using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlukeBox.Models;
using FlukeBox.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using File = TagLib.File;

namespace FlukeBox.Controllers {
    [Route("api/[controller]")]
    public class LibraryManagementController : Controller {
        // GET api/LibraryManagement/list
        [HttpGet("list")]
        public JsonResult List() {
            var metadataService = new MetadataService();
            var listOfFiles = Directory.EnumerateFiles(@"D:\Music\", "*.mp3", SearchOption.AllDirectories)
                                       .Select(path => metadataService.ReadTrack("x",path))
                                       .Where(track => track!=null)
                                       .ToList();
            return Json(
                listOfFiles,
                new JsonSerializerSettings {
                    Formatting = Formatting.Indented
                }
            );
        }
    }
}
