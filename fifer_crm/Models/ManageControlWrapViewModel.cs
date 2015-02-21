using AccessRepositories;
using CompanyContext;
using CompanyModel;
using CompanyRepositories;
using CRMModel;
using CRMRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskModel.Ticket;
using TicketRepositories;

namespace fifer_crm.Models
{
    public class ManageControlWrapViewModel
    {
        public Guid UserId { get; set; }
      
        public IEnumerable<SelectListItem> Categories { get; set; }

        public ManageControlWrapViewModel()
        {
            UserId = Guid.Empty;
        }

        public ManageControlWrapViewModel(Guid userId, bool onlyCRM = false)
        {
            UserId = userId;
            CRMLocalRepository crmRepository = new CRMLocalRepository(UserId);
            Categories = crmRepository.GetTaskCatergories();

        }
    }
}
