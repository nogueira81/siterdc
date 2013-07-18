using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace rdc.Models
{
    public class Interacao
    {
        public Int32 IDINTERACAO { get; set; }

        [Display(Name = "Interação da Reclamação")]
        [Required(ErrorMessage = "Campo Interação da Reclamação Obrigatório")]
        public string INFORMACOES { get; set; }

        [Display(Name = "Data da Interação")]
        [Required(ErrorMessage = "Campo Data da Reclamação Obrigatório")]
        public DateTime DATAINTERACAO { get; set; }

        [Display(Name = "Reclamação")]
        [Required(ErrorMessage = "Campo Reclamação Obrigatório")]
        [ForeignKey("reclamaco")]
        public Int32 IDRECLAMACAO { get; set; }
        public reclamaco reclamaco { get; set; }

        [Display(Name = "Nome do Cliente")]
        [Required(ErrorMessage = "Campo Nome do Cliente Obrigatório")]
        [ForeignKey("cliente")]
        public Int32 IDCLIENTE { get; set; }
        public cliente cliente { get; set; }

        public static interaco Createinteraco(global::System.Int32 iDINTERACAO, global::System.String iNFORMACOES, 
            global::System.DateTime dATAINTERACAO, global::System.Int32 iDRECLAMACAO, global::System.Int32 iDCLIENTE)
        {
            interaco interaco = new interaco();
            interaco.IDINTERACAO = iDINTERACAO;
            interaco.INFORMACOES = iNFORMACOES;
            interaco.DATAINTERACAO = dATAINTERACAO;
            interaco.IDRECLAMACAO = iDRECLAMACAO;
            interaco.IDCLIENTE = iDCLIENTE;
            return interaco;
        }
    }
}