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
                return RedirectToAction("Index");
            }
            //ViewBag.IDCLIENTE = new SelectList(db.clientes, "IDCLIENTE", "NOME", Reclamacao.IDCLIENTE);
            //Recuperar o registro do Cliente
            Reclamacao.cliente = db.clientes.First(c => c.IDCLIENTE == Reclamacao.IDCLIENTE);
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores.ToList().FindAll(f => f.ativo != "N"), "IDFORNECEDOR", "razaosocial", Reclamacao.IDFORNECEDOR);
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