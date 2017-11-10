using Aston.Entities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text;

namespace Aston.WebApi.Helper
{
    public class ConnectionStringHelper
    {
        private readonly ConnectionString _serviceSettings;
        public ConnectionStringHelper(IOptions<ConnectionString> serviceSettings)
        {
            _serviceSettings = serviceSettings.Value;           
        }
        public string DefaultConnection { get { return _serviceSettings.DefaultConnection; }  }
     

    }
}
