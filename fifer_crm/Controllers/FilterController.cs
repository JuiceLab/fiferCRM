using FilterModel;
using FinanceRepositories;
using Interfaces.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Controllers
{
    public class FilterController : BaseFiferController
    {
        // GET: Filter
        public ActionResult Finance(FinanceFilter filter)
        {
            FinanceBaseRepository repository = new FinanceBaseRepository(_userId);
            IFinanceStatInteractive financeStat = repository.GetFinanceStatInteractive(_userId, filter);

            return PartialView("FinanceFiltredContent", financeStat);
        }
    }
}