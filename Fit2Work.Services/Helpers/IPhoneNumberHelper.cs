namespace AnvilGroup.Services.Fit2Work.Services.Helpers {
    public interface IPhoneNumberHelper {
        bool IsValidMobileNumber(string number);
        int GetCountryCode(string number);
    }
}