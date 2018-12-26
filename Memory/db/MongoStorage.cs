using System;
using System.Collections.Generic;
using Memory.utils;
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
			_isSetup = true;
		}

		public void Save(string data, string topic)
		{
			long tsNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			saveDocument(data, topic, tsNow);
		}

		public bool IsAvailable
		{
			get
			{
				return _isSetup;
			}
		}

		// ========= PRIVATE MEMBERS ====================================
		// Dictionary<string, string> _metaData;
		MongoClient _dbClient;
		dynamic _database;
		dynamic _collection;
		bool _isSetup;

		private async void saveDocument(string data, string topic, long timestamp) {
			dynamic document = new BsonDocument
			{
				{"topic", topic},
				{"data", data},
				{"timestamp", timestamp}
			};
			await _collection.InsertOneAsync(document);
		}

		// ========= PUBLIC MEMBERS ====================================
	}
}
