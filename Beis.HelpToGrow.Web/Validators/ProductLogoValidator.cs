namespace Beis.HelpToGrow.Web.Validators
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    
    using Beis.HelpToGrow.Web.Models;
    using FluentValidation;
    using Microsoft.AspNetCore.Http;

    public class ProductLogoValidator : AbstractValidator<ProductLogoViewModel>
    {
        private const int MaxLogoSizeInKb = 250;

        private static IReadOnlyDictionary<string, string> AllowedLogoExtensionsAndContentTypes => new Dictionary<string, string>
        { 
            { ".BMP", "IMAGE/BMP" }, 
            { ".GIF", "IMAGE/GIF" },
            { ".JPG", "IMAGE/JPEG" },
            { ".JPEG", "IMAGE/JPEG" } 
        };

        public ProductLogoValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(p => p.File).NotNull().WithMessage("Please choose an appropriate file");
            RuleFor(p => p.File).Must(IsValidFileType).WithMessage("The selected file must be a JPEG, GIF or BMP");
            RuleFor(p => p.File).Must(IsFileNotEmpty).WithMessage("The selected file is empty");
            RuleFor(p => p.File).Must(IsValidFileSize).WithMessage("The selected file must be smaller than 250Kb");
        }

        private static bool IsFileNotEmpty(IFormFile file)
        {
            return SizeInKb(file.Length) != 0;
        }

        private static bool IsValidFileType(IFormFile file)
        {
            return AllowedLogoExtensionsAndContentTypes.Any(r => r.Key.Equals(Path.GetExtension(file.FileName)?.ToUpperInvariant()) 
                        && r.Value.Equals(file.ContentType.ToUpperInvariant()));
        }

        private static bool IsValidFileSize(IFormFile file)
        {
            return SizeInKb(file.Length) < MaxLogoSizeInKb;
        }

        private static double SizeInKb(long bytes)
        {
            const int unit = 1024;
            if (bytes < unit) return bytes;

            var res = (double)bytes / unit;
            return res;
        }
    }
}