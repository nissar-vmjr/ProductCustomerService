using ProductCustomerService.UIContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProductCustomerService.BusinessLayer
{
    public interface IBusinessProvider
    {
        Task<RsCustomer> SaveCustomerDetails(RqCustomer rqCustomer);
        RsCustomer GetCustomerDetails(long customerID, string userLogin);
        RsCustomerKeywordSearch GetCustomerSearchResults(string keyword);
        RsBalanceTransaction SaveBalanceTransaction(RqBalanceTransaction rqBalanceTransaction);
        RsCommon DeleteBalanceTransaction(RqDeleteTransaction rqDeleteTransaction);
        RsBalanceTransaction GetBalanceTransaction(long transactionID, string userlogin);
        RsSearchCustomerResult GetCustomerRecentTransactions(RqBalanceTransaction rqBalanceTransaction);
        RsProducts GetProducts();
        RsSubProducts GetSubProducts(long productID, string userlogin);
        RsSubproduct GetSubProductDetails(long subProductID, string userlogin);
        RsCommonData GetCommonData();
        RsProductTransaction SaveProductTransaction(RqProductTransact rqProductTransaction);
        RsCommon DeleteProductTransaction(RqDeleteTransaction rqDeleteTransaction);
        RsProductTransaction GetProductTransactionDetail(long transactionID, string userlogin);
        RsNotifications GetNotifications();
        RsCommon ChangeChequeStatus(RqChangeChequeStatus rqTransaction);
        RsCommon AddLineSale(RqLineSale rqLineSale);

        RsCommon AddLineSaleTracking(RqLineSaleTracking rqLineSaleTracking);

        RsLineSale GetPendingLineSales(string userlogin);

        RsLineSaleTrackings GetLineSaleTrackings(long lineSaleID, string userlogin);

        RsSearchCustomerResult SearchBalanceTransaction(RqSearch rqSearch);

        HttpResponseMessage ExportToExcel(RqSearch rqSearch);

        RsCustomers SearchCustomers(RqSearchCustomers rqSearchCustomers);

        Task<RsSubProducts> GetAllProducts();

        Task<RsCustomers> GetTopBuyingCustomers();

        Task<RsProductTransactions> SearchProductTransactions(RqProductTransactionsSearch rqProductTransactionsSearch);

        Task<RsCommon> DeleteLineSaleTracking(RqDeleteTransaction request);
        Task<RsCommon> DeleteLineSale(RqDeleteTransaction request);

        HttpResponseMessage ExportToExcel(RqProductTransactionsSearch rqProductTransactionsSearch);

        Task<RsLinesalesReport> GetLinesalesReport(RqSearch request);

        Task<RsSuppliers> GetSuppliers();

        Task<RsCommon> AddSupplierTracking(RqSupplier rqSupplier);

        bool AddDataTemplateFile(string filename);

        Task<RsCommon> AddNewProduct(RqProduct request);
        Task<RsCommon> AddNewSubProduct(RqSubproduct request);

        Task<RsCommon> AddNewSupplier(RqSupplierNew request);
    }
}
