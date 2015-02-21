using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfaces.Finance
{
    public interface ICustomerInfo
    {
        Guid Guid { get; set; }
        int CustomerId { get; set; }
        string PhotoPath { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string PositionName { get; set; }
        string Phone { get; set; }
        string Mail { get; set; }
        string Skype { get; set; } 
        string Comment { get; set; }
        Guid UserId { get; set; }
        Guid? CompanyId { get; set; }
    }
}
