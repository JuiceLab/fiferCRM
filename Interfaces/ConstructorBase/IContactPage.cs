using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.ConstructorBase
{
    public interface IContactPage : IPage
    {
        double? Longitude { get; set; }
        double? Latitude { get; set; }
    }
}
