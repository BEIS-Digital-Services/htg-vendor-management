﻿using AutoMapper;
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
    public class ConfirmPrimaryUserChangePostHandler : IRequestHandler<ConfirmPrimaryUserChangePostHandler.Context, ConfirmPrimaryUserChangePostHandler.Result>
    {
        private readonly IManageUsersRepository _manageUsersRepository;
        private readonly INotifyService _notifyService;
        private readonly IMapper _mapper;
        private readonly PrimaryUserChangePostHandlerOptions _options;

        public ConfirmPrimaryUserChangePostHandler(INotifyService notifyService, IManageUsersRepository manageUsersRepository, 
            IMapper mapper, IOptions<PrimaryUserChangePostHandlerOptions> options)
        {
            _notifyService = notifyService;
            _manageUsersRepository = manageUsersRepository;
            _mapper = mapper;
            _options = options.Value;
        }

        public async Task<Result> Handle(Context request, CancellationToken cancellationToken)
        {
            var result = new Result();
            var users = (await _manageUsersRepository.GetAllUsers(request.Adb2CId)).ToList();
            
            var primaryContact = users.SingleOrDefault(u => u.primary_contact);
            if (primaryContact?.userid != request.ChangeUserId)
            {
                return result;
            }

            //Send Notify email
            var emailPendingUsers = users.Where(x => !string.IsNullOrEmpty(x.access_link));
            var emailPendingUsersVm = _mapper.Map<IList<VendorCompanyUserViewModel>>(emailPendingUsers);
            await _notifyService.SendEmailNotificationToAdditionalUsers(request.ChangeUserId, emailPendingUsersVm, _options.VendorAdditionalUserTemplateId);

            result.PrimaryContactSameAsChangeUserId = true;
            return result;
        }

        public class PrimaryUserChangePostHandlerOptions
        {
            public string VendorAdditionalUserTemplateId { get; set; }
        }

        public struct Context : IRequest<Result>
        {
            public long ChangeUserId { get; set; }

            public string Adb2CId { get; set; }
        }

        public class Result
        {
            public bool PrimaryContactSameAsChangeUserId { get; internal set; }
        }
    }
}