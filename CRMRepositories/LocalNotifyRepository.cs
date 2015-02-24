using CRMLocalContext;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityRepository;
using TaskModel;
using EnumHelper;

namespace CRMRepositories
{
    public class LocalNotifyRepository : CRMRepository, IDisposable
    {
        public LocalCRMEntities LocalContext { get; set; }

        public LocalNotifyRepository(Guid userId)
            : base()
        {
            using (CompanyContext.CompanyViewEntities viewContext = new CompanyContext.CompanyViewEntities(AccessSettings.LoadSettings().CompanyViewsEntites))
            {
                var localDb = viewContext.UserInCompanyView.FirstOrDefault(m => m.UserId == userId).LocalDB;
                LocalContext = new LocalCRMEntities(AccessSettings.LoadSettings().LocalCrmEntites.Replace(_defaultLocalDB, localDb));
            }
        }

        public void CreateNotify(DateTime notifyAfter, Guid from, Guid to, Guid? objId, string msg)
        {
            LocalContext.InsertUnit(new UserNotify()
            {
                DateCreated = DateTime.Now,
                InitByUserId = from,
                NotifyAfter = notifyAfter,
                IsViewed = false,
                ObjectId = objId.HasValue ? objId.ToString() : string.Empty,
                Text = msg,
                User4Notify = to,
            });
        }

        public void SetViewedNotify(int itemId)
        {
            LocalContext.GetUnitById<UserNotify>(itemId).IsViewed = true;
            LocalContext.SaveChanges();
        }

        public IEnumerable<MessageViewModel> GetNotifies(Guid _userId)
        {
            var datetime = DateTime.Now;
            return LocalContext.UserNotifies
                .Where(m => m.User4Notify == _userId && m.NotifyAfter <= datetime && !m.IsViewed)
                  .Select(m => new MessageViewModel()
                  {
                      NotifyId = m.HistoryItemId,
                      Created = m.DateCreated,
                      UserId = m.InitByUserId.HasValue ? m.InitByUserId.Value : Guid.Empty,
                      Msg = m.Text,
                      Type = (byte)MsgType.Notify,
                       IsLocal = true,
                  })
                  .ToList();
        }
    }
}
