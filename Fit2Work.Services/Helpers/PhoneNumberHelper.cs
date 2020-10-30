using AnvilGroup.Library.Comms.Utils;
using PhoneNumbers;

namespace AnvilGroup.Services.Fit2Work.Services.Helpers {
    /* NOTE: Uses http://code.google.com/p/libphonenumber/ 
       based on https://www.twilio.com/blog/validating-phone-numbers-effectively-with-c-and-the-net-frameworks
    */
    public class PhoneNumberHelper : IPhoneNumberHelper {
        PhoneNumberUtil _util;
        public PhoneNumberHelper() {
            _util = PhoneNumberUtil.GetInstance();
        }

        /// <summary>
        /// Phone number needs to be in format 447001002003
        /// </summary>
        public bool IsValidMobileNumber(string number) {
            if (number.StartsWith("+")) {
                return false;
            }
            if (number.StartsWith("00")) {
                return false;
            }
            var phoneNumber = BuildPhoneNumber(number);
            if(phoneNumber == null) {
                return false;
            }
            return _util.IsPossibleNumberForType(phoneNumber, PhoneNumberType.MOBILE);
        }

        public int GetCountryCode(string number) {

            return RecipientHelper.GetCountryCodeFromMobile(number);
        }

        private PhoneNumber BuildPhoneNumber(string number) {
            PhoneNumber result = null;
            var sanitized = RecipientHelper.GetSanitizedMobileNumber(number);
            int countryCode = RecipientHelper.GetCountryCodeFromMobile(number);
            var regionCode = _util.GetRegionCodeForCountryCode(countryCode);
            try
            {
                result = _util.Parse(sanitized, regionCode);
            }
            catch
            {
                return null;
            }
            return result;
        }
    }
}
