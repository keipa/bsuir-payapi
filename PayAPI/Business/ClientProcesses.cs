using System;
using System.Linq;
using System.Net.Mail;
using PayAPI.Models;
using static PayAPI.Business.ServiceProcesses;

namespace PayAPI.Business
{
    public static class ClientProcesses
    {
//        private static Dictionary<Delegate, bool> AuthorizationProviders = new Dictionary<Delegate, bool>
//        {
//            {new Action<int, string>(SendAuthorizationCodeViaEmail), Convert.ToBoolean(ConfigurationManager.AppSettings["email"])},
//            {new Action<int, string>(SendAuthorizationCodeViaSMS), Convert.ToBoolean(ConfigurationManager.AppSettings["sms"])},
//        };
//        
        public static bool AreCredantialsValid(string infoCardId, int infoCvv, string infoCardHolderName)
        {
            using (var db = new BankContext())
            {
                try
                {
                    var card = db.Cards.FirstOrDefault(x => x.CardId == infoCardId);
                    return card.CVV == infoCvv && card.Owner.Name == infoCardHolderName;
                }
                catch (Exception e)
                {
                    LogException(e);
                    return false;
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
                    db.Cards.FirstOrDefault(x => x.CardId == infoCardId).AuthorizationCode = code;
                    db.SaveChanges();
                    return code;
                }
                catch (Exception e)
                {
                    LogException(e);
                    throw;
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
                    return false;
                }
            }
        }

        public static void ConnectCardAndDevice(string infoCardId, string infoDeviceHash)
        {
            using (var db = new BankContext())
            {
                try
                {
                    var card = db.Cards.FirstOrDefault(x => x.CardId == infoCardId);
                    var device = new Device {DeviceHash = infoDeviceHash, Owner = card.Owner, Name = "Phone"};
                    card.DevicesConnected.Add(device, false);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    LogException(e);
                    throw;
                }
            }
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
//                    foreach (var provider in AuthorizationProviders)
//                    {
//                        if (provider.Value == false) continue;
//                        provider.Key.DynamicInvoke(code, db, infoCardId);
//                    }
                }
                catch (Exception e)
                {
                    LogException(e);
                    throw;
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

        public static void ActivateCard(string infoCardId, int infoDeviceHash)
        {
            throw new NotImplementedException();
        }

        public static bool CheckPinExistance(string infoCardId, int infoPin)
        {
            throw new NotImplementedException();
        }

        public static bool IsBloked(int infoDeviceHash)
        {
            throw new NotImplementedException();
        }

        public static decimal GetCardValue(string cardCardId)
        {
            throw new NotImplementedException();
        }

        public static void DeactivateTokens()
        {
            throw new NotImplementedException();
        }

        public static void DeactivateCard(string infoCardId, string infoDeviceHash)
        {
            throw new NotImplementedException();
        }

        public static void ExecuteTransaction(string infoToken, string infoDestination, decimal infoAmount)
        {
            throw new NotImplementedException();
        }

        public static bool IsPossibleToTransferMoney(string infoToken, string infoDestination, decimal infoAmount)
        {
            throw new NotImplementedException();
        }

        public static bool IsTokenValidAndFresh(string infoToken)
        {
            throw new NotImplementedException();
        }
    }
}