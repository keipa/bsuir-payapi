using System;
using System.Configuration;
using System.Linq;
using PayAPI.Models;

namespace PayAPI.Business
{
    public static class ServiceProcesses
    {

        public static readonly int ddosLimit = Convert.ToInt32(ConfigurationManager.AppSettings["MaxDDosLimit"]);

        public static void LogException(Exception exception)
        {
            using (var db = new BankContext())
            {
                db.Logs.Add(new Log { Exception = exception.ToString(), BrokenAt = DateTime.Now });
                db.SaveChanges();
            }
        }

        public static void BanDevice(string infoDeviceHash)
        {
            using (var db = new BankContext())
            {
                try
                {
                    var device = db.Devices.FirstOrDefault(x => x.DeviceHash == infoDeviceHash);
                    device.WrongInputCount = 0;
                    device.BannedUntil = DateTime.Now + TimeSpan.FromMinutes(2);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    LogException(e);
                }
            }
        }

        public static bool DDosLimitReached(string infoDeviceHash)
        {
            using (var db = new BankContext())
            {
                try
                {
                    return db.Devices.FirstOrDefault(x => x.DeviceHash == infoDeviceHash).WrongInputCount >= ddosLimit;
                }
                catch (Exception e)
                {
                    LogException(e);
                    return false;
                }
            }
        }

        public static void AggregateDDosFor(string infoDeviceHash)
        {


            using (var db = new BankContext())
            {
                try
                {
                    db.Devices.FirstOrDefault(x => x.DeviceHash == infoDeviceHash).WrongInputCount += 1;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    LogException(e);
                }
            }
        }
    }
}