using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StraddleDisburseTransactionCore.Helpers
{
    public interface IClaimHelper
    {
        string GetClaimValue(string claimType);

        string GetUserEmail();

        string GetUserPhoneNumber();
    }
}
