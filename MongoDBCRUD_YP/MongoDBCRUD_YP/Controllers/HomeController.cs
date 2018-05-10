using MongoDB.Driver;
using MongoDBCRUD_YP.BLL;
using MongoDBModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MongoDBCRUD_YP.Controllers
{
    /// <summary>
    /// MongoDBDemo
    /// </summary>
    public class HomeController : ApiController
    {

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public List<User> GetData()
        {
            List<User> list = new List<User>();
            try
            {
                // list = new MongoDbService().Get<User>(a => a.Name==name);
                list = new MongoDbHelper<User>().QueryAll();
            }
            catch (Exception e)
            {
                list = null;
            }
           
            return list;
        }

        /// <summary>
        /// 新增一条数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse InsertData([FromBody]User request)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                var id = Guid.NewGuid().ToString();
                if (request == null)
                {
                    return new BaseResponse() { IsSuccess = false, Message = "请求参数不能为空" };
                }
                new MongoDbHelper<User>().Insert(request);
              
                response.IsSuccess = true;
                response.Message = "SUCCESS";
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = "提交MongoDB数据失败";
            }
            return response;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse UpdateData([FromBody]User request)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                var id = Guid.NewGuid().ToString();
                if (request == null)
                {
                    return new BaseResponse(){ IsSuccess = false,Message="请求参数不能为空" };
                }
                new MongoDbHelper<User>().Update(request);
                response.IsSuccess = true;
                response.Message = "SUCCESS";
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = "更新MongoDB数据失败";
            }
            return response;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseResponse DeleteData([FromBody]User request)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                var id = Guid.NewGuid().ToString();
                if (request == null)
                {
                    return new BaseResponse() { IsSuccess = false, Message = "请求参数不能为空" };
                }
                new MongoDbHelper<User>().Delete(request);
                response.IsSuccess = true;
                response.Message = "SUCCESS";
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = "删除MongoDB数据失败";
            }
            return response;
        }

    }
}
