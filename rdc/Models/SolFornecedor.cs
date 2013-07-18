using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using rdc.App_Code;
using System.Web.Mvc;

namespace rdc.Models
{
    public class SolFornecedor
    {
        public Int32 IDFORNECEDOR { get; set; }

        [Display(Name = "Razão Social")]
        public string razaosocial { get; set; }

        [Display(Name = "Nome Fantasia")]
        [Required(ErrorMessage = "Campo Nome Fantasia Obrigatório")]
        public string nomefantasia { get; set; }

        [Display(Name = "E-mail")]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "E-mail inválido")]
        public string email { get; set; }

        [Display(Name = "Home Page")]
        public string homepage { get; set; }

        [Display(Name = "CNPJ")]
        [Required(ErrorMessage = "Campo CNPJ Obrigatório")]
        [CustomValidationCNPJAttribute(ErrorMessage = "CNPJ Inválido")]
        public string cnpj
        {
            get
            {
                return Util.FormataString("##.###.###/####-##", _cnpj);
            }
            set
            {
                if (_cnpj != value)
                {
                    _cnpj = Util.RemoveNaoNumericos(value);
                }
            }
        }
        private string _cnpj;

        [Display(Name = "Endereço")]
        public string endereco { get; set; }

        [Display(Name = "CEP")]
        public string cep
        {
            get
            {
                return Util.FormataString("#####-###", _cep);
            }
            set
            {
                if (_cep != value)
                {
                    _cep = Util.RemoveNaoNumericos(value);
                }
            }
        }
        private string _cep;

        [Display(Name = "Telefone Fixo")]
        public string fonefixo { get; set; }

        [Display(Name = "Telefone Fixo(opcional)")]
        public string fonefixo2 { get; set; }

        [Display(Name = "Estado")]
        public string ESTADO { get; set; }

        [Display(Name = "Informações adicionais")]
        public string informacoes { get; set; }

        [Display(Name = "Cidade")]
        public Nullable<global::System.Int32> IDCIDADE { get; set; }

        [Display(Name = "Ativo?")]
        public string ativo { get; set; }

        [Display(Name = "CLIENTE")]
        public Nullable<global::System.Int32> IDCLIENTE { get; set; }

        public string ENVEMAILCLISOL { get; set; }

        public static fornecedore Createfornecedore(global::System.Int32 IDFORNECEDOR, global::System.String razaosocial,
            global::System.String nomefantasia, global::System.String email, global::System.String cnpj,
            global::System.String endereco, global::System.String cep, global::System.String fonefixo,
            global::System.String fonefixo2, global::System.String ESTADO, global::System.String informacoes,
            Nullable<global::System.Int32> IDCIDADE, global::System.String ativo, global::System.String homepage,
            Nullable<global::System.Int32> IDCLIENTE, global::System.String ENVEMAILCLISOL)
        {
            fornecedore fornecedore = new fornecedore();
            fornecedore.IDFORNECEDOR = IDFORNECEDOR;
            fornecedore.razaosocial = razaosocial;
            fornecedore.nomefantasia = nomefantasia;
            fornecedore.email = email;
            fornecedore.cnpj = cnpj;
            fornecedore.endereco = endereco;
            fornecedore.cep = cep;
            fornecedore.fonefixo = fonefixo;
            fornecedore.fonefixo2 = fonefixo2;
            fornecedore.ESTADO = ESTADO;
            fornecedore.informacoes = informacoes;
            fornecedore.IDCIDADE = IDCIDADE;
            fornecedore.ativo = ativo;
            fornecedore.homepage = homepage;
            fornecedore.IDCLIENTE = IDCLIENTE;
            fornecedore.ENVEMAILCLISOL = ENVEMAILCLISOL;
            return fornecedore;
        }
    }

/* Já existe no Model Fornecedor
    /// <summary> /// Validação customizada para CPF/// </summary> 
    public class CustomValidationCNPJAttribute : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public CustomValidationCNPJAttribute()
        {
        }
        /// <summary>     
        /// Validação server     
        /// </summary>     
        /// <param name="value"></param>     
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return true;
            bool valido = Util.ValidaCNPJ(value.ToString());
            return valido;
        }
        /// <summary>
        /// Validação client
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = this.FormatErrorMessage(null),
                ValidationType = "customvalidationcnpj"
            };
        }
    }
*/
}