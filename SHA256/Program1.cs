using System;
using System.Security.Cryptography;
using System.Text;

namespace MyApp;


internal class Program
{
    public static string EncodingSHA256(string text)
    {
        StringBuilder sb = new StringBuilder();
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
    
    static void Main(string[] args)
    {
        StringBuilder sb = new StringBuilder();

        using (FileStream file = new FileStream(Path.GetFullPath("../../../test.txt"), FileMode.Open))
        {
            using (StreamReader reader = new StreamReader(file))
            {
                sb.Append(reader.ReadToEnd());
            }
        }

        const string sha256 = "097a97d46ab69f79898e3ca61b927ddd967e07b0234f33a8e76603f2359f6c70";
        if (EncodingSHA256(sb.ToString()) == sha256)
            Console.WriteLine("Файл целостный!");
        else
            Console.WriteLine("Файл не целостный!");
            
    }
}
