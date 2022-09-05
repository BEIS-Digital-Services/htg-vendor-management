namespace Beis.VendorManagement.Web.Handlers.ManageUsers
{
    public class PrimaryUserChangePostHandler : IRequestHandler<PrimaryUserChangePostHandler.Context>
    {
        private readonly IManageUsersRepository _manageUsersRepository;
        private readonly INotifyService _notifyService;
        private readonly PrimaryUserChangePostHandlerOptions _options;

        public PrimaryUserChangePostHandler(INotifyService notifyService, IManageUsersRepository manageUsersRepository, IOptions<PrimaryUserChangePostHandlerOptions> options)
        {
            _notifyService = notifyService;
            _manageUsersRepository = manageUsersRepository;
            _options = options.Value;
        }

        public async Task<Unit> Handle(Context request, CancellationToken cancellationToken)
        {
            await _manageUsersRepository.UpdatePrimaryContact(request.UserId, request.CompanyId);
            var users = await _manageUsersRepository.GetAllUsers(request.Adb2CId);
            var usersVm = VendorCompanyUserMapper.Map(users);

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