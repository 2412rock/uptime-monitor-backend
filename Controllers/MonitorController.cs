using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OverflowBackend.Filters;
using Uptime_Monitor_Backend.Models.DB;
using Uptime_Monitor_Backend.Models.Requests;
using Uptime_Monitor_Backend.Services;

namespace Uptime_Monitor_Backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class MonitorController : ControllerBase
    {
        private readonly MonitorService _monitorService;
        public MonitorController(MonitorService monitorService)
        {
            _monitorService = monitorService;
        }

        [HttpPost]
        [Route("addMonitor")]
        [AuthorizationFilter]
        public async Task<IActionResult> AddMonitor([FromBody] AddMonitorReq req)
        {
            var result = await _monitorService.AddMonitor(req, (string)HttpContext.Items["username"]);
            return Ok(result);
        }

        [HttpGet]
        [Route("getMonitors")]
        [AuthorizationFilter]
        public async Task<IActionResult> GetMonitors()
        {
            var result = await _monitorService.GetMonitors((string)HttpContext.Items["username"]);
            return Ok(result);
        }

        [HttpPut]
        [Route("editMonitor")]
        [AuthorizationFilter]
        public async Task<IActionResult> EditMonitor([FromBody] EditMonitorReq req)
        {
            var result = await _monitorService.EditMonitor(req, (string)HttpContext.Items["username"]);
            return Ok(result);
        }
    }
}
