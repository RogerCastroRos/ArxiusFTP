using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace ArxiusFTP
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri uri = new Uri("ftp://10.0.1.220//home//g2");
            string opcio;

            do
            {
                MostrarMenu();
                opcio = DemanarOpcioMenu();

                if (opcio == "PF")
                {

                }
                else if (opcio == "BF")
                {
                    readFTP(uri);
                }
                else if (opcio == "FE")
                {

                }
                else if (opcio == "FV")
                {

                }
            } while (opcio != "PF" && opcio != "BF" && opcio != "FE" && opcio != "FV" && opcio != "S");
        }
        static void MostrarMenu()
        {
            Console.WriteLine("            MENU");
            Console.WriteLine("--------------------------------");
            Console.WriteLine("PF - Programa per pujar fitxers al servidor FTP");
            Console.WriteLine("BF - Programa per baixar fitxers des del servidor FTP");
            Console.WriteLine("FE - Formulari processat de fitxer EDI");
            Console.WriteLine("FV - Formulari de visualització del llistat");
            Console.WriteLine("S - Sortir");
        }
        static string DemanarOpcioMenu()
        {
            string opcio;

            Console.Write("\nESCULL UNA OPCIÓ: ");
            opcio = Console.ReadLine().ToUpper();

            return opcio;
        }
        public static void readFTP(Uri uri)
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            // This example assumes the FTP site uses anonymous logon.
            request.Credentials = new NetworkCredential("g2", "12345aA");
            request.EnableSsl = false;

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);

            //Console.WriteLine(reader.ReadToEnd());

            List<string> arxius = new List<string>();

            string line = "";

            line = reader.ReadLine();
            while (line != null)
            {
                string[] Liniaarxiu = line.Split(' ');
                string arxiu = Liniaarxiu[Liniaarxiu.Length - 1];
                if (arxiu.EndsWith(".edi"))
                {
                    arxius.Add(arxiu);
                }

                line = reader.ReadLine();
            }

            reader.Close();

            foreach (string item in arxius)
            {
                downloadFile(uri, item, response);
            }

            Console.ReadLine();
        }
        public static void downloadFile(Uri uri, String item, FtpWebResponse response)
        {
            FtpWebRequest ftpRequest;
            ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://10.0.1.220//home//g2//" + item);
            ftpRequest.Credentials = new NetworkCredential("g2", "12345aA");
            ftpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            FtpWebResponse resp = (FtpWebResponse)ftpRequest.GetResponse();

            Stream respStream = resp.GetResponseStream();
            StreamReader r = new StreamReader(respStream);
            string contingut = r.ReadToEnd();
            File.WriteAllText("../../Edis/" + item, contingut);

            moveFile(uri, item, response);

            Console.WriteLine(item + $" has been downloaded successfully {response.StatusDescription} ");
            r.Close();
            resp.Close();
        }
        public static void moveFile(Uri uri, String item, FtpWebResponse response)
        {
            // rename file
            var req = (FtpWebRequest)WebRequest.Create(uri + "//" + item);
            req.Credentials = new NetworkCredential("g2", "12345aA");
            req.Method = WebRequestMethods.Ftp.Rename;
            req.RenameTo = "Tractats//" + item;

            Console.WriteLine(item + $" has been moved successfully {response.StatusDescription} ");

            req.GetResponse().Close();
        }
    }
}