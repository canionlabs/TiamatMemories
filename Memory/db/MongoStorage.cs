using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Memory.db
{
	public class MongoStorage : IStorage
	{

		// ========= PUBLIC MEMBERS INTERFACES ====================================
		public StorageType StorageType => StorageType.MONGODB;

		public void Setup(string url, string dbName, string dbCollection)
		{
			_dbClient = new MongoClient(url);
			_database = _dbClient.GetDatabase(dbName);
			_collection = _database.GetCollection<BsonDocument>(dbCollection);
		}

		public async void Save(long unixTimestamp, string data)
		{
			dynamic document = new BsonDocument
			{
				{"timestamp", unixTimestamp},
				{"data", data}
			};
			await _collection.InsertOneAsync(document);

		}

		// ========= PRIVATE MEMBERS ====================================
		// Dictionary<string, string> _metaData;
		MongoClient _dbClient;
		dynamic _database;
		dynamic _collection;

		// ========= PUBLIC MEMBERS ====================================
		public bool isAvaliable()
		{
			// Incomplete
			return true;
		}
	}
}
