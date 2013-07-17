using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rdc.Models;
using rdc.App_Code;

namespace rdc.Controllers
{
    [Authorize(Roles = "ADM")]
    public class FornecedoresController : Controller
    {
        private sistemardcEntities db = new sistemardcEntities();

        //
        // GET: /Fornecedores/

        public ViewResult Index(string buscar = null)
        {
            //Alterado para comando pelo fato de ter passado a utilizar outra classe de "Cliente"(personalizada)
            //para obter o resultado da query.
            string qry = "select * from fornecedores order by razaosocial";
            if (!string.IsNullOrEmpty(buscar))
            {
                qry = "select * from fornecedores " +
                    " where upper(razaosocial) like upper('%" + buscar + "%')" +
                    " order by razaosocial";
            }

            IEnumerable<Fornecedor> Fornecedor = db.ExecuteStoreQuery<Fornecedor>(qry);
            return View(Fornecedor.ToList());
        }

        //
        // GET: /Fornecedores/Details/5

        public ViewResult Details(int id)
        {
            Fornecedor fornecedore = db.ExecuteStoreQuery<Fornecedor>("select * from fornecedores" +
                                                              " order by razaosocial").Single(c => c.IDFORNECEDOR == id);
            return View(fornecedore);
        }

        //GET: Ajax - Função para retornar as cidades de acordo com o estado selecionado
        public ActionResult ListaCidade(string id)
        {
            return Json(CidadeRepositorio.ListaCidade(id), JsonRequestBehavior.AllowGet);
        }


        //GET: Ajax - Função para retornar somente os dados do CEP
        public ActionResult GetCep(string id)
        {
            //Cria uma lista para retornar ao registro com o endereço completo
            List<TCEP> cepcompleto = new List<TCEP>();
            TCEP cepcomp;
            cepcomp = new TCEP();
            string scep = id;
            if (!string.IsNullOrEmpty(scep))
            {
                //Localizar o endereço do cliente através do CEP
                BuscaCep BscCEP = new BuscaCep();
                string strhtmlBuscaCEP = BscCEP.ConsultaCEP(scep);
                //Verificar se CEP encontrado para devolver informações para a tela
                if (!string.IsNullOrEmpty(strhtmlBuscaCEP))
                {

                    string[] values = strhtmlBuscaCEP.Split(new string[] { "<tr>", "</tr>", "<td>", "</td>" },StringSplitOptions.RemoveEmptyEntries);
                    //Lista o Endereco, Bairro, UF e CEP
                    List<string> listCEP = new List<string>(values);
                    //cliente.Endereço recebe Logradouro e Bairro
                    cepcomp.endereco = listCEP[0].Trim().ToString() + ", " + listCEP[1].Trim().ToString();
                    cepcomp.cidade = listCEP[2].Trim().ToString();
                    cepcomp.estado = listCEP[3].Trim().ToString();
                    cepcomp.cep = listCEP[4].Trim().ToString();
                    //Remover acento da cidade retornada caso haja
                    string input = listCEP[2].Trim().ToString();
                    byte[] bytes = System.Text.Encoding.GetEncoding("iso-8859-8").GetBytes(input);
                    input = System.Text.Encoding.UTF8.GetString(bytes);
                    //recuperar o IDCIDADE da tabela de Cidade - 
                    //Importante verificar a possibilidade de melhorar essa consulta abaixo - Thiago 22_05_2013
                    string command = "select * from cidades " +
                                    "where nomecidade = '" + input + "'" +
                                    "and idestado = '" + cepcomp.estado + "'";
                    IEnumerable<cidade> cidaderesult = db.ExecuteStoreQuery<cidade>(command);
                    var result = cidaderesult.ToList();
                    if (result.Count > 0)
                    {
                        cepcomp.cidade = result[0].IDCIDADE.ToString();
                        cepcompleto.Add(cepcomp);
                        return Json(cepcompleto[0], JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json("");

        }


        //
        // GET: /Fornecedores/Create

        public ActionResult Create()
        {
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text");
            ViewBag.IDCIDADE = new SelectList(string.Empty, "IDCIDADE", "NOMECIDADE");
            return View();
        } 

        //
        // POST: /Fornecedores/Create

        [HttpPost]
        public ActionResult Create(Fornecedor Fornecedor)
        {
            //Retirar não numéricos caso haja do CNPJ.
            string Fornecedor_CNPJ = Util.RemoveNaoNumericos(Fornecedor.cnpj);
            //Retornar CNPJ na tela com nome do Fornecedor caso já esteja cadastrado.
            fornecedore FORCNPJIGUAL = db.fornecedores.ToList().Find(F => F.cnpj == Fornecedor_CNPJ);
            if (FORCNPJIGUAL != null)
            {
                ModelState.AddModelError("", "Favor utilizar o Fornecedor " + FORCNPJIGUAL.razaosocial + " para Reclamar que já se encontra cadastrado e validado como Responsável por esse CNPJ!!");
            }
            if (ModelState.IsValid)
            {
                //Retirar não numéricos caso haja do CEP.
                string Fornecedor_CEP = Util.RemoveNaoNumericos(Fornecedor.cep);
                //Recuperar dados do Cliente dono da Reclamação que criou esse registro
                //OBS: Quando se cria nessa Action Create, é o Login e nas demais Actions serão o IDCLIENTE
                cliente CLIENTElOGIN = db.clientes.ToList().Find(x => x.LOGIN == User.Identity.Name);
                //Verificar se Fornecedor é Ativo para montar Email e enviá-lo, armazenar em informações e 
                //por último marcar o campo ENVEMAILCLISOL com 'S'
                Fornecedor.ENVEMAILCLISOL = "N";
                if (Fornecedor.auxativo)
                {
                    string Subemailclifor = "Novo Fornecedor Ativo no ReclameAgora";
                    string Bodmsemailclifor = 
                        "Prezado(a) <span color:Blue><b>" + CLIENTElOGIN.NOME.ToString() + "</b></span>,<br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        "Foi registrado Novo Fornecedor Ativo : <b>" + Fornecedor.razaosocial.ToString() + "</b> solicitado por você.<br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        "A partir desse momento, você pode entrar no Reclame Agora e inserir sua(s) Reclamação(ões) e ainda Interagir quando necessário : <br /><br />" +
                        "Para verificar e reclamar, acesse " +
                        "<a href=\"http://www.reclameagora.com.br/\" title=\"ReclameAgora\"> http://www.reclameagora.com.br/</a> <br /><br />" +
                        "Atenciosamente,<br />" +
                        "Equipe ReclameAgora";
                    //Início Chamada Enviar E-mail ao Cliente dono da Reclamação informando haver nova Interação
                    string retornoemail = SendMail.EnviaEmail(CLIENTElOGIN.EMAIL, CLIENTElOGIN.NOME, Subemailclifor, Bodmsemailclifor);
                    if (retornoemail != "NAOENVIADO")
                    {
                        Fornecedor.ENVEMAILCLISOL = "S";
                    }
                    //Fim da Chamada Enviar E-mail
                } 
                fornecedore fornecedore = Fornecedor.Createfornecedore(Fornecedor.IDFORNECEDOR, Fornecedor.razaosocial,
                    Fornecedor.nomefantasia, Fornecedor.email, Fornecedor_CNPJ, Fornecedor.endereco, Fornecedor_CEP,
                    Fornecedor.fonefixo, Fornecedor.fonefixo2, Fornecedor.ESTADO, Fornecedor.informacoes,
                    Fornecedor.IDCIDADE, Fornecedor.ativo, Fornecedor.homepage, 
                    CLIENTElOGIN.IDCLIENTE, Fornecedor.ENVEMAILCLISOL);
                db.fornecedores.AddObject(fornecedore);
                db.SaveChanges();

                return RedirectToAction("Index");  
            }
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text", Fornecedor.ESTADO);
            ViewBag.IDCIDADE = new SelectList(db.cidades.ToList().FindAll(x => x.IDESTADO == Fornecedor.ESTADO), "IDCIDADE", "NOMECIDADE", Fornecedor.IDCIDADE);
            return View(Fornecedor);
        }
        
        //
        // GET: /Fornecedores/Edit/5
 
        public ActionResult Edit(int id)
        {
            Fornecedor fornecedore = db.ExecuteStoreQuery<Fornecedor>("select * from fornecedores" +
                                                  " order by razaosocial").Single(c => c.IDFORNECEDOR == id);
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text", fornecedore.ESTADO);
            ViewBag.IDCIDADE = new SelectList(db.cidades.ToList().FindAll(x => x.IDESTADO == fornecedore.ESTADO), "IDCIDADE", "NOMECIDADE", fornecedore.IDCIDADE);
            return View(fornecedore);
        }

        //
        // POST: /Fornecedores/Edit/5

        [HttpPost]
        public ActionResult Edit(Fornecedor Fornecedor)
        {
            //Retirar não numéricos caso haja do CNPJ.
            string Fornecedor_CNPJ = Util.RemoveNaoNumericos(Fornecedor.cnpj);
            //Retornar CNPJ na tela com nome do Fornecedor caso já esteja cadastrado.
            Fornecedor FORCNPJIGUAL = db.ExecuteStoreQuery<Fornecedor>("select * from fornecedores" +
                                                  " order by razaosocial").ToList().Find(F => F.cnpj == Fornecedor_CNPJ);
            if (FORCNPJIGUAL != null)
            {
                ModelState.AddModelError("", "Favor utilizar o Fornecedor " + FORCNPJIGUAL.razaosocial + " para Reclamar que já se encontra cadastrado e validado como Responsável por esse CNPJ!!");
            }
            if (ModelState.IsValid)
            {
                //Retirar não numéricos caso haja do CPF e CEP.
                string Fornecedor_CEP = Util.RemoveNaoNumericos(Fornecedor.cep);
                //Recuperar dados do Cliente dono da Reclamação que criou esse registro
                //OBS: Quando se cria na Action Create, é o Login e nas demais Actions serão o IDCLIENTE como nesse caso.
                int for_idcliente = Fornecedor.IDCLIENTE.GetValueOrDefault(0);
                cliente CLIENTElOGIN = db.clientes.ToList().Find(x => x.IDCLIENTE == for_idcliente);
                //Verificar se Fornecedor é Ativo e ENVEMAILCLISOL == "N" para montar Email e enviá-lo, armazenar em informações e 
                //por último marcar o campo ENVEMAILCLISOL com 'S'
                if ((Fornecedor.auxativo) && (Fornecedor.ENVEMAILCLISOL == "N"))
                {
                    string Subemailclifor = "Novo Fornecedor Ativo no ReclameAgora";
                    string Bodmsemailclifor =
                        "Prezado(a) <span color:Blue><b>" + CLIENTElOGIN.NOME.ToString() + "</b></span>,<br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        "Foi registrado Novo Fornecedor como Ativo : <b>" + Fornecedor.razaosocial.ToString() + "</b> solicitado por você.<br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        "A partir desse momento, você pode entrar no Reclame Agora e inserir sua(s) Reclamação(ões) e ainda Interagir quando necessário. <br /><br />" +
                        "Para verificar e reclamar, acesse " +
                        "<a href=\"http://www.reclameagora.com.br/\" title=\"ReclameAgora\"> http://www.reclameagora.com.br/</a> <br /><br />" +
                        "Atenciosamente,<br />" +
                        "Equipe ReclameAgora";
                    //Início Chamada Enviar E-mail ao Cliente dono da Reclamação informando haver nova Interação
                    string retornoemail = SendMail.EnviaEmail(CLIENTElOGIN.EMAIL, CLIENTElOGIN.NOME, Subemailclifor, Bodmsemailclifor);
                    if (retornoemail != "NAOENVIADO")
                    {
                        Fornecedor.ENVEMAILCLISOL = "S";
                    }
                    //Fim da Chamada Enviar E-mail
                }
                fornecedore novofornecedore = Fornecedor.Createfornecedore(Fornecedor.IDFORNECEDOR, Fornecedor.razaosocial,
                    Fornecedor.nomefantasia, Fornecedor.email, Fornecedor_CNPJ, Fornecedor.endereco, Fornecedor_CEP,
                    Fornecedor.fonefixo, Fornecedor.fonefixo2, Fornecedor.ESTADO, Fornecedor.informacoes, 
                    Fornecedor.IDCIDADE, Fornecedor.ativo, Fornecedor.homepage,
                    Fornecedor.IDCLIENTE,Fornecedor.ENVEMAILCLISOL);
                db.fornecedores.Attach(novofornecedore);
                db.ObjectStateManager.ChangeObjectState(novofornecedore, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text", Fornecedor.ESTADO);
            ViewBag.IDCIDADE = new SelectList(db.cidades.ToList().FindAll(x => x.IDESTADO == Fornecedor.ESTADO), "IDCIDADE", "NOMECIDADE", Fornecedor.IDCIDADE);
            return View(Fornecedor);
        }

        //
        // GET: /Fornecedores/Delete/5
 
        public ActionResult Delete(int id)
        {
            Fornecedor fornecedore = db.ExecuteStoreQuery<Fornecedor>("select * from fornecedores" +
                                                              " order by razaosocial").Single(c => c.IDFORNECEDOR == id);
            return View(fornecedore);
        }

        //
        // POST: /Fornecedores/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Fornecedor Fornecedor = db.ExecuteStoreQuery<Fornecedor>("select * from fornecedores" +
                                                              " order by razaosocial").Single(c => c.IDFORNECEDOR == id);
            fornecedore fornecedore = Fornecedor.Createfornecedore(Fornecedor.IDFORNECEDOR, Fornecedor.razaosocial,
                    Fornecedor.nomefantasia, Fornecedor.email, Fornecedor.cnpj, Fornecedor.endereco, Fornecedor.cep,
                    Fornecedor.fonefixo, Fornecedor.fonefixo2, Fornecedor.ESTADO, Fornecedor.informacoes, 
                    Fornecedor.IDCIDADE, Fornecedor.ativo, Fornecedor.homepage,
                    Fornecedor.IDCLIENTE,Fornecedor.ENVEMAILCLISOL);
            db.fornecedores.Attach(fornecedore);
            db.fornecedores.DeleteObject(fornecedore);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.MsgDB = "Erro: Registro não pode ser excluído por estar relacionado a outras informações no sistema." +
                    " Segue erro do sistema: " + (e.InnerException).Message.ToString();
                return View(Fornecedor);
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}