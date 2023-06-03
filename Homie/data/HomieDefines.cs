using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Homie.utils;

namespace Homie.Data
{
	[HomieStruct]
	public class Device
	{
		// ========== PUBLIC MEMBERS ==================================================================================

		public string Id { get; set; }

		[HomieField("$homie")]
		public string HomieVersion { get; set; }

		[HomieField("$online")]
		public bool Online { get; set; }

		[HomieField("$name")]
		public string Name { get; set; }

		[HomieField("$localip")]
		public string LocalIP { get; set; }

		[HomieField("$mac")]
		public string MAC { get; set; }

		[HomieField("$implementation")]
		public string Implementation { get; set; }

		[HomieStruct]
		public Stat Stat { get; set; }

		[HomieStruct]
		public Firmware Firmware { get; set; }

		[HomieMap]
		public Dictionary<string, Node> Nodes { get; set; }

		public Device()
		{
			this.Stat = new Stat();
			this.Firmware = new Firmware();
			this.Nodes = new Dictionary<string, Node>();
		}
	}

	[HomieStruct("$stats")]
	public struct Stat
	{
		[HomieField("uptime")]
		public int Uptime { get; set; }

		[HomieField("signal")]
		public int Signal { get; set; }

		[HomieField("interval")]
		public int Interval { get; set; }
	}

	[HomieStruct("$fw")]
	public struct Firmware
	{
		[HomieField("name")]
		public string Name { get; set; }

		[HomieField("version")]
		public string Version { get; set; }

		[HomieField("checksum")]
		public string Checksum { get; set; }
	}

	[HomieMap("$nodes")]
	public class Node
	{
		public string Id;

		[HomieField("$type")]
		public string Type { get; set; }

		[HomieMap]
		public Dictionary<string, Property> Properties { get; set; }

		public Node()
		{
			this.Properties = new Dictionary<string, Property>();
		}
	}

	[HomieMap("$properties")]
	public struct Property
	{
		public string Name { get; set; }
		public string Value { get; set; }
	}
}
