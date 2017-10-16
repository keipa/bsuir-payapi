using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using PayAPI.Models;

namespace PayAPI.Controllers
{
    public class BankController : ApiController
    {
        // GET api/bank
        
        public string Get()
        {
            using (var db = new BankContext())
            {
                var user =  new User {Name = "sa"};
                db.Users.Add(user);
                db.SaveChanges();
            }
            return "done";
        }

        [HttpGet]
        public string Mem()
        {
            return "hello";
        }

    }
}
