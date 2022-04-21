namespace Beis.HelpToGrow.Web.Validators
{
    using System.Text.RegularExpressions;

    using Beis.HelpToGrow.Web.Models;
    using FluentValidation;

    public class UserValidator : AbstractValidator<UserViewModel>
    {
        public UserValidator()
        {
            RuleFor(u => u.FullName).NotEmpty().WithMessage("Enter your full name");
            RuleFor(u => u.FullName).MaximumLength(40).WithMessage("Name must be 40 characters or fewer");
            RuleFor(u=> u.FullName).Matches("^[a-zA-Z '.,-]{0,150}$").WithMessage("Name can only include letters and the characters space, full stop, comma, apostrophe, space or dash");

            RuleFor(u => u.Email).NotEmpty().WithMessage("Enter an email address, like name@example.com");
            RuleFor(u => u.Email).Must(VerifyEmailAddress).WithMessage("Enter an email address in the correct format, like name@example.com");
        }

        private static bool VerifyEmailAddress(string emailAddress)
        {
                return !string.IsNullOrWhiteSpace(emailAddress) && Regex.IsMatch(emailAddress,
                    @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }
}