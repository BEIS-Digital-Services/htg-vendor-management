namespace Beis.VendorManagement.Web.Models
{
    public class ProductViewModel : BaseViewModel
    {
        [Key]
        [Required]
        public long ProductId { get; set; }
    
        public string ProductName { get; set; }
    }
}