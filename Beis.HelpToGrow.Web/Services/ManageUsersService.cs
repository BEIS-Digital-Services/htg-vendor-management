using AutoMapper;
using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.HelpToGrow.Web.Models;
using Beis.HelpToGrow.Web.Services.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Web.Services
{
    public class ManageUsersService : IManageUsersService
    {
        private readonly IManageUsersRepository _manageUsersRepository;
        private readonly IMapper _mapper;

        public ManageUsersService(IManageUsersRepository manageUsersRepository, IMapper mapper)
        {
            _manageUsersRepository = manageUsersRepository;
            _mapper = mapper;
        }

        public async Task<IList<VendorCompanyUserViewModel>> GetAllUsers(string adb2CId)
        {
            var users = await _manageUsersRepository.GetAllUsers(adb2CId);
            return _mapper.Map<IList<VendorCompanyUserViewModel>>(users);
        }

        public async Task<VendorCompanyUserViewModel> GetUser(string adb2CId)
        {
            var existingUser = await _manageUsersRepository.GetUserSingle(adb2CId);
            return _mapper.Map<VendorCompanyUserViewModel>(existingUser);
        }

        public async Task<VendorCompanyUserViewModel> GetUser(long userId)
        {
            var existingUser = await _manageUsersRepository.GetUserSingle(userId);
            return _mapper.Map<VendorCompanyUserViewModel>(existingUser);
        }
    }
}