﻿namespace Beis.HelpToGrow.Core.Repositories.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Beis.Htg.VendorSme.Database.Models;

    public interface ICompanyUserRepository
    {
        Task<vendor_company> GetCompanyByUserSingle(string abd2CId);

        Task<vendor_company> GetCompanyByUserSingle(long userId);

        Task<vendor_company_user> GetUserByIdSingle(long id);
        
        Task<IList<product>> GetProductsByUserIdSingle(long id);
        
        Task UpdateCompany(vendor_company company);
        
        Task<vendor_company_user> GetUserIdByAdb2CUserId(string id);

        Task<vendor_company_user> GetUserByAccessLink(string accessLink);
    }
}