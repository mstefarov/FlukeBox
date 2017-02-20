using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FlukeBox.Jobs;
using FlukeBox.Models;
using FlukeBox.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlukeBox.Controllers {
    [Route("api/[controller]")]
    public class LibraryController : Controller {
        private readonly IJobRegistry jobs;

        public LibraryController(IJobRegistry jobs) {
            this.jobs = jobs;
        }

        [HttpPost("rescan")]
        public JsonResult Rescan() {
            IJob job = jobs.Create("Rescan", RescanMetadataJob);
            job.Run();
            return Json(job);
        }

        private static bool RescanMetadataJob(IProgress<ProgressReport> progress, CancellationToken cancelMe) {
            var metadataService = new MetadataService();
            string[] listOfFiles = Directory.EnumerateFiles(@"D:\Music\", "*.flac", SearchOption.AllDirectories).ToArray();
            List<TrackFull> result = new List<TrackFull>();
            int progressDone = 0;
            int progressTotal = listOfFiles.Length;

            foreach (string path in listOfFiles) {
                if (cancelMe.IsCancellationRequested) return false;
                progressDone++;
                progress.Report(new ProgressReport(progressDone, progressTotal, path));
                Console.WriteLine($"{progressDone}/{progressTotal}: {path}");
                result.Add(metadataService.ReadTrack(progressDone, path));
            }
            return true;
        }
    }
}
