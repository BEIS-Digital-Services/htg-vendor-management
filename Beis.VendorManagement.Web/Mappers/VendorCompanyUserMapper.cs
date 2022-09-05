namespace Beis.VendorManagement.Web.Mappers
{
    internal static class VendorCompanyUserMapper
    {
        internal static VendorCompanyUserViewModel Map(vendor_company_user vendorCompanyUser)
        {
            return GetVendorCompanyUserViewModel(vendorCompanyUser);
        }

        internal static vendor_company_user Map(VendorCompanyUserViewModel vendorCompanyUser)
        {
            return new vendor_company_user
            {
                access_link = vendorCompanyUser.AccessLink,
                adb2c = vendorCompanyUser.Adb2CId,
                companyid = vendorCompanyUser.CompanyId,
                email = vendorCompanyUser.Email,
                full_name = vendorCompanyUser.FullName,
                primary_contact = vendorCompanyUser.PrimaryContact,
                status = vendorCompanyUser.Status,
                userid = vendorCompanyUser.UserId
            };
        }

        internal static IEnumerable<VendorCompanyUserViewModel> Map(IEnumerable<vendor_company_user> vendorCompanyUsers)
        {
            return vendorCompanyUsers.Select(GetVendorCompanyUserViewModel);
        }

        private static VendorCompanyUserViewModel GetVendorCompanyUserViewModel(vendor_company_user vendorCompanyUser)
        {
            if (vendorCompanyUser == null)
            {
                return default;
            }

            return new VendorCompanyUserViewModel
            {
                AccessLink = vendorCompanyUser.access_link,
                Adb2CId = vendorCompanyUser.adb2c,
                CompanyId = vendorCompanyUser.companyid,
                Email = vendorCompanyUser.email,
                FullName = vendorCompanyUser.full_name,
                PrimaryContact = vendorCompanyUser.primary_contact,
                Status = vendorCompanyUser.status ?? false,
                UserId = vendorCompanyUser.userid
            };
        }
    }
}