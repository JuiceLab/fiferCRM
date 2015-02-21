using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel
{
    public class CompletionActEditModel : CompletionActModel
    {
        public Guid CustomerGuid { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string DealNumber { get; set; }
        [Required]
        public DateTime DealDate { get; set; }
    }
}
