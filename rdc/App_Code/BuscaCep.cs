using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.IO;

namespace rdc.App_Code
{
    public class BuscaCep
    {
        private string html;

        public string ConsultaCEP(string cependereco)
        {
            ConsultarCEP(cependereco);
            return html;
        }
        private void ConsultarCEP(string cep)
        {
            // Efetua a requisição ao site passando o CEP como querystring
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.buscacep.correios.com.br/servicos/dnec/consultaLogradouroAction.do?Metodo=listaLogradouro&CEP=" + cep + "&TipoConsulta=cep");
            int count;
            byte[] buf = new byte[1000];
            StringBuilder sb = new StringBuilder();
            string temp;

            // Recebe a resposta da requisição
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            // Transforma a requisio em um Stream
            Stream stream = httpWebResponse.GetResponseStream();

            // Carrega e despeja um buffer (array de bytes), apendando em um StringBuider enquanto o buffer não estiver vazio
            do
            {
                count = stream.Read(buf, 0, buf.Length);
                temp = Encoding.Default.GetString(buf, 0, count).Trim();
                sb.Append(temp);

            } while (count > 0);
            // Converte o String Buider para String
            html = sb.ToString();

            if (html.IndexOf("<?xml version = '1.0' encoding = 'ISO-8859-1'?>") != -1)
            {
                // Seleciona o String HTML de acordo com os delimitadores
                html = this.retornarHtml(html, "<?xml version = '1.0' encoding = 'ISO-8859-1'?>", "<table width=\"645\">");

                //Refinar mais ainda para retornar somente os valores dos registros
                html = this.retornarHtml(html, "<tr bgcolor=\"#ECF3F6\" onclick=\"javascript:detalharCep('1','2');\" style=\"cursor: pointer;\">", "</tr>");

                //Refinar as informações das linhas deixando somente as informações necessárias
                string delimIni = "<td ";
                string delimFim = "\">";
                string subDIniDFin = "<td>";
                html = ReplaceDelimIniDelimFim(html, delimIni, delimFim, subDIniDFin);
                //Deve-se sobrar somente 5 <td>'s abrindo e fechando as informações recuperadas 
                //<td width="268" style="padding: 2px">Rua Lagoa da Prata</td>
                //<td width="140" style="padding: 2px">Residencial Morro do So</td>
                //<td width="140" style="padding: 2px">Itaúna</td>
                //<td width="25" style="padding: 2px">MG</td>
                //<td width="65" style="padding: 2px">35680-286</td>
                //Limpar <table
                delimIni = "<table ";
                delimFim = "\">";
                subDIniDFin = "<table>";
                html = ReplaceDelimIniDelimFim(html, delimIni, delimFim, subDIniDFin);
                html = html.Replace("<table>", "");
                html = html.Replace("</table>", "");
                //Limpar <tr
                delimIni = "<tr ";
                delimFim = "\">";
                subDIniDFin = "<tr>";
                html = ReplaceDelimIniDelimFim(html, delimIni, delimFim, subDIniDFin);
                html = html.Replace("<tr>", "");
                html = html.Replace("</tr>", "");
            }
            else//caso não encontre o CEP ou o retorno não tenha os dados solicitados
            {
                html = null;
            }
        }

        //Retornar string fazendo o Replace de acordo com o Delimitador Inicial e Final em Loop percorrendo toda a string
        private string ReplaceDelimIniDelimFim(string texto, string DelimIni, string DelimFim, string substDIniDFin)
        {
            //Refinar as informações das linhas deixando somente as informações necessárias
            int posIni;
            int posFim;
            while (texto.IndexOf(DelimIni) != -1)
            {
                // Verifica a posio do delimitador inicial dentro do String HTML (soma a quantidade de caracteres do delimitador)
                posIni = texto.IndexOf(DelimIni);
                // Verifica a posio do ltimo caracter do delimitador final
                //posFim = html.IndexOf(delimFim) - posIni; --Alterado para 
                posFim = (texto.IndexOf(DelimFim, posIni) + 2) - posIni;
                // Retorna a String HTML de acordo com a posição inicial e a posio final
                texto = texto.Replace(texto.Substring(posIni, posFim), substDIniDFin);
            }
            return texto.Replace("\r\n", "").Replace("   ", "").Replace("\n","").Replace("\r","");
        }

        // Retorna o String HTML de acordo com os delimitadores
        private string retornarHtml(string html, string delimitadorInicial, string delimitadorFinal)
        {
            int posInicial;
            int posFinal;

            // Verifica se o delimitador inicial foi encontrado
            if (html.IndexOf(delimitadorInicial) != -1)
            {
                // Verifica a posio do delimitador inicial dentro do String HTML (soma a quantidade de caracteres do delimitador)
                posInicial = html.IndexOf(delimitadorInicial) + (delimitadorInicial.Length + 1);

                // Verifica a posio do ltimo caracter do delimitador final
                posFinal = html.IndexOf(delimitadorFinal, posInicial) - posInicial;

                // Retorna a String HTML de acordo com a posio inicial e a posio final
                html = html.Trim().Substring(posInicial, posFinal-1);
            }

            return html;
        } 
    }
}