using System;
using PayAPI.Models;

namespace PayAPI.Business
{
    public static class ServiceProcesses
    {
        public static void LogException(Exception exception)
        {
            using (var db = new BankContext())
            {
                db.Logs.Add(new Log {Exception = exception.ToString(), BrokenAt = DateTime.Now});
                db.SaveChanges();
            }
        }

        public static void BanDevice(int infoDeviceHash)
        {
            throw new NotImplementedException();
        }

        public static bool DDosLimitReached(int infoDeviceHash)
        {
            throw new NotImplementedException();
        }

        public static void AggregateDDosFor(int infoDeviceHash)
        {
            throw new NotImplementedException();
        }
    }
}