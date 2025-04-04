using System.Collections.Generic;
using System;
using Uptime_Monitor_Backend.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace Uptime_Monitor_Backend.Models
{
    public class MonitorDBContext : DbContext
    {
        public MonitorDBContext(DbContextOptions<MonitorDBContext> options) : base(options)
        { }
        public DbSet<DBUser> Users { get; set; }
        public DbSet<DBMonitor> Monitors { get; set; }
        public DbSet<DBUserSession> UserSessions { get; set; }
    }
}
