﻿using Beis.VendorManagement.Web.Models;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Beis.VendorManagement.Web.Validators
{
    public class ProductRedemptionUrlValidator : AbstractValidator<RedemptionUrlViewModel>
    {
        public ProductRedemptionUrlValidator()
        {
            CascadeMode = CascadeMode.Stop;
    
            RuleFor(p => p.RedemptionUrl).NotEmpty().WithMessage("Enter a URL, like www.example.com/redeem123");
            RuleFor(p => p.RedemptionUrl).Must(IsValid)
                .WithMessage("Enter a URL in the correct format, like www.example.com/redeem123");
        }

        private static bool IsValid(string redemptionUrl)
        {
            //Checks if the text is correct web page
            var pattern = "^(http|http(s)?://)?([\\w-]+\\.)+[.com|.co.uk|.org]+(\\[\\?%&=]*)?";
            
            return Regex.IsMatch(redemptionUrl, pattern);
        }
    }
}