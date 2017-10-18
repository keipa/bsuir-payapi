namespace PayAPI.InputModels
{
    public class AuthorizationInfo
    {
        public string CardId { get; set; }
        public int PIN { get; set; }
        public string DeviceHash { get; set; }
    }
}