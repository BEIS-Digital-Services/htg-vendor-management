using Beis.VendorManagement.Web.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Beis.VendorManagement.Web.Models
{
    public class UserViewModel : BaseViewModel
    {
        public UserViewModel()
        {
            HasToBeRemoved = true;
        }

        [Required]
        public long UserId { get; set; }

        [MaxLength(500)]
        [Required]
        public string FullName { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string Email { get; set; }
        
        [Required]
        public long CompanyId { get; set; }

        [Required]
        public bool? HasToBePrimaryContact { get; set; }

        public bool HasToBeRemoved { get; set; }

        public BackPagesEnum BackPage { get; set; }

        public bool HasTermsChecked { get; set; }
        
        public bool HasPrivacyPolicyChecked { get; set; }
        
        public bool ShowErrorMessage { get; set; }
    }

    public class VendorCompanyUserViewModel : UserViewModel
    {
        public bool Status { get; set; }
        
        [Required]
        public bool PrimaryContact { get; set; }
        
        [MaxLength(500)]
        public string AccessLink { get; set; }

        public string Adb2CId { get; set; }
    }
}