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
    public class FullControlWrapViewModel
    {
        public Guid UserId { get; set; }
        public IEnumerable<TicketPreview> AcctualTickets { get; set; }
        public IEnumerable<CompanyPreview> Companies { get; set; }

        public IEnumerable<SelectListItem> Services { get; set; }
        public IEnumerable<CityPreview> Cities { get; set; }

        public FullControlWrapViewModel()
        {
            UserId = Guid.Empty;
        }

        public FullControlWrapViewModel(Guid userId, bool onlyCRM = false)
        {
            UserId = userId;
            SupportTicketRepository repository = new SupportTicketRepository();
            AcctualTickets = repository.GetAcctualTickets();
            

            CompanyRepository companyRepository = new CompanyRepository();
            CRMRepository crmRepository = new CRMRepository();
            Services = crmRepository.GetServices(null, true);

            if (!onlyCRM)
            {
                Companies = companyRepository.GetCompanyPreviews();
                var existCompanies = Companies.Where(m => m.CompanyGuid.HasValue).Select(m => m.CompanyGuid.Value);
                Companies = Companies.Union(crmRepository.GetCompanyPreviews(existCompanies));
            }
            else
            {
                Companies = crmRepository.GetCompanyPreviews(new List<Guid>());
            }
           Cities = crmRepository.GetCities();
           var ids = AcctualTickets.Select(m => m.CreatedById).Distinct().ToList();
           ids.AddRange(Companies.Select(m => m.OwnerId));

           MembershipRepository membershipRepository = new MembershipRepository();
           var names =  membershipRepository.GetUserByIds(ids);
           foreach (var item in AcctualTickets)
           {
               item.CreatedBy = names.FirstOrDefault(m => m.UserId == item.CreatedById).Login;
           }

           foreach (var item in Companies)
           {
               var owner = names.FirstOrDefault(m => m.UserId == item.OwnerId);
               item.OwnerName = owner !=null? string.Format("{0} {1}", owner.FirstName, owner.LastName) : string.Empty;
           }
        }
    }
}
