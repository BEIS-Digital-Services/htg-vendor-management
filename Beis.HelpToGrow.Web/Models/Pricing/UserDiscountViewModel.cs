namespace Beis.HelpToGrow.Web.Models
{
    public class UserDiscountViewModel
    {
        public long? product_price_id { get; set; }

        public int max_licenses { get; set; }

        public int min_licenses { get; set; }

        public decimal discount_price { get; set; }
        
        public decimal discount_percentage { get; set; }
        
        public string discount_sku { get; set; }
    }
}