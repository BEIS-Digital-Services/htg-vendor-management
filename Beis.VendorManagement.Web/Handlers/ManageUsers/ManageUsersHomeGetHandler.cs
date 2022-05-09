using AutoMapper;
using Beis.VendorManagement.Repositories.Interface;
using Beis.VendorManagement.Web.Models;
using MediatR;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Web.Handlers.ManageUsers
{
    public class ManageUsersHomeGetHandler : IRequestHandler<ManageUsersHomeGetHandler.Context, Optional<IEnumerable<VendorCompanyUserViewModel>>>
    {
        private readonly IManageUsersRepository _manageUsersRepository;
        private readonly IMapper _mapper;

        public ManageUsersHomeGetHandler(IManageUsersRepository manageUsersRepository, IMapper mapper)
        {
            _manageUsersRepository = manageUsersRepository;
            _mapper = mapper;
        }

        public async Task<Optional<IEnumerable<VendorCompanyUserViewModel>>> Handle(Context request, CancellationToken cancellationToken)
        {
            var users = await _manageUsersRepository.GetAllUsers(request.Adb2CId);
            var usersVm = _mapper.Map<IEnumerable<VendorCompanyUserViewModel>>(users).ToList();

            usersVm.Sort((x, y) => x.FullName.CompareTo(y.FullName));
            usersVm.Sort((x, y) => y.PrimaryContact.CompareTo(x.PrimaryContact));

            return usersVm;
        }

        public struct Context : IRequest<Optional<IEnumerable<VendorCompanyUserViewModel>>>
        {
            public string Adb2CId { get; set; }
        }
    }
}