using System.ComponentModel.DataAnnotations;

namespace Beis.VendorManagement.Web.Models
{
    public class ProductViewModel
    {
        [Key]
        [Required]
        public long ProductId { get; set; }
    
        public string ProductName { get; set; }
    }
}