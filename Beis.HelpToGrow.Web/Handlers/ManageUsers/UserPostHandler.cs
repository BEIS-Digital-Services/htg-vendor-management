using AutoMapper;
using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.HelpToGrow.Web.Models;
using Beis.Htg.VendorSme.Database.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Web.Handlers.ManageUsers
{
    public class UserPostHandler : IRequestHandler<UserPostHandler.Context, bool>
    {

        private readonly IManageUsersRepository _manageUsersRepository;
        private readonly IMapper _mapper;

        public UserPostHandler(IManageUsersRepository manageUsersRepository, IMapper mapper)
        {
            _manageUsersRepository = manageUsersRepository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(Context request, CancellationToken cancellationToken)
        {
            if (request.UserId == 0)
            {
                var existingUser = await _manageUsersRepository.GetUserByEmailAndCompanyIdSingle(request.Email, request.CompanyId);
                if (existingUser != null) // new user with existing email address.
                {
                    return false; 
                }

                var vendorCompanyUserViewModel = new VendorCompanyUserViewModel
                {
                    FullName = request.FullName,
                    Email = request.Email.Trim(),
                    CompanyId = request.CompanyId,
                    Status = true,
                    PrimaryContact = false,
                    AccessLink = Guid.NewGuid().ToString()
                };

                var user = _mapper.Map<vendor_company_user>(vendorCompanyUserViewModel);
                await _manageUsersRepository.AddUser(user);
                return true;
            }

            await _manageUsersRepository.UpdateUser(request.UserId, request.FullName);
            return true;
        }

        public struct Context : IRequest<bool>
        {
            public long UserId { get; set; }

            public long CompanyId { get; set; }

            public string FullName { get; set; }

            public string Email { get; set; }
        }
    }
}