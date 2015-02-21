using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.ConstructorBase
{
    public interface IGalleryItem
    {
        int ItemId { get; set; }
        string Path { get; set; }
        string FullSizePath { get; }
        string Title { get; set; }
        string Alt { get; set; }
    }
}
