using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityRepository
{
    public interface IPartQuery
    {
        string FeildName { get; set; }
        object FieldValue { get; set; }
    }
}
