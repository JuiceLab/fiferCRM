using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.ConstructorBase
{
    public interface ISliderPage : IPage
    {
        IList<ISliderItem> SliderItems { get; set; }
    }
}
