using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskModel
{
    public class MessageViewModel
    {
        public int NotifyId { get; set; }

        public string ObjectId { get; set; }
        public byte Type { get; set; }
        public Guid UserId { get; set; }
        public string IconPath { get; set; }
        public string Title { get; set; }
        public string Msg { get; set; }
        public DateTime Created { get; set; }

        public bool IsViewed { get; set; }
        public bool IsLocal { get; set; }
    }
}
