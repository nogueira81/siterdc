using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Routing;
using rdc.Models;
using Criptografia;
using rdc.App_Code;

namespace rdc.Controllers
{
    public class LoginController : Controller
    {
        //Criar o objeto com as entidades a partir da conexão setada no web.config
        private sistemardcEntities db = new sistemardcEntities();

        public IFormsAuthenticationService FormsService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }

            base.Initialize(requestContext);
            

        }
        //
        // GET: /Login/

        public ActionResult Index(string msgLogin)
        {
            if (TempData["msgLogin"] != null)
            {
                ViewBag.msgLogin = TempData["msgLogin"];
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginModel model)
        {
            //Cria o objeto cliente somente com o filtro passado
            cliente cliente = db.clientes.ToList().Find(x => x.LOGIN == model.Login);
            //cliente cliente = db.clientes.Single(x => x.NOME == model.Login);
            if (cliente == null)
            {
                ModelState.AddModelError("", "Inválido Login ou Senha");
            }
            if (ModelState.IsValid)
            {
                //Descriptar senha vinda do banco de dados
                string texto = cliente.SENHA;
                //Está chave tem que ser a mesma que a do texto Encriptado.
                string key = "Criptografia";
                Criptografia.Criptografia crip = new Criptografia.Criptografia(CryptProvider.DES);
                crip.Key = key;
                string senhaDecrypt = crip.Decrypt(texto);
                
                //if (cliente != null && model.Login == cliente.LOGIN && model.Senha == cliente.SENHA) alterado para funcionar descriptografia
                if (model.Login == cliente.LOGIN && model.Senha == senhaDecrypt)
                {
                    //FormsAuthentication.SetAuthCookie(model.Login, false);
                    FormsService.SignIn(model.Login, false);

                    //Ao autenticar usuário no sistema, incluir ROLE ADM e NOR e ainda incluí-lo na ROLE respectiva
                    if (cliente.TIPOCLIENTE.ToUpper() == "ADM")
                    {
                        // Criando Role ADM
                        if (!Roles.RoleExists("ADM"))
                        {
                            Roles.CreateRole("ADM");
                        }
                        // Definindo Usuário para uma Role 
                        if(!Roles.IsUserInRole(cliente.LOGIN, "ADM")) 
                        {
                            //Antes de associar o Login a uma nova Role, excluir de associação de outras Roles.
                            string[] strRolesForUser = Roles.GetRolesForUser(cliente.LOGIN);
                            if (strRolesForUser.Length > 0)
                            {
                                Roles.RemoveUserFromRoles(cliente.LOGIN, strRolesForUser);
                            }
                            //Associar Login a Role
                            Roles.AddUserToRole(cliente.LOGIN, "ADM"); 
                        }
                    }
                    else if (cliente.TIPOCLIENTE.ToUpper() == "NOR")
                    {
                        // Criando Role NOR
                        if (!Roles.RoleExists("NOR"))
                        {
                            Roles.CreateRole("NOR");
                        }
                        // Definindo Usuário para uma Role 
                        if (!Roles.IsUserInRole(cliente.LOGIN, "NOR"))
                        {
                            //Antes de associar o Login a uma nova Role, excluir de associação de outras Roles.
                            string[] strRolesForUser = Roles.GetRolesForUser(cliente.LOGIN);
                            if (strRolesForUser.Length > 0)
                            {
                                Roles.RemoveUserFromRoles(cliente.LOGIN, strRolesForUser);
                            }
                            Roles.AddUserToRole(cliente.LOGIN, "NOR");
                        }
                    }
                    else
                    {
                        //Se entrar aqui significa que o Login não possui Papel e deve ser exibida a msg na tela com Logoff
                        ModelState.AddModelError("", "Login sem Grupo Definido: Contate o Administrador");
                        LogOff();
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Inválido Login ou Senha");
                }
            }
            return View();
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
        // GET: /Login/Registro/

        public ActionResult Registro(string name)
        {
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text");
            ViewBag.IDCIDADE = new SelectList(string.Empty, "IDCIDADE", "NOMECIDADE");
            Cliente Cliente = new Cliente();
            Cliente.INFORMACOES = Cliente.politicauso();
            return View(Cliente);
        }

        //
        // POST: /Login/Registro

        [HttpPost]
        public ActionResult Registro(Cliente Cliente)
        {
            //Verificar se Login já existe no sistema e informar ao usuário para que o mesmo possa cadastrar outro
            cliente user = db.clientes.FirstOrDefault(u => u.LOGIN.ToLower() == Cliente.LOGIN.ToLower());
            // Checa se usuário não existe para continuar a inserção
            if (user == null)
            {
                //Retirar não numéricos do CPF
                string Cliente_CPF = Util.RemoveNaoNumericos(Cliente.CPF);
                //Acrescentar checagem se CPF já existe cadastrado com alguem
                /* Entrar com essa checagem futuramente em outra versão com recuperação de Login por email automatico
                user = db.clientes.FirstOrDefault(u => u.CPF.ToLower() == Cliente_CPF.ToLower());
                if (user != null)
                {
                    ModelState.AddModelError("", "CPF já se encontra cadastrado no Reclame Agora!");
                }
                */
                //Acrescentar checagem se senha é igual a confirma senha
                if (Cliente.SENHA != Cliente.CONFIRMASENHA)
                {
                    ModelState.AddModelError("", "Campos Senha e Confirmar Senha não podem ser diferentes!");
                }
                if (!Cliente.auxpoliticauso)
                {
                    ModelState.AddModelError("", "Para Registrar-se é necessário marcar o check acordando com os Termos de Uso do Reclame Agora.");
                }
                //Checa se o formulário está válido antes de gravar
                if (ModelState.IsValid)
                {
                    Cliente.INFORMACOES = "Usuário marcou o check \"Li e estou de acordo com os termos de uso do Reclame Agora.\" em : " + DateTime.Now.ToString();
                    //Todo Novo cliente por Default é setado para o Grupo "NOR" de "Normal"
                    Cliente.TIPOCLIENTE = "NOR";
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
                    //Retirar não numéricos do CEP.
                    string Cliente_CEP = Util.RemoveNaoNumericos(Cliente.CEP);
                    //Convertendo o objeto personalizado "Cliente" para o objeto criado automáticamente no modelo "cliente"
                    cliente novocliente = ConverteCliente.Createcliente(Cliente.IDCLIENTE, Cliente.NOME, Cliente.EMAIL, Cliente.LOGIN,
                        Cliente.SENHA, Cliente.CONFIRMASENHA, Cliente_CEP, Cliente_CPF, Cliente.RG,
                        Cliente.ENDERECO, Cliente.IDCIDADE, Cliente.ESTADO, Cliente.INFORMACOES,
                        Cliente.NASCIMENTO, Cliente.TIPOCLIENTE, Cliente.FONEFIXO, Cliente.FONECELULAR);
                    //Assim o objeto de banco de dados não gera erro ao adicionar o objeto no banco para salvar
                    db.clientes.AddObject(novocliente);
                    db.SaveChanges();
                    //Início Chamada Enviar E-mail ao Cliente com seu Login e Senha
                    SendMail.EnviaEmail(Cliente.EMAIL, Cliente.NOME,
                        "Criação de Login e Senha do ReclameAgora",
                        "Prezado(a) <span color:Blue><b>" + Cliente.NOME.ToString() + "</b></span>,<br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        "Seu Login e Senha foram criados e seguem abaixo: <br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        "Login = <b>" + Cliente.LOGIN.ToString() + "</b>.<br /><br />" +
                        "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                        "Senha = <b>" + senhaemail.ToString() + "</b>.<br /><br />" +
                        "<a href=\"http://reclameagora.apphb.com/Login\" title=\"ReclameAgora\"> http://reclameagora.apphb.com/Login</a> <br /><br />" +
                        "Atenciosamente,<br />" +
                        "Equipe ReclameAgora");
                    //Fim da Chamada Enviar E-mail
                    //Armazenar no dicionário de dados TempData que trabalha por trás do objeto de sessão.
                    //Assim posso passar dados entre Actions no mesmo controlador
                    TempData["msgLogin"] = "Usuário " + Cliente.LOGIN + " Cadastrado com Sucesso e enviado para seu email! Faça o Login e Registre sua Reclamação.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ModelState.AddModelError("", "Login já existe no Reclame Agora. Favor criar um Login diferente.");
            }
            ViewBag.ESTADO = new SelectList(EstadoRepositorio.ListaEstados(), "Value", "Text");
            ViewBag.IDCIDADE = new SelectList(db.cidades.ToList().FindAll(x => x.IDESTADO == Cliente.ESTADO), "IDCIDADE", "NOMECIDADE", Cliente.IDCIDADE);
            return View(Cliente);
        }

        // **************************************
        // URL: /Login/LogOff => Sair do sistema
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //Início da Recuperação do Login e Senha

        //
        // GET: /Login/

        public ActionResult RecLoginEmail(string msgLogin)
        {
            if (TempData["RecLoginSenha"] != null)
            {
                ViewBag.RecLoginSenha = TempData["RecLoginSenha"];
            }
            TempData["RecLoginSenha"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult RecLoginEmail(LoginModel model)
        {
            TempData["RecLoginSenha"] = null;
            //Retirar não numéricos do CPF
            string Cliente_CPF = Util.RemoveNaoNumericos(model.cpf);
            model.cpf = Cliente_CPF;
            //Cria o objeto cliente somente com o filtro passado
            cliente cliente = db.clientes.ToList().Find(x => x.CPF ==model.cpf);
            if (cliente == null)
            {
                cliente = db.clientes.ToList().Find(x => x.EMAIL == model.email.ToLower());
                if (cliente == null)
                {
                    ModelState.AddModelError("", "CPF ou E-MAIL não se encontram vinculados a nenhum Login");
                    return View(model);
                }
            }
            //Descriptografar senha e confirmasenha antes de enviar a banco
            string texto = cliente.SENHA;
            //Está chave tem que ser a mesma que a do texto Encriptado.
            string key = "Criptografia";
            Criptografia.Criptografia crip = new Criptografia.Criptografia(CryptProvider.DES);
            crip.Key = key;
            cliente.SENHA = crip.Decrypt(texto);
            //Fim da Descriptografia
            //Início Chamada Enviar E-mail ao Cliente com seu Login e Senha
            SendMail.EnviaEmail(cliente.EMAIL, cliente.NOME,
                "Recuperação de Login e Senha do ReclameAgora",
                "Prezado(a) <span color:Blue><b>" + cliente.NOME.ToString() + "</b></span>,<br /><br />" +
                "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                "Seu Login e Senha foram recuperados e seguem abaixo: <br /><br />" +
                "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                "Login = <b>" + cliente.LOGIN.ToString() + "</b>.<br /><br />" +
                "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" +
                "Senha = <b>" + cliente.SENHA.ToString() + "</b>.<br /><br />" +
                "<a href=\"http://reclameagora.apphb.com/Login\" title=\"ReclameAgora\"> http://reclameagora.apphb.com/Login</a> <br /><br />" +
                "Atenciosamente,<br />" +
                "Equipe ReclameAgora");
            //Fim da Chamada Enviar E-mail
            TempData["RecLoginSenha"] = "Seu Login e Senha foram recuperados com sucesso e enviados em alguns instantes para o E-mail já cadastrado no sistema.";
            return RedirectToAction("RecLoginEmail", "Login");
        }


        //Fim da Recuperação do Login e Senha

        //Liberar o objeto da memória
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
