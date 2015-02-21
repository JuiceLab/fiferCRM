using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandingModel.GridModels
{
    public class GridPageModel
    {
        public int PageId { get; set; }
        [DisplayName("Клиент")]
        public string Client { get; set; }
        [DisplayName("Телефон")]
        public long Phone { get; set; }
        [DisplayName("Лэндинг")]
        public string LandingName { get; set; }
        [DisplayName("Адрес страницы")]
        public string LandingUrl { get; set; }
        [DisplayName("Создан")]
        public DateTime Created { get; set; }
    }
}
