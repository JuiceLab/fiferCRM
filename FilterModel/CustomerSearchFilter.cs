using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FilterModel
{
    public class CustomerSearchFilter
    {
        IFormatProvider provider = new CultureInfo("ru-RU").DateTimeFormat;

        public bool IsLegal { get; set; }
        public bool HasMeeting { get; set; }
        public bool HasCallTask { get; set; }
        public bool HasPayment { get; set; }
        public bool HasUnssigned { get; set; }
        public int? DistrictId { get; set; }
        public List<Guid> Cities { get; set; }
        public List<int> Services { get; set; }
        public List<byte> StatusId { get; set; }
        public List<Guid> AssignedBy { get; set; }
        public Dictionary<Guid, string> EmployeesPhoto { get; set; }

        public DateTime[] DateRange
        {
            get
            {
                return DateRangeInvariant != null ?
                    DateRangeInvariant.Select(m => Convert.ToDateTime(m, provider)).ToArray()
                    : null;
            }
        }

        public IList<string> DateRangeInvariant { get; set; }
        public List<int> Name { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public string Site { get; set; }
        public IEnumerable<dynamic> SearchResult { get; set; }

        public List<SelectListItem> AvailableNames { get; set; }
        
        public Dictionary<string, List<SelectListItem>> AvailableCities { get; set; }
        public Dictionary<string, List<SelectListItem>> AvailableServices { get; set; }


        public List<SelectListItem> AvailableStatuses { get; set; }

        public List<SelectListItem> AssignedAvailable { get; set; }

        public CustomerSearchFilter()
        {
            AssignedAvailable = new List<SelectListItem>();
            AvailableCities = new Dictionary<string, List<SelectListItem>>();
            AvailableStatuses = new List<SelectListItem>();
            AvailableNames = new List<SelectListItem>();
            EmployeesPhoto = new Dictionary<Guid, string>();
            Cities = new List<Guid>();
            AssignedBy = new List<Guid>();
            StatusId = new List<byte>();
            Name = new List<int>();
            IsLegal = true;
            HasCallTask = true;
            HasMeeting = true;
            HasPayment = true;
            Services = new List<int>();
            DateRangeInvariant = new List<string>() { 
                DateTime.Now.ToString("dd.MM.yyyy"),
                DateTime.Now.AddDays(1).ToString("dd.MM.yyyy") 
            };
            HasUnssigned = false;
        }

    }
}
