namespace PayAPI.InputModels
{
    public class NewTransaction
    {
        public string Token { get; set; }
        public decimal Amount { get; set; }
    }
}