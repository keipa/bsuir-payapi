using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using PayAPI.InputModels;
using PayAPI.Models;

namespace PayAPI.Controllers
{
    public class BankController : ApiController
    {
        // GET api/bank
       [HttpGet]

        public string Get()
        {
            return "PayAPI ";
        }

        [HttpPost]
        public bool AddCard([FromBody] CardInfo info)
        {
            if (!CredantialsValid(info.CardId, info.CVV, info.CardHolderName) || !CardExist(info.CardId)) return false;
            try
            {
                var code = GenerateAuthorizationCode(info.CardId); // random 213221
                SendAuthorizationCode(code, info.CardId); // via pin or email
                ConnectCardAndDevice(info.CardId, info.DeviceHash); // add device into cards device dict 
                return true; //access granted
            }
            catch (Exception e)
            {
                LogException(e);// into log 
                return false;
            }
        }

        [HttpPost]
        public bool ConfirmCardBy([FromBody] AuthorizationInfo info)
        {
            if (IsBloked(info.DeviceHash)) return false; // // also rise an exception
            if (!CheckPinExistance(info.CardId, info.PIN))
            {
                AggregateDDosFor(info.DeviceHash); //WrongInputCount++
                if (DDosLimitReached(info.DeviceHash)) BanDevice(info.DeviceHash); // also rise an exception
                return false;
            }
            try
            {
                ActivateCard(info.CardId, info.DeviceHash);
                return true;
            }
            catch (Exception e)
            {
                LogException(e); // into log 
                return false;
            }

        }

        [HttpGet]
        public string GetLogs()
        {
            return "Logs";
        }

        [HttpPost]
        public bool AddTransaction([FromBody] NewTransaction info)
        {
            if (!(IsTokenValidAndFresh(info.Token) &&
                  IsPossibleToTransferMoney(info.Token, info.Destination, info.Amount))) return false; // also rise an exception
            try
            {
                ExecuteTransaction(info.Token, info.Destination, info.Amount); 
            }
            catch (Exception e)
            {
                LogException(e); // into log 
                return false;
            }
            
            return false;
        }


        [HttpPost]
        public List<Token> RefreshTokenSet([FromBody] InstanceInfo info)
        {
            if (CredantialsValid(info.DeviceHash, info.CardId, info.CardholderName)) return new List<Token>();
            return GenerateNewTokensFor(info.DeviceHash);
        }


        [HttpPost]
        public List<Token> DeleteCard([FromBody] InstanceInfo info)
        {
            
        }


        [HttpPost]
        public List<CardInfo> DeleteCard([FromBody] InstanceInfo info)
        {

        }

        //[HttpPost]
        //public List<Token> RefreshTokenSet()
        //{
        //    return false;
        //}



    }
}
