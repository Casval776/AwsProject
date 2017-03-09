using System;
using System.Threading.Tasks;
using AwsDatabase.Global;
using AwsDatabase.Model;
using AwsFileWatcher.Controller;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace AwsDatabase
{
    public class AwsDbClient
    {
        #region Private Members
        private static readonly Lazy<AwsDbClient> DbClient = new Lazy<AwsDbClient>(() => new AwsDbClient());
        //private static readonly string ConnectionString = null; //Gonna have to update this later
        private const string DefaultDbName = "admin";
        private const string DefaultCollectionName = "default";
        #endregion

        #region Internal Members
        internal IMongoDatabase Database;
        internal IMongoCollection<AwsDbItem> Collection;
        #endregion

        #region Constructors
        private AwsDbClient()
        {
            //Initialize variables
            var mongoClient = new MongoClient();

            //Create DB and Collection
            Database = mongoClient.GetDatabase(DefaultDbName);
            Collection = Database.GetCollection<AwsDbItem>(DefaultCollectionName);
        }

        static AwsDbClient() { }
        #endregion

        #region Public Functions
        public async Task<bool> Insert(AwsFileStructure item)
        {
            try
            {
                var tmpItem = new AwsDbItem
                {
                    Json = JsonConvert.SerializeObject(item),
                    _id = Data.DefaultId
                };
                await Collection.InsertOneAsync(tmpItem);
                return true;
            }
            catch (Exception exc)
            {
                //@TODO: Add logging
                return false;
            }
        }

        public async Task<bool> Replace(AwsDbItem item)
        {
            try
            {
                var filter = Builders<AwsDbItem>.Filter.Eq(x => x._id, item._id);
                await Collection.ReplaceOneAsync(filter, item);
                return true;
            }
            catch (Exception exc)
            {
                //@TODO: Add logging
                return false;
            }
        }

        public static AwsDbClient GetInstance() => DbClient.Value;
        #endregion
    }
}
