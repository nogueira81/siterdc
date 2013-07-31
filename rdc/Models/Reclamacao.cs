using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using System.Data.Objects;

namespace rdc.Models
{

    public class Reclamacao
    {
        public Int32 idreclamacao { get; set; }

        [Display(Name = "Descrição da Reclamação")]
        [Required(ErrorMessage = "Campo Descrição da Reclamação Obrigatório")]
        public string titulo { get; set; }

        [Display(Name = "Detalhes da Reclamação")]
        [Required(ErrorMessage = "Campo Detalhes da Reclamação Obrigatório")]
        public string descricao { get; set; }

        [Display(Name = "Qual é a Situação da Reclamação agora?")]
        [Required(ErrorMessage = "Campo Situação da Reclamação Obrigatório")]
        public string tiposolucao { get; set; }

        [Display(Name = "Nome do Cliente")]
        [Required(ErrorMessage = "Campo Nome do Cliente Obrigatório")]
        [ForeignKey("cliente")]
        public Int32 IDCLIENTE { get; set; }
        public cliente cliente { get; set; }

        [Display(Name = "Nome do Fornecedor")]
        [Required(ErrorMessage = "Campo Nome do Fornecedor Obrigatório")]
        [ForeignKey("fornecedore")]
        public Int32 IDFORNECEDOR { get; set; }
        public fornecedore fornecedore { get; set; }


        [Display(Name = "Data da Compra")]
        [Required(ErrorMessage = "Campo Data da Compra Obrigatório")]
        public Nullable<DateTime> datacompra { get; set; }

        [Display(Name = "Data da Solução")]
        public Nullable<DateTime> datasolucao { get; set; }

        [Display(Name = "Data da Reclamação")]
        public DateTime datareclamacao { get; set; }

        [Display(Name = "Telefone de Contato")]
        [Required(ErrorMessage = "Campo Telefone de Contato Obrigatório")]
        public string fonecontato { get; set; }
        
        //Listar todas as Interações da Reclamação
        public List<interaco> Interacoes { get; set; }

        //Utilizado para inserir 1 interação apenas
        public Interacao Interacao { get; set; }

        public static reclamaco Createreclamaco(global::System.Int32 idreclamacao, global::System.String titulo,
            global::System.String descricao, global::System.String tiposolucao, global::System.Int32 iDCLIENTE,
            global::System.Int32 iDFORNECEDOR, global::System.Nullable<DateTime> datacompra,
            global::System.String fonecontato, global::System.DateTime datareclamacao,
            global::System.Nullable<DateTime> datasolucao = null)
        {
            reclamaco reclamaco = new reclamaco();
            reclamaco.idreclamacao = idreclamacao;
            reclamaco.titulo = titulo;
            reclamaco.descricao = descricao;
            reclamaco.tiposolucao = tiposolucao;
            reclamaco.IDCLIENTE = iDCLIENTE;
            reclamaco.IDFORNECEDOR = iDFORNECEDOR;
            reclamaco.datacompra = datacompra;
            reclamaco.fonecontato = fonecontato;
            //DateTime.Parse(string.Format("{0:dd/MM/yyyy}", DateTime.Now));
            reclamaco.datareclamacao = datareclamacao;
            reclamaco.datasolucao = datasolucao;
            return reclamaco;
        }
    }
}