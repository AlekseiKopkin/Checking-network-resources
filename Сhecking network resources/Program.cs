using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Threading;

namespace Сhecking_network_resources
{
    class Program
    {
        static void Main(string[] args)
        {
            Ping png = new Ping();
            while (true)
            {
                string[] ip_address = File.ReadAllLines("Data/ip address.txt");
                string not_available_ip = "";
                foreach (string one_address in ip_address)
                {
                    PingReply pingReply = png.Send(one_address);
                    string date = DateTime.Now.ToString("[dd.MM.yyyy HH:mm:ss.fff]");
                    if (pingReply.Status == IPStatus.Success)
                    {
                        File.AppendAllText("Data/result.txt", date + "\t" + one_address + "\t" + "available" + "\r\n");
                    }
                    else
                    {
                        File.AppendAllText("Data/result.txt", date + "\t" + one_address + "\t" + "not available" + "\r\n");
                        not_available_ip = not_available_ip + one_address + ", ";
                    }
                }
                Email(not_available_ip);
                Thread.Sleep(TimeSpan.FromHours(5));
            }
        }
        static void Email(string not_available_ip)
        {
            string[] email_address = File.ReadAllLines("Data/email address.txt");
            string mail_address = "name_mail@mail.ru";
            string password = "password";
            foreach (string one_email_address in email_address)
            {
                MailAddress from = new MailAddress(mail_address, "Name");
                MailAddress to = new MailAddress(one_email_address);
                MailMessage message = new MailMessage(from, to);
                message.Subject = "Падение ресурса";
                message.Body = not_available_ip + " данные ресурсы пали";
                SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
                smtp.Credentials = new NetworkCredential(mail_address, password);
                smtp.EnableSsl = true;
                smtp.Send(message);
            }
        }
    }
}
