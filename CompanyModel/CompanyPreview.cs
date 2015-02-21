using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyModel
{
    public class CompanyPreview
    {
        public int CompanyId { get; set; }
        public Guid? CompanyGuid { get; set; }
        public bool IsOurClient { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyName { get; set; }

        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public DateTime Created { get; set; }
    }
}
