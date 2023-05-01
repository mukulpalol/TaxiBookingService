using TaxiBookingService.Common;

namespace TaxiBookingServices.API.Auth.AuthServiceContract
{
    #region LoginResponseDTO
    public class LoginResponseDTO : ResponseBase
    {
        public string AccessToken { get; set; }
    }
    #endregion

    #region ClaimResponse
    public class ClaimResponseDTO : ResponseBase
    {
        public string Email { get; set; }
    }
    #endregion

    #region SignUpResponse
    public class SignUpResponseDTO : ResponseBase { }
    #endregion

}
