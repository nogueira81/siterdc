using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Web.Security;

namespace rdc.Models
{
    public class LoginModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Senha { get; set; }

        [Display(Name = "Infomar E-mail cadastrado no seu Login:")]
        public string email { get; set; }

        [Display(Name = "Infomar CPF cadastrado no seu Login:")]
        public string cpf { get; set; }

    }

    public class TEstados
    {
        
        public string value { get; set; }

        public string text { get; set; }

        public string selected { get; set; }

        public TEstados(string Val, string Tex, string Selec)
        {
            value = Val;
            text = Tex;
            selected = Selec;
        }

    }

    public class TCEP
    {

        public string cep { get; set; }

        public string endereco { get; set; }

        public string estado { get; set; }
        
        public string cidade { get; set; }

    }

    public interface IFormsAuthenticationService
    {
        void SignIn(string userName, bool createPersistentCookie);
        void SignOut();
    }

    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Valor não pode ser Nulo ou vazio.", "userName");

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}
