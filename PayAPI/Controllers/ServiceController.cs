using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using PayAPI.Models;
using static PayAPI.Business.ClientProcesses;

namespace PayAPI.Controllers
{
    public class ServiceController: ApiController
    {
        [System.Web.Http.HttpGet]
        public List<string> Logs()
        {
            using (var db  = new BankContext())
            {
                var logs = db.Logs.ToList();
                logs.Reverse();
                return logs.Select(log => ($"{log.Id}. [{log.BrokenAt}]: {log.Exception}")).ToList();
            }
        }
    }
}