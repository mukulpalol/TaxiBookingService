namespace TaxiBookingService.Common
{
    public enum ResponseResult
    {
        Success,
        Warning,
        Exception
    }

    public class ResponseBase
    {
        public ResponseResult ResponseResult { get; set; }
        public string ResponseMsg { get; set; }
    }

}