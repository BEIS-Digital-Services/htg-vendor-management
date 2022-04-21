namespace Beis.HelpToGrow.Web.Models.Pricing
{
    using System.Linq;
    using System.Collections.Generic;

    public class AdditionalDiscountsViewModel
    {
        public AdditionalDiscountsViewModel()
        {
            UserDiscounts = Enumerable.Empty<UserDiscountViewModel>();
        }

        public long ProductPriceId { get; set; }

        public long ProductId { get; set; }

        public string Adb2CId { get; set; }

        public string ProductName { get; set; }

        public bool HasPricing { get; set; }

        public bool ContractDurationDiscountFlag { get; set; }

        public string ContractDurationDiscountUnit { get; set; }

        public long ContractDurationDiscount { get; set; }

        public long ContractDurationDiscountPercentage { get; set; }

        public string ContractDurationDiscountDescription { get; set; }

        public bool PaymentTermsDiscountFlag { get; set; }

        public string PaymentTermsDiscountUnit { get; set; }

        public decimal PaymentTermsDiscount { get; set; }

        public decimal PaymentTermsDiscountPercentage { get; set; }

        public string PaymentTermsDiscountDescription { get; set; }

        public IEnumerable<UserDiscountViewModel> UserDiscounts { get; set; }
    }
}