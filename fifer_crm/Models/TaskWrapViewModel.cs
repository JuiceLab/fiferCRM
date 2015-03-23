﻿using AccessRepositories;
using CompanyModel;
using CompanyRepositories;
using CRMRepositories;
using FilterModel;
using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskModel;
using TaskModel.Task;
using TaskModel.Ticket;
using TicketRepositories;

namespace fifer_crm.Models
{
    public class TaskWrapViewModel : ICompanyInfo
    {
        public int CompanyId { get { return Company.CompanyId; } set { Company.CompanyId = value; } }

        public string Logo { get { return Company.Logo; } set { Company.Logo = value; } }

        public string Name { get { return Company.PublicCompanyName; } set { Company.PublicCompanyName = value; } }
        public string UserPhoto { get { return Company.UserPhoto; } set { Company.UserPhoto = value; } }

        public CompanyViewModel Company { get; set; }
        public IEnumerable<TicketPreview> SupportTickets { get; set; }

        public TaskSearchFilter SearchFilter { get; set; }
        public IEnumerable<MessageViewModel> Notifies{ get; set; }

        public Dictionary<string, IEnumerable<IMenuItem>> Menu { get; set; }

        public TaskWrapViewModel(Guid userId, bool onlySupport = false, EnumHelper.TaskStatus? status = null)
        {
            CompanyRepository companyRepository = new CompanyRepository();
            Company = companyRepository.GetShortCompanyInfoByUserId(userId);

            if (onlySupport)
            {
                SupportTicketRepository repository = new SupportTicketRepository();
                SupportTickets = repository.GetCustomerTickets(userId);
            }
            else
            {
                StaffRepository staffRepository = new StaffRepository();
                var users = staffRepository.GetSubordinatedUsers(userId);

                TaskTicketRepository _repository = new TaskTicketRepository();
                SearchFilter = new TaskSearchFilter()
                {
                    SearchResult = _repository.GetTaskTickets(users.Select(m => Guid.Parse(m.Value)), true , status != null? new List<byte>(){ (byte)status.Value} : new List<byte>()) ,
                    AssignedAvailable = users.ToList()
                };

                CRMLocalRepository localRepository = new CRMLocalRepository(userId);
                var periodTickets = localRepository.FindPeriodTickets(SearchFilter.SearchResult.Select(m => m.TaskId));
                foreach (var item in SearchFilter.SearchResult.Where(m=>periodTickets.Contains(m.TaskId) ))
                {
                    item.HasPeriod = true;
                }

                SearchFilter.AssignedAvailable.Insert(0, new SelectListItem() { Text = "Не закреплены за сотрудинками" });
                SearchFilter.AssignedAvailable.Insert(0, new SelectListItem() { Value = Guid.Empty.ToString(), Text = "Все" });

                var ids = SearchFilter.SearchResult.Select(m => m.CreatedById)
                    .Distinct()
                    .ToList();
                ids.AddRange(SearchFilter.SearchResult.Select(m => m.AssignedById));

                MembershipRepository membershipRepository = new MembershipRepository();
                var names = membershipRepository.GetUserByIds(ids);
                foreach (var item in SearchFilter.SearchResult)
                {
                    var createdId = names.FirstOrDefault(m => m.UserId == item.CreatedById);
                    item.CreatedBy = createdId.FirstName + " " + createdId.LastName;

                    var assignedId = names.FirstOrDefault(m => m.UserId == item.AssignedById);
                    item.AssignedBy = assignedId.FirstName + " " + assignedId.LastName;
                }
            }
        }

        public TaskWrapViewModel(Guid userId)
        {
            CompanyRepository companyRepository = new CompanyRepository();
            var company = companyRepository.GetShortCompanyInfoByUserId(userId);
        }
    }
}
