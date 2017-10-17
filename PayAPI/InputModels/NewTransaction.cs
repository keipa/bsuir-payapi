namespace PayAPI.InputModels
{
    public class NewTransaction
    {
        public string Token { get; set; }
        public string Destination { get; set; } //destination card to send money
        public decimal Amount { get; set; }
    }
}