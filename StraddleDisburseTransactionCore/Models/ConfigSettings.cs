using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleDisburseTransactionCore.Models
{
    public class StraddleConfig
    {
        public const string ConfigName = nameof(StraddleConfig);

        public int TransactionProcessingInterval { get; set; }
    }
}
