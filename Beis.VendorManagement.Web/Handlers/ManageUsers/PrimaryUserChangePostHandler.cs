using AutoMapper;
using Beis.VendorManagement.Repositories.Interface;
using Beis.VendorManagement.Web.Models;
using Beis.VendorManagement.Web.Services.Interface;
using MediatR;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Beis.VendorManagement.Web.Handlers.ManageUsers
{
    public class PrimaryUserChangePostHandler : IRequestHandler<PrimaryUserChangePostHandler.Context>
    {
        private readonly IManageUsersRepository _manageUsersRepository;
        private readonly INotifyService _notifyService;
        private readonly IMapper _mapper;
        private readonly PrimaryUserChangePostHandlerOptions _options;

        public PrimaryUserChangePostHandler(INotifyService notifyService, IManageUsersRepository manageUsersRepository, IMapper mapper, IOptions<PrimaryUserChangePostHandlerOptions> options)
        {
            _notifyService = notifyService;
            _manageUsersRepository = manageUsersRepository;
            _mapper = mapper;
            _options = options.Value;
        }

        public async Task<Unit> Handle(Context request, CancellationToken cancellationToken)
        {
            await _manageUsersRepository.UpdatePrimaryContact(request.UserId, request.CompanyId);
            var users = await _manageUsersRepository.GetAllUsers(request.Adb2CId);
            var usersVm = _mapper.Map<IEnumerable<VendorCompanyUserViewModel>>(users);

            //Send Notify email
            var emailPendingUsers = usersVm.Where(x => x.AccessLink != null).ToList();
            await _notifyService.SendEmailNotificationToAdditionalUsers(request.UserId, emailPendingUsers, _options.VendorAdditionalUserTemplateId);

            return Unit.Value;
        }

        public class PrimaryUserChangePostHandlerOptions
        {
            public string VendorAdditionalUserTemplateId { get; set; }
        }

        public struct Context : IRequest
        {
            public long UserId { get; set; }

            public long CompanyId { get; set; }

            public string Adb2CId { get; set; }
        }
    }
}