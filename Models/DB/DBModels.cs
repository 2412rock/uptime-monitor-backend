using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uptime_Monitor_Backend.Models.DB
{
    public class DBUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }


        public string Password { get; set; }

        // Navigation properties
    }

    public class DBMonitor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Username { get; set; }


        public string MonitorName { get; set; }


        public string MonitorType { get; set; } // Http/https/ping/ws/wss

        public int? Port { get; set; }

        public bool CheckCertificate { get; set; }
        public string? HttpMethod { get; set; }

        public string Domain { get; set; }

        public int? CheckInterval { get; set; }
        public bool IsUp { get; set; }
        public DateTime LastChecked { get; set; }

        // Navigation property
    }

    public class DBUserSession
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SessionId { get; set; }

        public string Username { get; set; }

        public string SessionToken { get; set; }

        public DateTime LastActiveTime { get; set; }

        public bool IsActive { get; set; }

        // Navigation property
    }
}
