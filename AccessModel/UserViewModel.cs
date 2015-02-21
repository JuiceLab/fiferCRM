using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessModel
{
    public class UserViewModel
    {
        public Guid UserId { get; set; }
        [DisplayName("Логин")]
        public string Login { get; set; }
        [DisplayName("Полное имя")]
        public string FullName { get; set; }
        [DisplayName("Компания")]
        public string Company { get; set; }
        [DisplayName("Отдел")]
        public string Department { get; set; }
        [DisplayName("Должность")]
        public string Position { get; set; }
        [DisplayName("Телефон")]
        public string Phone { get; set; }
        [DisplayName("Почта")]
        public string Mail { get; set; }
        [DisplayName("Логин")]
        public bool IsOnline { get; set; }
        [DisplayName("Послединй вход")]
        public DateTime LastLogin { get; set; }
     
    }
}
