using System;
namespace Homie.utils
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class HomieField : Attribute
	{
		public static string BASE_TOPIC = "homie";
		public string tag;

		public HomieField(string tag)
		{
			this.tag = string.Format(@"^{0}\/\w*\/{1}$", BASE_TOPIC, tag.Replace("$", "\\$"));
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class HomieStruct : Attribute
	{
		public string tag;

		public HomieStruct(string tag)
		{
			this.tag = tag;
		}
	}
}
