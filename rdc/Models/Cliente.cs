using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using rdc.App_Code;
using System.Data.Objects.DataClasses;
using System.Runtime.Serialization;

namespace rdc.Models
{
    public class Cliente
    {
        public Int32 IDCLIENTE { get; set; }

        [Display(Name = "Nome Completo")]
        [Required(ErrorMessage = "Campo Nome Obrigatório")]
        public string NOME { get; set; }

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "Campo CPF Obrigatório")]
        [CustomValidationCPFAttribute(ErrorMessage = "CPF Inválido")]
        public string CPF { 
            get 
            {
                return Util.FormataString("###.###.###-##", _CPF); 
            } 
            set
            {
                if (_CPF != value)
                {
                    _CPF = Util.RemoveNaoNumericos(value);
                }
            } }
        private string _CPF;

        [Display(Name = "Identidade")]
        public string RG { get; set; }

        [Display(Name = "Data de Nascimento")]
        public Nullable<DateTime> NASCIMENTO { get; set; }

        [Display(Name = "CEP")]
        public string CEP
        {
            get
            {
                return Util.FormataString("#####-###", _CEP);
            }
            set
            {
                if (_CEP != value)
                {
                    _CEP = Util.RemoveNaoNumericos(value);
                }
            }
        }
        private string _CEP;

        [Display(Name = "Estado que reside")]
        [Required(ErrorMessage = "Campo Estado Obrigatório")]
        public string ESTADO { get; set; }

        [Display(Name = "Cidade que reside")]
        [Required(ErrorMessage = "Campo Cidade Obrigatório")]
        public Nullable<global::System.Int32> IDCIDADE { get; set; }

        [Display(Name = "Para qual Fornecedor esse Login trabalha?")]
        public Nullable<global::System.Int32> IDFORNECEDOR { get; set; }

        [Display(Name = "Endereço que reside")]
        public string ENDERECO { get; set; }

        [Display(Name = "Telefone Fixo")]
        public string FONEFIXO { get; set; }

        [Display(Name = "Telefone Celular")]
        public string FONECELULAR { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Campo E-mail Obrigatório")]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "E-mail inválido")]
        public string EMAIL { get; set; }

        [Display(Name = "Login")]
        [Required(ErrorMessage = "Campo Login Obrigatório")]
        [StringLength(20, ErrorMessage = "O campo {0} deve ter pelo menos {2} caracteres.", MinimumLength = 5)]
        public string LOGIN { get; set; }

        [Display(Name = "Senha")]
        [Required(ErrorMessage = "Campo Senha Obrigatório")]
        [StringLength(20, ErrorMessage = "O campo {0} deve ter pelo menos {2} caracteres.", MinimumLength = 6)]
        public string SENHA { get; set; }

        [Display(Name = "Confirma Senha")]
        [Required(ErrorMessage = "Campo Confirma Senha Obrigatório")]
        [Compare("SENHA", ErrorMessage = "Os campos Senha e Confirma Senha não podem ser diferentes.")]
        public string CONFIRMASENHA { get; set; }

        [Display(Name = "Termos de Uso do Reclame Agora")]
        public string INFORMACOES { get; set; }

        [Display(Name = "Li e estou de acordo com os termos de uso do Reclame Agora.")]
        public bool auxpoliticauso { get; set; }

        [Display(Name = "Perfil")]
        public string TIPOCLIENTE { get; set; }

        public static string politicauso()
        {
            string polusorecagora = "" +
         "   Do Uso do Site ReclameAgora.com.br\r\n" +
         "\r\n" +
         "                O Reclame Agora! convida-o a visitar o seu site (http://reclameagora.apphb.com/) e lhe informa os Termos e Condições que regem o uso do mesmo e de seus serviços disponíveis.\r\n" +
         "\r\n" +
         "                Por favor, leia atentamente as condições abaixo estipuladas para que você possa usufruir dos serviços prestados pelo site e lembre-se que ao utilizá-los, você estará declarando ter ciência do presente Termos e Condições de Uso do Site e estará concordando com todas as suas cláusulas e condições.\r\n" +
         "\r\n" +
         "                Caso você não concorde com qualquer disposição destes Termos e Condições de Uso, por favor, não utilize nossos serviços.\r\n" +
         "\r\n" +
         "                1. Dos Serviços\r\n" +
         "\r\n" +
         "                1.1 O Reclame Agora é proprietário e titular exclusivo dos direitos do site http://reclameagora.apphb.com/ ,e também do domínio http://reclameagora.apphb.com/ site este que disponibiliza uma ferramenta de interação e intermediação de problemas entre o consumidor e fornecedor dentro do que prevê a legislação brasileira.\r\n" +
         "\r\n" +
         "                1.1.1 A qualquer momento poderão ser incluídas novas ferramentas, serviços e atividades no site do Reclame Agora, bem como excluídas as já existentes, sem qualquer aviso prévio ao USUÁRIO.\r\n" +
         "\r\n" +
         "                1.1.2 Toda e qualquer nova ferramenta ou atividade incluída no Reclame Agora, estará, automaticamente, vinculada e subordinada a todos os termos e condições do presente contrato.\r\n" +
         "\r\n" +
         "                2. Quem pode utilizar os serviços\r\n" +
         "\r\n" +
         "                2.1 Os serviços estão disponíveis somente a pessoas maiores de 18 (dezoito) anos e que possuam capacidade civil, de acordo com o estabelecido no Código Civil brasileiro.\r\n" +
         "\r\n" +
         "                2.2 O Reclame Agora destina-se a pessoas físicas nacionais e estrangeiras, submetendo-se estas últimas, desde já, à legislação brasileira.\r\n" +
         "\r\n" +
         "\r\n" +
         "                3. Da Utilização do Site\r\n" +
         "\r\n" +
         "                3.1 O Reclame Agora é um site de utilidade pública e dessa forma, seus serviços são inteiramente gratuitos para os USUÁRIOS. Através do Reclame Agora o USUÁRIO poderá utilizar a ferramenta de pesquisa para definir auxiliar em suas possíveis compras de bens de consumo ou de serviços de fornecedores devidamente cadastrados no Reclame Agora!\r\n" +
         "\r\n" +
         "                3.2 Para a utilização da ferramenta de pesquisa do site, não é necessário o cadastramento do USUÁRIO no site, porém, para as demais atividades como fazer uma reclamação ou participar de uma pesquisa o cadastro é obrigatório.\r\n" +
         "\r\n" +
         "                4. Do Cadastro do Usuário\r\n" +
         "\r\n" +
         "                4.1 Quando necessário o cadastro no site, o USUÁRIO deverá manifestar o seu consentimento com o presente Termos e Condições de Uso do Site eletronicamente, clicando no botão “eu concordo”.\r\n" +
         "\r\n" +
         "                4.2 Com a concordância expressa do USUÁRIO, caberá a este efetivar o seu cadastramento no site do Reclame Agora, fornecendo, nos campos específicos, todas as informações solicitadas, tais como endereço eletrônico, nome completo, CPF e RG, data de nascimento e CEP.\r\n" +
         "\r\n" +
         "                4.3 Ao se cadastrar, o USUÁRIO deverá escolher um nome de usuário (“login”) que devera ser seu e-mail e uma senha secreta, os quais serão pessoais e intransferíveis e garantirão o acesso do USUÁRIO ao site do Reclame Agora!, sendo possível navegar e participar das atividades em seu nome, acessar e alterar todas as suas informações pessoais e cadastros.\r\n" +
         "\r\n" +
         "                4.3.1 É de inteira responsabilidade do USUÁRIO manter a senha em sigilo, sendo o único e exclusivo responsável por toda atividade realizada com a sua conta, para o que é necessário digitar sua senha secreta. O uso do nome de usuário e senha é imprescindível para a utilização de alguns serviços do site.\r\n" +
         "\r\n" +
         "                4.3.2 O USUÁRIO compromete-se a, sob nenhuma hipótese, ceder, emprestar ou revelar sua senha a terceiros.\r\n" +
         "\r\n" +
         "                4.3.3 O USUÁRIO concorda e está obrigado a:\r\n" +
         "\r\n" +
         "                (a) notificar imediatamente o Reclame Agora! por escrito(enviando para o e-mail empresas@reclameAgora.com.br), sobre qualquer suspeita ou conhecimento de uso não autorizado da sua senha, dados cadastrais, conta ou qualquer outra quebra de segurança; e\r\n" +
         "                (b) sair de sua conta de usuário ao final de cada sessão e assegurar que esta não seja acessada por terceiros não autorizados. O Reclame Agora não será responsável por qualquer perda ou dano decorrente do descumprimento do disposto nesta cláusula por parte do USUÁRIO.\r\n" +
         "\r\n" +
         "                4.4 O USUÁRIO deverá registrar-se em formulário específico de cadastro, fornecendo informações verdadeiras, exatas e atuais sobre si mesmo, bem como deverá conservar e atualizar referidas informações imediatamente sempre que houver qualquer mudança dos dados, a fim de mantê-los verdadeiros, exatos e atuais.\r\n" +
         "\r\n" +
         "                4.5 Para o cadastramento no site, é expressamente proibida a criação de conta em nome de terceiros. O USUÁRIO se compromete a informar seu próprio endereço eletrônico a ser utilizado para o recebimento de mensagens advindas de seu cadastro.\r\n" +
         "\r\n" +
         "                4.6 Será de total e única responsabilidade do USUÁRIO quaisquer prejuízos decorridos pelo acesso não desejado, por terceiros, às informações inseridas e/ou disponibilizadas no site do Reclame Agora.\r\n" +
         "\r\n" +
         "                4.7 O Reclame Agora Compromete-se a não utilizar as informações cadastrais fornecidas pelo USUÁRIO para a realização de quaisquer atividades ilícitas, mas somente para aquelas expressamente permitidas pela legislação brasileira e/ou pelo presente instrumento.\r\n" +
         "\r\n" +
         "                4.8 O USUÁRIO poderá autorizar o Reclame Agora por meio de campos específicos no site, a locar o seu e-mail ou cadastro pessoal para terceiros, de determinado ramo de atividade escolhido pelo próprio USUÁRIO, para o recebimento de correspondências físicas e/ou eletrônicas (e-mail).\r\n" +
         "\r\n" +
         "                4.9 O Reclame Agora fica desde já autorizado a divulgar todo conteúdo criado pelo USUARIO sendo por intermédio de uma reclamação ou de uma pesquisa de opinião, e autorizado a repassar seu contato para imprensa para divulgação e reportagens que venha a contribuir de forma positiva e como alerta a toda a população Brasileira.\r\n" +
         "\r\n" +
         "                4.10 Todos e quaisquer dados ou informações fornecidas pelo USUÁRIO ao Reclame Agora!, poderão ser imediatamente reveladas por este em cumprimento à ordem judicial ou administrativa.\r\n" +
         "\r\n" +
         "                5. Da Propriedade Intelectual e da Responsabilidade pelas Informações\r\n" +
         "\r\n" +
         "                5.1 Todo material e serviço encontrado nas páginas do Reclame Agora! (textos, imagens, áudio, tecnologia, logotipos, slogans, marcas, expressões de propaganda, domínios, nomes comerciais, obras intelectuais), bem como os softwares que viabilizam as atividades, são de exclusiva propriedade do Reclame Agora, que detém todos os direitos autorais, licenciamentos e direitos de propriedade, marca ou patente a eles relativos. Qualquer violação dos direitos de propriedade pelo USUÁRIO resultará na sua responsabilidade, direta e pessoal, pelos atos praticados, nos termos da lei e com a aplicação das sanções cabíveis cíveis e criminais.\r\n" +
         "\r\n" +
         "                5.2 O USUÁRIO se compromete a não reproduzir, duplicar, copiar ou explorar os serviços prestados, softwares utilizados e/ou materiais disponibilizados pelo Reclame Agora, para quaisquer fins não expressamente autorizados no presente termo, sem sua autorização por escrito.\r\n" +
         "\r\n" +
         "                5.3 O USUÁRIO declara-se plenamente ciente de que não são de responsabilidade ou autoria do Reclame Agora, sob nenhuma hipótese: (i) as informações postadas no Reclame Agora Juntamente com seus possíveis comentários e replicas postadas no Reclame Agora\r\n" +
         "\r\n" +
         "                5.4 Nenhuma responsabilidade, de nenhuma espécie, seja ela de acessibilidade, veracidade, legalidade do conteúdo ou outra qualquer, poderá ser imputada ao Reclame Agora, Quando o USUÁRIO, por meio do site, adentrar-se em sites ou links de propriedade, administração e/ou controle de terceiros, tais como patrocinadores, parceiros, prestadores de serviços em geral, demais usuários, etc., os quais serão os únicos e exclusivos responsáveis.\r\n" +
         "\r\n" +
         "                5.5 Por sua vez, todo e qualquer material, dado ou informação, incluídos pelo USUÁRIO em qualquer página do Reclame Agora, será de plena e total responsabilidade deste, que responderá exclusivamente por quaisquer prejuízos que sejam causados a terceiros e/ou ao Reclame Agora.\r\n" +
         "\r\n" +
         "                5.6 O usuário quando postar uma reclamação no Reclame Agora ele tem a ciência que a mesma não poderá ser retirada de maneira alguma, mesmo respondida pela Reclamada, pois a mesma ficará como conteúdo de estatística do site, e como referêrencia para outros consumidores .\r\n" +
         "\r\n" +
         "                5.7 O USUÁRIO não poderá incluir comentários ilícitos no site do Reclame Agora, de forma a atribuir a alguém a prática de crime, imputar a alguém fato ofensivo à sua reputação, e, ofender alguém atentando contra sua dignidade ou decoro, e muito menos usar o site para se promover ou captar algum tipo de serviços em benefício próprio.\r\n" +
         "\r\n" +
         "                5.8 Ao utilizar o serviço do Reclame Agora!, o USUÁRIO deverá pautar suas opiniões e comentários em conformidade com a lei e a moral, não podendo usar o serviço para:\r\n" +
         "\r\n" +
         "                (a) submeter, postar, ou transmitir por qualquer meio, conteúdo que seja difamatório, calunioso, injurioso, abusivo, vulgar, obsceno, ou que de qualquer forma atente contra a moral e os bons costumes;\r\n" +
         "\r\n" +
         "                (b) submeter, postar ou transmitir por qualquer meio, conteúdo que infrinja ou viole direitos de terceiros, incluídos direitos da personalidade e de propriedade intelectual;\r\n" +
         "\r\n" +
         "                (c) submeter, postar ou transmitir por qualquer meio, vírus, arquivos corrompidos, ou quaisquer outros programas que possam danificar, ainda que momentaneamente, a operação de computador alheio;\r\n" +
         "\r\n" +
         "                (d) submeter, postar ou transmitir por qualquer meio, propaganda ou oferta de venda de produtos ou serviços com intuito comercial;\r\n" +
         "\r\n" +
         "                (e) submeter, postar ou transmitir por quaisquer meio, protestos, manifestações política ou religiosa, pirâmides, esquemas, spams, etc.; e\r\n" +
         "\r\n" +
         "                (f) submeter, postar ou transmitir por qualquer meio, conteúdo que seja contrário à lei.\r\n" +
         "\r\n" +
         "                (g) submeter, postar ou transmitir reclamações em duplicidade, ou seja, postar várias reclamações relativas a uma mesma relação de consumo.\r\n" +
         "\r\n" +
         "                5.9 O Reclame Agora não endossa as opiniões e comentários ofensivos e duvidosos dos USUÁRIOS, sendo ques estes sempre que detectados serão retirados do Reclame Agora a qualquer momento.\r\n" +
         "\r\n" +
         "                5.10 Caso chegue ao conhecimento do Reclame Agora qualquer violação às condições acima, fica o USUÁRIO ciente de que poderá ter suas opiniões editadas a fim de descaracterizar qualquer das condutas acima elencadas. A referida edição poderá ocorrer a qualquer momento a partir do conhecimento do Reclame Agora!! e sem a prévia notificação ao USUÁRIO. Fica certo que a opinião não poderá ter seu valor alterado, mas tão somente editada com o intuito de descaracterizar textos que estejam em desacordo com a lei e a moral.\r\n" +
         "\r\n" +
         "                5.11 O USUÁRIO autoriza o envio e a utilização dos seus dados cadastrais, informados no momento do cadastramento neste site, por empresas parceiras do Reclame Agora, bem como se declara ciente e concorda que, ao transmitir e/ou enviar ao Reclame Agora quaisquer materiais, informações e/ou dados, estará automaticamente cedendo os seus direitos de uso e divulgação ao Reclame Agora e aos demais usuários, que poderão utilizá-los livremente, e que está ciente de que também poderá ter suas informações e publicações indexadas em buscadores de conteúdo, tais como o Google.\r\n" +
         "\r\n" +
         "                5.12 Quando não for de sua própria e/ou exclusiva autoria, o USUÁRIO deverá possuir a expressa autorização do(s) legítimo(s) titular(es) de toda e qualquer espécie de material, informação e/ou dado que disponibilize para o uso e para a sujeição aos termos do presente contrato. Em qualquer caso, o USUÁRIO remetente será o único responsável perante o Reclame Agora! e terceiros, nos termos da lei, por quaisquer reclamações ou reivindicações de terceiros concernentes à autoria, uso indevido ou prejuízos relacionados ao uso de referido material.\r\n" +
         "\r\n" +
         "                5.13 Caso o Reclame Agora venha a ser responsabilizado, demandado judicial ou administrativamente, ou de qualquer forma inquirido pela veiculação ou uso indevido ou não autorizado de materiais, informações e/ou dados de propriedade e titularidade de terceiros, o USUÁRIO, como único responsável por sua remessa ao Reclame Agora, Será chamado para responder às acusações ou indagações perante o reclamante, devendo, se o caso, ressarci-lo integralmente pelas perdas e danos daí decorrentes.\r\n" +
         "\r\n" +
         "                5.14 O Reclame Agora!! Não se responsabiliza em nenhuma hipótese, e o USUÁRIO assume de forma integral o risco, de algum outro usuário e/ou terceiro copiar as informações que tenham sido enviadas e/ou inseridas no site do Reclame Agora!, para outros fins, mesmo que ilícitos, alheios ou não às atividades do Reclame Agora.\r\n" +
         "\r\n" +
         "                5.15 O USUÁRIO que encaminhar qualquer material para inserção no site, será obrigado perante o Reclame Agora a responder perante terceiros por toda e qualquer responsabilidade do mesmo oriunda, de qualquer natureza, seja ela financeira, patrimonial, autoral ou moral.\r\n" +
         "\r\n" +
         "                5.16 Qualquer informação ou material encaminhado, inserido e/ou enviado pelo USUÁRIO ao Reclame Agora não poderá jamais ter conteúdo obsceno, preconceituoso, discriminatório, pornográfico, ofensivo à moral, bons costumes, ética e/ou à legislação em geral, sob pena daquele responder nos termos da lei por perdas e danos causados à terceiros e/ou ao Reclame Agora, sem prejuízo das sanções criminais porventura cabíveis.\r\n" +
         "\r\n" +
         "                5.17 O USUÁRIO fica terminantemente proibido de:\r\n" +
         "\r\n" +
         "                (a) utilizar-se das atividades do Reclame Agora! para fins comerciais, mediante o envio de publicidade, correspondências institucionais, malas diretas, propagandas ou qualquer outro material de natureza semelhante; e\r\n" +
         "\r\n" +
         "                (b) enviar quaisquer espécies de arquivos e/ou material com vírus que possam causar danos ao software/hardware Reclame Agora!e/ou demais usuários.\r\n" +
         "\r\n" +
         "\r\n" +
         "                6. Da Ausência de Responsabilidade Reclame Agora\r\n" +
         "\r\n" +
         "                6.1 O Reclame Agora! envidará seus melhores esforços para manter o site acessível de forma constante, ininterrupta e isenta de quaisquer erros, no entanto, o Reclame Agora! Não será responsável por danos decorrentes de falha ou interrupção na prestação dos serviços. O USUÁRIO reconhece e aceita, que o Reclame Agora!! também não será responsável:\r\n" +
         "\r\n" +
         "                (a) pela impossibilidade de acesso ao site do Reclame Agora ou falha de comunicação com o mesmo decorrente de: (i) quaisquer defeitos ou inadequação dos equipamentos utilizados pelo USUÁRIO para acessar o Reclame Agora!, incluindo, mas não se limitando, aos softwares, hardwares, sistemas de processamento e quaisquer conexões de rede; (ii) inabilidade do USUÁRIO para operação dos seus equipamentos; (iii) falta de compreensão das instruções contidas no Reclame Agora; (iv) falhas na rede mundial de computadores (Internet) e provedores; (v) falhas nos sistemas, softwares e/ou hardwares do Reclame Agora; (vi) interrupções propositais realizadas pelo Reclame Agora por quaisquer motivos; (vii) interrupção das atividades do site; e (viii) caso fortuito ou força maior;\r\n" +
         "\r\n" +
         "                (b) por quaisquer prejuízos causados por gravação realizada pelo USUÁRIO (download) para os seus equipamentos próprios, de quaisquer arquivos eletrônicos existentes ou disponibilizados no site, quer pelo Reclame Agora, quer por outros usuários, quer por terceiros;\r\n" +
         "\r\n" +
         "                (c) por quaisquer prejuízos resultantes de, ou relacionados a, qualquer dos serviços ou trabalhos apresentados pelos anunciantes e/ou patrocinadores ou parceiros do Reclame Agora, tais como anúncios e promoções veiculados (inclusive, mas sem limitação, prejuízos resultantes do descumprimento pelos anunciantes e/ou patrocinadores ou parceiros das disposições aplicáveis pelo Código de Defesa do Consumidor), que serão de única responsabilidade dos mesmos;\r\n" +
         "\r\n" +
         "                (d)por quaisquer prejuízos advindos ao USUÁRIO pela utilização indevida e/ou dolosa por terceiros, dos materiais, dados pessoais e cadastrais fornecidos por aquele, que forem veiculados no Reclame Agora na forma descrita e prevista no presente contrato;\r\n" +
         "\r\n" +
         "                (e) por quaisquer prejuízos decorrentes da utilização indevida do nome de usuário e senha por terceiros;\r\n" +
         "\r\n" +
         "                (f) por eventual inviabilidade técnica de efetivo envio de informações ao Reclame Agora! decorrente da impossibilidade de acesso ou falha de comunicação atribuível ao Reclame Agora ou ainda, por falhas ordinárias ou extraordinárias, principalmente aquelas que possam resultar em perda de dados e informações previamente obtidas e armazenadas, fluxo de informações a serem obtidas, manutenção do cadastro do USUÁRIO junto ao Reclame Agora, bem como por interrupções momentâneas ou definitivas nos serviços;\r\n" +
         "\r\n" +
         "                (g)por danos civis ou criminais decorrentes da publicação e exposição de materiais visuais, auditivos ou textuais, inseridos no site do Reclame Agora por usuários, sendo certo que a responsabilidade é integralmente do USUÁRIO que as enviou;\r\n" +
         "\r\n" +
         "                (h)por atos de má-fé de terceiros que promovam a invasão do programa do Reclame Agora, tais como hackers, que acessem os dados cadastrais e pessoais fornecidos pelo USUÁRIO e que se utilizem ilicitamente dos mesmos para quaisquer fins. O Reclame Agora declara ter os cuidados razoáveis para evitar a invasão do sistema, mas não se responsabiliza e não pode se responsabilizar, pela inviolabilidade do mesmo;\r\n" +
         "\r\n" +
         "                (i)pela perda de dados e/ou informações eventualmente gravados pelo USUÁRIO no banco de dados do Reclame Agora!, seja por rescisão do contrato, seja por falha de sistema. Reclame Agora!não se obriga, ainda, a manter e/ou realizar qualquer espécie de 'back-up' dos materiais e dados fornecidos pelo USUÁRIO; e\r\n" +
         "\r\n" +
         "                (j) pela inadimplência do USUÁRIO às normas da lei ou do presente, que gere prejuízos a terceiros e/ou a outros usuários.\r\n" +
         "\r\n" +
         "\r\n" +
         "                7. Do Prazo, Rescisão e Modificação de Cláusulas\r\n" +
         "\r\n" +
         "                7.1 O presente contrato vigorará por tempo indeterminado ou durante o período em que o Reclame Agora! estiver disponibilizando os seus serviços via internet. O Reclame Agora!, no entanto, reserva-se o direito de, a qualquer momento, sem qualquer ônus ou aviso prévio, encerrar suas atividades ou limitar o uso de seus serviços.\r\n" +
         "\r\n" +
         "                7.2 O Reclame Agora reserva-se o direito de bloquear o acesso e rescindir de imediato, sem aviso prévio, a prestação dos serviços, toda vez que, a seu exclusivo critério, entender que há indícios de utilização fraudulenta ou ilícita do site do Reclame Agora! pelo USUÁRIO. Inclui-se na presente situação, entre outras, quaisquer indícios (a critério único e subjetivo do Reclame Agora!) de que o USUÁRIO fraudou ou infringiu o sistema de segurança do site ou até mesmo fez reclamações em duplicidade para prejudicar o fornecedor cadastrado.\r\n" +
         "\r\n" +
         "                7.2.1 Também poderá o Reclame Agora rescindir de imediato a prestação dos serviços do site, impedindo inclusive o recadastramento do USUÁRIO que tiver de qualquer maneira agido contra a moral, ética, bons costumes, lei, disposições deste termo ou, ainda, que tenha sido expulso do site do Reclame Agora.\r\n" +
         "\r\n" +
         "                7.3 Em todos os casos de cessação da prestação dos serviços previstos no presente instrumento, seja por decisão do USUÁRIO ou do Reclame Agora, seja por expulsão do USUÁRIO do site Reclame Agora, ou pelo encerramento das atividades do Reclame Agora!, as informações, imagens, textos, arquivos e quaisquer dados referentes ao USUÁRIO serão perdidos em sua totalidade, não cabendo a este qualquer espécie de indenização ou compensação.\r\n" +
         "\r\n" +
         "                7.3.1 O USUÁRIO declara-se ciente e concorda que, após o término da prestação dos serviços, suas informações, opiniões, textos, dados e/ou quaisquer materiais por ele encaminhados ao site do Reclame Agora poderão permanecer no site e ser utilizados pelo Reclame Agora!, sem quaisquer ônus para o Reclame Agora, com a permanência da sujeição desses materiais aos termos do presente instrumento por prazo indeterminado, ou até que o USUÁRIO desligado do Reclame Agora! solicite expressamente a sua retirada do site.\r\n" +
         "\r\n" +
         "                7.4 O Reclame Agora reserva-se, ainda, o direito de, a qualquer momento, alterar o disposto neste instrumento. Caso o USUÁRIO continue utilizando os serviços do Reclame Agora após a elaboração dos novos Termos e Condições de Uso do Site, este terá prosseguimento normal, estando configurada a sua aceitação pelo USUÁRIO.\r\n" +
         "\r\n" +
         "                7.4.1 Caso o USUÁRIO não concorde com qualquer disposição da nova versão dos Termos e Condições de Uso do Site, este deverá encaminhar a notificação ao Reclame Agora, informando sobre o seu não interesse em continuar utilizando os serviços do Reclame Agora, ficando vedado o seu acesso ao site.\r\n" +
         "\r\n" +
         "\r\n" +
         "                8. Das Penalidades\r\n" +
         "\r\n" +
         "                8.1 O descumprimento de quaisquer das cláusulas e condições do presente instrumento por parte do USUÁRIO poderá gerar penalidades administrativas a serem impostas ao mesmo pelo Reclame Agora\r\n" +
         "\r\n" +
         "                8.2 As penalidades passíveis de imposição pelo Reclame Agora sempre que o USUÁRIO tiver praticado uma infração à lei, aos direitos e interesses de terceiros e/ou de outros usuários e/ou aos termos do presente contrato serão:\r\n" +
         "\r\n" +
         "                1. ADVERTÊNCIA: O Reclame Agora! encaminhará ao e-mail do USUÁRIO uma advertência, toda vez que esse tiver incidido em uma infração leve;\r\n" +
         "                2. SUSPENSÃO: Reclame Agora!poderá suspender o acesso do USUÁRIO ao site por um prazo de 3 (três) a 30 (trinta) dias, toda vez que esse tiver incidido em uma infração grave, ou automaticamente no recebimento da terceira advertência; e\r\n" +
         "                3. EXPULSÃO: Reclame Agora! expulsará o USUÁRIO, proibindo definitivamente o seu acesso, toda vez que esse tiver incidido em uma infração gravíssima, ou automaticamente no recebimento da terceira suspensão.\r\n" +
         "\r\n" +
         "\r\n" +
         "                8.3 O Reclame Agora definirá a seu livre e exclusivo critério, o grau de gravidade da infração praticada, bem como, no caso da suspensão, o período da mesma.\r\n" +
         "\r\n" +
         "                8.4 As penalidades acima previstas não eximem o USUÁRIO infrator, à responder se for o caso, civil e criminalmente, por seus atos, perante o Reclame Agora, usuários e/ou terceiros prejudicados.\r\n" +
         "\r\n" +
         "                8.5 O USUÁRIO terá um prazo de 5 (cinco) dias para recorrer da pena de suspensão e/ou de advertência, mediante o envio de suas razões ao Reclame Agora, tendo o mesmo total autonomia e liberdade para acatar ou não os seus argumentos.\r\n" +
         "\r\n" +
         "\r\n" +
         "                9. Das Condições Gerais\r\n" +
         "\r\n" +
         "                9.1 O USUÁRIO, ao utilizar os serviços do Reclame Agora Aceita expressamente todas as cláusulas do presente instrumento, bem como as atividades que existem e que venham a existir no site do Reclame Agora, Reconhecendo-as como inocentes, saudáveis, de boa-fé e não-ofensivas, aceitando suas regras, instruções e condições constantes do próprio site, de forma absoluta e irrestrita, garantindo que não se sentirá de qualquer forma ultrajado, ofendido ou prejudicado pelas mesmas.\r\n" +
         "\r\n" +
         "                9.2 Caso o Reclame Agora! venha a criar alguma nova atividade, que de alguma forma o USUÁRIO não considere inocente, saudável, de boa-fé ou não-ofensiva ou ainda, que possa eventualmente ofendê-lo ou prejudicá-lo, o USUÁRIO obriga-se a cessar o uso dos serviços.\r\n" +
         "\r\n" +
         "                9.3 Os serviços de responsabilidade de terceiros e/ou ofertados por esses, tais como serviços de telecomunicações, provedor (Internet paga), entre outros que possam ser encontrados no site do Reclame Agora, deverão ser pagos diretamente aos terceiros prestadores de serviços, não cabendo ao Reclame Agora nenhuma responsabilidade sobre a cobrança e/ou qualidade dos mesmos.\r\n" +
         "\r\n" +
         "                9.4 Qualquer reclamação judicial ou extrajudicial que venha a ser formulada em face do Reclame Agora, em razão de material enviado pelo USUÁRIO, ou descumprimento ou infringência de qualquer cláusula ou disposição constante do presente instrumento, obrigará o USUÁRIO a responder integralmente e, se for o caso, regressivamente, por todas as perdas e danos causados ao Reclame Agora! e/ou à terceiros, ficando autorizada, desde já a denunciação da lide do USUÁRIO, nos exatos termos do art. 70, III, do Código de Processo Civil.\r\n" +
         "\r\n" +
         "                9.5 O domínio do site do Reclame Agora! é exclusivamente o http://reclameagora.apphb.com/ e http://reclameagora.apphb.com/ . Sendo assim, o Reclame Agora não se responsabilizará por quaisquer acessos do USUÁRIO à outros domínios, mesmo que tais se apresentem com as mesmas características do Reclame Agora.\r\n" +
         "\r\n" +
         "                9.6 O Reclame Agora não se responsabiliza nem pelo conteúdo nem pelas políticas e práticas de privacidade dos sites que apontam para o Reclame Agora e daqueles para os quais apontamos.\r\n" +
         "\r\n" +
         "                9.7 O Reclame Agora se reserva o direito de modificar a qualquer momento, de forma unilateral, sem prévia ou posterior notificação, os presentes Termos e Condições de Uso.\r\n" +
         "\r\n" +
         "                9.7.1 Ao utilizar os serviços do Reclame Agora, você aceita as cláusulas e condições do termo que estiver vigente na data do acesso e, portanto, deve verificar o mesmo frequentemente ou, previamente, cada vez que visitar o site.\r\n" +
         "\r\n" +
         "                9.8 O Reclame Agora mantém à disposição do USUÁRIO um canal para esclarecimento de dúvidas, sugestões e reclamações relativas aos serviços prestados, através do link http://www.reclameAgora.com.br/contato/ ou do e-mail empresas@reclameAgora.com.br\r\n" +
         "                9.9 Para uma maior segurança do Reclame Agora e dos usuários, é necessário que o USUÁRIO esteja atento aos seguintes pontos:\r\n" +
         "\r\n" +
         "                (a) Nunca forneça sua senha a terceiros. Ela é pessoal e intransferível;\r\n" +
         "                (b) ao criar a sua senha, não utilize senhas óbvias, tais como nome próprio, iniciais, com parentesco, data de nascimento, etc. Procure utilizar letras e números a fim de atender aos padrões e requisitos mínimos de segurança;\r\n" +
         "                (c) caso tenha fornecido a sua senha a terceiros, avise imediatamente o Reclame Agora! através do e-mail suporte@reclameagora.com.br, para que o mesmo proceda à alteração de sua senha;\r\n" +
         "                (d) a transmissão de vírus se dá através de e-mails que solicitam a digitação de senhas, ou que possuam informações de cobrança ou contêm arquivo anexado com vírus. Esteja atento e caso desconfie da procedência do e-mail, não o abra; e\r\n" +
         "                (e) a transmissão de vírus e de programas destrutivos que podem fazer com que o seu computador divulgue informações pessoais, também se dá comumente através do download de arquivos infectados. Portanto, se você não conhece quem os enviou ou caso você não o tenha solicitado, não efetue o download.\r\n" +
         "\r\n" +
         "                10. Da Lei Aplicável e Foro" +
         "                10.1 As presentes condições são regidas única e exclusivamente pelas leis da República Federativa do Brasil e qualquer discussão judicial que surja tendo por base sua interpretação ou aplicação deverá ser julgado por tribunais brasileiros, estando desde logo eleito o foro da cidade de Itaúna, Estado de Minas Gerais, por mais privilegiado que outro seja ou possa vir a ser.";
            return polusorecagora;
        }      
    }

    public class ConverteCliente
    {
        /// <summary>
        /// Criar um novo objeto cliente do para retornar o cliente criado automaticamente pelo projeto em Modelrdc.Designer.cs.
        /// </summary>
        /// <param name="iDCLIENTE">Initial value of the IDCLIENTE property.</param>
        /// <param name="nOME">Initial value of the NOME property.</param>
        /// <param name="eMAIL">Initial value of the EMAIL property.</param>
        /// <param name="lOGIN">Initial value of the LOGIN property.</param>
        /// <param name="sENHA">Initial value of the SENHA property.</param>
        /// <param name="cONFIRMASENHA">Initial value of the CONFIRMASENHA property.</param>
        public static cliente Createcliente(global::System.Int32 iDCLIENTE, global::System.String nOME,
            global::System.String eMAIL, global::System.String lOGIN, global::System.String sENHA,
            global::System.String cONFIRMASENHA, global::System.String cEP, global::System.String cPF,
            global::System.String rG, global::System.String eNDERECO, Nullable<global::System.Int32> iDCIDADE,
            global::System.String eSTADO, global::System.String iNFORMACOES, Nullable<global::System.DateTime> nASCIMENTO,
            global::System.String tIPOCLIENTE, global::System.String fONEFIXO, global::System.String fONECELULAR,
            Nullable<global::System.Int32> IDFORNECEDOR = null)
        {
            cliente cliente = new cliente();
            cliente.IDCLIENTE = iDCLIENTE;
            cliente.NOME = nOME;
            cliente.EMAIL = eMAIL;
            cliente.LOGIN = lOGIN;
            cliente.SENHA = sENHA;
            cliente.CONFIRMASENHA = cONFIRMASENHA;
            cliente.CEP = cEP;
            cliente.CPF = cPF;
            cliente.RG = rG;
            cliente.ENDERECO = eNDERECO;
            cliente.IDCIDADE = iDCIDADE;
            cliente.ESTADO = eSTADO;
            cliente.INFORMACOES = iNFORMACOES;
            cliente.NASCIMENTO = nASCIMENTO;
            cliente.TIPOCLIENTE = tIPOCLIENTE;
            cliente.FONEFIXO = fONEFIXO;
            cliente.FONECELULAR = fONECELULAR;
            cliente.IDFORNECEDOR = IDFORNECEDOR;
            return cliente;
        }
    /* Não será mais preciso essa função, somente a acima
    public static Cliente CreateCliente(global::System.Int32 iDCLIENTE, global::System.String nOME,
        global::System.String eMAIL, global::System.String lOGIN, global::System.String sENHA,
        global::System.String cONFIRMASENHA, global::System.String cEP, global::System.String cPF,
        global::System.String rG, global::System.String eNDERECO, Nullable<global::System.Int32> iDCIDADE,
        global::System.String eSTADO, global::System.String iNFORMACOES, Nullable<global::System.DateTime> nASCIMENTO,
        global::System.String tIPOCLIENTE, global::System.String fONEFIXO, global::System.String fONECELULAR)
    {
        Cliente Cliente = new Cliente();
        Cliente.IDCLIENTE = iDCLIENTE;
        Cliente.NOME = nOME;
        Cliente.EMAIL = eMAIL;
        Cliente.LOGIN = lOGIN;
        Cliente.SENHA = sENHA;
        Cliente.CONFIRMASENHA = cONFIRMASENHA;
        Cliente.CEP = cEP;
        Cliente.CPF = cPF;
        Cliente.RG = rG;
        Cliente.ENDERECO = eNDERECO;
        Cliente.IDCIDADE = iDCIDADE;
        Cliente.ESTADO = eSTADO;
        Cliente.INFORMACOES = iNFORMACOES;
        Cliente.NASCIMENTO = nASCIMENTO;
        Cliente.TIPOCLIENTE = tIPOCLIENTE;
        Cliente.FONEFIXO = fONEFIXO;
        Cliente.FONECELULAR = fONECELULAR;
        return Cliente;
    }
     */
    }
    /// <summary> /// Validação customizada para CPF/// </summary> 
    public class CustomValidationCPFAttribute : ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public CustomValidationCPFAttribute()
        {
        }
        /// <summary>     
        /// Validação server     
        /// </summary>     
        /// <param name="value"></param>     
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return true;
            bool valido = Util.ValidaCPF(value.ToString());
            return valido;
        }
        /// <summary>
        /// Validação client
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = this.FormatErrorMessage(null),
                ValidationType = "customvalidationcpf"
            };
        }
    }

}