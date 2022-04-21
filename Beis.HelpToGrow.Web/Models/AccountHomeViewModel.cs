namespace Beis.HelpToGrow.Web.Models
{
    using System.Collections.Generic;

    public class AccountHomeViewModel
    {
        public IList<ProductCategoryViewModel> Products { get; set; }
        
        public string RegistrationNumber { get; set; }
        
        public long CompanyId { get; set; }
        
        public string CompanyName { get; set; }

        public string Adb2CId { get; set; }

        public string ApiKey { get; set; }
    }
}