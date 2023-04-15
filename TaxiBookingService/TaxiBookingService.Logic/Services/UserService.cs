using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TaxiBookingService.Common;
using TaxiBookingService.DAL.Models;
using TaxiBookingService.DAL.Repositories;
using TaxiBookingServices.API.Service_Contract;

namespace TaxiBookingService.Logic.Services
{
    public interface IUserService
    {
        Task<ResponseBase> AddCustomer(CustomerAddDTO customerAdd);
        Task<ResponseBase> AddDriver(DriverAddDTO driverAdd);
        ClaimResponseDTO GetCurrentUser();
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<UserService> logger;
        private readonly ResponseBase responseBase;
        private readonly IHttpContextAccessor httpContext;

        #region Constructor
        public UserService(IUserRepository userRepository, ILogger<UserService> logger, IHttpContextAccessor httpContext)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            responseBase = new ResponseBase();
            this.httpContext = httpContext;

        }
        #endregion

        #region AddCustomer
        public async Task<ResponseBase> AddCustomer(CustomerAddDTO customerAdd)
        {
            try
            {
                if (customerAdd == null)
                {
                    responseBase.ResponseMsg = "Null input";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Error in AddCustomer: Null Input");
                    return responseBase;
                }
                if (await userRepository.UserEmailExists(customerAdd.Email) != null)
                {
                    responseBase.ResponseMsg = "Customer with this email already exists";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Error in AddCustomer: Customer with this email already exists");
                    return responseBase;
                }
                if (await userRepository.UserPhoneExists(customerAdd.PhoneNumber) != null)
                {
                    responseBase.ResponseMsg = "Customer with this phone number already exists";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Error in AddCustomer: Customer with this phone number already exists");
                    return responseBase;
                }
                User user = new()
                {
                    FirstName = customerAdd.FirstName,
                    LastName = customerAdd.LastName,
                    Email = customerAdd.Email,
                    Password = customerAdd.Password,
                    Dob = customerAdd.Dob,
                    Gender = customerAdd.Gender,
                    PhoneNumber = customerAdd.PhoneNumber,
                    RoleId = 2
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
        public async Task<ResponseBase> AddDriver(DriverAddDTO driverAdd)
        {
            try
            {
                if (driverAdd == null)
                {
                    responseBase.ResponseMsg = "Null input";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Error in AddCustomer: Null Input");
                    return responseBase;
                }
                if (await userRepository.UserEmailExists(driverAdd.Email) != null)
                {
                    responseBase.ResponseMsg = "Driver with this email already exists";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Error in AddDriver: Driver with this email already exists");
                    return responseBase;
                }
                if (await userRepository.UserPhoneExists(driverAdd.PhoneNumber) != null)
                {
                    responseBase.ResponseMsg = "Driver with this phone number already exists";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Error in AddDriver: Driver with this phone number already exists");
                    return responseBase;
                }
                if (await userRepository.VehicleExists(driverAdd.VehicleNumber) != null)
                {
                    responseBase.ResponseMsg = "Vehicle with this vehicle number already exists";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Error in AddDriver: Vehicle with this vehicle number already exists");
                    return responseBase;
                }
                if (await userRepository.LocationExists(driverAdd.LocationId) == null)
                {
                    responseBase.ResponseMsg = "LocationId invalid";
                    responseBase.ResponseResult = ResponseResult.Warning;
                    logger.LogWarning("Error in AddDriver: LocationId invalid");
                    return responseBase;
                }
                Vehicle vehicle = new Vehicle()
                {
                    VehicleNumber = driverAdd.VehicleNumber,
                    VehicleTypeId = driverAdd.VehicleTypeId,
                    ModelName = driverAdd.ModelName
                };
                User user = new User()
                {
                    FirstName = driverAdd.FirstName,
                    LastName = driverAdd.LastName,
                    Email = driverAdd.Email,
                    Password = driverAdd.Password,
                    Dob = driverAdd.Dob,
                    Gender = driverAdd.Gender,
                    PhoneNumber = driverAdd.PhoneNumber,
                    RoleId = 3
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
            try
            {
                var identity = httpContext.HttpContext.User.Identity as ClaimsIdentity;
                if (identity == null)
                {
                    ClaimResponseDTO claimResponseDTO = new ClaimResponseDTO()
                    {
                        ResponseMsg = "ClaimsIdentity is null",
                        ResponseResult = ResponseResult.Warning
                    };
                    logger.LogWarning("Error in GetCurrentUser: ClaimsIdentity is null");
                    return claimResponseDTO;
                }
                var userClaims = identity.Claims;
                string UserEmail = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value;
                ClaimResponseDTO claimResponse = new ClaimResponseDTO()
                {
                    Email = UserEmail,
                    ResponseResult = ResponseResult.Success
                };
                return claimResponse;
            }
            catch (Exception ex)
            {
                ClaimResponseDTO claimResponseDTO = new ClaimResponseDTO()
                {
                    ResponseMsg = $"{ex.Message}",
                    ResponseResult = ResponseResult.Exception
                };
                logger.LogError($"Error in GetCurrentUser: {ex.Message}");
                return claimResponseDTO;
            }
        }
        #endregion
    }
}
