using System;

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
		void Setup(string url, string dbName, string dbCollection);

		/// <summary>
		/// Save an event
		/// </summary>
		/// <param name="unixTimestamp">
		/// A long timestamp follwing the format of the Unix Timestamp
		/// </param>
		/// <param name="data">
		/// Received data
		/// </param>
		void Save(long unixTimestamp, string data);
	}
}
