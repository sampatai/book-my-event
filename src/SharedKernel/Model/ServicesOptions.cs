using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Model
{
    public class ServicesOptions
    {
        public ServiceConfig Auth { get; set; } = new ServiceConfig();
        public ServiceConfig WebApi { get; set; } = new ServiceConfig();
        public ServiceConfig ReactClient { get; set; } = new ServiceConfig();
    }

    public class ServiceConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
    }
}
