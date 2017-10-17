using System.Collections.Generic;

namespace PayAPI.InputModels
{
    public class RequestedCards
    {
        public List<SimpleCardInfo> cards;
    }

    public class SimpleCardInfo
    {
        public string CardId { get; set; }
        public string CardholderName { get; set; }
        public string BankName { get; set; }
    }
}