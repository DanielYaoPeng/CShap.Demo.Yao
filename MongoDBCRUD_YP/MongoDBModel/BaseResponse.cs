using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBModel
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


    public class BaseEntity
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid().ToString("N");
        }
        public string Id { get; set; }

        public string State { get; set; }

        public string CreateTime { get; set; }

        public string UpdateTime { get; set; }
    }
}
