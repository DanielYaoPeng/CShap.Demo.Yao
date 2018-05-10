using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoDBCRUD_YP.Models
{
    public class BaseResponse
    {
        public BaseResponse()
        {
            IsSuccess = false;
        }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }


    public  class BaseEntity
    {
        public ObjectId Id { get; set; }

        public string State { get; set; }

        public string CreateTime { get; set; }

        public string UpdateTime { get; set; }
    }
}