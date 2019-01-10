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

		[HomieMap("$nodes")]
		public Dictionary<string, Node> Nodes;

		public Device()
		{
			this.Stat = new Stat();
			this.Firmware = new Firmware();
			this.Nodes = new Dictionary<string, Node>();
		}

		public override string ToString()
		{
			return string.Format(
@"Name: {0}
Homie Version: {1}
Online: {2}
LocalIP: {3}
MAC: {4}
Stat: 
{5}
Firmware: 
{6}", Name, HomieVersion, Online, LocalIP, MAC, Stat, Firmware);
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

		public override string ToString()
		{
			return string.Format(
@"	Uptime: {0}
	Signal: {1}
	Interval: {2}", Uptime, Signal, Interval);
		}
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

		public override string ToString()
		{
			return string.Format(
@"	Name: {0}
	Version: {1}
	Checksum: {2}", Name, Version, Checksum);
		}
	}

	public class Node
	{
		public string Id;

		[HomieField("$type")]
		public string Type { get; set; }

		[HomieMap("$properties")]
		public Dictionary<string, Property> Properties { get; set; }

		public Node()
		{
			this.Properties = new Dictionary<string, Property>();
		}
	}

	public struct Property
	{
		public string Name { get; set; }
		public string Value { get; set; }
	}
}
