using System;
using System.Collections.Generic;

namespace Homie
{
	public class Node
	{
		public string Id { get; private set; }
		public string Type { get; private set; }
		public List<Property> Properties { get; private set; }
	}

	public struct Property
	{
		public string Name { get; private set; }
		public string Value { get; private set; }
	}
}
