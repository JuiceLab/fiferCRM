using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.ConstructorBase
{
    public interface ISliderItem
    {
        int ItemId { get; set; }
        int Pos { get; set; }
        bool IsBlank { get; set; }
        string Title { get; set; }
        string PhotoPath { get; set; }
        string HtmlContent { get; set; }
        string RedirectAnchor { get; set; }
    }
}
