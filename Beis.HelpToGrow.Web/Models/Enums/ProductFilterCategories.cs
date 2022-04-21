namespace Beis.HelpToGrow.Web.Models
{
    using System.ComponentModel.DataAnnotations;

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