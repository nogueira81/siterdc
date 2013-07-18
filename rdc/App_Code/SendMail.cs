using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;

namespace rdc.App_Code
{
    public class SendMail
    {
        public static string EnviaEmail(string To, string nmeTo, string Subject, string Body, bool IsBodyHtml = true)
        {
            bool AtivarEnvioEmail = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["AtivarEnvioEmail"].ToString());
            if (AtivarEnvioEmail)
            {
                //Recuperar 9 argumentos do webconfig para envio de e-mail
                string Host = System.Configuration.ConfigurationManager.AppSettings["Host"].ToString();
                string Port = System.Configuration.ConfigurationManager.AppSettings["Port"].ToString();
                string EnableSsl = System.Configuration.ConfigurationManager.AppSettings["EnableSsl"].ToString();
                string Sender = System.Configuration.ConfigurationManager.AppSettings["Sender"].ToString();
                string NameSender = System.Configuration.ConfigurationManager.AppSettings["NameSender"].ToString();
                string From = System.Configuration.ConfigurationManager.AppSettings["From"].ToString();
                string NameFrom = System.Configuration.ConfigurationManager.AppSettings["NameFrom"].ToString();
                string USN = System.Configuration.ConfigurationManager.AppSettings["USN"].ToString();
                string Pwd = System.Configuration.ConfigurationManager.AppSettings["Pwd"].ToString();

                SmtpClient client = new SmtpClient();
                client.Host = Host;
                client.Port = int.Parse(Port);
                // SSL Desativado - Há servidores que requer essa função ativa
                client.EnableSsl = bool.Parse(EnableSsl);
                client.Credentials = new NetworkCredential(USN, Pwd);


                MailMessage mail = new MailMessage();
                mail.Sender = new MailAddress(Sender, NameSender);
                mail.From = new MailAddress(From, NameFrom);
                mail.To.Add(new MailAddress(To, nmeTo));
                mail.Subject = Subject;
                mail.Body = Body;

                // - Corpo Em HTML
                mail.IsBodyHtml = IsBodyHtml;

                // Alta Prioridade
                mail.Priority = MailPriority.High;

                try
                {
                    client.Send(mail);
                    return "Enviado com sucesso";
                }
                catch
                {
                    return "NAOENVIADO";
                }
            }
            else
            {
                return "Envio de E-mail Desativado no Web.Config";
            }
        }

        /*
            Host List:
            smtp.gmail.com // Gmail
            smtp.live.com // Windows live / Hotmail
            smtp.mail.yahoo.com // Yahoo
            smtp.aim.com // AIM
            my.inbox.com // Inbox
         */
        //Rotina de Envio de E-Mail
        public static string SendEMail()
        {
            //NetworkCredential cred = new NetworkCredential("YourEmailAccountAddress", "EmailAccountPass");
            MailMessage msg = new MailMessage();
            msg.To.Add("thino@ig.com.br");  //[COLOR=#006400]// Add a new recipient to our msg.[/COLOR]
            msg.From = new MailAddress("thiago.nogueira.mota@gmail.com");   //[COLOR=#006400]// Read below.[/COLOR]
            msg.Subject = "Reclame Agora";             //[COLOR=#006400]       // Assign the subject of our message.[/COLOR]
            msg.Body = "Prezado,\r\n" +
                "Favor verificar suas reclamações"+
                "para atualizar as novas Interações com o Fornecedor";   //[COLOR=#006400]// Create the content(body) of our message. [/COLOR]
            NetworkCredential cred = new NetworkCredential("thiago.nogueira.mota@gmail.com", "thi260181");
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.Credentials = cred; // Send our account login details to the client.
            client.EnableSsl = true;   // Read below.
            try
            {
                client.Send(msg);          // Send our email.
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}