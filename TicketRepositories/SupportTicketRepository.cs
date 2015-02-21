using EnumHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskModel;
using TaskModel.Ticket;

namespace TicketRepositories
{
    public class SupportTicketRepository : BaseTicketRepository, IDisposable
    {
        public SupportTicketRepository()
            : base(){ }
        public IEnumerable<SelectListItem> GetCategories()
        {
            return Context.Categories
                .Where(m=>m.TypeId == (byte)CategoryType.Ticket)
                .ToList()
                .Select(m => new SelectListItem()
                {
                    Text = m.Name,
                    Value = m.CategoryId.ToString()
                });
        }

        public IEnumerable<TicketPreview> GetAcctualTickets()
        {
            var items = Context.CommentBodies
                .Select(m => m.GuidItem)
                .ToList()
                .GroupBy(m => m)
                .ToDictionary(m => m.Key, m => m.Count());

            return Context.Tickets
                .Where(m => m.TicketStatus != (byte)TicketStatus.Closed && m.Category.TypeId == (byte)CategoryType.Ticket)
                .ToList()
                .Select(m => new TicketPreview()
                {
                    Created = m.DateCreated,
                    CreatedById = m.CreatedBy,
                    Status = (TicketStatus)m.TicketStatus.Value,
                    TicketNumber = m.TicketNumber,
                    TicketId = m.TicketId,
                    Title = m.Title,
                    MsgCount = items[m.TicketId]
                }).ToList();
        }

        public IEnumerable<MessageViewModel> GetMessages4Item(Guid ticketId)
        {
            return Context.CommentBodies.Where(m => m.GuidItem == ticketId)
                 .Select(m => new MessageViewModel()
                 {
                     Created = m.DateCreated,
                     Msg = m.Comment,
                     UserId = m.UserId.Value
                 }).ToList();
        }

        public IEnumerable<TicketPreview> GetCustomerTickets(Guid guid)
        {
            var items = Context.CommentBodies
               .Select(m => m.GuidItem)
               .ToList()
               .GroupBy(m => m)
               .ToDictionary(m => m.Key, m => m.Count());

            return Context.Tickets
                .Where(m => m.CreatedBy == guid && m.Category.TypeId == (byte)CategoryType.Ticket)
                .ToList()
                .Select(m => new TicketPreview()
                {
                    Created = m.DateCreated,
                    CreatedById = m.CreatedBy,
                    Status = (TicketStatus)m.TicketStatus.Value,
                    TicketNumber = m.TicketNumber,
                    TicketId = m.TicketId,
                    Title = m.Title,
                    MsgCount = items[m.TicketId]
                }).ToList();
        }
    }
}
