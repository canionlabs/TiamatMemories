using System;
using System.Collections.Generic;

namespace Memory.db
{
	public enum StorageType
	{
		MONGODB

	}

	public interface IStorage
	{
		StorageType StorageType { get; }

		/// <summary>
		/// Declare your database url
		/// </summary>
		/// <param name="url">
		/// Database url, string following the format mongodb://localhost:27017
		/// </param>
		void Setup(string url);

		/// <summary>
		/// Save an event
		/// </summary>
		/// <param name="unixTimestamp">
		/// A string timestamp follwing the format of the Unix Timestamp
		/// </param>
		/// <param name="data">
		/// Received data
		/// </param>
		void Save(string unixTimestamp, string data);
	}
}
