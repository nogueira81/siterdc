using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rdc.Models;
using System.Web.Helpers;
using rdc.App_Code;

namespace rdc.Controllers
{
    public class HomeController : Controller
    {
        private sistemardcEntities db = new sistemardcEntities();

        public ActionResult Index(string buscar = null, string buscarEst = null, string buscarFor = null)
        {
            /*ViewBag.Message = "Bem vindo ao Reclame Agora!";
            if (buscarEst == "[Todos os Estados]")
            {
                buscarEst = null;                
            }
            if (buscarFor == "[Todos os Fornecedores]")
            {
                buscarFor = null;
            }*/
            //criar filtros para Buscar na tela
            string qry = "select * from reclamacoes where 0=0  ";

            qry = "select * from reclamacoes where 0=0 ";
            if (!string.IsNullOrEmpty(buscar))
            {
                qry = qry + " and upper(titulo) like upper('%" + buscar + "%')";
            }
            if (!string.IsNullOrEmpty(buscarEst))
            {
                qry = qry + " and IDCLIENTE in (select IDCLIENTE from clientes "
                    + " where upper(estado) like upper('%" + buscarEst + "%'))";
                //Retornar o filtro para a tela principal
                ViewBag.buscarEst = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text", buscarEst);
            }
            else
            {
                //Retornar sem o filtro para a tela principal
                ViewBag.buscarEst = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text");
            }
            if (!string.IsNullOrEmpty(buscarFor))
            {
                qry = qry + " and upper(idfornecedor) like upper('%" + buscarFor + "%')";
                //Retornar o filtro para a tela principal
                ViewBag.buscarFor = new SelectList(db.fornecedores, "IDFORNECEDOR", "razaosocial", buscarFor);
            }
            else
            {
                //Retornar sem o filtro para a tela principal
                ViewBag.buscarFor = new SelectList(db.fornecedores, "IDFORNECEDOR", "razaosocial");
            }

            qry = qry + " order by datacompra desc";
            IEnumerable<reclamaco> reclamadetalhes = db.ExecuteStoreQuery<reclamaco>(qry);
            //var reclamadetalhes =  db.reclamacoes.Include("cliente").Include("fornecedore");
            if (Request.IsAjaxRequest())
            {
                //Retornar com o Ajax somente a Parcial dos Produtos e não a página toda
                return PartialView("HomePartial", reclamadetalhes.ToList());
            }
            return View(reclamadetalhes.ToList());
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contato()
        {
            return View();
        }

        //Essa chamada popula o detalhe com 1 Reclamação recuperada do id vinda da página que chamou
        public ActionResult DetalheInteracao(int id)
        {
            Reclamacao Reclamacao = db.ExecuteStoreQuery<Reclamacao>("select * from reclamacoes" +
                      " order by titulo").Single(c => c.idreclamacao == id);
            return View(Reclamacao);
        }

        public ActionResult CreateInteracao(Interacao Interacao)
        {
            if (!Request.IsAuthenticated)
            {

                TempData["msgInteracao"] = "False";
            }
            else if (ModelState.IsValid && Request.IsAuthenticated)
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
                SendMail.EnviaEmail(interaco.cliente.EMAIL, interaco.cliente.NOME,
                    "Retorno da sua Reclamação no ReclameAgora",
                    "Prezado(a) <span color:Blue><b>" + interaco.cliente.NOME.ToString() + "</b></span>,<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Favor verificar sua Reclamação : <b>" + interaco.reclamaco.titulo.ToString() + "</b>.<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Foi inserida a seguinte Interação em sua Reclamação : <br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    Interacao.INFORMACOES.ToString() + "<br /><br />" +
                    "Para verificar, acesse " +
                    "<a href=\"http://www.reclameagora.com.br/\" title=\"ReclameAgora\"> http://www.reclameagora.com.br/</a> <br /><br />" +
                    "Atenciosamente,<br />" +
                    "Equipe ReclameAgora");
                //Fim da Chamada Enviar E-mail

                TempData["msgInteracao"] = "True";
            }
            return RedirectToAction("DetalheInteracao", new { id = Interacao.IDRECLAMACAO });
        }

        public ActionResult Ranking()
        {
            ViewBag.Message = "Ranking do Reclame Agora!";
            string commandCli = "select c.estado as est_cli, c.idcliente, c.login, count(r.idreclamacao) as qtderec, " +
                "(case when qtderecn IS NULL then 0 else qtderecn end) as qtderecn, " +
                "(case when qtderecs IS NULL then 0 else qtderecs end) as qtderecs " +
                "from clientes c " +
                "inner join reclamacoes r on c.idcliente = r.idcliente " +
                "left outer join (select idcliente, count(idreclamacao) as qtderecn " +
                                "from reclamacoes  " +
                                "where tiposolucao = 'N' " +
                                "group by idcliente) rn on rn.idcliente = c.idcliente  " +
                "left outer join (select idcliente, count(idreclamacao) as qtderecs  " +
                                "from reclamacoes  " +
                                "where tiposolucao <> 'N' " +
                                "group by idcliente) rs on rs.idcliente = c.idcliente " +
                "group by c.estado,c.idcliente, c.login,rn.qtderecn,rs.qtderecs  ";
            string commandFor = "select c.estado as est_for, c.idfornecedor, c.nomefantasia, count(r.idreclamacao) as qtderec, " +
                "(case when qtderecn IS NULL then 0 else qtderecn end) as qtderecn, " +
                "(case when qtderecs IS NULL then 0 else qtderecs end) as qtderecs " +
                "from fornecedores c " +
                "inner join reclamacoes r on c.idfornecedor = r.idfornecedor " +
                "left outer join (select idfornecedor, count(idreclamacao) as qtderecn " +
                                "from reclamacoes " +
                                "where tiposolucao = 'N' " +
                                "group by idfornecedor) rn on rn.idfornecedor = c.idfornecedor " +
                "left outer join (select idfornecedor, count(idreclamacao) as qtderecs " +
                                "from reclamacoes " +
                                "where tiposolucao <> 'N' " +
                                "group by idfornecedor) rs on rs.idfornecedor = c.idfornecedor " +
                "group by c.estado,c.idfornecedor, c.nomefantasia,rn.qtderecn,rs.qtderecs";
            HomeModel reclamacoes = new HomeModel();
            reclamacoes.HomeModelCli = db.ExecuteStoreQuery<HomeModelCli>(commandCli).ToList();
            //É necessário abrir outra conexão e fechar para obter o resulltado da nova consulta
            sistemardcEntities dbrec = new sistemardcEntities();
            reclamacoes.HomeModelFor = dbrec.ExecuteStoreQuery<HomeModelFor>(commandFor).ToList();
            dbrec.Dispose();
            return View(reclamacoes);
        }

        /*Grafico Tempo de Atendimento das Reclamações por Fornecedores
         SELECT idfornecedor, count(*) qtderec,
        round(avg((case (datasolucao-datacompra) > 0
        when 1 then DATEDIFF( DATE( datasolucao ) ,DATE( datacompra ))
        else DATEDIFF( DATE( NOW( ) ) ,DATE( datacompra ))
        end))) as avgdiasatend FROM reclamacoes
        group by idfornecedor
         */

        public ActionResult Grfavgdiasatend(string Tipo, string thema)
        {
            string commandFor = "SELECT r.idfornecedor, f.nomefantasia, count(*) qtderec, " +
                "round(avg((case (datasolucao-datacompra) > 0 " +
                "when 1 then DATEDIFF( DATE( datasolucao ) ,DATE( datacompra )) " +
                "else DATEDIFF( DATE( NOW( ) ) ,DATE( datacompra )) " +
                "end))) as avgdiasatend FROM reclamacoes r inner join fornecedores f on r.idfornecedor = f.idfornecedor " +
                "group by r.idfornecedor, f.nomefantasia";
            HomeModel reclamacoes = new HomeModel();
            var data = db.ExecuteStoreQuery<HomeModelAvgtdFor>(commandFor).ToList();
            if (data.Count >= 0)
            {
                string[] x;
                string[] y;
                int i = 0;
                int maiorqtde = 0;
                if (data.Count == 0)
                {
                    //Essa condição é para retornar um Gráfico mesmo sem informação, para evitar erro na imagem
                    x = new string[1];
                    y = new string[1];
                    x[0] = "Sem Dados no sistema";
                    y[0] = "0";
                    maiorqtde = 1;
                }
                else
                {
                    x = new string[data.Count];
                    y = new string[data.Count];
                }
                foreach (var item in data)
                {
                    x[i] = item.nomefantasia.Trim() + "\r\n" + "Reclamações: " + item.qtderec.ToString() +
                                                      "\r\n" + "Média Solução: " + "\r\n" +
                                                      item.avgdiasatend.ToString() + " Dias";
                    y[i] = item.avgdiasatend.ToString();
                    if (maiorqtde <= item.avgdiasatend || maiorqtde == item.avgdiasatend)
                    {
                        maiorqtde = item.avgdiasatend;
                    }
                    i++;
                }

                var Grafico = new Chart(width: 500, height: 400, theme: ChartTheme.Blue)
                        .SetYAxis("Média para Solucionar em Dias", 0)
                        .SetXAxis("Fornecedores", 0)
                        .AddSeries(
                            markerStep: maiorqtde,
                            axisLabel: "Media em Dias De Atendimento de Reclamações de compras por Fornecedores",
                            chartType: Tipo,
                            legend: "Relatório de Media em Dias de Reclamações",
                            xField: "Fornecedor",
                            yFields: "Total de Reclamações",
                            xValue: x,
                            yValues: y).GetBytes("png");
                return File(Grafico, "image/png");
            }
            return null;

        }


        public ActionResult Grafico(string Tipo)
        {
            string commandFor = "select c.estado as est_for, c.idfornecedor, c.nomefantasia, count(r.idreclamacao) as qtderec, " +
                "(case when qtderecn IS NULL then 0 else qtderecn end) as qtderecn, " +
                "(case when qtderecs IS NULL then 0 else qtderecs end) as qtderecs " +
                "from fornecedores c " +
                "inner join reclamacoes r on c.idfornecedor = r.idfornecedor " +
                "left outer join (select idfornecedor, count(idreclamacao) as qtderecn " +
                                "from reclamacoes " +
                                "where tiposolucao = 'N' " +
                                "group by idfornecedor) rn on rn.idfornecedor = c.idfornecedor " +
                "left outer join (select idfornecedor, count(idreclamacao) as qtderecs " +
                                "from reclamacoes " +
                                "where tiposolucao <> 'N' " +
                                "group by idfornecedor) rs on rs.idfornecedor = c.idfornecedor " +
                "group by c.estado,c.idfornecedor, c.nomefantasia,rn.qtderecn,rs.qtderecs";
            HomeModel reclamacoes = new HomeModel();
            var data = db.ExecuteStoreQuery<HomeModelFor>(commandFor).ToList();
            if (data.Count >= 0)
            {
                string[] x;
                string[] y;
                int i = 0;
                int maiorqtde = 0;
                if (data.Count == 0)
                {
                    //Essa condição é para retornar um Gráfico mesmo sem informação, para evitar erro na imagem
                    x = new string[1];
                    y = new string[1];
                    x[0] = "Sem Dados no sistema";
                    y[0] = "0";
                    maiorqtde = 1;
                }
                else
                {
                    x = new string[data.Count];
                    y = new string[data.Count];
                }
                foreach (var item in data)
                {
                    x[i] = item.nomefantasia.Trim() + "\r\n" + "Total: " + item.qtderec.ToString() +
                                                      "\r\n" + "Solucionadas: " + item.qtderecs.ToString();
                    y[i] = item.qtderec.ToString();
                    if (maiorqtde <= item.qtderec || maiorqtde == item.qtderec)
                    {
                        maiorqtde = item.qtderec;
                    }
                    i++;
                }
                var Grafico = new Chart(width: 500, height: 400, theme: ChartTheme.Vanilla3D)
                        .SetYAxis("Total de Reclamações", 0)
                        .SetXAxis("Fornecedores", 0)
                        .AddSeries(
                            markerStep: maiorqtde,
                            axisLabel: "Total de Reclamações de compras por Fornecedores",
                            chartType: Tipo,
                            legend: "Relatório de Reclamações",
                            xField: "Fornecedor",
                            yFields: "Total de Reclamações",
                            xValue: x,
                            yValues: y).GetBytes("png");
                return File(Grafico, "image/png");

            }
            return null;
        }

        [ActionName("DeleteInteracao")]
        public ActionResult DeleteInteracaoConfirmed(int id)
        {
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
                return RedirectToAction("DetalheInteracao", new { id = idreclamacao });
            }
            return RedirectToAction("DetalheInteracao", new { id = idreclamacao });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
