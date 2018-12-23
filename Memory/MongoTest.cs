using System;
using Xunit;
using Memory.db;
using Memory.utils;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Memory
{
    public class MongoStorageTests
    {
        private MongoStorage mongoStorage;
        private IMongoCollection<BsonDocument> collection;

        public MongoStorageTests()
        {
            string url = "mongodb://localhost:27017";
            string dbName = "tiamat";
            string collectionName = "events";

            mongoStorage = new MongoStorage();
            mongoStorage.Setup(url, dbName, collectionName);

            var client = new MongoClient(url);
            var database = client.GetDatabase(dbName);
            collection = database.GetCollection<BsonDocument>(collectionName);
        }

        [Fact]
        public void Avaliable()
        {
            Assert.True(mongoStorage.isAvaliable());
        }

        [Fact]
        public void SaveData()
        {
            long tsNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            mongoStorage.Save(tsNow, "Test");

            var filter = Builders<BsonDocument>.Filter.Eq("timestamp", tsNow);
            var document = collection.Find(filter).First();
            Assert.Equal(document["timestamp"], tsNow);
            Assert.True(mongoStorage.isAvaliable());
        }
    }
}
