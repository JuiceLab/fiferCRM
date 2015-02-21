using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyModel
{
    public class PassportViewModel
    {
        public int PassportId { get; set; }
        [DisplayName("Серия"), Required]
        public int Serial { get; set; }
        [DisplayName("Номер"), Required]
        public int Number { get; set; }
        [DisplayName("Кем выдан"), Required]
        public string WhoIssue { get; set; }
        [DisplayName("Код подразделения"), Required]
        public string CodeIssue { get; set; }
        [DisplayName("Место рождения"), Required]
        public string BirthLocation { get; set; }
        [DisplayName("Адрес прописки"), Required]
        public string RegLocation { get; set; }
        [DisplayName("Скан паспорта")]
        public List<string> ScanPath { get; set; }
        [DisplayName("Когда выдан"), Required]
        public string DateIssue { get; set; }
        public int EmployeeId { get; set; }

        [DisplayName("Область")]
        public int DistrictId { get; set; }

        [DisplayName("Город")]
        public int City { get; set; }
        public Guid? CityGuid { get; set; }

        public PassportViewModel()
        {
            ScanPath = new List<string>();
        }
    }
}
