using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBModel
{
    public class User : BaseEntity
    {
        public string Name { get; set; }

        public int Age { get; set; }
        
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
