using Interfaces.ConstructorBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteModel
{
    public class SliderItem : ISliderItem
    {
        public int ItemId { get; set; }
        [DisplayName("Позиция")]
        public int Pos { get; set; }
        [DisplayName("Открывать в новом окне")]
        public bool IsBlank { get; set; }
        [DisplayName("Название"), Required]
        public string Title { get; set; }
        [DisplayName("Фото"), Required]
        public string PhotoPath { get; set; }
        [DisplayName("Html-контент")]
        public string HtmlContent { get; set; }
        [DisplayName("Ссылка")]
        public string RedirectAnchor { get; set; }
    }
}
