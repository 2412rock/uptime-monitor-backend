using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uptime_Monitor_Backend.Models.Requests
{
    public class AddMonitorReq
    {
        public string MonitorName { get; set; }
        public string MonitorType { get; set; } // Http/https/ping/ws/wss
        public int? Port { get; set; }
        public string? HttpMethod { get; set; }
        public bool CheckCertificate { get; set; }
        public string Domain { get; set; }
        public int CheckInterval { get; set; }
    }
}
