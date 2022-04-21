namespace Beis.HelpToGrow.Web.Validators
{
    using System.Text.RegularExpressions;
    
    using Beis.HelpToGrow.Web.Models;
    using FluentValidation;

    public class ProductSummaryValidator : AbstractValidator<SummaryViewModel>
    {
        public ProductSummaryValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(p => p.DraftProductDescription).NotEmpty().WithMessage("Enter product summary");
            RuleFor(p => p.DraftProductDescription).MaximumLength(5000).WithMessage("Summary must be 5000 characters or fewer");
            RuleFor(p => p.DraftProductDescription).Must(IsValid).WithMessage("Summary can not contains links to web pages, downloads or images");
        }

        private bool IsValid(string description)
        {
            //Checks for web page, downloads or images in the text
            return !Regex.IsMatch(description, @"([a-zA-Z\d]+://)?((\w+:\w+@)?([a-zA-Z\d.-]+\.[A-Za-z]{2,4})(:\d+)?(/.*)?)");
        }
    }
}