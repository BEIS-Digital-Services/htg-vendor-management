using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.HelpToGrow.Web.Models;
using Beis.Htg.VendorSme.Database.Models;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Web.Handlers.Home
{
    public class IndexGetHandler : IRequestHandler<IndexGetHandler.Context, Optional<IndexGetHandler.Result>>
    {
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly IManageUsersRepository _manageUsersRepository;
        private readonly ISettingsProductTypesRepository _settingsProductTypesRepository;
        private readonly ILogger<IndexGetHandler> _logger;

        public IndexGetHandler(
            ICompanyUserRepository companyUserRepository,
            IManageUsersRepository manageUsersRepository,
            ISettingsProductTypesRepository settingsProductTypesRepository,
            ILogger<IndexGetHandler> logger)
        {
            _companyUserRepository = companyUserRepository;
            _manageUsersRepository = manageUsersRepository;
            _settingsProductTypesRepository = settingsProductTypesRepository;
            _logger = logger;
        }

        public async Task<Optional<Result>> Handle(Context request, CancellationToken cancellationToken)
        {
            if (request.UserId == null)
            {
                _logger.LogError("The Microsoft ADB2C user doesn't have id. NameIdentifier is missing");
                return default;
            }

            _logger.LogInformation($"The logged Microsoft ADB2C account id is: {request.UserId}");

            if (request.Email == null)
            {
                _logger.LogError("The Microsoft ADB2C user doesn't have email. Email is missing");
                return default;
            }

            _logger.LogInformation($"Authenticated user id is {request.UserId}");

            var user = await GetUser(request.UserId, request.AccessLinkId);
            if (user == null)
            {
                _logger.LogError($"There is not an user in the database for the logged Microsoft ADB2C account id: {request.UserId}");
                return default;
            }

            var company = await _companyUserRepository.GetCompanyByUserSingle(user.adb2c);
            var accessSecret = company.access_secret;
            if (string.IsNullOrWhiteSpace(accessSecret))
            {
                //Add a new API key
                accessSecret = Guid.NewGuid().ToString().ToUpper();
                company.access_secret = accessSecret;
                await _companyUserRepository.UpdateCompany(company);
            }

            return new Result
            {
                AccountHome = new AccountHomeViewModel
                {
                    Products = await GetProductsByUserId(user.userid),
                    RegistrationNumber = company.registration_id,
                    CompanyId = company.vendorid,
                    CompanyName = company.vendor_company_name,
                    Adb2CId = user.adb2c,
                    ApiKey = accessSecret
                }
            };
        }

        private async Task<vendor_company_user> GetUser(string adb2CUserId, string accessLinkId)
        {
            //Check if that is just created ADB2C user
            if (string.IsNullOrWhiteSpace(accessLinkId))
            {
                return await _companyUserRepository.GetUserIdByAdb2CUserId(adb2CUserId);
            }

            var newlyCreatedUser = await _companyUserRepository.GetUserByAccessLink(accessLinkId);
            if (newlyCreatedUser != null)
            {
                await UpdateNewUser(adb2CUserId, newlyCreatedUser.userid);
            }

            return newlyCreatedUser;
        }

        private async Task UpdateNewUser(string adb2CUserId, long userId)
        {
            var user = await _manageUsersRepository.GetUserSingle(userId);
            if (user != null)
            {
                await _manageUsersRepository.UpdateAdb2CForUser(userId, adb2CUserId);
            }
        }

        private async Task<IList<ProductCategoryViewModel>> GetProductsByUserId(long id)
        {
            var productCategories = new List<ProductCategoryViewModel>();

            var products = await _companyUserRepository.GetProductsByUserIdSingle(id);

            foreach (var product in products)
            {
                productCategories.Add(new ProductCategoryViewModel
                {
                    Product = new ProductViewModel
                    {
                        ProductName = product.product_name,
                        ProductId = product.product_id
                    },
                    TypeName = await _settingsProductTypesRepository.GetSettingsProductType(product.product_type)
                });
            }

            return productCategories;
        }

        public struct Context : IRequest<Optional<Result>>
        {
            public string UserId { get; set; }

            public string Email { get; set; }

            public string AccessLinkId { get; set; }
        }

        public class Result
        {
            public AccountHomeViewModel AccountHome { get; set; }
        }
    }
}