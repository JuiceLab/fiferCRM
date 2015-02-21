using Interfaces.ConstructorBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteModel
{
    public class GalleryItem : IGalleryItem
    {
        public int ItemId { get; set; }
        public string Path { get; set; }
        public string FullSizePath { 
            get { 
                return Path.Substring(0, Path.IndexOf(".")) + "_big" + Path.Substring(Path.IndexOf(".")); 
            }
        }

        [Required]
        public string Title { get; set; }
        public string Alt { get; set; }
    }
}
