using Microsoft.EntityFrameworkCore;
using OverflowBackend.Models.Response.DorelAppBackend.Models.Responses;
using Uptime_Monitor_Backend.Models;
using Uptime_Monitor_Backend.Models.DB;
using Uptime_Monitor_Backend.Models.Requests;

namespace Uptime_Monitor_Backend.Services
{
    public class MonitorService
    {
        private readonly MonitorDBContext _context;
        public MonitorService(MonitorDBContext context)
        {
            _context = context;
        }
        public async Task<Maybe<string>> AddMonitor(AddMonitorReq req, string username)
        {
            var maybe = new Maybe<string>();
            try
            {
                var model = new DBMonitor()
                {
                    Username = username,
                    MonitorName = req.MonitorName,
                    MonitorType = req.MonitorType,
                    CheckInterval = req.CheckInterval,
                    Domain = req.Domain

                };
                if (req.MonitorType == "Http" || req.MonitorType == "Https")
                {
                    model.Port = req.Port;
                    model.HttpMethod = req.HttpMethod;
                    model.CheckCertificate = req.CheckCertificate;
                    _context.Monitors.Add(model);
                    maybe.SetSuccess("Ok");
                }
                else if(req.MonitorType == "Ws" || req.MonitorType == "Wss")
                {
                    model.Port = req.Port;
                    model.CheckCertificate = req.CheckCertificate;
                    _context.Monitors.Add(model);
                    maybe.SetSuccess("Ok");
                }
                else if(req.MonitorType == "Ping")
                {
                    // nothing
                    _context.Monitors.Add(model);
                    maybe.SetSuccess("Ok");
                }
                else
                {
                    maybe.SetException("Invalid monitor type");
                }
                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                maybe.SetException("Something went wrong");
            }
            return maybe;
        }

        public async Task<Maybe<string>> EditMonitor(EditMonitorReq req, string username)
        {
            var maybe = new Maybe<string>();
            try
            {
                var entry = await _context.Monitors.FirstOrDefaultAsync(e => e.Id == req.Id);
                if(entry!= null && entry.Username != username)
                {
                    maybe.SetException("Access denied");
                    return maybe;
                }
                if(entry != null)
                {
                    entry.MonitorName = req.MonitorName;
                    entry.CheckInterval = req.CheckInterval;
                    entry.Domain = req.Domain;

                    if (req.MonitorType == "Http" || req.MonitorType == "Https")
                    {
                        entry.Port = req.Port;
                        entry.HttpMethod = req.HttpMethod;
                        entry.CheckCertificate = req.CheckCertificate;
                        _context.Monitors.Update(entry);
                        await _context.SaveChangesAsync();
                        maybe.SetSuccess("Ok");
                    }
                    else if (req.MonitorType == "Ws" || req.MonitorType == "Wss")
                    {
                        entry.Port = req.Port;
                        entry.CheckCertificate = req.CheckCertificate;
                        _context.Monitors.Update(entry);
                        await _context.SaveChangesAsync();
                        maybe.SetSuccess("Ok");
                    }
                    else if (req.MonitorType == "Ping")
                    {
                        // nothing
                        _context.Monitors.Update(entry);
                        await _context.SaveChangesAsync();
                        maybe.SetSuccess("Ok");
                    }
                    else
                    {
                        maybe.SetException("Invalid monitor type");
                    }
                }
                else
                {
                    maybe.SetException("No entry found");
                }
            }
            catch(Exception e)
            {
                maybe.SetException("Something went wrong");
            }

            return maybe;
        }


        public async Task<Maybe<List<DBMonitor>>> GetMonitors(string username)
        {
            var monitors = new List<DBMonitor>();
            var maybe = new Maybe<List<DBMonitor>>();
            try
            {
                var monitorsDb = await _context.Monitors
                        .Where(e => e.Username == username)
                        .ToListAsync();

                maybe.SetSuccess(monitorsDb);
            }
            catch(Exception e)
            {
                maybe.SetException("Something went wrong");
            }
            return maybe;
        }
    }
}
