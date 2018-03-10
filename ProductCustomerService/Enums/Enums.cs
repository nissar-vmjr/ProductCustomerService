using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductCustomerService
{
    public enum TransactionTypeEnum
    {
        Cash=1,
        Cheque=2,
        Online=3,
    }

    public enum ContactTypeEnum
    {
        Mobile=1,
        Landline=2,
    }

    public enum NotificationTypeEnum
    {
        ChequePending=1,
        StockAlert=2,
        CustomerAlert=3,
    }

    public enum ChequeStatusEnum
    {
        NotInitiated=1,
        Pending=2,
        Passed=3,
        Rejected=4,
        Closed=5
    }

    public enum TransactionsDataTemplate
    {
        Customer=1,
        Product=2
    }
}