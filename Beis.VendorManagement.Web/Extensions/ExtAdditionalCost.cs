namespace Beis.VendorManagement.Web.Extensions
{
	public static class ExtAdditionalCost
	{
		public static EnumAdditionalCostType? GetAdditionalCostType(this additional_cost additionalCostDetail)
		{
			if (!Enum.IsDefined(typeof(EnumAdditionalCostType), additionalCostDetail.additional_cost_type_id))
			{
				return null;
			}
			return (EnumAdditionalCostType)additionalCostDetail.additional_cost_type_id;
		}
	}
}
