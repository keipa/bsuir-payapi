using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using PayAPI.Models;
using static PayAPI.Business.ServiceProcesses;

namespace PayAPI.Business
{
    public static class ClientProcesses
    {
        private static Dictionary<Delegate, bool> AuthorizationProviders = new Dictionary<Delegate, bool>
        {
            {new Action<int,BankContext ,string>(SendAuthorizationCodeViaEmail), Convert.ToBoolean(ConfigurationManager.AppSettings["email"])},
            {new Action<int, BankContext,string>(SendAuthorizationCodeViaSMS), Convert.ToBoolean(ConfigurationManager.AppSettings["sms"])},
        };
        public static readonly int TokenSetCount = Convert.ToInt32(ConfigurationManager.AppSettings["TokenSetCount"]);
        public static readonly int ExpireAfterMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["ExpireAfterNminutes"]);

        public static bool AreCredantialsValid(string infoCardId, int infoCvv, string infoCardHolderName)
        {
            using (var db = new BankContext())
            {
                try
                {
                    var card = GetCardById(infoCardId, db);
                    return card.CVV == infoCvv && card.Owner.Name == infoCardHolderName;
                }
                catch (Exception e)
                {
                    LogException(e);
                    throw new HttpException(500, "Credentials are invalid");
                }
            }
        }

        public static int GenerateAuthorizationCode(string infoCardId)
        {
            using (var db = new BankContext())
            {
                try
                {
                    var code = new Random().Next(100000, 999999);
                    GetCardById(infoCardId, db).AuthorizationCode = code;
                    db.SaveChanges();
                    return code;
                }
                catch (Exception e)
                {
                    LogException(e);
                    throw new HttpException(500, "Authorization code generation error");
                }
            }
        }

        public static bool CardExist(string infoCardId)
        {
            using (var db = new BankContext())
            {
                try
                {
                    return db.Cards.Any(x => x.CardId == infoCardId);
                }
                catch (Exception e)
                {
                    LogException(e);
                    throw new HttpException(500, "Card is not exist");

                }
            }
        }

        public static void ConnectCardAndDevice(string infoCardId, string infoDeviceHash)
        {
            using (var db = new BankContext())
            {
                try
                {
                    var card = GetCardById(infoCardId, db);
                    var device = CreateDeviceIfNotExist(db, infoDeviceHash, card.Owner);
                    if (AreAnyDevicesConnected(db, card, device))
                    {
                        db.Activations.FirstOrDefault(x => x.Card == card && x.Device == device).isActive = false;
                    }
                    else
                    {
                        db.Activations.Add(new Activation { Card = card, Device = device, isActive = false });
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    LogException(e);
                    throw new HttpException(500, "Connection Card And Device error");
                }
            }
        }

        private static bool AreAnyDevicesConnected(BankContext db, Card card, Device device)
        {
            return db.Activations.Any(x => x.Card.CardId == card.CardId && x.Device.DeviceHash == device.DeviceHash);
        }

        private static Device CreateDeviceIfNotExist(BankContext db, string infoDeviceHash, User owner)
        {
            if (db.Devices.Any(x => x.DeviceHash == infoDeviceHash))
                return db.Devices.FirstOrDefault(x => x.DeviceHash == infoDeviceHash);
            var device = new Device { DeviceHash = infoDeviceHash, Owner = owner, Name = "Phone", BannedUntil = DateTime.Now };
            db.Devices.Add(device);
            return device;
        }

        public static void SendAuthorizationCodeViaEmail(int code, BankContext db, string infoCardId)
        {
            var to = GetEmailByCardId(db, infoCardId);
            var client = new SmtpClient();
            var email = new MailMessage();
            email.Body = code.ToString();
            email.To.Add(to);
            client.Send(email);
        }

        public static void SendAuthorizationCodeViaSMS(int code, BankContext db, string infoCardId)
        {
            var phone = GetPhoneByCardId(db, infoCardId);
        }

        public static void SendAuthorizationCode(int code, string infoCardId)
        {
            using (var db = new BankContext())
            {
                try
                {
                    foreach (var provider in AuthorizationProviders)
                    {
                        if (provider.Value == false) continue;
                        provider.Key.DynamicInvoke(code, db, infoCardId);
                    }
                }
                catch (Exception e)
                {
                    LogException(e);
                    throw new HttpException(500, "Send AuthorizationCode error");
                }
            }
        }

        private static object GetPhoneByCardId(BankContext db, string infoCardId)
        {
            return db.Cards.FirstOrDefault(x => infoCardId == x.CardId).Owner.phone;
        }

        private static string GetEmailByCardId(BankContext db, string infoCardId)
        {
            return db.Cards.FirstOrDefault(x => infoCardId == x.CardId).Owner.email;
        }

        public static void ActivateCard(string infoCardId, string infoDeviceHash)
        {
            using (var db = new BankContext())
            {
                try
                {
                    var card = db.Cards.FirstOrDefault(x => x.CardId == infoCardId);
                    var currentDevice = db.Devices.FirstOrDefault(x => x.DeviceHash == infoDeviceHash);
                    var activation = db.Activations.FirstOrDefault(x =>
                        x.Card.CardId == card.CardId && x.Device.DeviceHash == currentDevice.DeviceHash);
                    activation.isActive = true;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    LogException(e);
                    throw new HttpException(500, "Card activation error");
                }
            }
        }

        public static bool CheckPinExistance(string infoCardId, int infoPin)
        {
            using (var db = new BankContext())
            {
                try
                {
                    return db.Cards.FirstOrDefault(x => x.CardId == infoCardId).AuthorizationCode == infoPin;
                }
                catch (Exception e)
                {
                    LogException(e);
                    throw new HttpException(500, "PIN is not exist");
                }
            }
        }

        public static bool IsBloked(string infoDeviceHash)
        {
            using (var db = new BankContext())
            {
                try
                {
                    return db.Devices.FirstOrDefault(x => x.DeviceHash == infoDeviceHash).BannedUntil >= DateTime.Now;
                }
                catch (Exception e)
                {
                    LogException(e);
                    throw new HttpException(500, "Cannot check device status");
                }
            }
        }

        public static decimal GetCardValue(string cardCardId)
        {
            using (var db = new BankContext())
            {
                try
                {
                    return db.Cards.FirstOrDefault(x => x.CardId == cardCardId).Balance;
                }
                catch (Exception e)
                {
                    LogException(e);
                    return (decimal)-1.0;
                }
            }
        }

        public static void DeactivateTokens(string infoDeviceHash)
        {
            using (var db = new BankContext())
            {
                try
                {
                    var device = db.Devices.FirstOrDefault(x => x.DeviceHash == infoDeviceHash);
                    var tokens = db.Tokens.Where(x => x.RelatedDevice == device);
                    foreach (var token in tokens)
                    {
                        token.Used = true;
                    }
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    LogException(e);
                }
            }

        }

        public static void DeactivateCard(string infoCardId, string infoDeviceHash)
        {
            throw new NotImplementedException();
        }

        public static List<Guid> GenerateNewTokensFor(string infoDeviceHash, string cardid)
        {
            using (var db = new BankContext())
            {
                try
                {
                    var tokens = new List<Token>();
                    MakeTokensNotValid(infoDeviceHash, db);
                    var card = GetCardById(cardid, db);
                    var device = GetDeviceById(infoDeviceHash, db);
                    for (int i = 0; i < TokenSetCount; i++)
                    {
                        var newToken = new Token { Value = Guid.NewGuid(), Used = false, RelatedCard = card, ExpiredDate = DateTime.Now + TimeSpan.FromMinutes(ExpireAfterMinutes), RelatedDevice = device };
                        tokens.Add(newToken);
                        db.Tokens.Add(newToken);
                    }
                    db.SaveChanges();
                    return tokens.Select(x => x.Value).ToList();

                }
                catch (Exception e)
                {
                    LogException(e);
                    throw new HttpException(500, "Token Generation Error");
                }
            }
        }
        public static Dictionary<string, List<Guid>> GenerateNewTokensFor(string infoDeviceHash)
        {
            using (var db = new BankContext())
            {
                var tokenDict = new Dictionary<string, List<Guid>>();
                var cards = db.Activations.Where(x => x.Device.DeviceHash == infoDeviceHash).Select(x => x.Card).ToList();
                foreach (var card in cards)
                {

                    try
                    {
                        var tokens = new List<Token>();
                        MakeTokensNotValid(infoDeviceHash, db);
                        var device = GetDeviceById(infoDeviceHash, db);
                        for (int i = 0; i < TokenSetCount; i++)
                        {
                            var newToken = new Token { Value = Guid.NewGuid(), Used = false, RelatedCard = card, ExpiredDate = DateTime.Now + TimeSpan.FromMinutes(ExpireAfterMinutes), RelatedDevice = device };
                            tokens.Add(newToken);
                            db.Tokens.Add(newToken);
                        }
                        db.SaveChanges();
                        
                        tokenDict.Add(card.CardId,tokens.Select(x => x.Value).ToList());

                    }
                    catch (Exception e)
                    {
                        LogException(e);
                        throw new HttpException(500, "Token Generation Error");
                    }
                }
                return tokenDict;
            }

        }

        private static Card GetCardById(string cardid, BankContext db)
        {
            return db.Cards.FirstOrDefault(x => x.CardId == cardid);
        }

        private static Device GetDeviceById(string devicehash, BankContext db)
        {
            return db.Devices.FirstOrDefault(x => x.DeviceHash == devicehash);

        }

        private static void MakeTokensNotValid(string infoDeviceHash, BankContext db)
        {
            var device = db.Devices.FirstOrDefault(x => x.DeviceHash == infoDeviceHash);
            var tokens = db.Tokens.Where(x => x.RelatedDevice.DeviceHash == device.DeviceHash);
            if (tokens is null || !tokens.Any() )
            {
                return;
            }
            foreach (var token in tokens)
            {
                token.Used = true;
            }
            db.SaveChanges();
        }

        public static void ExecuteTransaction(string infoToken,  decimal infoAmount)
        {
            using (var db = new BankContext())
            {
                var fromCard = GetCardByToken(db, infoToken);
                decimal fromAmount = fromCard.Balance;
                try
                {
                    UseToken(db, infoToken);
                    fromCard.Balance -= infoAmount;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    fromCard.Balance = fromAmount;
                    db.SaveChanges();
                    throw;
                }
                

            }

        }

        private static void UseToken(BankContext db, string infoToken)
        {
            bool isUsed = true;
            db.Tokens.FirstOrDefault(x => x.Value.ToString() == infoToken).Used = isUsed;

        }


        private static Card GetCardByToken(BankContext db, string infotoken)
        {
            return db.Tokens.FirstOrDefault(x => x.Value.ToString() == infotoken).RelatedCard;
        } 

        public static bool IsPossibleToTransferMoney(string infoToken, decimal infoAmount)
        {
            using (var db = new BankContext())
            {
                try
                {
                    var fromCard = db.Tokens.FirstOrDefault(x => x.Value.ToString() == infoToken).RelatedCard;
                    return fromCard.Balance >= infoAmount;
                }
                catch (Exception e)
                {
                    LogException(e);
                    throw new HttpException(500, "It's not possible to transfer money");

                }
            }

        }

        public static bool IsTokenValidAndFresh(string infoToken)
        {
            using (var db = new BankContext())
            {
                try
                {
                    var token = db.Tokens.FirstOrDefault(x => x.Value.ToString() == infoToken);
                    bool exist = db.Tokens.Any(x => x.Value.ToString() == infoToken);
                    bool notUsed = !token.Used;
                    bool fresh = token.ExpiredDate <= DateTime.Now;
                    return notUsed && fresh && exist;
                }
                catch (Exception e)
                {
                    LogException(e);
                    throw new HttpException(500, "Token Is not valid");

                }
            }

        }
    }
}