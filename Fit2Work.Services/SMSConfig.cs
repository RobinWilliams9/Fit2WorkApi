namespace AnvilGroup.Services.Fit2Work.Services {
    public class SMSConfig {
        public SMSConfig() { }
        public SMSConfig(string configValue) {
            var valueArray = configValue.Split('|');
            CountryCode = int.Parse(valueArray[0]);
            AccountId = valueArray[1];
            ApiKey = valueArray[2];
            ApiPassword = valueArray[3];
            IsDefault = bool.Parse(valueArray[4]);
        }
        public string AccountId { get; set; }
        public int CountryCode { get; set; }
        public string ApiKey { get; set; }
        public string ApiPassword { get; set; }
        public bool IsDefault { get; set; }
    }
}
