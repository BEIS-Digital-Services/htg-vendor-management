using Beis.VendorManagement.Web.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Beis.VendorManagement.Web.Models
{
    public class SoftwareHomeViewModel
    {
        [Key]
        [Required]
        public long ProductId { get; set; }

        public string Adb2CId { get; set; }

        [MaxLength(50)]
        public string ProductName { get; set; }

        public string ProductTypeName { get; set; }

        public ProductStatus ProductStatus { get; set; }

        public string DraftProductDescription { get; set; }

        public bool HasCapabilities { get; set; }

        public bool HasProductSupport { get; set; }

        public bool HasPlatformDetails { get; set; }

        public bool CanSubmitForReview { get; set; }
    }
}