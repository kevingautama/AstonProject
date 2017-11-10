using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aston.Business.Utillities
{
    public class ConfigureSetting
    {
        private static string _connectionString;

        public static string GetConnectionString
        {
            get
            {
                return _connectionString;
            }

            set
            {
                _connectionString = value;
            }
           // return "User ID=escurityuser;Password=123;Host=192.168.1.99;Port=5432;Database=astondb;Pooling=true;";
        }
    }
}
