namespace Beis.HelpToGrow.Web.Models
{
    using Microsoft.AspNetCore.Http;
    using System.ComponentModel.DataAnnotations;

    public class ProductLogoViewModel
    {
        public long ProductId { get; set; }

        public string Adb2CId { get; set; }

        [MaxLength(50)]
        public string ProductName { get; set; }
        
        [MaxLength(5000)]
        public string ProductLogo { get; set; }
        
        public IFormFile File { get; set; }
    }
}