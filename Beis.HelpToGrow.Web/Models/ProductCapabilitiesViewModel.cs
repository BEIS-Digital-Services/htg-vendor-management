namespace Beis.HelpToGrow.Web.Models
{
    using System.Collections.Generic;
    
    using Microsoft.AspNetCore.Mvc.Rendering;
    
    public class ProductCapabilitiesViewModel
    {
        public long ProductId { get; set; }
        
        public string ProductName { get; set; }
        
        public string DraftAdditionalCapabilities { get; set; }
        
        public IList<SelectListItem> SettingsProductCapabilitiesList { get; set; }
        
        public ProductStatus Status { get; set; }

        public string Adb2CId { get; set; }

        public long ProductTypeId { get; set; }
    }
}