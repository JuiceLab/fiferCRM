using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessModel
{
    public class UserLockModel
    {
        public Guid UserId { get; set; }
        public string LockIps { get; set; }
        public bool IsTokenAccess { get; set; }

    }
}
