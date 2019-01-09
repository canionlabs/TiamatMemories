using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Homie.utils;

namespace Homie
{
	public class Device
	{
		// ========== PUBLIC MEMBERS ==================================================================================

		public string Id { get; private set; }

		public string HomieVersion { get; private set; }

		[HomieField("$online")]
		public bool Online { get; private set; }

		[HomieField("$name")]
		public string Name { get; private set; }

		[HomieField("$localip")]
		public string LocalIP { get; private set; }

		[HomieField("$mac")]
		public string MAC { get; private set; }

		[HomieStruct("$stats")]
		public Stat Stat { get; private set; }

		[HomieStruct("$fw")]
		public Firmware Firmware { get; private set; }

		[HomieStruct("$implementation")]
		public string Implementation { get; private set; }

		public List<Node> Nodes { get; private set; }
	}

	public struct Stat
	{
		[HomieField("uptime")]
		public int Uptime;

		[HomieField("signal")]
		public int Signal;

		[HomieField("interval")]
		public int Interval;
	}

	public struct Firmware
	{
		[HomieField("name")]
		public string Name;

		[HomieField("version")]
		public string Version;

		[HomieField("checksum")]
		public string Checksum;
	}
}
