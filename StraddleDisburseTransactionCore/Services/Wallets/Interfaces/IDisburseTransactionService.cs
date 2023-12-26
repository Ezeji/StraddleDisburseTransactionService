using StraddleDisburseTransactionCore.Models.DTO.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleDisburseTransactionCore.Services.Wallets.Interfaces
{
    public interface IDisburseTransactionService
    {
        /// <summary>
        /// Initiate disburse transaction.
        /// </summary>
        /// <param name="disburseTransactionDTO"></param>
        /// <returns></returns>
        Task<ServiceResponse<string>> InitiateDisburseTransactionAsync(DisburseTransactionDTO disburseTransactionDTO);

        /// <summary>
        /// Complete disburse transaction.
        /// </summary>
        /// <returns></returns>
        Task<ServiceResponse<string>> CompleteDisburseTransactionAsync();
    }
}
