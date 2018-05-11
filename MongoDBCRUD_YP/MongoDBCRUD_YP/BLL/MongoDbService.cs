using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace MongoDBCRUD_YP.BLL
{
    public class MongoDbService
    {
        private static readonly string connStr = ConfigurationManager.AppSettings["MongoDb"].ToString();//GlobalConfig.Settings["mongoConnStr"];

        private static readonly string dbName = ConfigurationManager.AppSettings["User"].ToString();//GlobalConfig.Settings["mongoDbName"];

        private static IMongoDatabase db = null;

        private static readonly object lockHelper = new object();

        private MongoDbService() { }

        public static IMongoDatabase GetDb()
        {
            if (db == null)
            {
                lock (lockHelper)
                {
                    if (db == null)
                    {
                        var client = new MongoClient(connStr);
                        db = client.GetDatabase(dbName);
                    }
                }
            }
            return db;
        }
    }



    public class MongoDbHelper<T> where T : BaseEntity
    {
        #region
        //public static MongoClient client;
        //public IMongoCollection<T> collection = null;


        //public static IMongoDatabase db = null;

        //public MongoDbHelper()
        //{
        //    client = new MongoClient("mongodb://127.0.0.1:27017");
        //    db = client.GetDatabase("User");
        //    collection = db.GetCollection<T>(typeof(T).Name);
        //}
        #endregion
        private IMongoDatabase db = null;
        private readonly MongoClient client;
        private IMongoCollection<T> collection = null;
        private static readonly string connStr = ConfigurationManager.AppSettings["MongoDb"].ToString();//GlobalConfig.Settings["mongoConnStr"];

        public MongoDbHelper()
        {
            // this.db = MongoDbService.GetDb();
            client = new MongoClient(connStr);
            //this.db=client.GetDatabase(dbName);
          
        }



        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Insert(T entity,string dbName)
        {
            this.db = client.GetDatabase(dbName);
            collection = db.GetCollection<T>(typeof(T).Name);
            var flag = ObjectId.GenerateNewId();
            //entity.GetType().GetProperty("Id").SetValue(entity, flag);
            entity.State = "y";
            entity.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            entity.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            collection.InsertOneAsync(entity);
            return entity;
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public void Modify(string id, string field, string value,string dbName)
        {
            this.db = client.GetDatabase(dbName);
            collection = db.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("Id", id);
            var updated = Builders<T>.Update.Set(field, value);
            UpdateResult result = collection.UpdateOneAsync(filter, updated).Result;
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        public void Update(T entity,string dbName)
        {
            try
            {
                this.db = client.GetDatabase(dbName);
                collection = db.GetCollection<T>(typeof(T).Name);
                var old = collection.Find(e => e.Id.Equals(entity.Id)).ToList().FirstOrDefault();

                foreach (var prop in entity.GetType().GetProperties())
                {
                    var newValue = prop.GetValue(entity);
                    var oldValue = old.GetType().GetProperty(prop.Name).GetValue(old);
                    if (newValue != null)
                    {
                        if (oldValue == null)
                            oldValue = "";
                        //if (!newValue.ToString().Equals(oldValue.ToString()))
                        //{
                            old.GetType().GetProperty(prop.Name).SetValue(old, newValue);
                        //}
                    }
                }
                old.State = "n";
                old.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                var filter = Builders<T>.Filter.Eq("Id", entity.Id);
                ReplaceOneResult result = collection.ReplaceOneAsync(filter, old).Result;
            }
            catch (Exception ex)
            {
                var aaa = ex.Message + ex.StackTrace;
                throw;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(T entity,string dbName)
        {
            this.db = client.GetDatabase(dbName);
            collection = db.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("Id", entity.Id);
            collection.DeleteOneAsync(filter);
        }
        /// <summary>
        /// 根据id查询一条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T QueryOne(string id)
        {
            return collection.Find(a => a.Id == id).ToList().FirstOrDefault();
        }
        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        public List<T> QueryAll(string dbName)
        {
            this.db = client.GetDatabase(dbName);
            collection = db.GetCollection<T>(typeof(T).Name);
            return collection.Find(a => a.State != "").ToList();
        }
        /// <summary>
        /// 根据条件查询一条数据
        /// </summary>
        /// <param name="express"></param>
        /// <returns></returns>
        public T QueryByFirst(Expression<Func<T, bool>> express,string dbName)
        {
            this.db = client.GetDatabase(dbName);
            collection = db.GetCollection<T>(typeof(T).Name);
            return collection.Find(express).ToList().FirstOrDefault();
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="list"></param>
        public void InsertBatch(List<T> list,string dbName)
        {
            this.db = client.GetDatabase(dbName);
            collection = db.GetCollection<T>(typeof(T).Name);
            collection.InsertManyAsync(list);
        }
        /// <summary>
        /// 根据Id批量删除
        /// </summary>
        public void DeleteBatch(List<ObjectId> list,string dbName)
        {
            this.db = client.GetDatabase(dbName);
            collection = db.GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.In("Id", list);
            collection.DeleteManyAsync(filter);
        }

        /// <summary>
        /// 未添加到索引的数据
        /// </summary>
        /// <returns></returns>
        public List<T> QueryToLucene(string dbName)
        {
            this.db = client.GetDatabase(dbName);
            collection = db.GetCollection<T>(typeof(T).Name);
            return collection.Find(a => a.State.Equals("y") || a.State.Equals("n")).ToList();
        }

       
        public async Task<UpdateResult> UpdateAsync( T t, string id)
        {
            try
            {
                //修改条件
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", id);
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (item.Name.ToLower() == "Id") continue;
                    list.Add(Builders<T>.Update.Set(item.Name, item.GetValue(t)));
                }



                var updatefilter = Builders<T>.Update.Combine(list);
                return await collection.UpdateOneAsync(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public  UpdateResult UpdateOne( T t, string id)
        {
            try
            {
                //修改条件
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("Id", id);
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (item.Name.ToLower() == "Id") continue;
                    list.Add(Builders<T>.Update.Set(item.Name, item.GetValue(t)));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return collection.UpdateOne(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public  void Updates<T>(string id, T entity, List<string> properts = null, bool replace = false) where T : User
        {
            if (entity == null)
                throw new NullReferenceException();

            var type = entity.GetType();
            ///修改的属性集合
            var list = new List<UpdateDefinition<T>>();

            foreach (var propert in type.GetProperties())
            {
                if (propert.Name.ToLower() != "id")
                {
                    if (properts == null || properts.Count < 1 || properts.Any(o => o.ToLower() == propert.Name.ToLower()))
                    {
                        var replaceValue = propert.GetValue(entity);
                        if (replaceValue != null)
                        {
                            list.Add(Builders<T>.Update.Set(propert.Name, replaceValue));
                        }
                        else if (replace)
                            list.Add(Builders<T>.Update.Set(propert.Name, replaceValue));
                    }
                }
            }
            #region 有可修改的属性
            if (list.Count > 0)
            {
                ///合并多个修改//new List<UpdateDefinition<T>>() { Builders<T>.Update.Set("Name", "111") }
                var builders = Builders<T>.Update.Combine(list);
                ///执行提交修改
               // collection.UpdateOne(o => o.Id == entity.Id, builders);
            }
            #endregion

        }


        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="host"></param>
        /// <param name="dic"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public  async Task<UpdateResult> UpdateManayAsync(T t, Dictionary<string, string> dic, FilterDefinition<T> filter)
        {
            try
            {
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (!dic.ContainsKey(item.Name)) continue;
                    var value = dic[item.Name];
                    list.Add(Builders<T>.Update.Set(item.Name, value));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return await collection.UpdateManyAsync(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }


}