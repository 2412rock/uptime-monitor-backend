using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using Uptime_Monitor_Backend.Models;
using Uptime_Monitor_Backend.Models.DB;

namespace Uptime_Monitor_Backend.Services
{
    public class MonitorChecker
    {
        private ConcurrentDictionary<int, DBMonitor> keyValuePairs = new ConcurrentDictionary<int, DBMonitor>();
        private readonly IDbContextFactory<MonitorDBContext> _dbContextFactory;
        public MonitorChecker(IDbContextFactory<MonitorDBContext> dbContextFactoryt)
        {
            _dbContextFactory = dbContextFactoryt;
        }

        public async Task Run()
        {
            while (true)
            {
                using (var _context = _dbContextFactory.CreateDbContext())
                {
                    var list = _context.Monitors.ToList();

                    foreach (DBMonitor monitor in list)
                    {
                        if (!keyValuePairs.ContainsKey(monitor.Id))
                        {
                            try
                            {
                                keyValuePairs.TryAdd(monitor.Id, monitor);
                                StartThread((monitor.Id));
                            }
                            catch
                            {
                                Console.WriteLine("Cant add value");
                            }
                        }
                    }

                    // await _context.SaveChangesAsync();
                }
                await Task.Delay(3000);
            }
            
        }

        public async Task MyAsyncThreadFunction(int monitorId)
        {
            while (true)
            {
                using (var _context = _dbContextFactory.CreateDbContext())
                {
                    // Do work
                    if (!_context.Monitors.Any(e => e.Id == monitorId))
                    {
                        //This monitor was deleted by user
                        return;
                    }
                    var monitor = _context.Monitors.FirstOrDefault(e => e.Id == monitorId);
                    try
                    {
                        if (monitor != null && monitor.MonitorType == "Http")
                        {
                            using (HttpClient client = new HttpClient())
                            {
                                var url = monitor.Url;
                                HttpResponseMessage response = await client.GetAsync(url);
                                if (!response.IsSuccessStatusCode)
                                {
                                    monitor.IsUp = false;
                                    monitor.LastChecked = DateTime.UtcNow;
                                    _context.Monitors.Attach(monitor);
                                    _context.Entry(monitor).Property(x => x.IsUp).IsModified = true;
                                    _context.Entry(monitor).Property(x => x.LastChecked).IsModified = true;
                                    _context.SaveChanges();
                                }
                                else
                                {
                                    monitor.IsUp = true;
                                    monitor.LastChecked = DateTime.UtcNow;
                                    _context.Monitors.Attach(monitor);
                                    _context.Entry(monitor).Property(x => x.IsUp).IsModified = true;
                                    _context.Entry(monitor).Property(x => x.LastChecked).IsModified = true;
                                    _context.SaveChanges();
                                }

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        monitor.IsUp = false;
                        monitor.LastChecked = DateTime.UtcNow;
                        try
                        {
                            _context.Monitors.Attach(monitor);
                            _context.Entry(monitor).Property(x => x.IsUp).IsModified = true;
                            _context.Entry(monitor).Property(x => x.LastChecked).IsModified = true;
                            _context.SaveChanges();
                        }
                        catch { }
                        
                    }
                    await Task.Delay(2000);
                }
                 
            }
             // Simulate an asynchronous operation
        }


        public void StartThread(int monitorId)
        {
            //// Using a lambda to call the async function and block on its completion
            //Thread newThread1 = new Thread(async (obj) => await MyAsyncThreadFunction(obj));
            //newThread1.Start("Data for async thread 1");
            ////newThread1.Join(); // Wait for the async operation to complete

            // Alternatively, starting the async function and not explicitly waiting
            Thread newThread2 = new Thread(() => MyAsyncThreadFunction(monitorId));
            newThread2.Start();
            //// Note: The main thread might finish before the async thread in this case.

            //Console.WriteLine("Main thread continues.");
            //Thread.Sleep(3000); // Give the second thread time to finish
            //Console.WriteLine("Main thread finished.");
        }
    }
}
