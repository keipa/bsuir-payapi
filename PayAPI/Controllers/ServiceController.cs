using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using PayAPI.Models;

namespace PayAPI.Controllers
{
    public class ServiceController : ApiController
    {
        [HttpGet]
        public List<string> Logs()
        {
            using (var db = new BankContext())
            {
                var logs = db.Logs.ToList();
                logs.Reverse();
                return logs.Select(log => $"{log.Id}. [{log.BrokenAt}]: {log.Exception}").ToList();
            }
        }
    }
}