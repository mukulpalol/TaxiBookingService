namespace TaxiBookingService.Common
{
    public class CheckValidation
    {
        #region NullCheck
        public static T NullCheck<T>(T value, string errorMessage) where T : class
        {
            if (value == null)
            {
                throw new Exception(errorMessage);
            }
            return value;
        }
        #endregion

        #region NotNullCheck
        public static T NotNullCheck<T>(T value, string errorMessage) where T : class
        {
            if (value != null)
            {
                throw new Exception(errorMessage);
            }
            return value;
        }
        #endregion
    }
}
