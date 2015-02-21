using AccessRepositories;
using EnumHelper;
using StateMachine.Accessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace fifer_crm.Helpers
{
    public class Actions4ProcessHelper
    {
        public object _wfLock = new object();
        private const string editorController = "Editors";

        public IEnumerable<SelectListItem> ManagmentTicket(Guid ticketId, Guid userId)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            MembershipRepository repository = new MembershipRepository();
            SupportAccessor supportWrap = new SupportAccessor();
            IEnumerable<string> availableCommands = supportWrap.GetBookmarks(ticketId);
            var strEnum = GetByteStringEnums<WFCommand>();

            var isCustomerAvailable = repository.IsUser4FunctionRole(userId, "Бог системы")
                || availableCommands.Any(m => m == WFCommand.Reject.ToString("d"));

            foreach (var item in availableCommands)
            {
                var selectItem = new SelectListItem() { Value = strEnum.FirstOrDefault(m => m == item) };
                switch ((WFCommand)Convert.ToByte(item))
                {
                    case WFCommand.Complete:
                        if (isCustomerAvailable)
                            selectItem.Text = "Запрос закрыт";
                        break;
                    case WFCommand.Comment:
                        selectItem.Text = "Оставить комментарий";
                        break;
                    case WFCommand.Submit:
                        if (isCustomerAvailable)
                            selectItem.Text = "Запрос выполнен";
                        break;
                    case WFCommand.Assigned:
                        break;
                    case WFCommand.Transfer:
                        if (isCustomerAvailable)
                            selectItem.Text = "Требуется больше времени";
                        break;
                    case WFCommand.Cancel:
                        if (isCustomerAvailable)
                            selectItem.Text = "Запрос доп. информации";
                        break;
                    case WFCommand.Reject:
                        selectItem.Text = "Запрос не выполнен";
                        break;
                    case WFCommand.Views:
                        selectItem.Text = "Принять в работу";
                        break;
                    default:
                        if (isCustomerAvailable)
                            selectItem.Text = ((WFCommand)Convert.ToByte(strEnum.FirstOrDefault(m => m == item))).GetStringValue();
                        break;
                }

                if (!string.IsNullOrEmpty(selectItem.Text))
                    result.Add(selectItem);
            }

            return result;
        }

        public IEnumerable<SelectListItem> ManagmentTask(Guid taskId, Guid guid)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            TaskAccessor taskWrap = new TaskAccessor();
            IEnumerable<string> availableCommands = taskWrap.GetBookmarks(taskId);
            var strEnum = GetByteStringEnums<WFTaskCommand>();

            foreach (var item in availableCommands)
            {
                var selectItem = new SelectListItem() { Value = strEnum.FirstOrDefault(m => m == item) };
                switch ((WFTaskCommand)Convert.ToByte(item))
                {
                    case WFTaskCommand.Complete:
                        selectItem.Text = "Выполнить задачу";
                        break;
                    case WFTaskCommand.Comment:
                        selectItem.Text = "Оставить комментарий";
                        break;
                    case WFTaskCommand.Expired:                     
                    case WFTaskCommand.Sсhedule:                       
                    case WFTaskCommand.Assign:
                    case WFTaskCommand.View:
                        break;
                    default:
                        selectItem.Text = ((WFCommand)Convert.ToByte(strEnum.FirstOrDefault(m => m == item))).GetStringValue();
                        break;
                }

                if (!string.IsNullOrEmpty(selectItem.Text))
                    result.Add(selectItem);
            }

            return result;
        }

        public List<string> GetByteStringEnums<T>()
        {
            var result = new List<string>();
            foreach (WFCommand item in Enum.GetValues(typeof(T)))
            {
                result.Add(item.ToString("d"));
            }
            return result;
        }
      
    }
}
