namespace Beis.VendorManagement.Web.Models
{
    public class ProductSubmitConfirmationViewModel : BaseViewModel
    {
        public long ProductId { get; set; }

        public string ProductName { get; set; }
        
        public string Email { get; set; }
    }
}