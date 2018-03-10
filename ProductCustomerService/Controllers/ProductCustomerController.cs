using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ProductCustomerService.UIContext;
using ProductCustomerService.BusinessLayer;
using System.Web.Http.Cors;
using System.Web;
using System.Threading.Tasks;

namespace ProductCustomerService.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    [CustomAuthorize]
    [RoutePrefix("api")]
    public class ProductCustomerController : BaseController
    {
        private IBusinessProvider businessProvider;

        public ProductCustomerController() : base()
        {
            businessProvider = new BusinessProvider();
        }
        //public ProductCustomerController(IBusinessProvider businessProvider)
        //{
        //    this.businessProvider = businessProvider;
        //}

        [Authorize]
        [Route("Customer/Save")]
        [HttpPost]
        public async Task<RsCustomer> SaveCustomer(RqCustomer request)
        {
            string userlogin = UserLogin;
            if (request == null)
                request = new RqCustomer();
            request.UserLogin = userlogin;
            var response =await  businessProvider.SaveCustomerDetails(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Edit/Save Customer details";
            response.Url = "api/Customer/Save";
            if (response.ResponseStatus != ResponseStatus.Success)
            {
                response.customer = null;
            }
            return response;
        }

        [Authorize]
        [Route("Customer/{customerID}")]
        [HttpGet]
        public RsCustomer GetCustomer(long customerID)
        {
            string userlogin = UserLogin;
            var response = businessProvider.GetCustomerDetails(customerID, userlogin);
            response.UserLogin = userlogin;
            response.WebAPIName = "Get Customer details";
            response.Url = "api/Customer/" + customerID.ToString();
            if (response.ResponseStatus != ResponseStatus.Success)
            {
                response.customer = null;
            }
            return response;
        }

        //[Authorize]
        [AllowAnonymous]
        [Route("Search/Customers/{keyword}")]
        [HttpGet]
        public RsCustomerKeywordSearch SearchCustomerWithKeyword(string keyword)
        {
            //string userlogin = UserLogin;
            var response = businessProvider.GetCustomerSearchResults(keyword);
            //response.UserLogin = userlogin;
            response.WebAPIName = "Search Customers by keyword";
            response.Url = "api/Search/Cutomers/" + keyword;
            if (response.ResponseStatus != ResponseStatus.Success)
            {
                response.customers = null;
            }
            return response;
        }


        //[Authorize]
        [AllowAnonymous]
        [Route("Search/Customer")]
        [HttpPost]
        public RsSearchCustomerResult GetCustomerRecentTransactions(RqBalanceTransaction request)
        {

            //string userlogin = UserLogin;
            request.UserLogin = "nissar";
            var response = businessProvider.GetCustomerRecentTransactions(request);
            //response.UserLogin = userlogin;
            response.WebAPIName = "Search Customers by keyword";
            response.Url = "api/Search/Customer";
            if (response.ResponseStatus != ResponseStatus.Success)
            {
                response.transactions = null;
            }
            return response;
        }

        [Authorize]
        [Route("Customer/Transaction/Save")]
        [HttpPost]
        public RsBalanceTransaction SaveBalanceTransaction(RqBalanceTransaction request)
        {
            string userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = businessProvider.SaveBalanceTransaction(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Save balance transaction";
            response.Url = "api/Customer/Transaction/Save";
            if (response.ResponseStatus != ResponseStatus.Success)
            {
                response.balanceTransaction = null;
            }
            return response;
        }

        [Authorize]
        [Route("Customer/Transaction/Delete")]
        [HttpPost]
        public RsCommon DeleteBalanceTransaction(RqDeleteTransaction request)
        {
            string userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = businessProvider.DeleteBalanceTransaction(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Delete balance transaction";
            response.Url = "api/Customer/Transaction/Delete";

            return response;
        }

        [Authorize]
        [Route("Customer/Transaction/{transactionID}")]
        [HttpGet]
        public RsBalanceTransaction GetBalanceTransaction(long transactionID)
        {
            string userlogin = UserLogin;
            var response = businessProvider.GetBalanceTransaction(transactionID, userlogin);
            response.UserLogin = userlogin;
            response.WebAPIName = "Get balance transaction details";
            response.Url = "api/Customer/Transaction/" + transactionID.ToString(); ;
            if (response.ResponseStatus != ResponseStatus.Success)
            {
                response.balanceTransaction = null;
            }
            return response;
        }

        //[Authorize]
        [AllowAnonymous]
        [Route("Products/Get")]
        [HttpGet]
        public RsProducts GetProducts()
        {
            var response = businessProvider.GetProducts();
            response.WebAPIName = "Get Products";
            response.Url = "api/Products/Get";
            return response;
        }

        //[Authorize]
        [AllowAnonymous]
        [Route("Product/{productID}/Subproducts")]
        [HttpGet]
        public RsSubProducts GetSubProducts(long productID)
        {
            var response = businessProvider.GetSubProducts(productID, null);
            response.WebAPIName = "Get Subproducts";
            response.Url = "api/Product/" + productID + "/Subproducts";
            return response;
        }

        //[Authorize]
        [AllowAnonymous]
        [Route("Common/Data")]
        [HttpGet]
        public RsCommonData GetCommonData()
        {
            var response = businessProvider.GetCommonData();
            response.WebAPIName = "Get Common Data";
            response.Url = "api/Common/Data";
            return response;
        }

        [Authorize]
        [Route("Product/Transaction/Save")]
        [HttpPost]
        public RsProductTransaction SaveProductTransaction(RqProductTransact request)
        {
            string userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = businessProvider.SaveProductTransaction(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Save product transaction";
            response.Url = "api/Product/Transaction/Save";
            if (response.ResponseStatus != ResponseStatus.Success)
            {
                response.transaction = null;
            }
            return response;
        }

        [Authorize]
        [Route("Product/Transaction/Delete")]
        [HttpPost]
        public RsCommon DeleteProductTransaction(RqDeleteTransaction request)
        {
            string userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = businessProvider.DeleteProductTransaction(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Delete product transaction";
            response.Url = "api/Product/Transaction/Delete";

            return response;
        }

        [Authorize]
        [Route("Product/Transaction/{transactionID}")]
        [HttpGet]
        public RsProductTransaction GetProductTransaction(long transactionID)
        {
            string userlogin = UserLogin;
            var response = businessProvider.GetProductTransactionDetail(transactionID, userlogin);
            response.UserLogin = userlogin;
            response.WebAPIName = "Get product transaction details";
            response.Url = "api/Product/Transaction/" + transactionID.ToString(); ;
            if (response.ResponseStatus != ResponseStatus.Success)
            {
                response.transaction = null;
            }
            return response;
        }

        [Authorize]
        [Route("LineSale/Add")]
        [HttpPost]
        public RsCommon AddLineSale(RqLineSale request)
        {
            string userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = businessProvider.AddLineSale(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Add line sale";
            response.Url = "api/linesale/add";
            return response;
        }


        [Authorize]
        [Route("LineSale/Tracking/Add")]
        [HttpPost]
        public RsCommon AddLineSaleTracking(RqLineSaleTracking request)
        {
            string userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = businessProvider.AddLineSaleTracking(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Add line sale tracking";
            response.Url = "api/linesale/tracking/add";
            return response;
        }

        [Authorize]
        [Route("LineSales/Get/Pending")]
        [HttpGet]
        public RsLineSale GetPendingLineSales()
        {
            string userlogin = UserLogin;
            var response = businessProvider.GetPendingLineSales(userlogin);
            response.UserLogin = userlogin;
            response.WebAPIName = "Get pending line sales";
            response.Url = "api/linesales/get/pending";
            return response;
        }

        [Authorize]
        [Route("LineSale/trackings/{linesaleID}")]
        [HttpGet]
        public RsLineSaleTrackings GetLineSaleTrackings(long linesaleID)
        {
            string userlogin = UserLogin;
            var response = businessProvider.GetLineSaleTrackings(linesaleID, userlogin);
            response.UserLogin = userlogin;
            response.WebAPIName = "Get pending line sale trackings";
            response.Url = "api/linesale/trackings/" + linesaleID.ToString();
            return response;
        }

        [Authorize]
        [Route("Notifications")]
        [HttpGet]
        public RsNotifications GetAllNotifications()
        {
            string userlogin = UserLogin;
            var response = businessProvider.GetNotifications();
            response.UserLogin = userlogin;
            response.WebAPIName = "Get all alerts";
            response.Url = "api/Notifications";
            return response;
        }

        [Authorize]
        [Route("Notifications/Cheque/Update")]
        [HttpPost]
        public RsCommon ChangeChequeStatus(RqChangeChequeStatus request)
        {
            string userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = businessProvider.ChangeChequeStatus(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Update cheque status";
            response.Url = "api/notifications/cheque/update";
            return response;
        }

        [Authorize]
        [Route("Search/Customer/Transactions")]
        [HttpPost]
        public RsSearchCustomerResult SearchCustomerTransactions(RqSearch request)
        {
            var userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = businessProvider.SearchBalanceTransaction(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Search Customer transactions";
            response.Url = "api/search/customer/transactions";
            response.pageIndex = request.pageIndex;
            response.pageSize = request.pageSize;
            response.isDescending = request.isDescending;
            return response;
        }

        [Authorize]
        [Route("Search/Customer/Transactions/Download")]
        [HttpGet]
        public HttpResponseMessage ExportToExcel()
        {
            var request = new RqSearch();
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["id"]))
            {
                request.ID = Convert.ToInt64(HttpContext.Current.Request.QueryString["id"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["startdate"]))
            {
                request.startDate = HttpContext.Current.Request.QueryString["startdate"];
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["enddate"]))
            {
                request.endDate = HttpContext.Current.Request.QueryString["enddate"];
            }
            var response = businessProvider.ExportToExcel(request);

            return response;
        }

        [Authorize]
        [Route("Search/Customers")]
        [HttpPost]
        public RsCustomers SearchCustomers(RqSearchCustomers request)
        {
            string userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = businessProvider.SearchCustomers(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Search Customers";
            response.Url = "api/search/customers";
            response.pageIndex = request.pageIndex;
            response.pageSize = request.pageSize;
            response.isDescending = request.isDescending;
            return response;
        }

        [Authorize]
        [Route("Products")]
        [HttpGet]
        public async Task<RsSubProducts> GetAllProducts()
        {
            string userlogin = UserLogin;
            var response = await businessProvider.GetAllProducts();
            response.UserLogin = userlogin;
            response.WebAPIName = "Get all products";
            response.Url = "api/products";
            return response;
        }

        [Authorize]
        [Route("TopCustomers")]
        [HttpGet]
        public async Task<RsCustomers> GetTopCustomers()
        {
            string userlogin = UserLogin;
            var response = await businessProvider.GetTopBuyingCustomers();
            response.UserLogin = userlogin;
            response.WebAPIName = "Get top customers";
            response.Url = "api/TopCustomers";
            return response;
        }

        [Authorize]
        [Route("Search/Product/Transactions")]
        [HttpPost]
        public async Task<RsProductTransactions> SearchProductTransactions(RqProductTransactionsSearch request)
        {
            var userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = await businessProvider.SearchProductTransactions(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Search product transactions";
            response.Url = "api/search/product/transactions";
            response.pageIndex = request.pageIndex;
            response.pageSize = request.pageSize;
            response.isDescending = request.isDescending;
            return response;
        }

        [Authorize]
        [Route("LineSale/Tracking/Delete")]
        [HttpPost]
        public async Task<RsCommon> DeleteLineSaleTracking(RqDeleteTransaction request)
        {
            string userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = await businessProvider.DeleteLineSaleTracking(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Delete line sale tracking";
            response.Url = "api/linesale/tracking/delete";
            return response;
        }

        [Authorize]
        [Route("LineSale/Delete")]
        [HttpPost]
        public async Task<RsCommon> DeleteLineSale(RqDeleteTransaction request)
        {
            string userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = await businessProvider.DeleteLineSale(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Delete line sale";
            response.Url = "api/linesale/delete";
            return response;
        }

        [Authorize]
        [Route("Search/Product/Transactions/Download")]
        [HttpGet]
        public HttpResponseMessage ExportToExcelProductTransactions()
        {
            var request = new RqProductTransactionsSearch();
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["productid"]))
            {
                request.productID = Convert.ToInt64(HttpContext.Current.Request.QueryString["productid"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["subproductid"]))
            {
                request.subProductID = Convert.ToInt64(HttpContext.Current.Request.QueryString["subproductid"]);
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["startdate"]))
            {
                request.startDate = HttpContext.Current.Request.QueryString["startdate"];
            }
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["enddate"]))
            {
                request.endDate = HttpContext.Current.Request.QueryString["enddate"];
            }
            var response = businessProvider.ExportToExcel(request);

            return response;
        }

        [Authorize]
        [Route("Linesales/DayReport")]
        [HttpPost]
        public async Task<RsLinesalesReport> GetLinesalesDayReport(RqSearch request)
        {
            var userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = await businessProvider.GetLinesalesReport(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Get linesales day report";
            response.Url = "api/linesales/dayreport";
            return response;
        }

        [Authorize]
        [Route("Suppliers")]
        [HttpGet]
        public async Task<RsSuppliers> GetSuppliers()
        {
            var userlogin = UserLogin;
            var response =await businessProvider.GetSuppliers();
            response.UserLogin = userlogin;
            response.WebAPIName = "Get suppliers";
            response.Url = "api/suppliers";
            return response;
        }

        [Authorize]
        [Route("Supplier/Tracking/Add")]
        [HttpPost]
        public async Task<RsCommon> AddSupplierTracking(RqSupplier request)
        {
            var userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = await businessProvider.AddSupplierTracking(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Add supplier tracking";
            response.Url = "api/supplier/tracking/add";
            return response;
        }

        [Authorize]
        [Route("Product/Add")]
        [HttpPost]
        public async Task<RsCommon> AddNewProduct(RqProduct request)
        {
            var userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = await businessProvider.AddNewProduct(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Add new product";
            response.Url = "api/product/add";
            return response;
        }

        [Authorize]
        [Route("SubProduct/Add")]
        [HttpPost]
        public async Task<RsCommon> AddNewSubProduct(RqSubproduct request)
        {
            var userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = await businessProvider.AddNewSubProduct(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Add new subproduct";
            response.Url = "api/subproduct/add";
            return response;
        }

        [Authorize]
        [Route("Supplier/Add")]
        [HttpPost]
        public async Task<RsCommon> AddNewSupplier(RqSupplierNew request)
        {
            var userlogin = UserLogin;
            request.UserLogin = userlogin;
            var response = await businessProvider.AddNewSupplier(request);
            response.UserLogin = userlogin;
            response.WebAPIName = "Add new supplier";
            response.Url = "api/supplier/add";
            return response;
        }

        [AllowAnonymous]
        [Route("AddDataTemplate")]
        [HttpPost]
        public bool AddDataTemplate(RqFile file)
        {
            return businessProvider.AddDataTemplateFile(file.filename);
        }
    }
}
