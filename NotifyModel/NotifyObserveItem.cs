using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifyModel
{
    public class NotifyObserveItem
    {
        public int NotifyId { get; set; }
        public Guid? OwnerId { get; set; }
        public Guid? FromUserId { get; set; }
        public Guid? ToUserId { get; set; }
        public string AppendMsg { get; set; }

        public string ObjectId { get; set; }
    }
}
