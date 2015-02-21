using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteBoardModel.EditorModel
{
    public class PasteBoardPageEditModel
    {
        public int PageId { get; set; }
        [DisplayName("Порядок в меню")]
        public int? PositionId { get; set; }
        public int? ParentId { get; set; }
        public Guid ProjectId { get; set; }
        [Required, DisplayName("Анонс"), MaxLength(256)]
        public string Notice { get; set; }
        public string Text { get; set; }
        [Required, DisplayName("Адрес страницы"), MaxLength(128)]
        public string Anchor { get; set; }
        [Required, DisplayName("Название страницы"), MaxLength(128)]
        public string Title { get; set; }
        [DisplayName("Описание страницы"), MaxLength(512)]
        public string Description { get; set; }
        [DisplayName("Ключевые слова"), MaxLength(256)]
        public string Keywords { get; set; }
        [DisplayName("Виджет репоста в соц.сети")]
        public bool IsSocialRepost { get; set; }
    }
}
