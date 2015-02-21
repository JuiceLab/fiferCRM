using Interfaces.Mailer;
using LogContext;
using Mvc.Mailer;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService
{
    public class GeneralMailer : MailerBase, IGeneralMailer
    {
        protected string _codePrefix;
        public string RegisterCode { get; set; }
        public int MailRegisterId { get; set; }

        public GeneralMailer()
        {
            MasterName = "_Layout";
            InitMailId();
        }

        public virtual MvcMailMessage GeneralEmail(string body, string[] addrs, string subj)
        {
            ViewBag.Data = body;

            //var addrList = addrs.Split(',');

            var msg = Populate(x =>
            {
                x.Subject = subj;
                x.ViewName = "GeneralEmail";
                //x.To.Add(addrs);
                x.IsBodyHtml = true;

            });

            foreach (var addr in addrs)
            {
                msg.To.Add(addr);
            }
            msg.Send();

            return msg;
        }
        public virtual string DoNotify<T>(Guid objId,string bodyText, T mailType) where T : struct, IConvertible
        {
            return string.Empty;
        }

        protected string SendBodyText(string[]addrs, string subj, string bodyText)
        {
            ViewBag.Data = bodyText;

            //var addrList = addrs.Split(',');

            var msg = Populate(x =>
            {
                x.Subject = subj;
                x.ViewName = "GeneralEmail";
                //x.To.Add(addrs);
                x.IsBodyHtml = true;

            });

            foreach (var addr in addrs)
            {
                msg.To.Add(addr);
            }
            msg.Send();

            return msg.Body;
        }

        protected void UpdateMailName(string name)
        {
            using (LogEntities context = new LogEntities(AccessSettings.LoadSettings().LogEntites))
            {
                var reg = context.MailRegistrations.FirstOrDefault(m => m.MailRegistrationId == MailRegisterId);
                reg.Name = name;
                reg.RegisterCode = _codePrefix + reg.MailRegistrationId;
                context.SaveChanges();
                ViewBag.RegisterCode = reg.RegisterCode;
                RegisterCode = reg.RegisterCode;
            }
        }

        private void InitMailId()
        {
            using (LogEntities context = new LogEntities(AccessSettings.LoadSettings().LogEntites))
            {
                var mailReg = new MailRegistration()
                {
                    rowguid = Guid.NewGuid(),
                    IsSuccess = true,
                    Created = DateTime.Now
                };
                context.MailRegistrations.Add(mailReg);
                context.SaveChanges();
                MailRegisterId = mailReg.MailRegistrationId;
            }
        }
    }
}
