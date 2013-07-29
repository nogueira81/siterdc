using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using rdc.Models;
using Criptografia;
using rdc.App_Code;

namespace rdc.Controllers
{

    public class ClientesController : Controller
    {
        private sistemardcEntities db = new sistemardcEntities();

        //
        // GET: /Clientes/
        [Authorize(Roles = "ADM")]
        public ViewResult Index(string buscar = null)
        {
            //Alterado para comando pelo fato de ter passado a utilizar outra classe de "Cliente"(personalizada)
            //para obter o resultado da query.
            string qrycliente = "select * from clientes order by nome";
            if (!string.IsNullOrEmpty(buscar))
            {
                qrycliente = "select * from clientes "+
                    " where upper(nome) like upper('%"+buscar+"%')"+
                    " order by nome";
            }
            IEnumerable<Cliente> Clientes = db.ExecuteStoreQuery<Cliente>(qrycliente);
            return View(Clientes.ToList());
            //return View(db.clientes.ToList());
        }

        //
        // GET: /Clientes/Details/5
        [Authorize(Roles = "ADM")]
        public ViewResult Details(int id)
        {
            Cliente Cliente = db.ExecuteStoreQuery<Cliente>("select * from clientes" +
                                                              " order by nome").Single(c => c.IDCLIENTE == id);
            //cliente cliente = db.clientes.Single(c => c.IDCLIENTE == id); Método pelo Model comentado não mais preciso
            //Descriptar senha vinda do banco de dados
            string texto = Cliente.SENHA;
            //Está chave tem que ser a mesma que a do texto Encriptado.
            string key = "Criptografia";
            Criptografia.Criptografia crip = new Criptografia.Criptografia(CryptProvider.DES);
            crip.Key = key;
            string senhaDecrypt = crip.Decrypt(texto);
            Cliente.SENHA = senhaDecrypt;
            Cliente.CONFIRMASENHA = senhaDecrypt;
            return View(Cliente);
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

                    string[] values = strhtmlBuscaCEP.Split(new string[] { "<tr>", "</tr>", "<td>", "</td>" },
                                                                StringSplitOptions.RemoveEmptyEntries);
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
                    cepcomp.cidade = result[0].IDCIDADE.ToString();
                    cepcompleto.Add(cepcomp);
                    return Json(cepcompleto[0], JsonRequestBehavior.AllowGet);
                }
            }
            return Json("");

        }


        //
        // GET: /Clientes/Create
        [Authorize(Roles = "ADM")]
        public ActionResult Create()
        {
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text");
            ViewBag.IDCIDADE = new SelectList(string.Empty, "IDCIDADE", "NOMECIDADE");
            return View();
        } 

        //
        // POST: /Clientes/Create
        [Authorize(Roles = "ADM")]
        [HttpPost]
        public ActionResult Create(Cliente Cliente)
        {
            //Acrescentar checagem se senha é igual a confirma senha
            if (Cliente.SENHA != Cliente.CONFIRMASENHA)
            {
                ModelState.AddModelError("", "Campos Senha e Confirmar Senha não podem ser diferentes!");
            }
            //Checa se o formulário está válido antes de gravar
            if (ModelState.IsValid)
            {
                //Criptografar senha e confirmasenha antes de enviar a banco
                string texto = Cliente.SENHA;
                //Está chave tem que ser a mesma que a do texto Encriptado.
                string key = "Criptografia";
                Criptografia.Criptografia crip = new Criptografia.Criptografia(CryptProvider.DES);
                crip.Key = key;
                Cliente.SENHA = crip.Encrypt(texto);
                Cliente.CONFIRMASENHA = Cliente.SENHA;
                //Fim da criptografia
                //Retirar não numéricos caso haja do CPF e CEP.
                string Cliente_CEP = Util.RemoveNaoNumericos(Cliente.CEP);
                string Cliente_CPF = Util.RemoveNaoNumericos(Cliente.CPF);
                //Convertendo o objeto personalizado "Cliente" para o objeto criado automáticamente no modelo "cliente"
                cliente novocliente = ConverteCliente.Createcliente(Cliente.IDCLIENTE, Cliente.NOME, Cliente.EMAIL, Cliente.LOGIN,
                    Cliente.SENHA, Cliente.CONFIRMASENHA, Cliente_CEP, Cliente_CPF, Cliente.RG,
                    Cliente.ENDERECO, Cliente.IDCIDADE, Cliente.ESTADO, Cliente.INFORMACOES,
                    Cliente.NASCIMENTO, Cliente.TIPOCLIENTE, Cliente.FONEFIXO, Cliente.FONECELULAR);
                //Formatar data antes de inserir
                string.Format("{0:dd/MM/yyyy}", novocliente.NASCIMENTO);
                //Assim o objeto de banco de dados não gera erro ao adicionar o objeto no banco para salvar
                db.clientes.AddObject(novocliente);
                db.SaveChanges();
                //Armazenar no dicionário de dados TempData que trabalha por trás do objeto de sessão.
                //Assim posso passar dados entre Actions no mesmo controlador
                //TempData["msgLogin"] = "Usuário " + Cliente.LOGIN + " Cadastrado com Sucesso! Faça o Login e Registre sua Reclamação.";
                return RedirectToAction("Index");
            }
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text");
            ViewBag.IDCIDADE = new SelectList(db.cidades.ToList().FindAll(x => x.IDESTADO == Cliente.ESTADO), "IDCIDADE", "NOMECIDADE", Cliente.IDCIDADE);
            return View(Cliente);
        }
        
        //
        // GET: /Clientes/Edit/5
        [Authorize(Roles = "ADM")]
        public ActionResult Edit(int id)
        {
            Cliente Cliente = db.ExecuteStoreQuery<Cliente>("select * from clientes" +
                                                                          " order by nome").Single(c => c.IDCLIENTE == id);
            //cliente cliente = db.clientes.Single(c => c.IDCLIENTE == id);
            //Descriptar senha vinda do banco de dados
            string texto = Cliente.SENHA;
            //Está chave tem que ser a mesma que a do texto Encriptado.
            string key = "Criptografia";
            Criptografia.Criptografia crip = new Criptografia.Criptografia(CryptProvider.DES);
            crip.Key = key;
            string senhaDecrypt = crip.Decrypt(texto);
            Cliente.SENHA = senhaDecrypt;
            Cliente.CONFIRMASENHA = senhaDecrypt;
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text", Cliente.ESTADO);
            ViewBag.IDCIDADE = new SelectList(db.cidades.ToList().FindAll(x => x.IDESTADO == Cliente.ESTADO), "IDCIDADE", "NOMECIDADE", Cliente.IDCIDADE);
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores.ToList().FindAll(f => f.ativo != "N"), "IDFORNECEDOR", "razaosocial", Cliente.IDFORNECEDOR);
            return View(Cliente);
        }

        //
        // POST: /Clientes/Edit/5
        [Authorize(Roles = "ADM")]
        [HttpPost]
        public ActionResult Edit(Cliente Cliente)
        {
            //Acrescentar checagem se senha é igual a confirma senha
            if (Cliente.SENHA != Cliente.CONFIRMASENHA)
            {
                ModelState.AddModelError("", "Campos Senha e Confirmar Senha não podem ser diferentes!");
            }
            //Fornecedor não pode ter perfil ADM
            if (Cliente.TIPOCLIENTE == "ADM" && Cliente.IDFORNECEDOR != null)
            {
                ModelState.AddModelError("", "Fornecedor não pode ter Perfil Administrador do Sistema!");
            }
            //Checa se o formulário está válido antes de gravar
            if (ModelState.IsValid)
            {
                //Criptografar senha e confirmasenha antes de enviar a banco
                string texto = Cliente.SENHA;
                //Está chave tem que ser a mesma que a do texto Encriptado.
                string key = "Criptografia";
                Criptografia.Criptografia crip = new Criptografia.Criptografia(CryptProvider.DES);
                crip.Key = key;
                Cliente.SENHA = crip.Encrypt(texto);
                Cliente.CONFIRMASENHA = Cliente.SENHA;
                //Fim da criptografia
                //Retirar não numéricos caso haja do CPF e CEP.
                string Cliente_CEP = Util.RemoveNaoNumericos(Cliente.CEP);
                string Cliente_CPF = Util.RemoveNaoNumericos(Cliente.CPF);
                //Convertendo o objeto personalizado "Cliente" para o objeto criado automáticamente no modelo "cliente"
                cliente novocliente = ConverteCliente.Createcliente(Cliente.IDCLIENTE, Cliente.NOME, Cliente.EMAIL, Cliente.LOGIN,
                    Cliente.SENHA, Cliente.CONFIRMASENHA, Cliente_CEP, Cliente_CPF, Cliente.RG,
                    Cliente.ENDERECO, Cliente.IDCIDADE, Cliente.ESTADO, Cliente.INFORMACOES,
                    Cliente.NASCIMENTO, Cliente.TIPOCLIENTE, Cliente.FONEFIXO, Cliente.FONECELULAR,Cliente.IDFORNECEDOR);
                //Formatar data antes de inserir
                string.Format("{0:dd/MM/yyyy}", novocliente.NASCIMENTO);
                //Assim o objeto de banco de dados não gera erro ao adicionar o objeto no banco para salvar
                db.clientes.Attach(novocliente);
                db.ObjectStateManager.ChangeObjectState(novocliente, EntityState.Modified);
                db.SaveChanges();
                //Armazenar no dicionário de dados TempData que trabalha por trás do objeto de sessão.
                //Assim posso passar dados entre Actions no mesmo controlador
                //TempData["msgLogin"] = "Usuário " + Cliente.LOGIN + " Cadastrado com Sucesso! Faça o Login e Registre sua Reclamação.";
                return RedirectToAction("Index");
            }
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text", Cliente.ESTADO);
            ViewBag.IDCIDADE = new SelectList(db.cidades.ToList().FindAll(x => x.IDESTADO == Cliente.ESTADO), "IDCIDADE", "NOMECIDADE", Cliente.IDCIDADE);
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores.ToList().FindAll(f => f.ativo != "N"), "IDFORNECEDOR", "razaosocial", Cliente.IDFORNECEDOR);
            
            return View(Cliente);
        }

        //
        // GET: /Clientes/Delete/5
        [Authorize(Roles = "ADM")]
        public ActionResult Delete(int id)
        {
            Cliente Cliente = db.ExecuteStoreQuery<Cliente>("select * from clientes" +
                                                              " order by nome").Single(c => c.IDCLIENTE == id);
            //cliente cliente = db.clientes.Single(c => c.IDCLIENTE == id); Método pelo Model comentado não mais preciso
            //Descriptar senha vinda do banco de dados
            string texto = Cliente.SENHA;
            //Está chave tem que ser a mesma que a do texto Encriptado.
            string key = "Criptografia";
            Criptografia.Criptografia crip = new Criptografia.Criptografia(CryptProvider.DES);
            crip.Key = key;
            string senhaDecrypt = crip.Decrypt(texto);
            Cliente.SENHA = senhaDecrypt;
            Cliente.CONFIRMASENHA = senhaDecrypt;
            return View(Cliente);
        }

        //
        // POST: /Clientes/Delete/5
        [Authorize(Roles = "ADM")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Cliente Cliente = db.ExecuteStoreQuery<Cliente>("select * from clientes" +
                                                  " order by nome").Single(c => c.IDCLIENTE == id);
            //cliente cliente = db.clientes.Single(c => c.IDCLIENTE == id); Método pelo Model comentado não mais preciso
            cliente novocliente = ConverteCliente.Createcliente(Cliente.IDCLIENTE, Cliente.NOME, Cliente.EMAIL, Cliente.LOGIN,
                    Cliente.SENHA, Cliente.CONFIRMASENHA, Cliente.CEP, Cliente.CPF, Cliente.RG,
                    Cliente.ENDERECO, Cliente.IDCIDADE, Cliente.ESTADO, Cliente.INFORMACOES,
                    Cliente.NASCIMENTO, Cliente.TIPOCLIENTE, Cliente.FONEFIXO, Cliente.FONECELULAR);
            db.clientes.Attach(novocliente);
            db.clientes.DeleteObject(novocliente);
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                ViewBag.MsgDB = "Erro: Registro não pode ser excluído por estar relacionado a outras informações no sistema."+
                    " Segue erro do sistema: " + (e.InnerException).Message.ToString();
                string texto = Cliente.SENHA;
                //Está chave tem que ser a mesma que a do texto Encriptado.
                string key = "Criptografia";
                Criptografia.Criptografia crip = new Criptografia.Criptografia(CryptProvider.DES);
                crip.Key = key;
                string senhaDecrypt = crip.Decrypt(texto);
                Cliente.SENHA = senhaDecrypt;
                Cliente.CONFIRMASENHA = senhaDecrypt;
                return View(Cliente);
            }
            
            return RedirectToAction("Index");
        }


        /*Controle das Views da Área do Cliente*/

        //
        // GET: /AreaCliente/
        [Authorize]
        public ViewResult AreaCliente()
        {
            //Alterado para comando pelo fato de ter passado a utilizar outra classe de "Cliente"(personalizada)
            //para obter o resultado da query.
            string qrycliente = "select * from clientes order by nome";
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                qrycliente = "select * from clientes " +
                    " where Login = '" + User.Identity.Name + "'" +
                    " order by nome";
            }
            IEnumerable<Cliente> Clientes = db.ExecuteStoreQuery<Cliente>(qrycliente);
            //Enviar retornor de Alteração de Senha para a Área do Cliente
            if (TempData["msgAreaCliSenha"] != null)
            {
                ViewBag.msgAreaCliSenha = TempData["msgAreaCliSenha"];
            }
            else
            {
                ViewBag.msgAreaCliSenha = null;
            }
            TempData["msgAreaCliSenha"] = null;
            return View(Clientes.ToList());
            //return View(db.clientes.ToList());
        }

        //
        // GET: /Clientes/AreaCliDet/5
        [Authorize]
        public ViewResult AreaCliDet(int id)
        {
            Cliente Cliente = db.ExecuteStoreQuery<Cliente>("select * from clientes" +
                                                              " order by nome").Single(c => c.IDCLIENTE == id);
            //cliente cliente = db.clientes.Single(c => c.IDCLIENTE == id); Método pelo Model comentado não mais preciso
            //Descriptar senha vinda do banco de dados
            string texto = Cliente.SENHA;
            //Está chave tem que ser a mesma que a do texto Encriptado.
            string key = "Criptografia";
            Criptografia.Criptografia crip = new Criptografia.Criptografia(CryptProvider.DES);
            crip.Key = key;
            string senhaDecrypt = crip.Decrypt(texto);
            Cliente.SENHA = senhaDecrypt;
            Cliente.CONFIRMASENHA = senhaDecrypt;
            return View(Cliente);
        }

        //
        // GET: /Clientes/AreaCliEdit/5
        [Authorize]
        public ActionResult AreaCliEdit(int id)
        {
            Cliente Cliente = db.ExecuteStoreQuery<Cliente>("select * from clientes" +
                                                                          " order by nome").Single(c => c.IDCLIENTE == id);
            //cliente cliente = db.clientes.Single(c => c.IDCLIENTE == id);
            //Descriptar senha vinda do banco de dados
            string texto = Cliente.SENHA;
            //Está chave tem que ser a mesma que a do texto Encriptado.
            string key = "Criptografia";
            Criptografia.Criptografia crip = new Criptografia.Criptografia(CryptProvider.DES);
            crip.Key = key;
            string senhaDecrypt = crip.Decrypt(texto);
            Cliente.SENHA = senhaDecrypt;
            Cliente.CONFIRMASENHA = senhaDecrypt;
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text", Cliente.ESTADO);
            ViewBag.IDCIDADE = new SelectList(db.cidades.ToList().FindAll(x => x.IDESTADO == Cliente.ESTADO), "IDCIDADE", "NOMECIDADE", Cliente.IDCIDADE);
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores.ToList().FindAll(f => f.ativo != "N"), "IDFORNECEDOR", "razaosocial", Cliente.IDFORNECEDOR);
            return View(Cliente);
        }

        //
        // POST: /Clientes/AreaCliEdit/5
        [Authorize]
        [HttpPost]
        public ActionResult AreaCliEdit(Cliente Cliente)
        {
            //Acrescentar checagem se senha é igual a confirma senha
            if (Cliente.SENHA != Cliente.CONFIRMASENHA)
            {
                ModelState.AddModelError("", "Campos Senha e Confirmar Senha não podem ser diferentes!");
            }
            //Fornecedor não pode ter perfil ADM
            if (Cliente.TIPOCLIENTE == "ADM" && Cliente.IDFORNECEDOR != null)
            {
                ModelState.AddModelError("", "Fornecedor não pode ter Perfil Administrador do Sistema!");
            }
            //Checa se o formulário está válido antes de gravar
            if (ModelState.IsValid)
            {
                //Criptografar senha e confirmasenha antes de enviar a banco
                string texto = Cliente.SENHA;
                //Está chave tem que ser a mesma que a do texto Encriptado.
                string key = "Criptografia";
                Criptografia.Criptografia crip = new Criptografia.Criptografia(CryptProvider.DES);
                crip.Key = key;
                Cliente.SENHA = crip.Encrypt(texto);
                Cliente.CONFIRMASENHA = Cliente.SENHA;
                //Fim da criptografia
                //Retirar não numéricos caso haja do CPF e CEP.
                string Cliente_CEP = Util.RemoveNaoNumericos(Cliente.CEP);
                string Cliente_CPF = Util.RemoveNaoNumericos(Cliente.CPF);
                //Convertendo o objeto personalizado "Cliente" para o objeto criado automáticamente no modelo "cliente"
                cliente novocliente = ConverteCliente.Createcliente(Cliente.IDCLIENTE, Cliente.NOME, Cliente.EMAIL, Cliente.LOGIN,
                    Cliente.SENHA, Cliente.CONFIRMASENHA, Cliente_CEP, Cliente_CPF, Cliente.RG,
                    Cliente.ENDERECO, Cliente.IDCIDADE, Cliente.ESTADO, Cliente.INFORMACOES,
                    Cliente.NASCIMENTO, Cliente.TIPOCLIENTE, Cliente.FONEFIXO, Cliente.FONECELULAR, Cliente.IDFORNECEDOR);
                //Assim o objeto de banco de dados não gera erro ao adicionar o objeto no banco para salvar
                db.clientes.Attach(novocliente);
                db.ObjectStateManager.ChangeObjectState(novocliente, EntityState.Modified);
                db.SaveChanges();
                //Armazenar no dicionário de dados TempData que trabalha por trás do objeto de sessão.
                //Assim posso passar dados entre Actions no mesmo controlador
                //TempData["msgLogin"] = "Usuário " + Cliente.LOGIN + " Cadastrado com Sucesso! Faça o Login e Registre sua Reclamação.";
                return RedirectToAction("AreaCliente");
            }
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text", Cliente.ESTADO);
            ViewBag.IDCIDADE = new SelectList(db.cidades.ToList().FindAll(x => x.IDESTADO == Cliente.ESTADO), "IDCIDADE", "NOMECIDADE", Cliente.IDCIDADE);
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores.ToList().FindAll(f => f.ativo != "N"), "IDFORNECEDOR", "razaosocial", Cliente.IDFORNECEDOR);

            return View(Cliente);
        }


        //
        // GET: /Clientes/AreaCliSenha/5
        [Authorize]
        public ActionResult AreaCliSenha(int id)
        {
            Cliente Cliente = db.ExecuteStoreQuery<Cliente>("select * from clientes" +
                                                                          " order by nome").Single(c => c.IDCLIENTE == id);
            //cliente cliente = db.clientes.Single(c => c.IDCLIENTE == id);
            //Descriptar senha vinda do banco de dados
            string texto = Cliente.SENHA;
            //Está chave tem que ser a mesma que a do texto Encriptado.
            string key = "Criptografia";
            Criptografia.Criptografia crip = new Criptografia.Criptografia(CryptProvider.DES);
            crip.Key = key;
            string senhaDecrypt = crip.Decrypt(texto);
            Cliente.SENHA = senhaDecrypt;
            Cliente.CONFIRMASENHA = senhaDecrypt;
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text", Cliente.ESTADO);
            ViewBag.IDCIDADE = new SelectList(db.cidades.ToList().FindAll(x => x.IDESTADO == Cliente.ESTADO), "IDCIDADE", "NOMECIDADE", Cliente.IDCIDADE);
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores.ToList().FindAll(f => f.ativo != "N"), "IDFORNECEDOR", "razaosocial", Cliente.IDFORNECEDOR);
            return View(Cliente);
        }

        //
        // POST: /Clientes/AreaCliSenha/5
        [Authorize]
        [HttpPost]
        public ActionResult AreaCliSenha(Cliente Cliente)
        {
            TempData["msgAreaCliSenha"] = null;
            //Acrescentar checagem se senha é igual a confirma senha
            if (Cliente.SENHA != Cliente.CONFIRMASENHA)
            {
                ModelState.AddModelError("", "Campos Senha e Confirmar Senha não podem ser diferentes!");
            }
            //Fornecedor não pode ter perfil ADM
            if (Cliente.TIPOCLIENTE == "ADM" && Cliente.IDFORNECEDOR != null)
            {
                ModelState.AddModelError("", "Fornecedor não pode ter Perfil Administrador do Sistema!");
            }
            //Checa se o formulário está válido antes de gravar
            if (ModelState.IsValid)
            {
                //Criptografar senha e confirmasenha antes de enviar a banco
                string texto = Cliente.SENHA;
                string senhaemail = Cliente.SENHA;
                //Está chave tem que ser a mesma que a do texto Encriptado.
                string key = "Criptografia";
                Criptografia.Criptografia crip = new Criptografia.Criptografia(CryptProvider.DES);
                crip.Key = key;
                Cliente.SENHA = crip.Encrypt(texto);
                Cliente.CONFIRMASENHA = Cliente.SENHA;
                //Fim da criptografia
                //Retirar não numéricos caso haja do CPF e CEP.
                string Cliente_CEP = Util.RemoveNaoNumericos(Cliente.CEP);
                string Cliente_CPF = Util.RemoveNaoNumericos(Cliente.CPF);
                //Convertendo o objeto personalizado "Cliente" para o objeto criado automáticamente no modelo "cliente"
                cliente novocliente = ConverteCliente.Createcliente(Cliente.IDCLIENTE, Cliente.NOME, Cliente.EMAIL, Cliente.LOGIN,
                    Cliente.SENHA, Cliente.CONFIRMASENHA, Cliente_CEP, Cliente_CPF, Cliente.RG,
                    Cliente.ENDERECO, Cliente.IDCIDADE, Cliente.ESTADO, Cliente.INFORMACOES,
                    Cliente.NASCIMENTO, Cliente.TIPOCLIENTE, Cliente.FONEFIXO, Cliente.FONECELULAR, Cliente.IDFORNECEDOR);
                //Assim o objeto de banco de dados não gera erro ao adicionar o objeto no banco para salvar
                db.clientes.Attach(novocliente);
                db.ObjectStateManager.ChangeObjectState(novocliente, EntityState.Modified);
                db.SaveChanges();
                //Início Chamada Enviar E-mail ao Cliente com seu Login e Senha
                SendMail.EnviaEmail(Cliente.EMAIL, Cliente.NOME,
                    "Alteração de Senha do Login do ReclameAgora",
                    "Prezado(a) <span color:Blue><b>" + Cliente.NOME.ToString() + "</b></span>,<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "A Senha do seu Login foi alterada e segue abaixo: <br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Login = <b>" + Cliente.LOGIN.ToString() + "</b>.<br /><br />" +
                    "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                    "Senha = <b>" + senhaemail.ToString() + "</b>.<br /><br />" +
                    "<a href=\"http://reclameagora.apphb.com/\" title=\"ReclameAgora\"> http://reclameagora.apphb.com/</a> <br /><br />" +
                    "Atenciosamente,<br />" +
                    "Equipe ReclameAgora");
                //Fim da Chamada Enviar E-mail
                //Armazenar no dicionário de dados TempData que trabalha por trás do objeto de sessão.
                //Assim posso passar dados entre Actions no mesmo controlador
                TempData["msgAreaCliSenha"] = "Usuário " + Cliente.LOGIN + " teve sua senha alterada com Sucesso!";
                return RedirectToAction("AreaCliente");
            }
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text", Cliente.ESTADO);
            ViewBag.IDCIDADE = new SelectList(db.cidades.ToList().FindAll(x => x.IDESTADO == Cliente.ESTADO), "IDCIDADE", "NOMECIDADE", Cliente.IDCIDADE);
            ViewBag.IDFORNECEDOR = new SelectList(db.fornecedores.ToList().FindAll(f => f.ativo != "N"), "IDFORNECEDOR", "razaosocial", Cliente.IDFORNECEDOR);

            return View(Cliente);
        }


        /*Fim do Controle das Views da Área do Cliente*/

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}