using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingService.Common
{
    public enum RideStatus
    {
        Searching = 1,
        Booked = 2,
        RideStarted = 3,
        RideCompleted = 4,
        RideCancelled = 5,
        NoDriversAvailable = 6
    }

    public enum VehicleTypeEnum
    {
        Hatchback = 1,
        Sedan = 2,
        SUV = 3
    }

    public enum CancelReasonEnum
    {
        DriverDeniedDuty = 1,
        EnteredWrongDropLocation = 2,
        DriverArrivedTooEarly = 3,
        ChangeOfPlans = 4
    }

    public enum RoleEnum
    {
        Admin = 1,
        Customer = 2,
        Driver = 3
    }

    public enum SettingsEnum
    {
        CancellationFactor = 1,
        DriverRange = 2
    }
}
