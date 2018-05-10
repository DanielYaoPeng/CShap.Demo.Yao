using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MongoDBCRUD_YP.BLL
{
    public class MongoLogic
    {
        private static MongoClient client;
        private static IMongoDatabase database;
        //本地配置
        private readonly static string MongoDBConnectionStr = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
        //数据库名称 
        private static string DefaultDataBaseName = ConfigurationManager.ConnectionStrings["person"].ConnectionString;


        public MongoLogic()
        {
            GetConnection(DefaultDataBaseName);
        }

        /// <summary>
        /// 构造函数 指定数据库
        /// </summary>
        /// <param name="dataBaseName"></param>
        public MongoLogic(string dataBaseName)
        {
            GetConnection(dataBaseName);
        }

        private static void GetConnection(string dataBaseName)
        {
            client = new MongoClient(MongoDBConnectionStr);
            database = client.GetDatabase(dataBaseName);
        }

        /// <summary>
        /// 异步插入一条数据，手动输入collection name
        /// </summary>
        public Task InsertAsync<T>(string collectionName, T obj)
        {
            if (database == null)
            {
                throw new Exception("没有指定数据库");
            }
            var collection = database.GetCollection<T>(collectionName);
            return collection.InsertOneAsync(obj);
        }

        /// <summary>
        /// 异步插入一条数据，采用类型T的完全限定名作为collection name
        /// </summary>
        public Task InsertAsync<T>(T obj)
        {
            return InsertAsync(typeof(T).FullName, obj);
        }

        /// <summary>
        /// 异步插入多条数据，手动输入collection name
        /// </summary>
        public Task BatchInsertAsync<T>(string collectionName, IEnumerable<T> objs)
        {
            if (database == null)
            {
                throw new Exception("没有指定数据库");
            }
            if (objs == null)
            {
                throw new ArgumentException();
            }
            var collection = database.GetCollection<T>(collectionName);
            return collection.InsertManyAsync(objs);
        }

        /// <summary>
        /// 异步插入多条数据，采用类型T的完全限定名作为collection name
        /// </summary>
        public Task BatchInsertAsync<T>(IEnumerable<T> objs)
        {
            return BatchInsertAsync(typeof(T).FullName, objs);
        }

        /// <summary>
        /// 插入一条数据
        /// </summary>
        public void Insert<T>(T obj)
        {
            InsertAsync(obj).Wait();
        }

        /// <summary>
        /// 插入多条数据
        /// </summary>
        public void Insert<T>(IEnumerable<T> objs)
        {
            BatchInsertAsync(objs).Wait();
        }

        /// <summary>
        /// MongoDB C# Driver的Find方法，返回IFindFluent。手动输入collection name
        /// </summary>
        public IFindFluent<T, T> Find<T>(string collectionName, FilterDefinition<T> filter, FindOptions options = null)
        {
            if (database == null)
            {
                throw new Exception("没有指定数据库");
            }
            var collection = database.GetCollection<T>(collectionName);
            return collection.Find(filter, options);
        }

        /// <summary>
        /// MongoDB C# Driver的Find方法，返回IFindFluent。采用类型T的完全限定名作为collection name
        /// </summary>
        public IFindFluent<T, T> Find<T>(FilterDefinition<T> filter, FindOptions options = null)
        {
            return Find(typeof(T).FullName, filter, options);
        }

        /// <summary>
        /// 取符合条件的数据 sort中多个排序条件逗号分隔，默认asc
        /// </summary>
        public List<T> Get<T>(Expression<Func<T, bool>> condition, int skip, int limit, string sort)
        {
            return Get(new List<Expression<Func<T, bool>>> { condition }, skip, limit, sort);
        }

        public List<T> Get<T>(Expression<Func<T, bool>> condition)
        {
            return Get(condition, 0, 0, null);
        }

        /// <summary>
        /// 取符合条件的数据 sort中多个排序条件逗号分隔，默认asc
        /// </summary>
        public List<T> Get<T>(List<Expression<Func<T, bool>>> conditions, int skip, int limit, string sort)
        {
            if (conditions == null || conditions.Count == 0)
            {
                conditions = new List<Expression<Func<T, bool>>> { x => true };
            }
            var builder = Builders<T>.Filter;
            var filter = builder.And(conditions.Select(x => builder.Where(x)));

            var ret = new List<T>();
            try
            {
                List<SortDefinition<T>> sortDefList = new List<SortDefinition<T>>();
                if (sort != null)
                {
                    var sortList = sort.Split(',');
                    for (var i = 0; i < sortList.Length; i++)
                    {
                        var sl = Regex.Replace(sortList[i].Trim(), @"\s+", " ").Split(' ');
                        if (sl.Length == 1 || (sl.Length >= 2 && sl[1].ToLower() == "asc"))
                        {
                            sortDefList.Add(Builders<T>.Sort.Ascending(sl[0]));
                        }
                        else if (sl.Length >= 2 && sl[1].ToLower() == "desc")
                        {
                            sortDefList.Add(Builders<T>.Sort.Descending(sl[0]));
                        }
                    }
                }
                var sortDef = Builders<T>.Sort.Combine(sortDefList);
                ret = Find(filter).Sort(sortDef).Skip(skip).Limit(limit).ToListAsync().Result;
            }
            catch (Exception e)
            {
                //异常处理
            }
            return ret;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public List<T> Get<T>(List<Expression<Func<T, bool>>> conditions)
        {
            return Get(conditions, 0, 0, null);
        }

   


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        public async Task<long> Modify<T>(string collection, string id, string field, string value)
        {

            var db = client.GetDatabase(typeof(T).FullName);
            var coll = db.GetCollection<T>(collection);
            var filter = Builders<T>.Filter.Eq("Id", ObjectId.Parse(id));
            var updated = Builders<T>.Update.Set(field, value);
            var result = await coll.UpdateManyAsync(filter, updated).ConfigureAwait(false);
            return result.ModifiedCount;
        }

       

    }
}