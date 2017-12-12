using System;
using System.Collections.Generic;
using System.Web.Http;
using PayAPI.InputModels;
using PayAPI.Models;
using PayAPI.OutputModels;
using static PayAPI.Business.ClientProcesses;
using static PayAPI.Business.ServiceProcesses;
using System.Linq;
using System.Web;

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
            AreCredantialsValid(info.CardId, info.CVV, info.CardHolderName);
            CardAlreadyConnected(info.CardId, info.DeviceHash);
            var code = GenerateAuthorizationCode(info.CardId); // random 213221
            SendAuthorizationCode(code, info.CardId); // via pin or email
            ConnectCardAndDevice(info.CardId, info.DeviceHash); // add device into cards device dict 
            return true; //access granted
        }

        
        [HttpPost]
        public bool ConfirmCardBy([FromBody] AuthorizationInfo info)
        {
            IsBloked(info.DeviceHash); // // also rise an exception
            if (!CheckPinExistance(info.CardId, info.PIN))
            {
                AggregateDDosFor(info.DeviceHash); //WrongInputCount++
                if (DDosLimitReached(info.DeviceHash)) BanDevice(info.DeviceHash); // also rise an exception
                throw new HttpException(500, "Device is banned");
            }
            ActivateCard(info.CardId, info.DeviceHash);
            return true;

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
            IsTokenValidAndFresh(info.Token);
            IsPossibleToTransferMoney(info.Token, info.Amount);
            IsCardActive(info.Token);
            try
            {
                ExecuteTransaction(info.Token, info.Amount);
            }
            catch (Exception e)
            {
                LogException(e); // into log 
                throw new HttpException(500, "Transatction execution error");
            }
            return true;
        }



        [HttpGet]
        public List<CardValues> GetCards([FromUri] string deviceHash)
        {
            using (var db = new BankContext())
            {
                var list = new List<CardValues>();
                var cards = db.Activations.Where(x => x.Device.DeviceHash == deviceHash && x.isActive ).Select(x => x.Card);
                if (!cards.Any()) return new List<CardValues>();
                foreach (var card in cards)
                {
                    list.Add(new CardValues { CardId = card.CardId, Value = GetCardValue(card.CardId), CardHolderName = card.Owner.Name});
                }
                return list;
            }
        }


        [HttpPost]
        public bool DeleteCard([FromBody] CardInfo info)
        {
            if (!CardExist(info.CardId))
                throw new HttpException(500, "Credantials error");
            try
            {
                DeactivateCard(info.CardId, info.DeviceHash);
                DeactivateTokens(info.DeviceHash);
                return true;
            }
            catch (Exception e)
            {
                LogException(e); // into log 
                throw new HttpException(500, "Transatction execution error");
            }
        }



    }
}