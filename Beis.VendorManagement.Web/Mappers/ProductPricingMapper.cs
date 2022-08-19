namespace Beis.VendorManagement.Web.Mappers
{
    internal static class ProductPricingMapper
    {
        internal static AdditionalDiscountsViewModel MapAdditionalDiscountDetails(product_price productPrice)
        {
            return new AdditionalDiscountsViewModel
            {
                ProductPriceId = productPrice.product_price_id,
                ProductId = productPrice.productid,
                ContractDurationDiscountFlag = productPrice.contract_duration_discount_flag,
                ContractDurationDiscountUnit = !string.IsNullOrWhiteSpace(productPrice.contract_duration_discount_unit)
                    ? productPrice.contract_duration_discount_unit : "n/a",
                ContractDurationDiscount = productPrice.contract_duration_discount,
                ContractDurationDiscountPercentage = productPrice.contract_duration_discount_percentage,
                ContractDurationDiscountDescription = !string.IsNullOrWhiteSpace(productPrice.contract_duration_discount_description)
                    ? productPrice.contract_duration_discount_description : "n/a",
                PaymentTermsDiscountFlag = productPrice.payment_terms_discount_flag,
                PaymentTermsDiscountUnit = !string.IsNullOrWhiteSpace(productPrice.payment_terms_discount_unit)
                    ? productPrice.payment_terms_discount_unit : "n/a",
                PaymentTermsDiscount = productPrice.payment_terms_discount,
                PaymentTermsDiscountPercentage = productPrice.payment_terms_discount_percentage,
                PaymentTermsDiscountDescription = !string.IsNullOrWhiteSpace(productPrice.contract_duration_discount_description)
                    ? productPrice.payment_terms_discount_description : "n/a"
            };
        }

        internal static DiscountPeriodViewModel MapDiscountPeriodDetails(product_price productPrice)
        {
            return new DiscountPeriodViewModel
            {
                ProductPriceId = productPrice.product_price_id,
                ProductId = productPrice.productid,
                DiscountTermNo = productPrice.discount_term_no,
                DiscountTermUnit = productPrice.discount_term_unit,
                DiscountFlag = productPrice.discount_flag,
                DiscountPrice = productPrice.discount_price,
                DiscountPercentage = productPrice.discount_percentage,
                DiscountApplicationDescription = productPrice.discount_application_description
            };
        }

        internal static MinimumCommitmentViewModel MapMinimumCommitmentDetails(product_price productPrice)
        {
            return new MinimumCommitmentViewModel
            {
                ProductPriceId = productPrice.product_price_id,
                ProductId = productPrice.productid,
                CommitmentNo = productPrice.commitment_no,
                CommitmentUnit = productPrice.commitment_unit,
                CommitmentFlag = productPrice.commitment_flag,
                MinNoUsers = productPrice.min_no_users
            };
        }

        internal static IList<ProductPriceViewModel> MapProductPriceDetails(IEnumerable<product_price> productPrices)
        {
            return productPrices.Select(p => new ProductPriceViewModel
            {
                ProductPriceId = p.product_price_id,
                ProductPriceTitle = p.product_price_title,
                ProductPriceSku = p.product_price_sku
            }).ToList();
        }

        internal static IList<UserDiscountViewModel> MapUserDiscountDetails(IEnumerable<user_discount> productPrices)
        {
            return productPrices.Select(u => new UserDiscountViewModel
            {
                ProductPriceId = u.product_price_id,
                DiscountPercentage = u.discount_percentage,
                DiscountPrice = u.discount_price,
                MaxLicenses = u.max_licenses,
                MinLicenses = u.min_licenses,
                DiscountSku = !string.IsNullOrWhiteSpace(u.discount_sku) ? u.discount_sku : "n/a"
            }).ToList();
        }
    }
}