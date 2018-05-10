using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoDBCRUD_YP.Models
{
    public class User: BaseEntity
    {
        public string _id { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime BirthDateTime { get; set; }

        public User Son { get; set; }

        public Sex Sex { get; set; }

        public List<int> NumList { get; set; }

        public List<string> AddressList { get; set; }
    }

    public enum Sex
    {
        Man = 1,
        Woman = 2
    }

   
}