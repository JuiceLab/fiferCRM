using CompanyContext;
using CRMLocalContext;
using EnumHelper.Payment;
using FinanceModel;
using Interfaces.Finance;
using Settings;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumHelper;
using FilterModel;
using EntityRepository;
using CRMModel;
using System.Web.Mvc;
using TaskModel;

namespace FinanceRepositories
{
    public class FinanceBaseRepository : IDisposable
    {
        private IFormatProvider provider = new CultureInfo("ru-RU").DateTimeFormat;

        protected const string _defaultLocalDB = "fifer_localcrm_0";

        public LocalCRMEntities LocalContext { get; set; }

        public FinanceBaseRepository(Guid userId)
        {
            using (CompanyContext.CompanyViewEntities viewContext = new CompanyContext.CompanyViewEntities(AccessSettings.LoadSettings().CompanyViewsEntites))
            {
                var localDb = viewContext.UserInCompanyView.FirstOrDefault(m => m.UserId == userId).LocalDB;
                LocalContext = new LocalCRMEntities(AccessSettings.LoadSettings().LocalCrmEntites.Replace(_defaultLocalDB, localDb));
            }
        }

        public void Dispose()
        {
            LocalContext.Database.Connection.Close();
            LocalContext.Dispose();
        }

        public void CreatePaymentInvoice(Guid[] ids, string baseAppend, PaymentEditModel model)
        {

            using (LocalCRMEntities localContext = new LocalCRMEntities(AccessSettings.LoadSettings().LocalCrmEntites.Replace(_defaultLocalDB, baseAppend)))
            {
                // set todo paymentDetailsChoice
                var payment = new PaymentInvoice()
                {
                    IsCash = model.IsCash,
                    PaymentBefore = !string.IsNullOrEmpty(model.PayBeforeInvariant) ?
                       Convert.ToDateTime(model.PayBeforeInvariant, provider)
                       : DateTime.Now.Date.AddDays(1),
                    Comment = model.Comment,
                    Created = DateTime.Now,
                    CreatedBy = model.OwnerId,
                    TotalValue = model.PaymentDetails.Sum(m => Convert.ToDecimal(m.Qty) * m.Cost),
                    StatusId = (byte)PaymentStatus.Novelty,
                    PaymentNumber = GenerateNumber("INV-", 8),
                    FromEmployee = ids.ElementAt(0),
                    FromCompany = ids.ElementAt(1),
                    C_FromLegalEntityDetailID = LocalContext.Customers
                       .FirstOrDefault(m => m.CustomerGuid == model.InvoicedInfo.CustomerInfo.UserId)
                       .LegalEntity.LegalEnityDetails.FirstOrDefault().C_LegalEntityId
                };
                localContext.InsertUnit(payment);
                localContext.InsertUnits(model.PaymentDetails.Select(m => new PaymentDetail()
                {
                    ItemCost = m.Cost,
                    C_InvoiceId = payment.PaymentInvoiceId,
                    Qty = m.Qty,
                    ServiceFullName = m.Name
                }));
            }
        }

        public IFinanceStatInteractive GetFinanceStatInteractive(Guid _userId, IFinanceFilter filter)
        {

            var today = DateTime.Now.Date;

            IFinanceStatInteractive model = new FinanceStatInteractive();
            model.FinanceFilter = filter != null && filter.DateRange != null ? filter : new FinanceFilter();
            var before = model.FinanceFilter.DateRange.ElementAt(0);
            var after = model.FinanceFilter.DateRange.ElementAt(1);
            model.Transaction = LocalContext
                .Payments.Where(m => m.PaymentBefore >= before && m.PaymentBefore <= after)
                .ToList()
                .Select(m => new PaymentViewModel()
                {
                    Type = m.TotalValue > 0 ? (byte)PaymentType.Payment : (byte)PaymentType.Expense,
                    IsCash = m.IsCash,
                    PaymentId = m.PaymentId,
                    TransactionId = m.PaymentId,
                    Status = ((PaymentStatus)m.StatusId).GetStringValue(),
                    Number = m.PaymentNumber,
                    Total = m.TotalValue,
                    IsExpired = m.SubmitedDate < today,
                    Comment = m.Comment,
                    PayBefore = m.PaymentBefore,
                    Title = string.Empty
                }).ToList()
                .Union(
                LocalContext
                .PaymentInvoices.Where(m => m.PaymentBefore >= before && m.PaymentBefore <= after)
                .ToList()
                .Select(m => new PaymentViewModel()
                {
                    Type = (byte)PaymentType.PaymentInvoice,
                    IsCash = m.IsCash,
                    PaymentId = m.PaymentInvoiceId,
                    TransactionId = m.PaymentInvoiceId,
                    Status = ((PaymentStatus)m.StatusId).GetStringValue(),
                    Number = m.PaymentNumber,
                    Total = m.TotalValue,
                    IsExpired = m.SubmitedDate < today,
                    Comment = m.Comment,
                    PayBefore = m.PaymentBefore,
                    Title = string.Empty
                }).ToList())
                .GroupBy(m => m.PayBefore.Date)
                .ToDictionary(m => m.Key, m => m.Cast<IPayment>());

            return model;
        }

        public IEnumerable<PaymentViewModel> GetPayments(DateTime dateBefore, Guid? companyId = null)
        {
            var today = DateTime.Now.Date;
            var items = LocalContext
                  .Payments.Where(m => m.PaymentBefore >= dateBefore);
            if (companyId.HasValue)
                items = items.Where(m => m.LegalEnityDetail.LegalEntity.CompanyGuid == companyId.Value);
            return items
                  .ToList()
                  .Select(m => new PaymentViewModel()
                  {
                      Type = m.TotalValue > 0 ? (byte)PaymentType.Payment : (byte)PaymentType.Expense,
                      IsCash = m.IsCash,
                      PaymentId = m.PaymentId,
                      TransactionId = m.PaymentId,
                      Status = ((PaymentStatus)m.StatusId).GetStringValue(),
                      Number = m.PaymentNumber,
                      Total = m.TotalValue,
                      IsExpired = m.SubmitedDate < today,
                      Comment = m.Comment,
                      PayBefore = m.PaymentBefore,
                      Title = string.Empty,
                      CompanyGuid = m.LegalEnityDetail.LegalEntity.CompanyGuid
                  }).ToList();
        }

        public PaymentEditModel GetPaymentModel(int? paymentId, byte type, int? companyId = null)
        {
            PaymentEditModel model = new PaymentEditModel()
            {
                Type = type
            };

            var customerId = Guid.Empty;

            if (companyId.HasValue)
            {
                var payment = LocalContext.Payments
                    .Where(m=>m.StatusId != (byte)PaymentStatus.Payed && m.StatusId != (byte)PaymentStatus.ActClosed
                     && m.LegalEnityDetail.C_LegalEntityId == companyId)
                    .OrderBy(m=>m.PaymentBefore)
                    .FirstOrDefault();
                if (payment != null)
                {
                    model.IsCash = payment.IsCash;
                    model.OwnerId = payment.CreatedBy;
                    model.Total = payment.TotalValue;
                    model.PayBefore = payment.PaymentBefore;
                    model.PayBeforeInvariant = payment.PaymentBefore.ToString("dd.MM.yyyy");
                    model.Comment = payment.Comment;
                    model.Number = payment.PaymentNumber;
                    model.TransactionId = payment.PaymentId;
                    model.TotalSubmitted = payment.TotalPayed.HasValue ? payment.TotalPayed.Value : payment.TotalValue;
                    model.PaymentId = payment.PaymentId;
                    model.Type = type;
                    model.StatusId = payment.StatusId;
                    model.Status = ((PaymentStatus)payment.StatusId).GetStringValue();
                    model.InvoicedInfo = new CompanyPaymentInfoEdit()
                    {
                        LegalEnitityId = payment.C_LegalEntityDetailId,
                        From = payment.CreatedBy,
                        Created = payment.Created
                    };
                    customerId = payment.Recipient;
                }
            }
            else if (paymentId.HasValue)
            {
                switch ((PaymentType)type)
                {
                    case PaymentType.Payment:
                    case PaymentType.Expense:
                        var payment = LocalContext.GetUnitById<Payment>(paymentId.Value);
                        model.IsCash = payment.IsCash;
                        model.OwnerId = payment.CreatedBy;
                        model.Total = payment.TotalValue;
                        model.PayBefore = payment.PaymentBefore;
                        model.PayBeforeInvariant = payment.PaymentBefore.ToString("dd.MM.yyyy");
                        model.Comment = payment.Comment;
                        model.Number = payment.PaymentNumber;
                        model.TransactionId = payment.PaymentId;
                        model.TotalSubmitted = payment.TotalPayed.HasValue ? payment.TotalPayed.Value : payment.TotalValue;
                        model.PaymentId = payment.PaymentId;
                        model.Type = type;
                        model.StatusId = payment.StatusId;
                        model.Status = ((PaymentStatus)payment.StatusId).GetStringValue();
                        model.InvoicedInfo = new CompanyPaymentInfoEdit()
                        {
                            LegalEnitityId = payment.C_LegalEntityDetailId,
                            From = payment.CreatedBy,
                            Created = payment.Created
                        };
                        customerId = payment.Recipient;
                        break;
                    case PaymentType.PaymentInvoice:
                        var paymentInvoice = LocalContext.GetUnitById<PaymentInvoice>(paymentId.Value);
                        model.IsCash = paymentInvoice.IsCash;
                        model.OwnerId = paymentInvoice.CreatedBy;
                        model.Total = paymentInvoice.TotalValue;
                        model.PayBefore = paymentInvoice.PaymentBefore;
                        model.PayBeforeInvariant = paymentInvoice.PaymentBefore.ToString("dd.MM.yyyy");
                        model.Comment = paymentInvoice.Comment;
                        model.Number = paymentInvoice.PaymentNumber;
                        model.TransactionId = paymentInvoice.PaymentInvoiceId;
                        model.PaymentId = paymentInvoice.PaymentInvoiceId;
                        model.TotalSubmitted = paymentInvoice.TotalPayed.HasValue ? paymentInvoice.TotalPayed.Value : paymentInvoice.TotalValue;
                        model.Type = type;
                        model.StatusId = paymentInvoice.StatusId;
                        model.Status = ((PaymentStatus)paymentInvoice.StatusId).GetStringValue();
                        model.InvoicedInfo = new CompanyPaymentInfoEdit()
                        {
                            LegalEnitityId = paymentInvoice.C_FromLegalEntityDetailID,
                            From = paymentInvoice.CreatedBy,
                            Created = paymentInvoice.Created
                        };
                        customerId = paymentInvoice.FromEmployee;
                        break;
                }
            }
            if(model.PaymentId !=0)
            {
                var customer = LocalContext.Customers.FirstOrDefault(m => m.CustomerGuid == customerId);
                if (customer != null)
                {
                    model.InvoicedInfo.FromCompany = customer.C_LegalEntityId.HasValue ?
                        new Nullable<Guid>(customer.LegalEntity.CompanyGuid)
                        : null;
                    model.InvoicedInfo.CustomerInfo = new CustomerEditModel()
                        {
                            Comment = customer.Comment,
                            CustomerId = customer.CustomerId,
                            CompanyId = customer.C_LegalEntityId.HasValue ? customer.LegalEntity.CompanyGuid : Guid.Empty,
                            FirstName = customer.FirstName,
                            LastName = customer.LastName,
                            Mail = customer.Mail,
                            Phone = customer.Phone,
                            PositionName = customer.Position,
                            UserId = customer.CustomerGuid

                        };
                    model.InvoicedInfo.CompanyLegalInfo = customer.C_LegalEntityId.HasValue ?
                        new CRMCompanyEditModel()
                        {
                            LegalEntityId = customer.LegalEntity.LegalEntityId,
                            LegalName = customer.LegalEntity.PublicName,
                            Phones = customer.LegalEntity.Phones,
                            Mails = customer.LegalEntity.Mails,
                            Address = model.InvoicedInfo.LegalEnitityId.HasValue ?
                            LocalContext.GetUnitById<LegalEnityDetail>(model.InvoicedInfo.LegalEnitityId.Value).GeoLocation.Address
                            : null
                        }
                        : null;
                }
            }
            return model;
        }

        public FinanceClause GetFinanceClause()
        {
            FinanceClause model = new FinanceClause();
            model.PaymentServices = LocalContext.LegalActivities.Select(m => new ActivityServiceItem()
            {
                ActivityId = m.LegalActivityId,
                ActivityName = m.CompanyService.CompanyService2.Name + " " + m.CompanyService.Name,
                Cost = m.Cost,
                Tax = m.EmployeePercentBonus,
                Name = m.ActivityFullName
            });
            model.Expenses = LocalContext.Expenses.ToList()
                .Select(m => new SelectListItem()
                {
                    Text = m.Name,
                    Value = m.ExpenseId.ToString()
                }).ToList();
            return model;
        }

        public ActivityServiceItem GetActivityService(int? serviceId)
        {
            ActivityServiceItem model = new ActivityServiceItem();
            if (serviceId.HasValue)
            {
                var existService = LocalContext.GetUnitById<LegalActivity>(serviceId.Value);
                model.Name = existService.ActivityFullName;
                model.ActivityName = existService.ActivityFullName;
                model.Cost = existService.Cost;
                model.Tax = existService.EmployeePercentBonus;
                model.ActivityId = existService.LegalActivityId;
            }

            return model;
        }

        public void UpdateActivityService(ActivityServiceItem model, Guid? companyId)
        {
            if (model.ActivityId != 0)
            {
                var existService = LocalContext.GetUnitById<LegalActivity>(model.ActivityId);
                existService.ActivityFullName = model.ActivityName;
                existService.Cost = model.Cost;
                existService.EmployeePercentBonus = model.Tax;
                LocalContext.SaveChanges();
            }
            else
            {
                var existService = LocalContext.CompanyServices.FirstOrDefault(m => m.CompanyServiceId == model.ParentId);
                LocalContext.InsertUnit(new LegalActivity()
                {
                    ActivityFullName = existService.Name + " " + model.Name.Trim(),
                    Cost = model.Cost,
                    EmployeePercentBonus = model.Tax,
                    C_ServiceId = model.ParentId.Value,
                    C_LegalEntityId = LocalContext.LegalEnityDetails.FirstOrDefault(m => m.LegalEntity.CompanyGuid == companyId).C_LegalEntityId
                });
            }
        }

        public SelectListItem GetExpense(int? expenseId)
        {
            SelectListItem model = new SelectListItem();
            if (expenseId.HasValue)
            {
                var expense = LocalContext.GetUnitById<Expense>(expenseId.Value);
                model.Text = expense.Name;
                model.Value = expense.ExpenseId.ToString();
            }
            return model;
        }

        public void UpdateExpense(SelectListItem model)
        {
            if (!string.IsNullOrEmpty(model.Value))
            {
                var expense = LocalContext.GetUnitById<Expense>(int.Parse(model.Value));
                expense.Name = model.Text;
                LocalContext.SaveChanges();
            }
            else
            {
                LocalContext.InsertUnit(new Expense()
                {
                    Name = model.Text
                });
            }
        }

        public void UpdatePaymentModel(PaymentEditModel model)
        {
            if (model.PaymentId == 0)
            {
                // set todo paymentDetailsChoice
                var payment = new Payment()
                {
                    IsCash = model.IsCash,
                    PaymentBefore = !string.IsNullOrEmpty(model.PayBeforeInvariant) ?
                       Convert.ToDateTime(model.PayBeforeInvariant, provider)
                       : DateTime.Now.Date.AddDays(1),
                    Comment = model.Comment,
                    Created = DateTime.Now,
                    CreatedBy = model.OwnerId,
                    TotalValue = model.PaymentDetails.Sum(m => Convert.ToDecimal(m.Qty) * m.Cost),
                    StatusId = (byte)PaymentStatus.Novelty,
                    PaymentNumber = GenerateNumber("PMT-", 8),
                    Recipient = model.InvoicedInfo.CustomerInfo.UserId,
                    C_LegalEntityDetailId = LocalContext.Customers
                        .FirstOrDefault(m => m.CustomerGuid == model.InvoicedInfo.CustomerInfo.UserId)
                        .LegalEntity.LegalEnityDetails.FirstOrDefault().C_LegalEntityId
                };
                LocalContext.InsertUnit(payment);
                LocalContext.InsertUnits(model.PaymentDetails.Select(m => new PaymentDetail()
                {
                    ItemCost = m.Cost,
                    C_PaymentId = payment.PaymentId,
                    Qty = m.Qty,
                    ItemTaxPercent = m.HasNDS ? new Nullable<double>(0.18) : null,
                    ServiceFullName = m.Name
                }));
            }
            else
            {
                var exist = LocalContext.GetUnitById<Payment>(model.PaymentId);
                exist.IsCash = model.IsCash;
                exist.PaymentBefore = string.IsNullOrEmpty(model.PayBeforeInvariant) ?
                   Convert.ToDateTime(model.PayBeforeInvariant, provider)
                   : DateTime.Now.Date.AddDays(1);
                exist.Comment = model.Comment;
                exist.StatusId = model.StatusId;
                exist.Recipient = model.InvoicedInfo.CustomerInfo.UserId;
                LocalContext.SaveChanges();
            }
        }

        public void ChangePaymentModelState(PaymentEditModel model)
        {
            UpdatePaymentModel(model);

            var exist = LocalContext.GetUnitById<Payment>(model.PaymentId);
            if (model.StatusId == (byte)PaymentStatus.WaitPays)
            {
                exist.TotalPayed = model.TotalSubmitted.HasValue ? model.TotalSubmitted.Value : exist.TotalValue;
                exist.SubmitedBy = model.OwnerId;
                exist.SubmitedDate = DateTime.Now;
                LocalContext.SaveChanges();
            };
        }

        public IList<PaymentDetailViewModel> GetDetails(int? paymentId)
        {
            if (!paymentId.HasValue)
                return new List<PaymentDetailViewModel>();

            return LocalContext.PaymentDetails.Where(m => m.C_PaymentId == paymentId)
                .Select(m => new PaymentDetailViewModel()
                {
                    ItemId = m.PaymentInvoiceDetailId,
                    Cost = m.ItemCost,
                    Name = m.ServiceFullName,
                    Qty = m.Qty,
                    Tax = m.ItemTaxPercent
                }).ToList();
        }

        public PaymentDetailViewModel GetServiceDetail(int serviceId, int qty)
        {
            var service = LocalContext.GetUnitById<LegalActivity>(serviceId);
            PaymentDetailViewModel model = new PaymentDetailViewModel()
                {
                    Name = service.ActivityFullName,
                    Cost = service.Cost.HasValue ? service.Cost.Value : 0,
                    Qty = qty,
                    Total = service.Cost.HasValue ? service.Cost.Value * qty : 0
                };
            return model;
        }

        private string GenerateNumber(string prefix, int qty)
        {
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder(prefix);
            do
            {
                for (int i = 0; i < qty; i++)
                {
                    sb.Append(rnd.Next(0, 9));
                }
                var val = sb.ToString();
                if (LocalContext.Payments.Any(m => m.PaymentNumber == val) || LocalContext.PaymentInvoices.Any(m => m.PaymentNumber == val))
                    sb = new StringBuilder();
            } while (sb.Length == 0);
            return sb.ToString();
        }

        public void UpdateExpenseModel(PaymentEditModel model)
        {
            if (model.PaymentId == 0)
            {
                // set todo paymentDetailsChoice
                var payment = new Payment()
                {
                    IsCash = model.IsCash,
                    PaymentBefore = !string.IsNullOrEmpty(model.PayBeforeInvariant) ?
                       Convert.ToDateTime(model.PayBeforeInvariant, provider)
                       : DateTime.Now.Date.AddDays(1),
                    Comment = model.Comment,
                    Created = DateTime.Now,
                    CreatedBy = model.OwnerId,
                    TotalValue = -1 * model.PaymentDetails.Sum(m => Convert.ToDecimal(m.Qty) * m.Cost),
                    StatusId = (byte)PaymentStatus.Payed,
                    Recipient = Guid.Empty,
                    PaymentNumber = GenerateNumber("EXP-", 8),

                };
                LocalContext.InsertUnit(payment);
                LocalContext.InsertUnits(model.PaymentDetails.Select(m => new PaymentDetail()
                {
                    ItemCost = m.Cost,
                    C_PaymentId = payment.PaymentId,
                    Qty = m.Qty,
                    ItemTaxPercent = m.HasNDS ? new Nullable<double>(0.18) : null,
                    ServiceFullName = m.Name
                }));
            }
            else
            {
                var exist = LocalContext.GetUnitById<Payment>(model.PaymentId);
                exist.IsCash = model.IsCash;
                exist.PaymentBefore = string.IsNullOrEmpty(model.PayBeforeInvariant) ?
                   Convert.ToDateTime(model.PayBeforeInvariant, provider)
                   : DateTime.Now.Date.AddDays(1);
                exist.Comment = model.Comment;
                exist.StatusId = model.StatusId;
                exist.Recipient = model.InvoicedInfo.CustomerInfo.UserId;
                LocalContext.SaveChanges();
            }
        }

        public Dictionary<Guid, List<CompletionActModel>> GetCompletionActs(DateTime dateTime, Guid? companyId = null)
        {
            var items = LocalContext.CompletionActs
                .Where(m => m.Created > dateTime);
            if (companyId.HasValue)
                items = items
                    .Where(m => m.CompanyGuid == companyId);
            return items
                .Select(m => new CompletionActModel()
            {
                ActId = m.CompletionActId,
                CompanyId = m.CompanyGuid,
                Comment = m.Comment,
                Created = m.Created,
                StrPayments = m.Payments,
                Total = m.TotalValue,
            })
            .OrderByDescending(m => m.Created)
            .GroupBy(m => m.CompanyId)
            .ToDictionary(m => m.Key, m => m.ToList());
        }

        public CompletionActEditModel GetCompletionAct(int? actId)
        {
            CompletionActEditModel model = new CompletionActEditModel();
            if (actId.HasValue)
            {
                var existAct = LocalContext.GetUnitById<CompletionAct>(actId.Value);
                model.ActId = actId.Value;
                model.Comment = existAct.Comment;
                model.Total = existAct.TotalValue;
                model.Created = existAct.Created;
                model.CompanyId = existAct.CompanyGuid;
                model.StrPayments = existAct.Payments;
                model.DealDate = existAct.DealDate;
                model.DealNumber = existAct.DealNumber;
                model.Title = existAct.Title;
                model.CustomerGuid = existAct.CustomerGuid;
            }
            return model;
        }

        public void AddOrUpdateAct(CompletionActEditModel model, Guid userId)
        {
            var payments = LocalContext.Payments.Where(m => model.Payments.Contains(m.PaymentId));
            if (model.ActId == 0)
            {
                LocalContext.InsertUnit(new CompletionAct()
                {
                    Created = DateTime.Now,
                    Comment = model.Comment,
                    CompanyGuid = model.CompanyId,
                    CreatedBy = userId,
                    DealDate = model.DealDate,
                    DealNumber = model.DealNumber,
                    Title = model.Title,
                    TotalValue = payments.Where(m=>m.PaymentDetails.Count > 0 ).SelectMany(m => m.PaymentDetails).ToList().Sum(m => new decimal(m.Qty) * m.ItemCost),
                    Payments = model.StrPayments,
                    CustomerGuid = model.CustomerGuid
                });
            }
            else
            {
                var existAct = LocalContext.GetUnitById<CompletionAct>(model.ActId);
                existAct.TotalValue = payments.SelectMany(m => m.PaymentDetails).ToList().Sum(m => new decimal(m.Qty) * m.ItemCost);
                existAct.Payments = model.StrPayments;
                existAct.CustomerGuid = model.CustomerGuid;
            }

            foreach (var item in payments)
            {
                item.StatusId = (byte)PaymentStatus.ActClosed;
            }

            LocalContext.SaveChanges();
        }

        public List<PaymentViewModel> GetPaymentsByIds(List<int> list, Guid? companyId = null, bool isPayed = true)
        {
            var today = DateTime.Now.Date;
            var items = LocalContext
                  .Payments.Where(m => list.Contains(m.PaymentId));
            if (companyId.HasValue)
                if (isPayed)
                {
                    items = items.Union(LocalContext
                      .Payments.Where(m => m.LegalEnityDetail.LegalEntity.CompanyGuid == companyId.Value && m.StatusId == (byte)PaymentStatus.Payed));
                }
                else { 
                     items = items.Union(LocalContext
                      .Payments.Where(m => m.LegalEnityDetail.LegalEntity.CompanyGuid == companyId.Value && m.StatusId != (byte)PaymentStatus.Payed && m.StatusId != (byte)PaymentStatus.ActClosed));
                     
                }
            return items
                   .ToList()
                   .Select(m => new PaymentViewModel()
                   {
                       Type = m.TotalValue > 0 ? (byte)PaymentType.Payment : (byte)PaymentType.Expense,
                       IsCash = m.IsCash,
                       PaymentId = m.PaymentId,
                       TransactionId = m.PaymentId,
                       Status = ((PaymentStatus)m.StatusId).GetStringValue(),
                       Number = m.PaymentNumber,
                       Total = m.TotalValue,
                       IsExpired = m.SubmitedDate < today,
                       Comment = m.Comment,
                       PayBefore = m.PaymentBefore,
                       Title = m.LegalEnityDetail.LegalEntity.LegalName,
                       TotalSubmitted = m.TotalPayed.HasValue ? m.TotalPayed.Value : m.TotalValue,
                       CompanyGuid = m.LegalEnityDetail.LegalEntity.CompanyGuid
                   }).ToList();
        }


        public Dictionary<Guid, int> GetLastPayments(IEnumerable<Guid> companies)
        {
          return  LocalContext.Payments.Where(m => m.StatusId != (byte)PaymentStatus.ActClosed && m.StatusId != (byte)PaymentStatus.Payed)
                    .Select(m => new { id = m.LegalEnityDetail.LegalEntity.CompanyGuid, paymentId = m.PaymentId, date = m.PaymentBefore })
                    .GroupBy(m => m.id)
                    .ToDictionary(m => m.Key, m => m.FirstOrDefault().paymentId);
        }

        public List<MessageViewModel> GetPaymentHistory(Guid companyId)
        {
            return new List<MessageViewModel>();
        }
    }
}
