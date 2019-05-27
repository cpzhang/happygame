using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using dbcontext;
namespace gamemaster.Controllers
{
    public class GameMasterController : ControllerBase
    {
        private DBContext dbcontext_;
        public IConfiguration Configuration { get; }
        public GameMasterController(dbcontext.DBContext db, IConfiguration configuration)
        {
            dbcontext_ = db;
            Configuration = configuration;
        }
        public static void WriteLine(string message)
        {
            Console.WriteLine(DateTime.Now.ToString());
            Console.WriteLine(message);
        }

        // GET api/values/5
        [HttpGet("[action]")]
        public ActionResult<string> test()
        {
            return "value";
        }
        [HttpGet("[action]")]
        public async Task logAsync(string logString, string stackTrace, byte type)
        {
            WriteLine(Request.HttpContext.ToString());
            try
            {
                await dbcontext_.UnityLog.AddAsync(
                new UnityLog
                {
                    Ip = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    LogString = logString,
                    StackTrace = stackTrace,
                    Type = type,
                    Time = DateTime.Now
                }
                );
                await dbcontext_.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                WriteLine(ex.ToString());
            }
            
        }

    }
}
