using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StraddleDisburseTransactionCore.Constants;
using StraddleDisburseTransactionCore.Models;
using StraddleDisburseTransactionCore.Models.DTO.Shared;
using StraddleDisburseTransactionCore.Services.Wallets.Interfaces;
using StraddleDisburseTransactionData.Enums;
using StraddleDisburseTransactionData.Models.Wallets;
using StraddleDisburseTransactionRepository;
using StraddleDisburseTransactionRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace StraddleDisburseTransactionCore.Services.Wallets
{
    public class DisburseTransactionService : IDisburseTransactionService
    {
        private readonly IGenericRepository<WalletTransaction> _transactionRepo;
        private readonly IConfiguration _configuration;

        public DisburseTransactionService(IGenericRepository<WalletTransaction> transactionRepo, IConfiguration configuration)
        {
            _transactionRepo = transactionRepo;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<string>> InitiateDisburseTransactionAsync(DisburseTransactionDTO disburseTransactionDTO)
        {
            if (disburseTransactionDTO == null)
            {
                return ServiceResponse<string>.Failed(ServiceMessages.ParameterEmptyOrNull);
            }

            WalletTransaction? walletTransaction = await _transactionRepo.Query()
                                                                         .OrderByDescending(transaction => transaction.DateCreated)
                                                                         .FirstOrDefaultAsync(transaction => transaction.SourceAccountId == disburseTransactionDTO.SourceAccountId);

            if (walletTransaction == null)
            {
                return ServiceResponse<string>.Failed(DisburseTransactionServiceConstants.TransactionNotFound);
            }

            walletTransaction.TransactionReference = disburseTransactionDTO.TransactionReference;
            walletTransaction.IsAmountLiened = true;
            walletTransaction.DateUpdated = DateTime.UtcNow;

            await _transactionRepo.SaveChangesToDbAsync();

            return ServiceResponse<string>.Success(string.Empty, ServiceMessages.Success);
        }

        public async Task<ServiceResponse<string>> CompleteDisburseTransactionAsync()
        {
            StraddleConfig straddleConfig = new();
            _configuration.GetSection(StraddleConfig.ConfigName).Bind(straddleConfig);

            List<WalletTransaction> walletTransactions = await _transactionRepo.Query()
                                                                               .Where(transaction => transaction.TransactionStatus == (int)TransactionStatus.Pending
                                                                               && transaction.IsAmountLiened)
                                                                               .ToListAsync();

            if (walletTransactions == null)
            {
                return ServiceResponse<string>.Failed(DisburseTransactionServiceConstants.TransactionNotFound);
            }

            foreach (WalletTransaction walletTransaction in walletTransactions)
            {
                //check if the transaction has reached the allowable duration or interval for processing transactions
                if ((DateTime.UtcNow.Date == walletTransaction.DateCreated.Value.Date) 
                    && (DateTime.UtcNow.Hour - walletTransaction.DateCreated.Value.Hour) == straddleConfig.TransactionProcessingInterval)
                {
                    //TODO: Make API call to process the transactions and further act on the output or response
                    //from the external API. If successful, perform some db operations such as performing a debit
                    //and unliening the amount, reflect the debit on the wallet balance, update transaction status.etc
                    //If unsuccessful, should a retry happen or customer be notified on the status of the transaction instantly.etc?
                }
            }

            return ServiceResponse<string>.Success(string.Empty, ServiceMessages.Success);
        }

    }
}
