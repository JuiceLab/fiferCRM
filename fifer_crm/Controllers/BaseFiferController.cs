using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccessModel;
using AccessRepositories;
using System.Web.Routing;
using AuthService.FiferMembership;
using CompanyRepositories;
using System.Web.Security;
using CRMRepositories;
using System.Net;
using XMLModel.Sypexgeo;
using XMLModel;
using System.Text;
using fifer_crm.Models;
using Interfaces.CRM;

namespace fifer_crm.Controllers
{
    [Authorize]
    public class BaseFiferController : Controller
    {
        protected Guid _userId;
        protected bool isKeyEmployee = false;

        protected override void Initialize(RequestContext requestContext)
        {
            try
            {
                _userId = (Guid)Membership.GetUser().ProviderUserKey;
                MembershipRepository repository = new MembershipRepository();
                isKeyEmployee = repository.IsUser4FunctionRole(_userId, "Руководитель");
            }
            catch
            { }
            base.Initialize(requestContext);
        }

        protected Dictionary<string, IEnumerable<IMenuItem>> GetMenu(string currentName)
        {
            Dictionary<string, IEnumerable<IMenuItem>> menu = new Dictionary<string, IEnumerable<IMenuItem>>();
            List<MenuViewModel> model = new List<MenuViewModel>();
            if (isKeyEmployee)
            {
                model.Add(new MenuViewModel()
                {
                    Icon = "fa-briefcase",
                    Name = "Данные компании",
                    Url = "/ERP/company/indexcompany"
                });
                model.Add(new MenuViewModel()
                {
                    Icon = "fa-sitemap",
                    Name = "Отделы",
                    Url = "/department/indexdepartments"
                });
                model.Add(new MenuViewModel()
                {
                    Icon = "fa-group",
                    Name = "Сотрудники",
                    Url = "/ERP/employee/indexemployees"
                });
                model.Add(new MenuViewModel()
                {
                    Icon = "fa-tags",
                    Name = "Посещаемость",
                    Url = "/ERP/employee/indexvisiting"
                });
                model.Add(new MenuViewModel()
                {
                    Icon = "fa-globe",
                    Name = "Сайты компании",
                    Url = "/WebSite/SiteManage/Index"
                });
                
                if (model.Any(m => m.Name == currentName))
                    model.FirstOrDefault(m => m.Name == currentName).IsActive = true;
                menu.Add("company_data", model);
            }

            var modelCRM = new List<MenuViewModel>();

            modelCRM.Add(new MenuViewModel()
            {
                Icon = "fa-briefcase",
                Name = "Текущие задачи",
                Url = "/Workspace/Ordinary/indexEmployee"
            });

            modelCRM.Add(new MenuViewModel()
            {
                Icon = "fa-group",
                Name = "Клиенты",
                Url = "/Workspace/Head/Index"
            });
            modelCRM.Add(new MenuViewModel()
            {
                Icon = "fa-tasks",
                Name = "Активные задачи",
                Url = "/task/maintask/indextask"
            });
            modelCRM.Add(new MenuViewModel()
            {
                Icon = "fa-briefcase",
                Name = "Рабочий профиль ",
                //-file:///D:/templates/coral/coral-v2.0.0/html/non-ajax/admin_sidebar_fusion/social_account.html
                Url = "/Workspace/ordinary/indexEmployee"
            });
            modelCRM.Add(new MenuViewModel()
            {
                Icon = "fa-briefcase",
                Name = "Поддержка",
               Url = "/Demo/Index?viewName=support_questions.html"
            });
            modelCRM.Add(new MenuViewModel()
            {
                Icon = "fa-briefcase",
                 //или другой таймлайн редактируемый
                Name = "+-Встречи ",
                Url = "/Demo/Index?viewName=medical_appointments.html"
            });
            modelCRM.Add(new MenuViewModel()
            {
                Icon = "fa-briefcase",
                Name = "+-Звонки",
                Url = " /Demo/Index?viewName=medical_patients.html"
            });
            modelCRM.Add(new MenuViewModel()
            {
                 Icon = "Задачи",              
                 Name = "Сообщения",
                Url = "/Demo/Index?viewName=support_tickets.html"
            });


            if (modelCRM.Any(m => m.Name == currentName))
                 modelCRM.FirstOrDefault(m => m.Name == currentName).IsActive = true;
             menu.Add("crm_data", modelCRM);

             var modelHeadPosition = new List<MenuViewModel>();

             modelHeadPosition.Add(new MenuViewModel()
             {
                 Icon = "fa-briefcase",
                 Name = "Активность",
                 
                 Url = "/Demo/Index?viewName=index.html"
             });
  modelHeadPosition.Add(new MenuViewModel()
             {
                 Icon = "fa-briefcase",
                 Name = "План отдела ",
                
                 Url = "/Demo/Index?viewName=courses_listing.html"
             });
             modelHeadPosition.Add(new MenuViewModel()
             {
                 Icon = "fa-briefcase",
                 Name = "Статистика отдела",
                //список контактов компаний
                //шапка: задачи звонки встречи приход/расход
                //далее статистика по сотрудниками и типам задачи + встречи  до конца мес и платежи на этой неделе, просрочено
                //вконце список активных и не очень контактов внутри отдела
                Url = "/Demo/Index?viewName=dashboard_overview.html"
             });

            modelHeadPosition.Add(new MenuViewModel()
             {
                 Icon = "fa-briefcase",
                 Name = "Инфо о сотрудниках",
                //нет шаблона
                 Url = "/ERP/employee/indexEmployees"
             });
            

             if (modelHeadPosition.Any(m => m.Name == currentName))
                 modelHeadPosition.FirstOrDefault(m => m.Name == currentName).IsActive = true;
             menu.Add("head_data", modelHeadPosition);

            
            var modelFinance = new List<MenuViewModel>();

            modelFinance.Add(new MenuViewModel()
            {
                Icon = "fa-dollar",
                Name = "Финансовая активность",
                Url = "/Workspace/Finance/Index"
            });
            modelFinance.Add(new MenuViewModel()
            {
                Icon = "fa-clocks",
                Name = "Акты работ",
                Url = "/Finances/PaymentActs/Index"
            });

           

            modelFinance.Add(new MenuViewModel()
            {
                Icon = "fa-table",
                Name = "Статьи доходов и расходов",
                Url = "/Finances/ActivityService/Index"
            });
         
            //Компании сводная по отдельным компаниям
            //-Отчеты

              if (modelFinance.Any(m => m.Name == currentName))
                    modelFinance.FirstOrDefault(m => m.Name == currentName).IsActive = true;
                menu.Add("finance_data", modelFinance);

            var modelTask = new List<MenuViewModel>();
           
            modelTask.Add(new MenuViewModel()
            {
                Icon = "fa-archive",
                Name = "Архив задач",
                Url = "/task/maintask/indexcompletedtask"
            });

            modelTask.Add(new MenuViewModel()
            {
                Icon = "fa-question-circle",
                Name = "Вопросы тех. поддержки",
                Url = "/support/indextickets"
            });

            modelTask.Add(new MenuViewModel()
            {
                Icon = "fa-warning",
                Name = "Уведомления",
                Url = "/notify/indexnotifies"
            });


            if (modelTask.Any(m => m.Name == currentName))
                modelTask.FirstOrDefault(m => m.Name == currentName).IsActive = true;
            menu.Add("company_task", modelTask);

            var modelCompanies = new List<MenuViewModel>();
            modelCompanies.Add(new MenuViewModel()
            {
                Icon = "fa-suitcase",
                Name = "Компании",
                Url = "/CRM/LegalEntity/Index"
            });

            modelCompanies.Add(new MenuViewModel()
            {
                Icon = "fa-phone",
                Name = "Звонки",
                Url = "/Task/Call/Index"
            });

            modelCompanies.Add(new MenuViewModel()
            {
                Icon = "fa-coffee",
                Name = "Встречи",
                Url = "/Task/Meeting/Index"
            });

            if (modelCompanies.Any(m => m.Name == currentName))
                modelCompanies.FirstOrDefault(m => m.Name == currentName).IsActive = true;
            menu.Add("company_crm", modelCompanies);

            return menu;
        }

        protected void InitSubordinatedUsers()
        {
            StaffRepository staffRepository = new StaffRepository();
            ViewBag.Assign = staffRepository.GetSubordinatedUsers(_userId);
        }
    }
}