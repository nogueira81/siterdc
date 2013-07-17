using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace rdc.Models
{
    public class Cidade 
    { 
        public Cidade(string id, string siglaUf, string nome) 
        { 
            this.Id = id; 
            this.SiglaUf = siglaUf; 
            this.Nome = nome; 
        } 
        public string Id { get; set; } 
        public string SiglaUf { get; set; } 
        public string Nome { get; set; } 
    }

    public class CidadeList
    {
        public CidadeList(string Value, string Text)
        {
            this.Value = Value;
            this.Text = Text;
        }
        public string Value { get; set; }
        public string Text { get; set; }
    } 
}