namespace PayAPI.InputModels
{
    public class AuthorizationInfo
    {
        public string CardId { get; set; }
        public int PIN { get; set; }
        public int DeviceHash { get; set; }
    }
}