using System;
using Microsoft.AspNetCore.Mvc;

namespace FlukeBox.Jobs {
    [Route("api/[controller]")]
    public class JobsController : Controller {
        private readonly IJobRegistry _registry;

        public JobsController(IJobRegistry registry) {
            this._registry = registry;
        }

        [HttpGet("running")]
        public JsonResult GetRunning() {
            return Json(_registry.GetRunningJobs());
        }

        [HttpGet("recent")]
        public JsonResult GetRecent() {
            return Json(_registry.GetRecentJobs(TimeSpan.FromDays(7)));
        }
        
        [HttpGet("{guid}")]
        public JsonResult GetByGuid(Guid guid) {
            return Json(_registry.Find(guid));
        }

        [HttpDelete("{guid}")]
        public JsonResult Test(Guid guid) {
            IJob job = _registry.Find(guid);
            job?.Cancel();
            return Json(new {
                Guid = guid,
                Result = (job != null),
                Status = job?.State
            });
        }
    }
}
