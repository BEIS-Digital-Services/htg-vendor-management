using System.Collections.Generic;

namespace Beis.VendorManagement.Web.Models
{
    public class AccountHomeViewModel : BaseViewModel
    {
        public IList<ProductCategoryViewModel> Products { get; set; }
        
        public string RegistrationNumber { get; set; }
        
        public long CompanyId { get; set; }
        
        public string CompanyName { get; set; }

        public string Adb2CId { get; set; }

        public string ApiKey { get; set; }
    }
}