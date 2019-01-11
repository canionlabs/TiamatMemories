using System;
namespace Homie.utils
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class HomieField : Attribute
	{
		public string tag;

		public HomieField(string tag)
		{
			this.tag = tag.Replace("$", "\\$");
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
	public class HomieStruct : Attribute
	{
		public string tag;

		public HomieStruct(string tag = "\\w*")
		{
			this.tag = tag.Replace("$", "\\$");
		}
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
	public class HomieMap : Attribute
	{
		public string tag;

		public HomieMap(string tag = "\\w*")
		{
			this.tag = tag.Replace("$", "\\$");
		}
	}
}
