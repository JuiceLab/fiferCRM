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
using TicketModel;
using System.Web.Security;
using TicketRepositories;
using EnumHelper;
using fifer_crm.Helpers;
using TaskModel;
using TaskModel.Ticket;
using LogService.FilterAttibute;
using System.ComponentModel;
using fifer_crm.Models;

namespace fifer_crm.Controllers
{
    [Authorize, CRMLogAttribute]
    public class SupportController : BaseFiferController
    {
        [DisplayName("Страница вопросов в службу поддержки")]
        public ActionResult IndexTickets()
        {
            SupportTicketRepository repository = new SupportTicketRepository();

            var model = new TaskWrapViewModel(_userId, true);
            model.Menu = GetMenu("Вопросы тех. поддержки");

            return View(model);
        }

        [DisplayName("Создание вопроса в службу поддержки")]
        public ActionResult TicketEdit()
        {
            SupportTicket ticket = new SupportTicket()
            {
                CreatedBy = _userId,
            };
            SupportTicketRepository repository = new SupportTicketRepository();
            ViewBag.Categories = repository.GetCategories();
            ViewBag.Command = (byte)WFCommand.Create;
            return PartialView(ticket);
        }

        [DisplayName("Обработка вопроса службы поддержки")]
        public ActionResult TicketProcess(Guid ticketId)
        {
            SupportTicket ticket = new SupportTicket();
            ticket.Load(ticketId);
            ticket.OwnerId = _userId;
            return PartialView(ticket);
        }

        [DisplayName("История сообщений по вопросу службы поддержки")]
        public ActionResult TicketHistory(Guid ticketId, string ticketNumber)
        {
            SupportTicketRepository repository = new SupportTicketRepository();
            IEnumerable<MessageViewModel> messages = repository.GetMessages4Item(ticketId);
            MembershipRepository accessRepository = new MembershipRepository();
            var users = accessRepository.GetUserByIds(messages.Select(m => m.UserId).Distinct());
            foreach (var item in messages)
            {
                item.Title = users.FirstOrDefault(m => m.UserId == item.UserId).Login;
            }
            ViewBag.Number = ticketNumber;
            return PartialView(messages);
        }

        [DisplayName("Загрузка доступных действий с вопросом службы поддержки")]
        public ActionResult Actions4Ticket(Guid ticketId)
        {
            Actions4ProcessHelper actionPanel = new Actions4ProcessHelper();
            var model = actionPanel.ManagmentTicket(ticketId, _userId);
            return PartialView(model);
        }

        [DisplayName("Обновление таблицы вопросов службы поддержки")]
        public ActionResult GetTicketTable(bool isOwner, string userId)
        {
            SupportTicketRepository repository = new SupportTicketRepository();
            IEnumerable<TicketPreview> model = null;
            if (isOwner)
                model =repository.GetAcctualTickets();
            else
                model = repository.GetCustomerTickets(Guid.Parse(userId));
            return PartialView("AcctualTicketsTable", model);
        }

    }
}