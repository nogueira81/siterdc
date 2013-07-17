using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using rdc.Models;

namespace rdc.Models
{
    public class HomeModel
    {
        public List<HomeModelCli> HomeModelCli { get; set; }
        public List<HomeModelFor> HomeModelFor { get; set; }
    }

    public class HomeModelCli
    {
        public string est_cli { get; set; }

        public string login { get; set; }

        public string idcliente { get; set; }

        public int qtderec { get; set; }

        public int qtderecn { get; set; }

        public int qtderecs { get; set; }

        public string fornecedor { get; set; }
    }

    public class HomeModelFor
    {
        public string est_for { get; set; }

        //public string login { get; set; }

        public string idcliente { get; set; }

        public int qtderec { get; set; }

        public int qtderecn { get; set; }

        public int qtderecs { get; set; }

        public string nomefantasia { get; set; }
    }

    public class HomeModelAvgtdFor
    {

        public string idfornecedor { get; set; }

        public int qtderec { get; set; }

        public int avgdiasatend { get; set; }

        public string nomefantasia { get; set; }
    }
 

}