namespace Beis.VendorManagement.Web.Handlers.ManageUsers
{
    public class UserPostHandler : IRequestHandler<UserPostHandler.Context, bool>
    {
        private readonly IManageUsersRepository _manageUsersRepository;

        public UserPostHandler(IManageUsersRepository manageUsersRepository)
        {
            _manageUsersRepository = manageUsersRepository;
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

                var user = VendorCompanyUserMapper.Map(new VendorCompanyUserViewModel
                {
                    FullName = request.FullName,
                    Email = request.Email.Trim(),
                    CompanyId = request.CompanyId,
                    Status = true,
                    PrimaryContact = false,
                    AccessLink = Guid.NewGuid().ToString()
                });
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