using Microsoft.Extensions.DependencyInjection;
using TaxiBookingService.Logic.ServicesContract;

namespace TaxiBookingService.Logic.Services
{
    public interface IBookRideServiceSelector
    {
        IRideService2 GetGatewayByIdentifier(long identifier);
    }
    public class BookRideServiceSelector : IBookRideServiceSelector
    {
        private readonly IServiceProvider serviceProvider;

        public BookRideServiceSelector(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IRideService2 GetGatewayByIdentifier(long identifier)
        {
            switch (identifier)
            {
                case 1:
                    return serviceProvider.GetService<IRideServiceCar>();
                case 2:
                    return serviceProvider.GetService<IRideServiceBike>();
                default:
                    return null;
            }
        }
    }
}
