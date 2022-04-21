using AutoMapper;
using Beis.HelpToGrow.Web.Models;
using Beis.HelpToGrow.Web.Models.Pricing;
using Beis.Htg.VendorSme.Database.Models;

namespace Beis.HelpToGrow.Web
{
    public class AutoMap : Profile
    {
        public AutoMap()
        {
            CreateMap<product, ProductViewModel>().ReverseMap()
            .ForMember(d => d.product_id, s => s.MapFrom(m => m.ProductId))
            .ForMember(d => d.product_name, s => s.MapFrom(m => m.ProductName));

            CreateMap<ProductViewModel, product>().ReverseMap()
            .ForMember(d => d.ProductId, s => s.MapFrom(m => m.product_id))
            .ForMember(d => d.ProductName, s => s.MapFrom(m => m.product_name));

            CreateMap<vendor_company, VendorCompanyViewModel>()
                .ForMember(d => d.VendorCompanyName, s => s.MapFrom(e => e.vendor_company_name))
                .ForMember(d => d.VendorCompanyAddress1, s => s.MapFrom(e => e.vendor_company_address_1))
                .ForMember(d => d.VendorCompanyAddress2, s => s.MapFrom(e => e.vendor_company_address_2))
                .ForMember(d => d.VendorCompanyCity, s => s.MapFrom(e => e.vendor_company_city))
                .ForMember(d => d.VendorCompanyPostcode, s => s.MapFrom(e => e.vendor_company_postcode))
                .ForMember(d => d.RegistrationId, s => s.MapFrom(e => e.registration_id))
                .ReverseMap();

            CreateMap<vendor_company_user, VendorCompanyUserViewModel>()
                .ForMember(d => d.PrimaryContact, s => s.MapFrom(e => e.primary_contact))
                .ForMember(d => d.Status, s => s.MapFrom(e => e.status))
                .ForMember(d => d.AccessLink, s => s.MapFrom(e => e.access_link))
                .ForMember(d => d.CompanyId, s => s.MapFrom(e => e.companyid))
                .ForMember(d => d.FullName, s => s.MapFrom(e => e.full_name))
                .ForMember(d => d.UserId, s => s.MapFrom(e => e.userid))
                .ForMember(d => d.Email, s => s.MapFrom(e => e.email))
                .ForMember(d => d.Adb2CId, s => s.MapFrom(e => e.adb2c))
                .ReverseMap();

            CreateMap<product_filter, ProductFiltersModel>().ReverseMap();

            CreateMap<product_capability, ProductCapabilitiesModel>().ReverseMap();

            CreateMap<vendor_company_user, UserViewModel>().ReverseMap();

            CreateMap<user_discount, UserDiscountViewModel>()
                .ForMember(d => d.discount_sku, s => s.MapFrom(s => !string.IsNullOrWhiteSpace(s.discount_sku) ? s.discount_sku : "n/a"))
                .ReverseMap();

            CreateMap<ProductLogoViewModel, product>().ReverseMap()
                .ForMember(d => d.ProductId, s => s.MapFrom(m => m.product_id))
                .ForMember(d => d.ProductName, s => s.MapFrom(m => m.product_name))
                .ForMember(d => d.ProductLogo, s => s.MapFrom(m => m.product_logo));

            CreateMap<SummaryViewModel, product>().ReverseMap()
                .ForMember(d => d.ProductId, s => s.MapFrom(m => m.product_id))
                .ForMember(d => d.ProductName, s => s.MapFrom(m => m.product_name))
                .ForMember(d => d.DraftProductDescription, s => s.MapFrom(m => m.draft_product_description))
                .ForMember(d => d.ProductStatus, s => s.MapFrom(s => (ProductStatus)s.status));

            CreateMap<RedemptionUrlViewModel, product>().ReverseMap()
                .ForMember(d => d.ProductId, s => s.MapFrom(m => m.product_id))
                .ForMember(d => d.ProductName, s => s.MapFrom(m => m.product_name))
                .ForMember(d => d.RedemptionUrl, s => s.MapFrom(m => m.redemption_url));

            CreateMap<SkuViewModel, product>().ReverseMap()
                .ForMember(d => d.ProductId, s => s.MapFrom(m => m.product_id))
                .ForMember(d => d.ProductName, s => s.MapFrom(m => m.product_name))
                .ForMember(d => d.ProductSku, s => s.MapFrom(m => m.product_SKU));

            CreateMap<ProductSubmitConfirmationViewModel, product>().ReverseMap()
                .ForMember(d => d.ProductId, s => s.MapFrom(m => m.product_id))
                .ForMember(d => d.ProductName, s => s.MapFrom(m => m.product_name));

            CreateMap<product_price, MinimumCommitmentViewModel>()
                .ForMember(d => d.ProductPriceId, s => s.MapFrom(s => s.product_price_id))
                .ForMember(d => d.ProductId, s => s.MapFrom(s => s.productid))
                .ForMember(d => d.CommitmentNo, s => s.MapFrom(s => s.commitment_no))
                .ForMember(d => d.CommitmentUnit, s => s.MapFrom(s => s.commitment_unit))
                .ForMember(d => d.CommitmentFlag, s => s.MapFrom(s => s.commitment_flag))
                .ForMember(d => d.MinNoUsers, s => s.MapFrom(s => s.min_no_users))
                .ReverseMap();

            CreateMap<product_price, DiscountPeriodViewModel>()
                .ForMember(d => d.ProductPriceId, s => s.MapFrom(s => s.product_price_id))
                .ForMember(d => d.ProductId, s => s.MapFrom(s => s.productid))
                .ForMember(d => d.DiscountTermNo, s => s.MapFrom(s => s.discount_term_no))
                .ForMember(d => d.DiscountTermUnit, s => s.MapFrom(s => s.discount_term_unit))
                .ForMember(d => d.DiscountFlag, s => s.MapFrom(s => s.discount_flag))
                .ForMember(d => d.DiscountPrice, s => s.MapFrom(s => s.discount_price))
                .ForMember(d => d.DiscountPercentage, s => s.MapFrom(s => s.discount_percentage))
                .ForMember(d => d.DiscountApplicationDescription, s => s.MapFrom(s => s.discount_application_description))
                .ReverseMap();

            CreateMap<product_price, AdditionalDiscountsViewModel>()
                .ForMember(d => d.ProductPriceId, s => s.MapFrom(s => s.product_price_id))
                .ForMember(d => d.ProductId, s => s.MapFrom(s => s.productid))
                .ForMember(d => d.ContractDurationDiscountFlag, s => s.MapFrom(s => s.contract_duration_discount_flag))
                .ForMember(d => d.ContractDurationDiscountUnit, s => s.MapFrom(s => !string.IsNullOrWhiteSpace(s.contract_duration_discount_unit) ? s.contract_duration_discount_unit : "n/a"))
                .ForMember(d => d.ContractDurationDiscount, s => s.MapFrom(s => s.contract_duration_discount))
                .ForMember(d => d.ContractDurationDiscountPercentage, s => s.MapFrom(s => s.contract_duration_discount_percentage))
                .ForMember(d => d.ContractDurationDiscountDescription, s => s.MapFrom(s => !string.IsNullOrWhiteSpace(s.contract_duration_discount_description) ? s.contract_duration_discount_description : "n/a"))
                .ForMember(d => d.PaymentTermsDiscountFlag, s => s.MapFrom(m => m.payment_terms_discount_flag))
                .ForMember(d => d.PaymentTermsDiscountUnit, s => s.MapFrom(s => !string.IsNullOrWhiteSpace(s.payment_terms_discount_unit) ? s.payment_terms_discount_unit : "n/a"))
                .ForMember(d => d.PaymentTermsDiscount, s => s.MapFrom(s => s.payment_terms_discount))
                .ForMember(d => d.PaymentTermsDiscountPercentage, s => s.MapFrom(s => s.payment_terms_discount_percentage))
                .ForMember(d => d.PaymentTermsDiscountDescription, s => s.MapFrom(s => !string.IsNullOrWhiteSpace(s.contract_duration_discount_description) ? s.payment_terms_discount_description : "n/a"))
                .ReverseMap();


            CreateMap<product_price, ProductPriceViewModel>()
                .ForMember(d => d.ProductPriceId, s => s.MapFrom(s => s.product_price_id))
                .ForMember(d => d.ProductPriceTitle, s => s.MapFrom(s => s.product_price_title))
                .ForMember(d => d.ProductPriceSku, s => s.MapFrom(s => s.product_price_sku))
                .ReverseMap();
        }
    }
}