using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessModel
{
    public class RegisterModel
    {
        [DisplayName("Логин"),
        Required(ErrorMessage = "Поле 'Логин' обязательно для заполнения"),
        StringLength(30, ErrorMessage = "Логин должен быть от 3 до 30 символов.", MinimumLength = 3)]
        public string Login { get; set; }


        [DisplayName("Фамилия"),
        Required(ErrorMessage = "Поле 'Фамилия' обязательно для заполнения"),
        StringLength(50, ErrorMessage = "Фамилия должна быть длиной от 3 до 50 символов.", MinimumLength = 3)]
        public string FirstName { get; set; }

        [DisplayName("Имя"),
        Required(ErrorMessage = "Поле 'Имя' обязательно для заполнения"),
        StringLength(50, ErrorMessage = "Имя должен быть длиной от 3 до 50 символов.", MinimumLength = 3)]
        public string LastName { get; set; }


        [DisplayName("Пароль"), DataType(DataType.Password),
        Required(ErrorMessage = "Поле 'Пароль' обязательно для заполнения"),
        StringLength(30, ErrorMessage = "Пароль должен быть от 3 до 20 символов.", MinimumLength = 3)]
        public string Pass { get; set; }

        [DisplayName("Повторите пароль"),
        DataType(DataType.Password),
        Compare("Pass", ErrorMessage = "Пароли не совпадают"),
        Required(ErrorMessage = "Поле 'Повторите пароль' обязательно для заполнения")]
        public string Pass_repeat { get; set; }

        public string Salt { get; set; }

        public string HashPassword
        {
            get
            {
                if (!String.IsNullOrEmpty(Pass))
                {
                    return Salt + GetMd5Hash(Salt + Pass);
                }
                else
                    return null;
            }
        }

        [DisplayName("Email"),
        DataType(DataType.EmailAddress),
        Required(ErrorMessage = "Поле 'Email' обязательно для заполнения")]
        public string Email { get; set; }

        [DisplayName("Контактный телефон"),Required(ErrorMessage = "Контактный телефон обязателен для заполнения")]
        public string Phone { get; set; }

        protected string GetMd5Hash(string value)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(value);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }
    }
}
