namespace Beis.VendorManagement.Web.Validators
{
    public class IpAddressValidator : AbstractValidator<RangesViewModel>
    {
        public IpAddressValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(p => p.IpAddresses).NotEmpty().WithMessage("Please enter valid Ip Addresses");
            RuleFor(p => p.IpAddresses).MaximumLength(500).WithMessage("Ip Addresses can have a maximum of 500 characters");
            RuleFor(p => p.IpAddresses).Must(CheckDuplicateIpAddresses).WithMessage("Please enter different Ip Addresses");
            RuleFor(p => p.IpAddresses).Must(CheckIpAddressesFormat).WithMessage("Please enter valid Ip Addresses");
        }

        private static bool CheckIpAddressesFormat(string modelItem)
        {
            var ipAddresses = GetIpAddresses(modelItem);
            foreach (var ipAddress in ipAddresses)
            {
                var ipAddressOnly = ipAddress.Contains('/')
                    ? ipAddress.Substring(0, ipAddress.LastIndexOf('/'))
                    : ipAddress;

                //Need to check the number of dots as the TryParse will treat any number as 0.0.0.<number> enforcing the user to enter correct format
                if (ipAddressOnly.Count(c => c == '.') != 3 || !System.Net.IPAddress.TryParse(ipAddressOnly, out var temp))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckDuplicateIpAddresses(string modelItem)
        {
            var ipAddresses = GetIpAddresses(modelItem);

            return ipAddresses.Length == ipAddresses.Distinct().Count();
        }

        private static string[] GetIpAddresses(string modelItem)
        {
            return modelItem.Trim().Split(';');
        }
    }
}