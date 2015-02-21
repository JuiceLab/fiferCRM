using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteModel
{
    public class FeedbackModel
    {
        [Required]
        public string ClientName { get; set;}
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Mail { get; set; }
        [Required]
        public string Text { get; set; }
    }
}
