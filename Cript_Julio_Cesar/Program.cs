using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Cript_Julio_Cesar
{
    class Program
    {
        static void Main(string[] args)
        {
            string TOKEN = "c8e1e1a6a5be3ee933591de5a6271e248dad3309";
            string strRequest = "https://api.codenation.dev/v1/challenge/dev-ps/generate-data?token=" + TOKEN;
            string strPost = "https://api.codenation.dev/v1/challenge/dev-ps/submit-solution?token=" + TOKEN;
            string responseFromServer;

            //Get Json do site
            WebRequest request = WebRequest.Create(strRequest);
            WebResponse response = request.GetResponse();
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
            }
            response.Close();

            //Descriptografa a mensagem
            Message message = JsonConvert.DeserializeObject<Message>(responseFromServer);
            message.Decript();
            message.toSHA1();

            //Salva a mensagem
            string answer = JsonConvert.SerializeObject(message, Formatting.Indented);
            File.WriteAllText("answer.json", answer);
        }
    }

    class Message
    {
        public int numero_casas { get; set; }
        public string token { get; set; }
        public string cifrado { get; set; }
        public string decifrado { get; set; }
        public string resumo_criptografico { get; set; }

        Message()
        {
        }

        public void Decript()
        {
            char[] alphabetic = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

            foreach (char chCif in cifrado.ToCharArray())
            {
                /*if (chCif.Equals(char.Parse(" ")))
                {
                    decifrado += "";
                }
                else */
                if (!alphabetic.Contains(chCif))
                {
                    decifrado += chCif;
                }
                else
                {
                    int pos = 0;
                    foreach (var chAlph in alphabetic)
                    {
                        if (chAlph.Equals(chCif))
                        {
                            decifrado += alphabetic[pos - numero_casas];
                            break;
                        }
                        pos++;
                    }
                }
            }
        }
        public void toSHA1()
        {
            using (var sha1 = new SHA1Managed())
            {
                resumo_criptografico = BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(decifrado))).Replace("-", "").ToLower();
            }
        }


    }
}
