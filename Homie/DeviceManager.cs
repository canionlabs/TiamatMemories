using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Homie.Data;
using Homie.utils;

namespace Homie
{
	public static class DeviceManager
	{
		public static void ProcessField(string mainTopic, string data, string baseTopic, PropertyInfo property, ref object obj)
		{
			HomieField @header = property.GetCustomAttribute<HomieField>();

			baseTopic = baseTopic.JoinTopic(@header.tag);

			Console.WriteLine(baseTopic);

			if (Regex.IsMatch(mainTopic, baseTopic))
			{
				//Console.WriteLine("We have a match");

				if (property.IsString())
				{
					property.SetValue(obj, data);
				}
				else if (property.IsInt())
				{
					property.SetValue(obj, data.ParseInt());
				}
				else if (property.IsBool())
				{
					property.SetValue(obj, data.ParseBool());
				}
			}
		}

		public static void ProcessMap(string mainTopic, string data, string baseTopic, PropertyInfo property, ref object obj)
		{
			var map = property.GetValue(obj);
			var @generics = map.GetType().GenericTypeArguments;
			var targetType = @generics[1];

			var @header = targetType.GetCustomAttribute<HomieMap>();

			baseTopic = baseTopic.JoinTopic(@header.tag);

			if (Regex.IsMatch(mainTopic, baseTopic))
			{
				Console.WriteLine("Processing property {0} with data {1}", property.Name, data);

				var keys = data.Split(",");
				MethodInfo method = map.GetType().GetMethod("TryAdd", @generics);

				foreach(var key in keys)
				{
					var item = Activator.CreateInstance(targetType);

					method.Invoke(map, new object[] { key, item });

					Console.WriteLine("Creating new {0} with key {1}", targetType, key);
				}
			}
		}

		public static void ProcessStruct(string mainTopic, string data, string baseTopic, ref object obj)
		{
			Type @type = obj.GetType();
			//Console.WriteLine(@type.Name);

			HomieStruct @header = @type.GetCustomAttribute<HomieStruct>();

			if (@header != null)
			{
				baseTopic = baseTopic.JoinTopic(@header.tag);

				//Console.WriteLine(baseTopic);

				foreach (var property in @type.GetProperties())
				{
					foreach (var attr in property.GetCustomAttributes())
					{
						if (attr is HomieField)
						{
							ProcessField(mainTopic, data, baseTopic, property, ref obj);
						}

						if (attr is HomieStruct)
						{
							object @struct = property.GetValue(obj);
							ProcessStruct(mainTopic, data, baseTopic, ref @struct);

							property.SetValue(obj, @struct);
						}

						if (attr is HomieMap)
						{
							ProcessMap(mainTopic, data, baseTopic, property, ref obj);
						}
					}
				}
			}
		}

		public static void Process(string topic, string data, Device device)
		{
			string @header = device.GetType().GetCustomAttribute<HomieStruct>().tag;

			foreach (PropertyInfo property in device.GetType().GetProperties())
			{
				HomieField homieField = property.GetCustomAttribute<HomieField>();

				foreach (var attr in property.GetCustomAttributes())
				{
					if (attr is HomieField)
					{
						HomieField @field = attr as HomieField;
					}

					if (attr is HomieStruct)
					{
						HomieStruct @struct = attr as HomieStruct;
					}
				}


				if (homieField != null)
				{
					if (Regex.IsMatch(topic, homieField.tag))
					{
						if (property.IsString())
						{
							Console.WriteLine("{0} : {1}", property.Name, data);
						}
						else if (property.IsInt())
						{
							Console.WriteLine("{0} : {1}", property.Name, data.ParseInt());
							Console.WriteLine(data.ParseInt());
						}
						else if (property.IsBool())
						{
							Console.WriteLine("{0} : {1}", property.Name, data.ParseBool());
						}
					}
				}
			}
		}
	}
}
