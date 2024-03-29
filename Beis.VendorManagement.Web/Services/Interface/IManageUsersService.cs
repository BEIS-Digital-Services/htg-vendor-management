﻿namespace Beis.VendorManagement.Web.Services.Interface
{
    public interface IManageUsersService
    {
        Task<IList<VendorCompanyUserViewModel>> GetAllUsers(string adb2CId);
     
        Task<VendorCompanyUserViewModel> GetUser(string adb2CId);

        Task<VendorCompanyUserViewModel> GetUser(long userId);
    }
}