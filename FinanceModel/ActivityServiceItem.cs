using Interfaces.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel
{
    public class ActivityServiceItem : IActivityServiceItem
    {
        public int ActivityId { get; set; }
        public int? ParentId { get; set; }
        public decimal? Cost { get; set; }
        public double? Tax { get; set; }        
        [Required]
        public string Name { get; set; }
        public string ActivityName { get; set; }


    }
}
