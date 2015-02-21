using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifyModel
{
    public class UserNotifyModel
    {
        public bool IsActive { get; set; }
        public int NotifyId { get; set; }
        public bool FromMe { get; set; }
        public bool ToMe { get; set; }
        public bool UnderMe { get; set; }
        public UserNotifyModel()
        {
            
        }
    }
}
