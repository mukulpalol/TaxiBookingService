using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Security.Claims;
using TaxiBookingService.Common;
using TaxiBookingService.DAL.Models;
using TaxiBookingService.DAL.RepositoriesContract;
using TaxiBookingService.Logic.ServicesContract;
using TaxiBookingServices.API.Auth.AuthServiceContract;

namespace TaxiBookingService.Logic.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<UserService> logger;
        private readonly SignUpResponseDTO responseBase;
        private readonly IHttpContextAccessor httpContext;

        #region Constructor
        public UserService(IUserRepository userRepository, ILogger<UserService> logger, IHttpContextAccessor httpContext)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            responseBase = new SignUpResponseDTO();
            this.httpContext = httpContext;

        }
        #endregion

        #region AddCustomer
        public async Task<SignUpResponseDTO> AddCustomer(CustomerAddDTO customerAdd)
        {
            try
            {
                customerAdd = CheckValidation.NullCheck(customerAdd, "Null input");
                var userEmailCheck = await userRepository.UserEmailExists(customerAdd.User.Email);
                userEmailCheck = CheckValidation.NotNullCheck(userEmailCheck, "Customer with this email already exists");
                var userPhoneCheck = await userRepository.UserPhoneExists(customerAdd.User.PhoneNumber);
                userPhoneCheck = CheckValidation.NotNullCheck(userPhoneCheck, "Customer with this phone number already exists");
                DateTime dt = DateTime.ParseExact(customerAdd.User.Dob, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                User user = new()
                {
                    FirstName = customerAdd.User.FirstName,
                    LastName = customerAdd.User.LastName,
                    Email = customerAdd.User.Email.ToLower(),
                    Password = customerAdd.User.Password,
                    Dob = dt,
                    Gender = customerAdd.User.Gender.ToLower(),
                    PhoneNumber = customerAdd.User.PhoneNumber,
                    RoleId = (int)RoleEnum.Customer
                };
                Customer customer = new Customer()
                {
                    User = user,
                    AmountDue = 0,
                    Rating = 0,
                    TotalRides = 0
                };
                await userRepository.AddCustomer(user, customer);
                responseBase.ResponseMsg = "Customer added successfully";
                responseBase.ResponseResult = ResponseResult.Success;
                return responseBase;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in AddCustomer: {ex.Message}");
                responseBase.ResponseMsg = $"Error in AddCustomer: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
            }
        }
        #endregion
        
        #region AddDriver
        public async Task<SignUpResponseDTO> AddDriver(DriverAddDTO driverAdd)
        {
            try
            {
                driverAdd = CheckValidation.NullCheck(driverAdd, "Null Input");
                var userEmailCheck = await userRepository.UserEmailExists(driverAdd.User.Email);
                userEmailCheck = CheckValidation.NotNullCheck(userEmailCheck, "Driver with this email already exists");
                var userPhoneCheck = await userRepository.UserPhoneExists(driverAdd.User.PhoneNumber);
                userPhoneCheck = CheckValidation.NotNullCheck(userPhoneCheck, "Driver with this phone number already exists");
                var vehicleCheck = await userRepository.VehicleExists(driverAdd.VehicleNumber);
                vehicleCheck = CheckValidation.NotNullCheck(vehicleCheck, "Vehicle with this vehicle number already exists");
                var locationCheck = await userRepository.LocationExists(driverAdd.LocationId);
                locationCheck = CheckValidation.NullCheck(locationCheck, "Invalid location");                
                Vehicle vehicle = new Vehicle()
                {
                    VehicleNumber = driverAdd.VehicleNumber,
                    VehicleTypeId = driverAdd.VehicleTypeId,
                    ModelName = driverAdd.ModelName
                };
                DateTime dt = DateTime.ParseExact(driverAdd.User.Dob, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                User user = new User()
                {
                    FirstName = driverAdd.User.FirstName,
                    LastName = driverAdd.User.LastName,
                    Email = driverAdd.User.Email.ToLower(),
                    Password = driverAdd.User.Password,
                    Dob = dt,
                    Gender = driverAdd.User.Gender.ToLower(),
                    PhoneNumber = driverAdd.User.PhoneNumber,
                    RoleId = (int)RoleEnum.Driver
                };
                Driver driver = new Driver()
                {
                    LocationId = driverAdd.LocationId,
                    Rating = 0,
                    TotalRides = 0,
                    DrivingLicenseNumber = driverAdd.DrivingLicenseNumber,
                    Vehicle = vehicle,
                    User = user
                };
                await userRepository.AddDriver(vehicle, user, driver);
                responseBase.ResponseMsg = "Driver added successfully";
                responseBase.ResponseResult = ResponseResult.Success;
                return responseBase;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error in AddDriver: {ex.Message}");
                responseBase.ResponseMsg = $"Error in AddDriver: {ex.Message}";
                responseBase.ResponseResult = ResponseResult.Exception;
                return responseBase;
            }
        }
        #endregion       

        #region GetCurrentUser
        public ClaimResponseDTO GetCurrentUser()
        {
            ClaimResponseDTO claimResponse = new ClaimResponseDTO();
            try
            {
                var identity = httpContext.HttpContext.User.Identity as ClaimsIdentity;
                if (identity == null)
                {
                    claimResponse.ResponseMsg = "ClaimsIdentity is null";
                    claimResponse.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Error in GetCurrentUser: ClaimsIdentity is null");
                    return claimResponse;
                }
                var userClaims = identity.Claims;
                string UserEmail = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value;
                claimResponse.Email = UserEmail;
                claimResponse.ResponseResult = ResponseResult.Success;
                return claimResponse;
            }
            catch (Exception ex)
            {
                claimResponse.ResponseMsg = $"{ex.Message}";
                claimResponse.ResponseResult = ResponseResult.Exception;
                logger.LogError($"Error in GetCurrentUser: {ex.Message}");
                return claimResponse;
            }
        }
        #endregion        
    }
}
