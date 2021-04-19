using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class EmailBusiness
    {
        public void configClient(string emailReceive, string title, string content)
        {
            var fromAddress = new MailAddress(SystemParam.EMAIL_CONFIG);
            var toAddress = new MailAddress(emailReceive);
            string fromPassword = SystemParam.PASS_CONFIG;

            var smtp = new SmtpClient
            {
                Host = SystemParam.HOST_DEFAULT,
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = title,
                Body = content
            })
            {
                smtp.Send(message);
            }
        }



    }
}
