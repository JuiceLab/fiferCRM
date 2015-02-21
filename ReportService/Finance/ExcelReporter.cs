using CompanyContext;
using CompanyModel;
using CRMRepositories;
using ExtensionHelpers;
using FinanceModel;
using FinanceRepositories;
using OfficeOpenXml;
using Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Finance
{
    public class ExcelReporter
    {
        private Guid _userId { get; set; }

        public ExcelReporter(Guid userId)
        {
            _userId = userId;
        }

        public byte[] GetPayment(string templateSrc, int paymentId, byte type)
        {
            FileInfo template = new FileInfo(templateSrc);
            var tbl = new DataTable("Платежи");
            tbl.Columns.Add("№").DataType = typeof(int);
            tbl.Columns.Add("Наименование услуги");
            tbl.Columns.Add("Единица измерения");
            tbl.Columns.Add("Количество").DataType = typeof(int);
            tbl.Columns.Add("Цена").DataType = typeof(decimal);
            tbl.Columns.Add("Сумма").DataType = typeof(decimal);

            int n = 1;

            var total = 0m;

            FinanceBaseRepository repository = new FinanceBaseRepository(_userId);
            CRMLocalRepository localRepository = new CRMLocalRepository(_userId);
            PaymentEditModel model = repository.GetPaymentModel(paymentId, type);
            model.PaymentDetails = repository.GetDetails(paymentId);
            LegalEntityViewModel legalDetail = localRepository.GetLegalDetail(model.InvoicedInfo.LegalEnitityId);
            foreach (var row in model.PaymentDetails)
            {
                var cost = row.Cost * new decimal(row.Qty);
                tbl.Rows.Add(
                    n,
                    row.Name,
                    "шт.",
                    row.Qty,
                    row.Cost,
                    row.Cost * new decimal(row.Qty)
                    );

                total += cost;
                n++;
            }

            byte[] resultArray = null;
            using (CompanyViewEntities contextView = new CompanyViewEntities(AccessSettings.LoadSettings().CompanyViewsEntites))
            {
                using (CompanyEntities context = new CompanyEntities(AccessSettings.LoadSettings().CompanyEntites))
                {
                    var user = contextView.UserInCompanyView.FirstOrDefault(m => m.UserId == _userId);
                    var companyInfo = context.Companies.FirstOrDefault(m => m.CompanyId == user.CompanyId);
                    using (var pck = new ExcelPackage(template, true))
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets[1];

                        const int startRow = 25;

                        ws.Cells["D1"].Value = model.InvoicedInfo.CompanyLegalInfo.LegalName;
                        ws.Cells["B3"].Value = "тел.:" + model.InvoicedInfo.CompanyLegalInfo.Phones;
                        ws.Cells["B6"].Value = "ИНН " + legalDetail.INN;
                        ws.Cells["C6"].Value = "КПП " + legalDetail.KPP;
                        ws.Cells["C7"].Value = model.InvoicedInfo.CompanyLegalInfo.LegalName;
                        ws.Cells["G8"].Value = legalDetail.RS;
                        ws.Cells["C9"].Value = "";
                        ws.Cells["G9"].Value = "";
                        ws.Cells["G10"].Value = "";
                        ws.Cells["B13"].Value = string.Format("СЧЕТ № {0} от {1} {2} {3}г.", paymentId,
                            model.InvoicedInfo.Created.Day,
                            CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames[model.InvoicedInfo.Created.Month - 1],
                            model.InvoicedInfo.Created.Year);

                        ws.Cells["B15"].Value = string.Format("Исполнитель:  ИНН {0} КПП {1} {2} {3}   тел.: {4}", legalDetail.INN, legalDetail.KPP, model.InvoicedInfo.CompanyLegalInfo.LegalName, legalDetail.PaymentLocation, model.InvoicedInfo.CompanyLegalInfo.Phones);

                        ws.Cells["B18"].Value = string.Format("Заказчик: {0} ИНН {1} КПП {2}", companyInfo.CompanyName, companyInfo.LegalEntity.INN, companyInfo.LegalEntity.KPP);

                        ws.Cells["G27"].Value = total;
                        ws.Cells["B29"].Value = "всего наименований " + n + ", на сумму";
                        ws.Cells["B30"].Value = RusNumbersHelper.RurPhrase(total);
                        var companyHead =  contextView.UserInCompanyView.FirstOrDefault(m => m.UserId == companyInfo.CreatedBy);
                        ws.Cells["B32"].Value = string.Format("Директор  _____________________   ( {0} {1}.{2}.)", companyHead.FirstName, companyHead.LastName.ToUpper().FirstOrDefault(),string.IsNullOrEmpty(companyHead.Patronymic)? new char() : companyHead.Patronymic.ToUpper().FirstOrDefault());


                        var style = ws.Row(25).Style;

                        ws.InsertRow(25, tbl.Rows.Count, 17);
                        ws.Cells["B25"].LoadFromDataTable(tbl, false);
                        ws.DeleteRow(startRow + tbl.Rows.Count, 1);
                        resultArray = pck.GetAsByteArray();
                    }
                }
            }
            return resultArray;
        }
    }
}
