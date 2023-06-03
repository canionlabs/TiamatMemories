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
		public static void Process(string topic, string data, string baseTopic, Device device)
		{
			//object obj = device;
			//ProcessStruct(topic, data, baseTopic, ref obj);
		}

		public static void ProcessField(string mainTopic, string data, string baseTopic, PropertyInfo property, ref object obj)
		{
			HomieField @header = property.GetCustomAttribute<HomieField>();

			var testTopic = baseTopic.JoinTopic(@header.tag);

			//Console.WriteLine(baseTopic);

			if (Regex.IsMatch(mainTopic, testTopic))
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

			var keys = data.Split(",");

			if (Regex.IsMatch(mainTopic, baseTopic.JoinTopic(@header.tag)))
			{
				Console.WriteLine("Processing property {0} with data {1}", property.Name, data);

				MethodInfo methodAdd = map.GetType().GetMethod("TryAdd", @generics);

				foreach(var key in keys)
				{
					var item = Activator.CreateInstance(targetType);

					methodAdd.Invoke(map, new object[] { key, item });

					Console.WriteLine("Creating new {0} with key {1}", targetType, key);
				}
			}

			MethodInfo method = map.GetType().GetMethod("TryGetValue");

			foreach (var key in keys)
			{
				if (Regex.IsMatch(mainTopic, baseTopic.JoinTopic(key)))
				{
					object[] arguments = { "key", null };

					bool result = (bool) method.Invoke(map, arguments);

					if (result)
					{
						var item = arguments[1];

						if (item != null)
						{
							Console.WriteLine("I have a item of type {0}", item.GetType().Name);
						}
					}
				}
			}
		}

		public static void ProcessMapItem(string mainTopic, string data, ref object obj)
		{

		}

		public static void ProcessStruct(string mainTopic, string data, string baseTopic, ref object obj)
		{
			Type @type = obj.GetType();

			HomieStruct @header = @type.GetCustomAttribute<HomieStruct>();

			if (@header != null)
			{
				baseTopic = baseTopic.JoinTopic(@header.tag);

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
	}
}
