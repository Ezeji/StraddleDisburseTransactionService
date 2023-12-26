using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleDisburseTransactionCore.Models
{
    public static class ConfigSettingsModule
    {
        public static void AddConfigSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StraddleConfig>(configuration.GetSection(StraddleConfig.ConfigName));
        }
    }
}
