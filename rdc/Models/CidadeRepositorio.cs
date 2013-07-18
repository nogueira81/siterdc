using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace rdc.Models
{
    public class CidadeRepositorio
    {
        public static IList<Cidade> ListaCidade(string SiglaUf) 
        {
            sistemardcEntities db = new sistemardcEntities();
            IEnumerable<cidade> cidadesuf = db.cidades.Where(x => x.IDESTADO == SiglaUf).ToList();
            List<Cidade> cidades = new List<Cidade>();
            //Preencher o List com as cidades localizadas - ex: cidades.Add(new Cidade("7", "MG", "Belo Horizonte")); 
            foreach (var item in cidadesuf)
            {
                cidades.Add(new Cidade(item.IDCIDADE.ToString(), item.IDESTADO, item.NOMECIDADE)); 
            }
            db.Dispose();
            return cidades.ToList(); //.Where(x => x.SiglaUf == SiglaUf).ToList(); 
        }
        //Criada para retornar todas as cidades do tabela no banco de dados
        public static IList<Cidade> ListaCidade()
        {
            sistemardcEntities db = new sistemardcEntities();
            IEnumerable<cidade> cidadesuf = db.cidades.ToList();
            List<Cidade> cidades = new List<Cidade>();
            //Preencher o List com as cidades localizadas - ex: cidades.Add(new Cidade("7", "MG", "Belo Horizonte")); 
            foreach (var item in cidadesuf)
            {
                cidades.Add(new Cidade(item.IDCIDADE.ToString(), item.IDESTADO, item.NOMECIDADE));
            }
            db.Dispose();
            return cidades.ToList();
        }
        //Listagem abaixo criada somente para utilizar nas views que necessitam de SelectItem com "Text" e "Value"
        //como nome dos campos.
        public static IList<CidadeList> ListaCidades()
        {
            sistemardcEntities db = new sistemardcEntities();
            IEnumerable<cidade> cidadesuf = db.cidades.ToList();
            List<CidadeList> cidades = new List<CidadeList>();
            //Preencher o List com as cidades localizadas
            foreach (var item in cidadesuf)
            {
                cidades.Add(new CidadeList(item.IDCIDADE.ToString(), item.NOMECIDADE));
            }
            db.Dispose();
            return cidades.ToList();
        }
    }
}