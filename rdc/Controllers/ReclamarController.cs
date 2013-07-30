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
    public class ReclamarController : Controller
    {
        private sistemardcEntities db = new sistemardcEntities();

        //GET: Ajax - Função para retornar os fornecedores de acordo com o filtro do idfor
        public ActionResult listaFornec(string id)
        {
            return Json(FiltraFornec.ListaFornec(id), JsonRequestBehavior.AllowGet);
        }


        //
        // GET: /Reclamar/

        public ViewResult Index()
        {
            if (TempData["msgsolfor"] != null)
            {
                ViewBag.msgsolfor = TempData["msgsolfor"];
            }
            TempData["msgsolfor"] = null;
            if (TempData["msgcadrec"] != null)
            {
                ViewBag.msgcadrec = TempData["msgcadrec"];
            }
            TempData["msgcadrec"] = null;
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores.ToList().FindAll(f => f.ativo != "N"), "IDFORNECEDOR", "razaosocial");
            return View();
        }

        [HttpPost]
        public ActionResult Index(Reclamacao Reclamacao)
        {
            if (ModelState.IsValid)
            {
                //Selecionar o Cliente Logado no sistema
                Reclamacao.IDCLIENTE = db.clientes.ToList().Find(x => x.LOGIN == User.Identity.Name).IDCLIENTE;
                //Realizar a conversão para o objeto do modelrdc.Designer.cs antes de gravar no banco
                reclamaco novoreclamaco = Reclamacao.Createreclamaco(Reclamacao.idreclamacao, Reclamacao.titulo,
                    Reclamacao.descricao, Reclamacao.tiposolucao, Reclamacao.IDCLIENTE, Reclamacao.IDFORNECEDOR,
                    Reclamacao.datacompra, Reclamacao.fonecontato);
                db.reclamacoes.AddObject(novoreclamaco);
                db.SaveChanges();
                TempData["msgcadrec"] = "Reclamação registrada com sucesso e enviada ao seu e-mail!\r\n" +
                                        "Para verificar o andamento da sua Reclamação, clique ";
                //Início Chamada Enviar E-mail ao Cliente dono da Reclamação informando haver nova Interação
                //Recuperar dados do Cliente dono da Reclamação que originou essa interação
                Reclamacao.cliente = db.clientes.First(x => x.IDCLIENTE == Reclamacao.IDCLIENTE);
                string Subemailclifor = "Nova Reclamação registrada no ReclameAgora";
                string Bodmsemailclifor =
                    "Prezado(a) <span color:Blue><b>" + Reclamacao.cliente.NOME.ToString() + "</b></span>,<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Foi registrada sua Reclamação : <b>" + Reclamacao.titulo.ToString() + "</b>.<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "O Nome do Fornecedor informado é : <b>" + novoreclamaco.fornecedore.razaosocial.ToString() + "</b>.<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Foi inserida a seguinte Interação em sua Reclamação : <br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    Reclamacao.descricao.ToString() + "<br /><br />" +
                    "Para verificar, acesse " +
                    "<a href=\"http://reclameagora.apphb.com/\" title=\"ReclameAgora\"> http://reclameagora.apphb.com/</a> <br /><br />" +
                    "Atenciosamente,<br />" +
                    "Equipe ReclameAgora";
                //Início Chamada Enviar E-mail ao Cliente dono da Reclamação informando haver nova Interação
                //Envio ao Cliente
                SendMail.EnviaEmail(Reclamacao.cliente.EMAIL, Reclamacao.cliente.NOME, Subemailclifor, Bodmsemailclifor);
                //Fim da Chamada Enviar E-mail
                //Envio ao Fornecedor informando havernova interação
                //Recuperar dados do Cliente dono da Reclamação que originou essa interação
                novoreclamaco.fornecedore = db.fornecedores.First(x => x.IDFORNECEDOR == novoreclamaco.IDFORNECEDOR);
                string Subemailfor = "Nova Reclamação registrada no ReclameAgora para Fornecedor";
                string Bodmsemailfor =
                    "Prezado Fornecedor <span color:Blue><b>" + novoreclamaco.fornecedore.razaosocial.ToString() + "</b></span>,<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Foi registrada a Reclamação : <b>" + Reclamacao.titulo.ToString() + "</b>.<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Essa Reclamação foi registrada pelo Cliente  : <b>" + Reclamacao.cliente.NOME.ToString() + "</b>.<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Foi inserida a seguinte Interação em sua Reclamação : <br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    Reclamacao.descricao.ToString() + "<br /><br />" +
                    "Para verificar, acesse " +
                    "<a href=\"http://reclameagora.apphb.com/\" title=\"ReclameAgora\"> http://reclameagora.apphb.com/</a> <br /><br />" +
                    "Atenciosamente,<br />" +
                    "Equipe ReclameAgora";
                SendMail.EnviaEmail(novoreclamaco.fornecedore.email, novoreclamaco.fornecedore.razaosocial,
                    Subemailfor, Bodmsemailfor);
                //Fim da Chamada Enviar E-mail
                return RedirectToAction("Index");  
            }

            //ViewBag.IDCLIENTE = new SelectList(db.clientes, "IDCLIENTE", "NOME", Reclamacao.IDCLIENTE);
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores.ToList().FindAll(f => f.ativo != "N"), "IDFORNECEDOR", "razaosocial", Reclamacao.IDFORNECEDOR);
            return View(Reclamacao);
        }

        public ViewResult RecInterIndex()
        {
            TempData["actionname"] = "RecInterIndex";
            cliente cliente = db.clientes.ToList().Find(x => x.LOGIN == User.Identity.Name);
            string qry = "select distinct r.* from reclamacoes r " +
                "inner join interacoes i on r.idreclamacao = i.idreclamacao " +
                "where i.idcliente = " + cliente.IDCLIENTE + " "+
                "and r.idcliente <> i.idcliente " +
                "order by i.datainteracao desc";
            //Filtrar Reclamações de Outros Clientes que o usuário logado Interagiu.
            //Portanto, não pode ter Reclamação do Cliente logado.
            IEnumerable<Reclamacao> Reclamacao = db.ExecuteStoreQuery<Reclamacao>(qry);//.ToList().FindAll(x => x.IDCLIENTE != cliente.IDCLIENTE);
            return View(Reclamacao.ToList());
        }
        
        public ViewResult RecIndex()
        {
            TempData["actionname"] = "RecIndex";
            cliente cliente = db.clientes.ToList().Find(x => x.LOGIN == User.Identity.Name);
            string qry = "select * from reclamacoes order by datacompra desc";
            IEnumerable<Reclamacao> Reclamacao = db.ExecuteStoreQuery<Reclamacao>(qry).ToList().FindAll(x => x.IDCLIENTE==cliente.IDCLIENTE);
            //var reclamacoes = db.reclamacoes.Include("cliente").Include("fornecedore").ToList().FindAll(x => x.IDCLIENTE==cliente.IDCLIENTE);
            //.Include("cliente").Include("fornecedore");
            return View(Reclamacao.ToList());
        }

        //
        // GET: /Reclamar/Details/5

        public ViewResult Details(int id)
        {
            Reclamacao reclamaco = db.ExecuteStoreQuery<Reclamacao>("select * from reclamacoes" +
                                                  " order by titulo").Single(c => c.idreclamacao == id);
            //Recuperar o registro do Cliente
            reclamaco.cliente = db.clientes.First(c => c.IDCLIENTE == reclamaco.IDCLIENTE);
            //Recupera o registro do Fornecedor
            reclamaco.fornecedore = db.fornecedores.First(c => c.IDFORNECEDOR == reclamaco.IDFORNECEDOR);
            return View(reclamaco);
        }

        //
        // GET: /Reclamar/Details/5

        public ViewResult DetRecInt(int id)
        {
            Reclamacao reclamaco = db.ExecuteStoreQuery<Reclamacao>("select * from reclamacoes" +
                      " order by titulo").Single(c => c.idreclamacao == id);
            //Recuperar 1 registro do Cliente
            reclamaco.cliente = db.clientes.First(c => c.IDCLIENTE == reclamaco.IDCLIENTE);
            //Recupera 1 registro do Fornecedor
            reclamaco.fornecedore = db.fornecedores.First(c => c.IDFORNECEDOR == reclamaco.IDFORNECEDOR);
            //Recupera os registros de interações caso existam para cada reclamação
            reclamaco.Interacoes = db.ExecuteStoreQuery<interaco>("select * from interacoes" +
                          " order by DATAINTERACAO desc").ToList().FindAll(c => c.IDRECLAMACAO == id);
            //Percorrer todas as Interações dessa reclamação e preencher o Cliente com todos os dados
            foreach (var item in reclamaco.Interacoes)
            {
                item.cliente = db.clientes.First(c => c.IDCLIENTE == item.IDCLIENTE);
                item.reclamaco.cliente.LOGIN = reclamaco.cliente.LOGIN;
            }
            reclamaco.Interacao = new Interacao();
            reclamaco.Interacao.IDRECLAMACAO = id;
            
            return View(reclamaco);
        }

        public ActionResult RecInteracoes(int id)
        {
            Reclamacao reclamaco = db.ExecuteStoreQuery<Reclamacao>("select * from reclamacoes" +
                      " order by titulo").Single(c => c.idreclamacao == id);
            reclamaco.Interacoes = db.ExecuteStoreQuery<interaco>("select * from interacoes" +
                                      " order by DATAINTERACAO desc").ToList().FindAll(c => c.IDRECLAMACAO == id);
            //Interacao.cliente = db.clientes.Single(x => x.IDCLIENTE == Interacao.IDCLIENTE);
            return PartialView(reclamaco);
        }

        public ActionResult CreateInteracao(Interacao Interacao, string actionname = null)
        {
            TempData["msgIntReclamar"] = "False";
            if (ModelState.IsValid)
            {
                //Converter Interacao em interaco
                interaco interaco = Interacao.Createinteraco(Interacao.IDINTERACAO, Interacao.INFORMACOES,
                                              Interacao.DATAINTERACAO, Interacao.IDRECLAMACAO, Interacao.IDCLIENTE);
                //Rotina para inserir no interação
                interaco.IDCLIENTE = db.clientes.ToList().Find(x => x.LOGIN == User.Identity.Name).IDCLIENTE;
                //Realizar a conversão para o objeto do modelrdc.Designer.cs antes de gravar no banco
                db.interacoes.AddObject(interaco);
                db.SaveChanges();

                //Início Chamada Enviar E-mail ao Cliente dono da Reclamação informando haver nova Interação
                //Recuperar Reclamação dessa Interação
                interaco.reclamaco = db.reclamacoes.First(x => x.idreclamacao == Interacao.IDRECLAMACAO);
                //Recuperar dados do Cliente dono da Reclamação que originou essa interação
                interaco.cliente = db.clientes.First(x => x.IDCLIENTE == interaco.reclamaco.IDCLIENTE);
                string Subemailclifor = "Retorno da sua Reclamação no ReclameAgora";
                string Bodmsemailclifor =
                    "Prezado(a) <span color:Blue><b>" + interaco.cliente.NOME.ToString() + "</b></span>,<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Favor verificar sua Reclamação : <b>" + interaco.reclamaco.titulo.ToString() + "</b>.<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Foi inserida a seguinte Interação em sua Reclamação : <br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    Interacao.INFORMACOES.ToString() + "<br /><br />" +
                    "Para verificar, acesse " +
                    "<a href=\"http://reclameagora.apphb.com/\" title=\"ReclameAgora\"> http://reclameagora.apphb.com/</a> <br /><br />" +
                    "Atenciosamente,<br />" +
                    "Equipe ReclameAgora";
                //Início Chamada Enviar E-mail ao Cliente dono da Reclamação informando haver nova Interação
                //Envio ao Cliente
                SendMail.EnviaEmail(interaco.cliente.EMAIL, interaco.cliente.NOME, Subemailclifor, Bodmsemailclifor);
                //Envio ao Fornecedor informando havernova interação
                //Recuperar dados do Cliente dono da Reclamação que originou essa interação
                interaco.reclamaco.fornecedore = db.fornecedores.First(x => x.IDFORNECEDOR == interaco.reclamaco.IDFORNECEDOR);
                string Subemailfor = "Retorno da sua Reclamação no ReclameAgora";
                string Bodmsemailfor =
                    "Prezado Fornecedor <span color:Blue><b>" + interaco.reclamaco.fornecedore.razaosocial.ToString() + "</b></span>,<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Favor verificar a Reclamação : <b>" + interaco.reclamaco.titulo.ToString() + "</b>" +
                    " registrada pelo Cliente : <b>" + interaco.cliente.NOME.ToString() + "</b> sobre a sua Empresa.<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Foi inserida a seguinte Interação nessa Reclamação : <br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    Interacao.INFORMACOES.ToString() + "<br /><br />" +
                    "Para verificar, acesse " +
                    "<a href=\"http://reclameagora.apphb.com/\" title=\"ReclameAgora\"> http://reclameagora.apphb.com/</a> <br /><br />" +
                    "Atenciosamente,<br />" +
                    "Equipe ReclameAgora";
                SendMail.EnviaEmail(interaco.reclamaco.fornecedore.email, interaco.reclamaco.fornecedore.razaosocial, 
                    Subemailfor, Bodmsemailfor);
                //Fim da Chamada Enviar E-mail

                TempData["msgIntReclamar"] = "True";
            }
            TempData["actionname"] = actionname;
            return RedirectToAction("DetRecInt", new { id = Interacao.IDRECLAMACAO });
        }

        //
        // GET: /Reclamar/CreateFor
        // Esse controller do Create é para solicitar a criação de um novo Fornecedor no sistema 
        //caso o usuário não o localize
        public ActionResult CreateFor()
        {
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text");
            ViewBag.IDCIDADE = new SelectList(string.Empty, "IDCIDADE", "NOMECIDADE");
            return View();
        } 
        
        //
        // POST: /Reclamar/CreateFor

        [HttpPost]
        public ActionResult CreateFor(SolFornecedor Fornecedor)
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
                //Completar com as informações obrigatórias(ativo,razaosocial,email)
                Fornecedor.ativo = "N";
                if (string.IsNullOrEmpty(Fornecedor.razaosocial))
                {
                    Fornecedor.razaosocial = Fornecedor.nomefantasia;
                }
                Fornecedor.email = "verificarfornecedor@fornecedor.com.br";
                //Retirar não numéricos caso haja do CEP.
                string Fornecedor_CEP = Util.RemoveNaoNumericos(Fornecedor.cep);
                //Setar Email como não enviado, pois essa tarefa somente poderá ser realizada pelo ADM
                //do sistema após a checagem da solicitação do registro e atribuir o novo Fornecedor como Ativo.
                Fornecedor.ENVEMAILCLISOL = "N";
                //Recuperar IDCLIENTE do Login
                cliente CLIENTElOGIN = db.clientes.ToList().Find(x => x.LOGIN == User.Identity.Name);
                fornecedore fornecedore = SolFornecedor.Createfornecedore(Fornecedor.IDFORNECEDOR, Fornecedor.razaosocial,
                    Fornecedor.nomefantasia, Fornecedor.email, Fornecedor_CNPJ, Fornecedor.endereco, Fornecedor_CEP,
                    Fornecedor.fonefixo, Fornecedor.fonefixo2, Fornecedor.ESTADO, Fornecedor.informacoes,
                    Fornecedor.IDCIDADE, Fornecedor.ativo, Fornecedor.homepage, 
                    CLIENTElOGIN.IDCLIENTE, Fornecedor.ENVEMAILCLISOL);
                db.fornecedores.AddObject(fornecedore);
                db.SaveChanges();
                //Chamada Enviar E-mail Cliente
                string Sender = System.Configuration.ConfigurationManager.AppSettings["ToSuporte"].ToString();
                string NameSender = System.Configuration.ConfigurationManager.AppSettings["NameToSuporte"].ToString();
                string Subemailclifor = "Solicitação de Criação de Novo Fornecedor no ReclameAgora";
                string Bodmsemailclifor =
                    "Prezado(a) <span color:Blue><b>" + NameSender + "</b></span>,<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Foi solicitado a criação de um novo Fornecedor : <b>" + Fornecedor.razaosocial.ToString() + 
                    "</b> para inserir uma nova Reclamação. <br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "O CNPJ informado é : <b>" + Fornecedor.cnpj.ToString() + "</b>.<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "O Fornecedor ficará ATIVO após seu processamento pela Equipe do ReclameAgora e então será lhe enviado um novo E-Mail para realizar sua Reclamação. <b><br /><br />" + 
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Para verificar, acesse " +
                    "<a href=\"http://reclameagora.apphb.com/\" title=\"ReclameAgora\"> http://reclameagora.apphb.com/</a> <br /><br />" +
                    "Atenciosamente,<br />" +
                    "Equipe ReclameAgora";
                //Enviar para o Email do Web.config
                SendMail.EnviaEmail(Sender, NameSender, Subemailclifor, Bodmsemailclifor);
                //Agora, enviar E-Mail para o Cliente
                Sender = CLIENTElOGIN.EMAIL;
                NameSender = CLIENTElOGIN.NOME;
                SendMail.EnviaEmail(Sender, NameSender, Subemailclifor, Bodmsemailclifor);
                //Fim da Chamada Enviar E-mail
                TempData["msgsolfor"] = "Solicitação de Cadastro de Fornecedor relizada com sucesso!\r\n"+
                                        "Aguarde o processamento do novo Fornecedor para realizar sua Reclamação.";
                return RedirectToAction("Index");
            }
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text", Fornecedor.ESTADO);
            ViewBag.IDCIDADE = new SelectList(db.cidades.ToList().FindAll(x => x.IDESTADO == Fornecedor.ESTADO), "IDCIDADE", "NOMECIDADE", Fornecedor.IDCIDADE);
            return View(Fornecedor);
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

                    string[] values = strhtmlBuscaCEP.Split(new string[] { "<tr>", "</tr>", "<td>", "</td>" }, StringSplitOptions.RemoveEmptyEntries);
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
        // GET: /Reclamar/Edit/5
 
        public ActionResult Edit(int id)
        {
            Reclamacao reclamaco = db.ExecuteStoreQuery<Reclamacao>("select * from reclamacoes" +
                                                              " order by titulo").Single(c => c.idreclamacao == id);
            TempData["TipSolRecdb"] = reclamaco.tiposolucao;
            //Recuperar o registro do Cliente
            reclamaco.cliente = db.clientes.First(c => c.IDCLIENTE == reclamaco.IDCLIENTE);
            //ViewBag.IDCLIENTE = new SelectList(db.clientes, "IDCLIENTE", "NOME", reclamaco.IDCLIENTE);
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores.ToList().FindAll(f => f.ativo != "N"), "IDFORNECEDOR", "razaosocial", reclamaco.IDFORNECEDOR);
            return View(reclamaco);
            /*
            reclamaco reclamaco = db.reclamacoes.Single(r => r.idreclamacao == id);
            ViewBag.IDCLIENTE = new SelectList(db.clientes, "IDCLIENTE", "NOME", reclamaco.IDCLIENTE);
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores, "IDFORNECEDOR", "razaosocial", reclamaco.IDFORNECEDOR);
            return View(reclamaco);
             */
        }

        //
        // POST: /Reclamar/Edit/5

        [HttpPost]
        public ActionResult Edit(Reclamacao Reclamacao)
        {
            if ((Reclamacao.tiposolucao == "C" || Reclamacao.tiposolucao == "I") && Reclamacao.datasolucao == null)
            {
                ModelState.AddModelError("", "Favor informar a Data da Solução ao Encerrá-la!!");
            } 
            else if ((Reclamacao.tiposolucao == "C" || Reclamacao.tiposolucao == "I") && (Reclamacao.datasolucao < Reclamacao.datacompra) )
            {
                ModelState.AddModelError("", "Data da Solução tem que ser maior ou igual a Data da Compra!!");
            }
            if (ModelState.IsValid)
            {
                if (Reclamacao.tiposolucao == "N" && Reclamacao.datasolucao != null)
                {
                    Reclamacao.datasolucao = null;
                }
                reclamaco novoreclamaco = Reclamacao.Createreclamaco(Reclamacao.idreclamacao, Reclamacao.titulo,
                    Reclamacao.descricao, Reclamacao.tiposolucao, Reclamacao.IDCLIENTE, Reclamacao.IDFORNECEDOR,
                    Reclamacao.datacompra, Reclamacao.fonecontato, Reclamacao.datasolucao);
                db.reclamacoes.Attach(novoreclamaco);
                db.ObjectStateManager.ChangeObjectState(novoreclamaco, EntityState.Modified);
                db.SaveChanges();
                //Início Chamada Enviar E-mail ao Cliente dono da Reclamação informando haver alteração no Staus da Rec
                //Recuperar Descrição do Tipo da Solução
                string TipSolRecdb = TempData["TipSolRecdb"].ToString();
                if (TipSolRecdb != Reclamacao.tiposolucao)
                {
                    string desctiposolucaodb = null;
                    if (TipSolRecdb == "I")
                        desctiposolucaodb = "Resolvido e Usuário Insatisfeito";
                    else if (TipSolRecdb == "C")
                        desctiposolucaodb = "Resolvido e Usuário Satisfeito";
                    else
                        desctiposolucaodb = "Sem Solução do Fornecedor";
                    string desctiposolucao = null;
                    if (Reclamacao.tiposolucao == "I")
                        desctiposolucao = "Resolvido e Usuário Insatisfeito";
                    else if (Reclamacao.tiposolucao == "C")
                        desctiposolucao = "Resolvido e Usuário Satisfeito";
                    else
                        desctiposolucao = "Sem Solução do Fornecedor";
                    //Recuperar dados do Cliente dono da Reclamação que originou essa interação
                    Reclamacao.cliente = db.clientes.First(x => x.IDCLIENTE == Reclamacao.IDCLIENTE);
                    //Chamada Enviar E-mail Cliente
                    string Sender = Reclamacao.cliente.EMAIL;
                    string NameSender = Reclamacao.cliente.NOME;
                    string Subemailclifor = "Alteração do Tipo da Solução de sua Reclamação no ReclameAgora";
                    string Bodmsemailclifor =
                        "Prezado(a) <span color:Blue><b>" + NameSender.ToString() + "</b></span>,<br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        "A Reclamação : <b>" + Reclamacao.titulo.ToString() + "</b>.<br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        "Com a Interação principal dessa Reclamação : <br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        Reclamacao.descricao.ToString() + "<br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        "Teve seu Tipo de Solução alterado de <b>" + desctiposolucaodb +
                        "</b> para <b>" + desctiposolucao + "</b>. <br /><br />" +
                        "Para verificar, acesse " +
                        "<a href=\"http://reclameagora.apphb.com/\" title=\"ReclameAgora\"> http://reclameagora.apphb.com/</a> <br /><br />" +
                        "Atenciosamente,<br />" +
                        "Equipe ReclameAgora";
                    //Enviar para o Email do Web.config
                    SendMail.EnviaEmail(Sender, NameSender, Subemailclifor, Bodmsemailclifor);
                    //Avisar ao Fornecedor que a Reclamação teve seu Status Alterado
                    Sender = novoreclamaco.fornecedore.email;
                    NameSender = novoreclamaco.fornecedore.razaosocial;
                    Bodmsemailclifor =
                        "Prezado Fornecedor <span color:Blue><b>" + NameSender.ToString() + "</b></span>,<br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        "A Reclamação : <b>" + Reclamacao.titulo.ToString() + "</b>.<br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        "Com a Interação principal dessa Reclamação : <br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        Reclamacao.descricao.ToString() + "<br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        "Teve seu Tipo de Solução alterado de <b>" + desctiposolucaodb +
                        "</b> para <b>" + desctiposolucao + "</b>. <br /><br />" +
                        "Para verificar, acesse " +
                        "<a href=\"http://reclameagora.apphb.com/\" title=\"ReclameAgora\"> http://reclameagora.apphb.com/</a> <br /><br />" +
                        "Atenciosamente,<br />" +
                        "Equipe ReclameAgora";
                    SendMail.EnviaEmail(Sender, NameSender, Subemailclifor, Bodmsemailclifor);
                    //Fim da Chamada Enviar E-mail
                }
                //Retornar com o valor nullo para essa variável
                TempData["TipSolRecdb"] = null;
                return RedirectToAction("RecIndex");
            }
            //ViewBag.IDCLIENTE = new SelectList(db.clientes, "IDCLIENTE", "NOME", Reclamacao.IDCLIENTE);
            //Recuperar o registro do Cliente
            Reclamacao.cliente = db.clientes.First(c => c.IDCLIENTE == Reclamacao.IDCLIENTE);
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores.ToList().FindAll(f => f.ativo != "N"), "IDFORNECEDOR", "razaosocial", Reclamacao.IDFORNECEDOR);
            //Retornar com o valor nullo para essa variável
            TempData["TipSolRecdb"] = null;
            return View(Reclamacao);
        }

        // Esse controler não irá possui opção de exclusão
        // GET: /Reclamar/Delete/5
        /*
        public ActionResult Delete(int id)
        {
            reclamaco reclamaco = db.reclamacoes.Single(r => r.idreclamacao == id);
            return View(reclamaco);
        }

        //
        // POST: /Reclamar/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            reclamaco reclamaco = db.reclamacoes.Single(r => r.idreclamacao == id);
            db.reclamacoes.DeleteObject(reclamaco);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        */

        [ActionName("DeleteInteracao")]
        public ActionResult DeleteInteracaoConfirmed(int id, string actionname = null)
        {
            TempData["actionname"] = actionname;
            interaco Interacoes = db.ExecuteStoreQuery<interaco>("select * from interacoes" +
                                                              " order by DATAINTERACAO").Single(c => c.IDINTERACAO == id);
            int idreclamacao = Interacoes.IDRECLAMACAO;
            db.interacoes.Attach(Interacoes);
            db.interacoes.DeleteObject(Interacoes);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.MsgDB = "Erro: Registro não pode ser excluído por estar relacionado a outras informações no sistema." +
                    " Segue erro do sistema: " + (e.InnerException).Message.ToString();
                return RedirectToAction("DetRecInt", new { id = idreclamacao });
            }
            return RedirectToAction("DetRecInt", new { id = idreclamacao });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}