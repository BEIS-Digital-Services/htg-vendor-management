namespace Beis.VendorManagement.Web.Validators
{
    public class ProductRedemptionUrlValidator : AbstractValidator<RedemptionUrlViewModel>
    {
        public ProductRedemptionUrlValidator()
        {
            CascadeMode = CascadeMode.Stop;
    
            RuleFor(p => p.RedemptionUrl).NotEmpty().WithMessage("Enter a URL, like http(s)://example.com/redeem123");
            RuleFor(p => p.RedemptionUrl).Must(IsValid)
                .WithMessage("Enter a URL in the correct format, like http(s)://example.com/redeem123");
        }

        private static bool IsValid(string redemptionUrl)
        {
            //Checks if the text is correct web page
            const string pattern = "((http|https)://)[a-zA-Z0-9@:%._\\+~#?&//=]{2,256}\\.[a-z]{2,6}\\b([-a-zA-Z0-9@:%._\\+~#?&//=]*)"; 

            var result = Regex.IsMatch(redemptionUrl, pattern);
            return result;
        }
    }
}