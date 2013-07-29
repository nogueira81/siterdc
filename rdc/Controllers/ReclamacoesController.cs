using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rdc.Models;
using System.Data.Objects.DataClasses;
using System.Data.Objects;
using rdc.App_Code;

namespace rdc.Controllers
{
    [Authorize(Roles = "ADM")]
    public class ReclamacoesController : Controller
    {
        private sistemardcEntities db = new sistemardcEntities();

        //
        // GET: /reclamacoes/

        public ViewResult Index(string buscar = null)
        {
            //Alterado para comando pelo fato de ter passado a utilizar outra classe de "Cliente"(personalizada)
            //para obter o resultado da query.
            string qry = "select * from reclamacoes order by datacompra desc";
            if (!string.IsNullOrEmpty(buscar))
            {
                qry = "select * from reclamacoes " +
                    " where upper(titulo) like upper('%" + buscar + "%')" +
                    " order by datacompra desc";
            }

            IEnumerable<Reclamacao> Reclamacao = db.ExecuteStoreQuery<Reclamacao>(qry);
                        
            return View(Reclamacao.ToList());
        }

        //
        // GET: /reclamacoes/Details/5

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
        // GET: /reclamacoes/Create

        public ActionResult Create()
        {
            ViewBag.IDCLIENTE = new SelectList(db.clientes, "IDCLIENTE", "NOME");
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores, "IDFORNECEDOR", "razaosocial");
            return View();
        } 

        //
        // POST: /reclamacoes/Create

        [HttpPost]
        public ActionResult Create(Reclamacao Reclamacao)
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
                return RedirectToAction("Index");  
            }

            //ViewBag.IDCLIENTE = new SelectList(db.clientes, "IDCLIENTE", "NOME", Reclamacao.IDCLIENTE);
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores, "IDFORNECEDOR", "razaosocial", Reclamacao.IDFORNECEDOR);
            return View(Reclamacao);
        }
        
        //
        // GET: /reclamacoes/Edit/5
 
        public ActionResult Edit(int id)
        {
            Reclamacao reclamaco = db.ExecuteStoreQuery<Reclamacao>("select * from reclamacoes" +
                                                                          " order by titulo").Single(c => c.idreclamacao == id);
            TempData["TipSolRecdb"] = reclamaco.tiposolucao;
            //Recuperar o registro do Cliente
            reclamaco.cliente = db.clientes.First(c => c.IDCLIENTE == reclamaco.IDCLIENTE);
            //ViewBag.IDCLIENTE = new SelectList(db.clientes, "IDCLIENTE", "NOME", reclamaco.IDCLIENTE);
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores, "IDFORNECEDOR", "razaosocial", reclamaco.IDFORNECEDOR);
            return View(reclamaco);
        }

        //
        // POST: /reclamacoes/Edit/5

        [HttpPost]
        public ActionResult Edit(Reclamacao Reclamacao)
        {
            if ((Reclamacao.tiposolucao == "C" || Reclamacao.tiposolucao == "I") && Reclamacao.datasolucao == null)
            {
                ModelState.AddModelError("", "Favor informar a Data da Solução ao Encerrá-la!!");
            }
            else if ((Reclamacao.tiposolucao == "C" || Reclamacao.tiposolucao == "I") && (Reclamacao.datasolucao < Reclamacao.datacompra))
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
                return RedirectToAction("Index");
            }
            //ViewBag.IDCLIENTE = new SelectList(db.clientes, "IDCLIENTE", "NOME", Reclamacao.IDCLIENTE);
            //Recuperar o registro do Cliente
            Reclamacao.cliente = db.clientes.First(c => c.IDCLIENTE == Reclamacao.IDCLIENTE);
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores.ToList().FindAll(f => f.ativo != "N"), "IDFORNECEDOR", "razaosocial", Reclamacao.IDFORNECEDOR);
            TempData["TipSolRecdb"] = null;
            return View(Reclamacao);
        }

        //
        // GET: /reclamacoes/Delete/5
 
        public ActionResult Delete(int id)
        {
            Reclamacao reclamaco = db.ExecuteStoreQuery<Reclamacao>("select * from reclamacoes" +
                                                                          " order by titulo").Single(c => c.idreclamacao == id);
            //Recuperar o registro do Cliente
            reclamaco.cliente = db.clientes.First(c => c.IDCLIENTE == reclamaco.IDCLIENTE);
            //Recuperar o registro do Fornecedor
            reclamaco.fornecedore = db.fornecedores.First(c => c.IDFORNECEDOR == reclamaco.IDFORNECEDOR);
            return View(reclamaco);
        }

        //
        // POST: /reclamacoes/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Reclamacao Reclamacao = db.ExecuteStoreQuery<Reclamacao>("select * from reclamacoes" +
                                                              " order by titulo").Single(c => c.idreclamacao == id);
            reclamaco novoreclamaco = Reclamacao.Createreclamaco(Reclamacao.idreclamacao, Reclamacao.titulo,
                    Reclamacao.descricao, Reclamacao.tiposolucao, Reclamacao.IDCLIENTE, Reclamacao.IDFORNECEDOR,
                    Reclamacao.datacompra, Reclamacao.fonecontato);
            db.reclamacoes.Attach(novoreclamaco);
            db.reclamacoes.DeleteObject(novoreclamaco);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.MsgDB = "Erro: Registro não pode ser excluído por estar relacionado a outras informações no sistema." +
                    " Segue erro do sistema: " + (e.InnerException).Message.ToString();
                //Recuperar o registro do Cliente
                Reclamacao.cliente = db.clientes.First(c => c.IDCLIENTE == Reclamacao.IDCLIENTE);
                //Recuperar o registro do Fornecedor
                Reclamacao.fornecedore = db.fornecedores.First(c => c.IDFORNECEDOR == Reclamacao.IDFORNECEDOR);
                return View(Reclamacao);
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