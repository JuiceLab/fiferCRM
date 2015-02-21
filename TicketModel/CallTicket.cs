using CRMContext;
using Interfaces.CRM;
using Interfaces.Support;
using Settings;
using SupportContext;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketModel
{
    [MetadataType(typeof(ITicket))]
    [Serializable]
    public class CallTicket : ICallTicket
    {
        private IFormatProvider ruDateFormat = new CultureInfo("ru-RU").DateTimeFormat;
        
        private object lockObj = new object();
        public Guid TicketId { get; set; }

        public Guid? MasterId { get; set; }

        public string Title { get; set; }

        public Guid TicketOwnerId { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateStarted { get; set; }
        [Required]
        public string DateStartedStr { get; set; }

        [Required]
        public string TimeStartedStr { get; set; }

        public DateTime? TimeToExpired { get; set; }

        public string TicketStatusStr { get; set; }
        [Required]
        public string Msg { get; set; }

        public byte? TicketStatus { get; set; }

        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CurrentComment { get; set; }

        public Guid CurrentCommentId { get; set; }

        public string AssignedName { get; set; }

        public Guid? Assigned { get; set; }

        public DateTime? DateAssigned { get; set; }

        public DateTime DateModified { get; set; }

        public byte Priority { get; set; }
        public string Phone { get; set; }

        public Guid CreateMaster(string masterType, Guid wfGuid, Guid? masterId)
        {
            throw new NotImplementedException();
        }

        public Guid? ObjId { get; set; }

        public object EnvelopObj { get; set; }

        public IDictionary<string, IEnumerable<Guid>> CommonActionTickets { get; set; }

        public void Create()
        {
            using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                lock (lockObj)
                {
                    context.CallTickets.Add(new SupportContext.CallTicket()
                    { 
                        Assigned = Assigned,
                        CallStatus = TicketStatus.HasValue ? TicketStatus.Value : (byte)EnumHelper.TaskStatus.Novelty,
                        CallTicketNumber = GenerateNumber(),
                        CreatedBy = OwnerId,
                        CustomerGuid = ObjId.Value,
                        DateStarted = DateStarted.Value,
                        Message = Msg,
                        Title = Title,
                        Phone = string.IsNullOrEmpty(Phone) ? "Задан" : Phone,
                    });
                    context.SaveChanges();
                }
            }
        }

        public void Load(Guid guid)
        {
            using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                var existCall = context.CallTickets.FirstOrDefault(m => m.CallTicketId == guid);
                TicketId = existCall.CallTicketId;
                Assigned = existCall.Assigned;
                TicketStatus = existCall.CallStatus;
                CreatedBy = existCall.CreatedBy;
                ObjId = existCall.CustomerGuid;
                DateStarted = existCall.DateStarted;
                Msg = existCall.Message;
                Title = existCall.Title;
                Phone = existCall.Phone;
                DateStartedStr = existCall.DateStarted.ToString("dd.MM.yyyy");
                TimeStartedStr = existCall.DateStarted.ToString("HH:mm");
                context.SaveChanges();
            }
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(DateStartedStr))
                DateStarted = Convert.ToDateTime(DateStartedStr, ruDateFormat).Add(Convert.ToDateTime(TimeStartedStr).TimeOfDay);

            if (TicketId == Guid.Empty)
                Create();
            else {
                using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
                {

                    var existCall = context.CallTickets.FirstOrDefault(m => m.CallTicketId == TicketId);
                    existCall.Assigned = Assigned;
                    existCall.CallStatus = TicketStatus.HasValue ? TicketStatus.Value : (byte)EnumHelper.TaskStatus.Novelty;
                    existCall.CreatedBy = OwnerId;
                    existCall.CustomerGuid = ObjId.Value;
                    existCall.DateStarted = DateStarted.Value;
                    existCall.Message = Msg;
                    existCall.Title = Title;
                    existCall.Phone = Phone;
                    context.SaveChanges();
                }
            }
        }

        public void AutoClose()
        {
            throw new NotImplementedException();
        }

        public void LoadTicketByObjId(Guid objID)
        {
            throw new NotImplementedException();
        }

        public Guid ChangeStatus(int statusId, string customStatus = "")
        {
            throw new NotImplementedException();
        }

        public void ChangePreview(bool value)
        {
            throw new NotImplementedException();
        }

        public string Status { get; set; }

        public bool IsNonePreview { get; set; }
        public Guid OwnerId { get; set; }

        private string GenerateNumber()
        {
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder();
            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {              
                    do
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            sb.Append(rnd.Next(0, 9));
                        }
                        var val = sb.ToString();
                        if (wfContext.CallTickets.Any(m => m.CallTicketNumber == val))
                            sb = new StringBuilder();
                    } while (sb.Length == 0);
                return sb.ToString();
            }
        }

    }
}
