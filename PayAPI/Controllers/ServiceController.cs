using System;
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


        [HttpGet]
        public List<string> Users()
        {
            using (var db = new BankContext())
            {
                var users = db.Users.ToList();
                var cards = db.Cards.ToList();
                var devices = db.Devices.ToList();
                return users.Select(usr => $"{usr.Id};" +
                                           $"[{usr.Name}];" +
                                           $"{usr.email};" +
                                           "Cards:" + (cards.All(x => x.Owner != usr) ? "no cards; " : string.Join(", ", cards.Where(x => x.Owner == usr).Select(x => x.CardId ?? string.Empty).ToList())) + ":" +
                                           "Devices:" + (devices.All(x => x.Owner != usr) ? "no devices;" : string.Join(", ", devices.Where(x => x.Owner == usr).Select(x => x.DeviceHash ?? string.Empty).ToList()))).ToList();
            }
        }



        //header
        //Content-Type:
        // body
        //{
        //    "email": "nickolas199624@gmail.com",
        //    "phone": "3333333",
        //    "Name": "Nikolay",
        //}

        [HttpPost]
        public void AddUser(User user)
        {

            using (var db = new BankContext())
            {
                if (db.Users.Any(x => x.Name == user.Name)) return;
                db.Users.Add(new User { Name = user.Name, email = user.email, phone = user.phone });
                db.SaveChanges();
            }
        }


        //header
        //Content-Type:
        // body
        //{
        //    "cardid": "0000 1111 2222 3333",
        //    "balance": "0",
        //    "CVV": "123",
        //    "Owner" :{
        //        "Name":"Nikolay"
        //    }
        //}

        [HttpPost]
        public void AddCard(Card card)
        {
            using (var db = new BankContext())
            {
                if (db.Cards.Any(x => x.CardId == card.CardId)) return;
                var owner = db.Users.FirstOrDefault(x => x.Name == card.Owner.Name);
                var new_card = new Card
                {
                    CardId = card.CardId,
                    Balance = card.Balance,
                    CVV = card.CVV,
                    Owner = owner,
                    connected = false
                };
                db.Cards.Add(new_card);

                db.SaveChanges();
            }
        }

    }
}