using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.ConstructorBase
{
    public interface IGalleryPage : IPage
    {
        string GalleryName { get; set; }
        string GalleryPath { get; set; }
        IList<IGalleryItem> GalleryItems { get; set; }
    }
}
