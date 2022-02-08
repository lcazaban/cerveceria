using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Cerveceria.Web.BLL
{
    public class MailManager
    {
        private string _emailOrigen { get; set; }
        private string _contraseña { get; set; }

        public MailManager()
        {
            var path = Path.GetFullPath(SystemEnums.Constantes.Path);
            var config = new ConfigurationManager(path);
            this._emailOrigen = config.GetValue<string>(SystemEnums.Constantes.Section, SystemEnums.Constantes.EmailOrigen);
            this._contraseña = config.GetValue<string>(SystemEnums.Constantes.Section, SystemEnums.Constantes.Password);
        }
        public bool SendMail(string asunto, string mensaje)
        {
            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential(this._emailOrigen.Trim(), this._contraseña.Trim());
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;

            try
            {
                MailAddress
                    maFrom = new MailAddress(this._emailOrigen.Trim(), "Hermanos Rocamayo Cerveceria", Encoding.UTF8),
                    maTo = new MailAddress(this._emailOrigen.Trim(), "Hermanos Rocamayo Cerveceria", Encoding.UTF8);
                MailMessage mmsg = new MailMessage(maFrom.Address, maTo.Address);
                mmsg.Body = mensaje;
                mmsg.BodyEncoding = Encoding.UTF8;
                mmsg.IsBodyHtml = true;
                mmsg.Subject = asunto;
                mmsg.SubjectEncoding = Encoding.UTF8;

                client.Send(mmsg);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool SendMailSomee(string emailDestino, string asunto, string mensaje)
        {
            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential(this._emailOrigen.Trim(), this._contraseña.Trim());
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;

            try
            {
                MailAddress
                    maFrom = new MailAddress(this._emailOrigen.Trim(), "Hermanos Rocamayo Cerveceria", Encoding.UTF8),
                    maTo = new MailAddress(emailDestino.Trim(), "Cliente", Encoding.UTF8);
                MailMessage mmsg = new MailMessage(maFrom.Address, maTo.Address);
                mmsg.Body = mensaje;
                mmsg.BodyEncoding = Encoding.UTF8;
                mmsg.IsBodyHtml = true;
                mmsg.Subject = asunto;
                mmsg.SubjectEncoding = Encoding.UTF8;

                client.Send(mmsg);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
