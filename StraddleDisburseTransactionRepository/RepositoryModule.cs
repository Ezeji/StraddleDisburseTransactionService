using Microsoft.Extensions.DependencyInjection;
using StraddleDisburseTransactionData.Models.Wallets;
using StraddleDisburseTransactionRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleDisburseTransactionRepository
{
    public static class RepositoryModule
    {
        public static void AddCoreRepository(this IServiceCollection services)
        {
            //Transaction
            services.AddScoped<IGenericRepository<WalletCustomer>, GenericRepository<WalletCustomer>>();
            services.AddScoped<IGenericRepository<WalletAccount>, GenericRepository<WalletAccount>>();
            services.AddScoped<IGenericRepository<WalletTransaction>, GenericRepository<WalletTransaction>>();
        }
    }
}
