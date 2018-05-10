using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MongoDBCRUD_YP.BLL
{
    /// <summary>
    /// MongoDB CRUD操作
    /// </summary>
    public class MongoService
    {
        private readonly string _connString = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
        private readonly MongoClient _mongoClient;
     

        #region 初始化
        public MongoService()
        {
            ConventionRegistry.Register("IgnoreExtraElements", new ConventionPack { new IgnoreExtraElementsConvention(true) }, type => true);
            _mongoClient = new MongoClient(_connString);

        }

        protected virtual IMongoDatabase CreateDatabase()
        {
            string connectionString = null;
            if (!string.IsNullOrEmpty(_connString))
            {
                //从配置文件中获取对应的连接信息
                connectionString = ConfigurationManager.ConnectionStrings[_connString].ConnectionString;
            }
          

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(new MongoUrl(connectionString).DatabaseName);

            return database;
        }

        #endregion

        #region 增
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合（表）</param>
        /// <param name="entity">实体(文档)</param>
        public void Add<T>(string database, string collection, T entity) 
        {
            AddAsync(database, collection, entity).Wait();
        }

      

        /// <summary>
        /// 增
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="database">库</param>
        /// <param name="collection">集合（表）</param>
        /// <param name="entity">实体(文档)</param>
        /// <returns></returns>
        public async Task AddAsync<T>(string database, string collection, T entity) 
        {
            var db = _mongoClient.GetDatabase(database);
            var coll = db.GetCollection<T>(collection);
            await coll.InsertOneAsync(entity).ConfigureAwait(false);
        }

        #endregion

        #region 删

        #endregion

        #region 改

        #endregion

        #region 查

        #endregion
    }
}