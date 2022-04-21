namespace Beis.HelpToGrow.Web.Models.Pricing
{
    public class DiscountPeriodViewModel
    {
        public int ProductPriceId { get; set; }
        
        public long ProductId { get; set; }

        public string Adb2CId { get; set; }

        public string ProductName { get; set; }

        public bool DiscountFlag { get; set; }

        public int DiscountTermNo { get; set; }

        public string DiscountTermUnit { get; set; }

        public decimal DiscountPrice { get; set; }

        public decimal DiscountPercentage { get; set; }

        public string DiscountApplicationDescription { get; set; }
    }
}