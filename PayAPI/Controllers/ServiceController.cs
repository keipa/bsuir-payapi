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
        public IEnumerable<User> Users()
        {
            using (var db = new BankContext())
            {
                return db.Users.ToList();
            }
        }

        [HttpGet]
        public IEnumerable<Card> Cards()
        {
            using (var db = new BankContext())
            {
                return db.Cards.ToList();
            }
        }

      



        [HttpGet]
        public IEnumerable<Device> Devices()
        {
            using (var db = new BankContext())
            {
                return db.Devices.ToList();
            }
        }

        [HttpPost]
        public void DeleteUser(User user)
        {

            using (var db = new BankContext())
            {
                if (db.Users.Any(x => x.Name != user.Name)) return;
                var dbuser = db.Users.FirstOrDefault(x => x.Name != user.Name);
                db.Users.Remove(dbuser);
                db.SaveChanges();
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

        //do not use in prod)
        [HttpPost]
        public void AddDevice(Device device)
        {

            using (var db = new BankContext())
            {
                var owner = db.Users.FirstOrDefault(x => x.Name == device.Owner.Name);
                if (db.Devices.Any(x => x.Name == device.Name || x.Owner == owner)) return;
                db.Devices.Add(new Device {Owner = owner, DeviceHash = "SERVICE", Name = "PHONE", });
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
                var new_card = new Card
                {
                    CardId = card.CardId,
                    Balance = card.Balance,
                    CVV = card.CVV,
                    Owner = db.Users.FirstOrDefault(x => x.Name == card.Owner.Name),
                    connected = false
                };
                db.Cards.Add(new_card);

                db.SaveChanges();

            }
        }

    }
}