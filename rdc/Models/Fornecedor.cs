using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using rdc.App_Code;
using System.Web.Mvc;

namespace rdc.Models
{
    public class Fornecedor
    {
        public Int32 IDFORNECEDOR { get; set; }

        [Display(Name = "Razão Social")]
        [Required(ErrorMessage = "Campo Razão Social Obrigatório")]
        public string razaosocial { get; set; }

        [Display(Name = "Nome Fantasia")]
        [Required(ErrorMessage = "Campo Nome Fantasia Obrigatório")]
        public string nomefantasia { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Campo E-mail Obrigatório")]
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
        [Required(ErrorMessage = "Campo Endereço Obrigatório")]
        public string endereco { get; set; }

        [Display(Name = "CEP")]
        [Required(ErrorMessage = "Campo CEP Obrigatório")]
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
        [Required(ErrorMessage = "Telecone Fixo Obrigatório")]
        public string fonefixo { get; set; }

        [Display(Name = "Telefone Fixo(opcional)")]
        public string fonefixo2 { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "Campo Estado Obrigatório")]
        public string ESTADO { get; set; }

        [Display(Name = "Informações adicionais")]
        public string informacoes { get; set; }

        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "Campo Cidade Obrigatório")]
        public Nullable<global::System.Int32> IDCIDADE { get; set; }

        [Display(Name = "Ativo?")]
        [Required(ErrorMessage = "Ativo Obrigatório")]
        public string ativo { get; set; }

        [Display(Name = "Ativo?")]
        public bool auxativo
        {
            get
            {
                if (bool.Equals(ativo, "A"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set 
            {
                if (value)
                {
                    ativo = "A";
                }
                else
                {
                    ativo = "N";
                }
            }
        }

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

    public class FiltraFornec
    {
        public string IdFor { get; set; }
        public string NomeFor { get; set; }

        public FiltraFornec(string idfor, string nomefor)
        {
            this.IdFor = idfor;
            this.NomeFor = nomefor;
        }

        public static IList<FiltraFornec> ListaFornec(string idlist)
        {
            sistemardcEntities db = new sistemardcEntities();
            IEnumerable<fornecedore> fornecsfil;
            //Se for vazio, preencher com todos os fornecedores Ativos, senão filtrar
            if (string.IsNullOrEmpty(idlist))
            {
                fornecsfil = db.fornecedores.ToList().FindAll(f => f.ativo != "N").ToList();
            } 
            else 
            {
                idlist = idlist.ToUpper();
                fornecsfil = db.fornecedores.ToList().FindAll(f => f.razaosocial.ToUpper().IndexOf(idlist) >= 0).ToList().FindAll(f => f.ativo != "N").ToList();
            }
            List<FiltraFornec> fornecs = new List<FiltraFornec>();
            //Preencher o List com as cidades localizadas - ex: cidades.Add(new Cidade("7", "MG", "Belo Horizonte")); 
            foreach (var item in fornecsfil)
            {
                fornecs.Add(new FiltraFornec(item.IDFORNECEDOR.ToString(), item.razaosocial.ToString()));
            }
            db.Dispose();
            return fornecs.ToList(); //.Where(x => x.SiglaUf == SiglaUf).ToList(); 
        }
    }


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

}