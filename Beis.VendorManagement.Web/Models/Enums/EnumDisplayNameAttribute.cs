namespace Beis.VendorManagement.Web.Models.Enums
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumDisplayNameAttribute : Attribute
    {
        public string DisplayName { get; set; }
    }
}