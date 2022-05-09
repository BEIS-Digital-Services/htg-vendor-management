using System.ComponentModel.DataAnnotations;

namespace Beis.VendorManagement.Web.Models.Enums
{
    public enum ProductFilterCategories
    {
        [Display(Name = "Support")]
        Support = 1,
    
        [Display(Name = "Training")]
        Training = 2,
        
        [Display(Name = "Platform")]
        Platform = 3
    }
}