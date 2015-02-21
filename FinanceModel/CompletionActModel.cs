using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceModel
{
    public class CompletionActModel
    {
        public Guid CompanyId { get; set; }

        public int ActId { get; set; }

        [Required]
        public string Comment { get; set; }
        public DateTime Created { get; set; }
        public decimal Total { get; set; }
        public List<int> Payments
        {
            get
            {
                return string.IsNullOrEmpty(StrPayments) ? new List<int>()
                    : StrPayments.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(m => int.Parse(m))
                        .ToList();
            }
        }
        public string StrPayments { get; set; }
    }
}
