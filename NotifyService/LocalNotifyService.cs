using EnumHelper.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskModel;
using TaskModel.CRM;
using TaskModel.Task;
using EnumHelper;

namespace NotifyService
{
    public class LocalNotifyService
    {
        public List<MessageViewModel> GetTaskItems(IEnumerable<TaskPreview> items)
        {
            return items.Select(m => new MessageViewModel()
            {
                Created = m.Created,
                Msg = m.Msg,
                ObjectId = m.TaskId.ToString(),
                Title = string.Format("#{0} {1}. {2}. {3}", 
                    m.TaskNumber,
                    m.Title,
                    m.Status.GetStringValue(), m.DateStarted.HasValue? (m.DateStarted.Value.Date == DateTime.Now.Date? m.DateStarted.Value.ToShortTimeString() : m.DateStarted.Value.ToShortDateString() ): string.Empty),
                IconPath = m.CreatedByPhotoPath,
                UserId = m.CreatedById
            }).ToList();
        }

        public List<MessageViewModel> GetTaskCallItems(IEnumerable<CallTaskPreview> items)
        {
            return items.Select(m => new MessageViewModel()
            {
                Created = m.Date,
                Msg = string.Format("Телефон {0}. {1}", m.Phone, m.CustomerName),
                ObjectId = m.CallTaskId.ToString(),
                IconPath = !string.IsNullOrEmpty(m.CompanyPhoto)? m.CompanyPhoto : m.CustomerPhoto,
                Title = string.Format("{1} #{2}", m.OwnerName, m.CompanyName, m.TaskNumber),
                UserId = m.OwnerId,
            }).ToList();
        }

        public List<MessageViewModel> GetMeetings(IEnumerable<MeetingTaskPreview> items)
        {
            return items.Select(m => new MessageViewModel()
            {
                Created = m.Date,
                ObjectId = m.MeetingId.ToString(),
                Title = string.Format("Телефон {0}. {1}", m.Phone, m.CustomerName),
                UserId = m.OwnerId,
                IconPath = !string.IsNullOrEmpty(m.CompanyPhoto) ? m.CompanyPhoto : m.CustomerPhoto
            }).ToList();
        }
    }
}
