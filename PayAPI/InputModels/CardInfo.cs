namespace PayAPI.InputModels
{
    public class CardInfo
    {
        
        public string CardId { get; set; }
        public int CVV { get; set; }
        public string CardHolderName { get; set; }
        public string DeviceHash { get; set; }
    }
}