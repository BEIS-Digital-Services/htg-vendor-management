namespace Beis.VendorManagement.Web.Models
{
    public class VendorCompanyViewModel : BaseViewModel
    {
        public VendorCompanyViewModel()
        {
            HasUserAuthorised = false;
        }

        [MaxLength(50)]
        [Required]
        public string RegistrationId { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string VendorCompanyName { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string VendorCompanyAddress1 { get; set; }
        
        [MaxLength(500)]
        public string VendorCompanyAddress2 { get; set; }
        
        [MaxLength(50)]
        public string VendorCompanyCity { get; set; }
        
        [MaxLength(8)]
        [Required]
        public string VendorCompanyPostcode { get; set; }
        
        [MaxLength(500)]
        public string IpAddresses { get; set; }

        public long PrimaryUserId { get; set; }
        
        public bool HasUserAuthorised { get; set; }
        
        public bool ShowErrorMessage { get; set; }
    }
}