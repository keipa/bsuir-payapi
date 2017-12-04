using System;
using System.Collections.Generic;
using System.Web.Http;
using PayAPI.InputModels;
using PayAPI.Models;
using PayAPI.OutputModels;
using static PayAPI.Business.ClientProcesses;
using static PayAPI.Business.ServiceProcesses;
using System.Linq;

namespace PayAPI.Controllers
{
    public class BankController : ApiController
    {
        // GET api/get
        [HttpGet]
        public string Get()
        {
            return "PayAPI";
        }

        [HttpPost]
        public bool AddCard([FromBody] CardInfo info)
        {
            try
            {
                if (!AreCredantialsValid(info.CardId, info.CVV, info.CardHolderName) || !CardExist(info.CardId))
                    return false;
                var code = GenerateAuthorizationCode(info.CardId); // random 213221
                SendAuthorizationCode(code, info.CardId); // via pin or email
                ConnectCardAndDevice(info.CardId, info.DeviceHash); // add device into cards device dict 
                return true; //access granted
            }
            catch (Exception e)
            {
                LogException(e); // into log 
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

        [HttpPost]
        public List<Guid> RefreshTokenSet([FromBody] InstanceInfo info)
        {
            //if(CardActive())
            return GenerateNewTokensFor(info.DeviceHash, info.CardId);
        }

        [HttpPost]
        public Dictionary<string, List<Guid>> RefreshTokens([FromBody]  string device)
        {
            return GenerateNewTokensFor(device);
        }

        [HttpPost]
        public bool AddTransaction([FromBody] NewTransaction info)
        {
            if (!(IsTokenValidAndFresh(info.Token) &&
                  IsPossibleToTransferMoney(info.Token, info.Amount)))
                return false; // also rise an exception
            try
            {
                ExecuteTransaction(info.Token, info.Destination, info.Amount);
            }
            catch (Exception e)
            {
                LogException(e); // into log 
                return false;
            }
            return true;
        }



        [HttpGet]
        public List<CardValues> GetCards([FromUri] string deviceHash)
        {
            using (var db = new BankContext())
            {
                var list = new List<CardValues>();
                var cards = db.Activations.Where(x => x.Device.DeviceHash == deviceHash).Select(x => x.Card);
                foreach (var card in cards)
                {
                    if (!CardExist(card.CardId)) return new List<CardValues>();
                    list.Add(new CardValues { CardId = card.CardId, Value = GetCardValue(card.CardId) });
                }
                return list;
            }
        }


        [HttpPost]
        public bool DeleteCard([FromBody] CardInfo info)
        {
            if (!AreCredantialsValid(info.CardId, info.CVV, info.CardHolderName) || !CardExist(info.CardId))
                return false;
            try
            {
                DeactivateCard(info.CardId, info.DeviceHash);
                DeactivateTokens(info.DeviceHash);
                return true;
            }
            catch (Exception e)
            {
                LogException(e); // into log 
                return false;
            }
        }


      
    }
}