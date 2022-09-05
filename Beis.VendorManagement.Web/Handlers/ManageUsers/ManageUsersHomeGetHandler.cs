namespace Beis.VendorManagement.Web.Handlers.ManageUsers
{
    public class ManageUsersHomeGetHandler : IRequestHandler<ManageUsersHomeGetHandler.Context, Optional<ManageUsersHomeViewModel>>
    {
        private readonly IManageUsersRepository _manageUsersRepository;

        public ManageUsersHomeGetHandler(IManageUsersRepository manageUsersRepository)
        {
            _manageUsersRepository = manageUsersRepository;
        }

        public async Task<Optional<ManageUsersHomeViewModel>> Handle(Context request, CancellationToken cancellationToken)
        {
            var users = await _manageUsersRepository.GetAllUsers(request.Adb2CId);
            var usersVm = VendorCompanyUserMapper.Map(users).ToList();
            usersVm.Sort((x, y) => x.FullName.CompareTo(y.FullName));
            usersVm.Sort((x, y) => y.PrimaryContact.CompareTo(x.PrimaryContact));

            var model = new ManageUsersHomeViewModel
            {
                Users = usersVm,
                ContentKey = AnalyticConstants.ManageUsersManageUsersHome
            };

            return model;
        }

        public struct Context : IRequest<Optional<ManageUsersHomeViewModel>>
        {
            public string Adb2CId { get; set; }
        }
    }
}