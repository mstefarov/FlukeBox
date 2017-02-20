using System;
using Microsoft.AspNetCore.Mvc;

namespace FlukeBox.Jobs {
    [Route("api/[controller]")]
    public class JobsController : Controller {
        private readonly IJobRegistry registry;

        public JobsController(IJobRegistry registry) {
            this.registry = registry;
        }

        [HttpGet("running")]
        public JsonResult GetRunning() {
            return Json(registry.GetRunningJobs());
        }

        [HttpGet("recent")]
        public JsonResult GetRecent() {
            return Json(registry.GetRecentJobs(TimeSpan.FromDays(7)));
        }
        
        [HttpGet("{guid}")]
        public JsonResult GetByGuid(Guid guid) {
            return Json(registry.Find(guid));
        }

        [HttpDelete("{guid}")]
        public JsonResult Test(Guid guid) {
            IJob job = registry.Find(guid);
            job?.Cancel();
            return Json(new {
                Guid = guid,
                Result = (job != null),
                Status = job?.State
            });
        }
    }
}
