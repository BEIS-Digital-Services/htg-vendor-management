namespace Beis.HelpToGrow.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ProductViewModel
    {
        [Key]
        [Required]
        public long ProductId { get; set; }
    
        public string ProductName { get; set; }
    }
}