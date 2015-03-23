using CompanyContext;
using CompanyModel;
using CompanyRepositories;
using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskModel;

namespace fifer_crm.Models
{
    public class CompanyWrapViewModel : ICompanyInfo
    {
        public Guid UserId { get; set; }
        public string CompanyName { get; set; }
        public Dictionary<string, IEnumerable<IMenuItem>> Menu { get; set; }
        public CompanyViewModel Company { get; set; }
        public IEnumerable<PositionViewModel> Positions { get; set; }
        public IEnumerable<DepartmentViewModel> Departments { get; set; }
        public IEnumerable<WorkGroupViewModel> WorkGroups { get; set; }
        public IEnumerable<EmployeeViewModel> Employees { get; set; }
        
        public IEnumerable<TimeSheetViewModel> TimeSheetEmployees { get; set; }

        public IEnumerable<MessageViewModel> EmployeesActions { get; set; }
        public string GetPositionName(int id) {
            return Positions.FirstOrDefault(m => m.PosId == id).Name;
        }

        public CompanyWrapViewModel()
        {
            UserId = Guid.Empty;
        }

        public CompanyWrapViewModel(Guid userId)
        {
            UserId = userId;
            CompanyRepository repository = new CompanyRepository();
            Company = repository.GetShortCompanyInfoByUserId(userId);
        }

        public int CompanyId { get { return Company.CompanyId; } set { Company.CompanyId = value; } }

        public string Logo { get { return Company.Logo; } set { Company.Logo = value; } }

        public string Name { get { return Company.PublicCompanyName; } set { Company.PublicCompanyName = value; } }

        public string UserPhoto { get { return Company.UserPhoto; } set { Company.UserPhoto = value; } }

    }
}
