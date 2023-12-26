using Microsoft.Extensions.DependencyInjection;
using StraddleDisburseTransactionCore.Helpers;
using StraddleDisburseTransactionCore.Services.Common.Interfaces;
using StraddleDisburseTransactionCore.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StraddleDisburseTransactionCore.Services.Wallets.Interfaces;
using StraddleDisburseTransactionCore.Services.Wallets;

namespace StraddleDisburseTransactionCore.Services
{
    public static class ServicesModule
    {
        public static void AddServices(this IServiceCollection services)
        {
            //Common
            services.AddScoped<IHttpService, HttpService>();

            //Wallets
            services.AddScoped<IDisburseTransactionService, DisburseTransactionService>();

            //Helpers
            services.AddScoped<IClaimHelper, ClaimHelper>();
        }
    }
}
