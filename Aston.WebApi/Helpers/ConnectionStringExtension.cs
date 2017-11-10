using Aston.Entities;
using Aston.WebApi.Helper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aston.WebApi.Helpers
{
    public class ConnectionStringExtension : ConnectionStringHelper
    {
        private readonly ConnectionString _appSetting;
        public ConnectionStringExtension(IOptions<ConnectionString> serviceSettings) : base(serviceSettings)
        {
            _appSetting = serviceSettings.Value;
        }

        public new string DefaultConnection { get { return _appSetting.DefaultConnection; }  }
      
    }
}
