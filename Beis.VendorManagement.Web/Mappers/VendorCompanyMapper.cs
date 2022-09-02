namespace Beis.VendorManagement.Web.Mappers
{
    internal static class VendorCompanyMapper
    {
        internal static VendorCompanyViewModel Map(vendor_company vendorCompany)
        {
            if (vendorCompany == null)
            {
                return default;
            }

            return new VendorCompanyViewModel
            {
                IpAddresses = vendorCompany.ipaddresses,
                RegistrationId = vendorCompany.registration_id,
                VendorCompanyAddress1 = vendorCompany.vendor_company_address_1,
                VendorCompanyAddress2 = vendorCompany.vendor_company_address_2,
                VendorCompanyCity = vendorCompany.vendor_company_city,
                VendorCompanyName = vendorCompany.vendor_company_name,
                VendorCompanyPostcode = vendorCompany.vendor_company_postcode
            };
        }
    }
}