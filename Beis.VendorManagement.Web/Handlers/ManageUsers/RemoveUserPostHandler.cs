namespace Beis.VendorManagement.Web.Handlers.ManageUsers
{
    public class RemoveUserPostHandler : IRequestHandler<RemoveUserPostHandler.Context>
    {
        private readonly IManageUsersRepository _manageUsersRepository;
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly INotifyService _notifyService;
        private readonly RemoveUserPostHandlerOptions _options;
        
        public RemoveUserPostHandler(INotifyService notifyService, IManageUsersRepository manageUsersRepository, 
            ICompanyUserRepository companyUserRepository, IOptions<RemoveUserPostHandlerOptions> options)
        {
            _notifyService = notifyService;
            _manageUsersRepository = manageUsersRepository;
            _companyUserRepository = companyUserRepository;
            _options = options.Value;
        }

        public async Task<Unit> Handle(Context request, CancellationToken cancellationToken)
        {
            //Send Notify email
            var user = await _companyUserRepository.GetUserByIdSingle(request.CurrentUserId);
            var emailUsers = new List<VendorCompanyUserViewModel>
            {
                new VendorCompanyUserViewModel { Email = user.email, CompanyId = user.companyid }
            };

            await _notifyService.SendEmailNotification(request.CurrentUserId, emailUsers, _options.VendorUserDeletedTemplateId);

            await _manageUsersRepository.DeleteUser(request.CurrentUserId);

            return Unit.Value;
        }

        public class RemoveUserPostHandlerOptions
        {
            public string VendorUserDeletedTemplateId { get; set; }
        }

        public struct Context : IRequest
        {
            public long CurrentUserId { get; internal set; }
        }
    }
}