using Beis.VendorManagement.Web.Mappers;

namespace Beis.VendorManagement.Web.Services
{
    public class ManageUsersService : IManageUsersService
    {
        private readonly IManageUsersRepository _manageUsersRepository;

        public ManageUsersService(IManageUsersRepository manageUsersRepository)
        {
            _manageUsersRepository = manageUsersRepository;
        }

        public async Task<IList<VendorCompanyUserViewModel>> GetAllUsers(string adb2CId)
        {
            var users = await _manageUsersRepository.GetAllUsers(adb2CId);
            return (IList<VendorCompanyUserViewModel>)VendorCompanyUserMapper.Map(users);
        }

        public async Task<VendorCompanyUserViewModel> GetUser(string adb2CId)
        {
            var existingUser = await _manageUsersRepository.GetUserSingle(adb2CId);
            return VendorCompanyUserMapper.Map(existingUser);
        }

        public async Task<VendorCompanyUserViewModel> GetUser(long userId)
        {
            var existingUser = await _manageUsersRepository.GetUserSingle(userId);
            return VendorCompanyUserMapper.Map(existingUser);
        }
    }
}