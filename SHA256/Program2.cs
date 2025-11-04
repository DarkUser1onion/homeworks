using System;
using System.Security.Cryptography;
using System.Text;

namespace MyApp;

internal class Program
{
    public static string SHA256encoder(string text)
    {
        StringBuilder builder = new StringBuilder();
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
        }
        
        return builder.ToString(); 
    }
    
    static void Main(string[] args)
    {
        List<string> listPass = new List<string>();

        Console.Clear();   
        Console.WriteLine("Parsing rockyou.txt...");
        
        using (FileStream fileStream = new FileStream("../../../rockyou.txt", FileMode.Open))
        {
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    listPass.Add(line);
                }
            }
        }

        Console.WriteLine("Completed rockyou.txt...");
        Thread.Sleep(1000);
        Console.Clear();
        
        // Int64 counter = 1;
        // foreach (string s in listPass)
        // {
        //     Console.Write(counter++ + " : " + s + "\n");
        // }

        string tempSHA;
        string passwordSHA;
        
        Console.Write("Введите пароль, который будет искаться по хешу: ");
        string pass = Console.ReadLine();
        
        passwordSHA = SHA256encoder(pass);

        foreach (string s in listPass)
        {
            Console.WriteLine($"Пробую \"{s}\" пароль");
            if(SHA256encoder(s) == passwordSHA)
            {
                tempSHA = SHA256encoder(s);
                Console.WriteLine($"\nНайдено совпадение! : {tempSHA} --- {s}");
                return;
            }
        }
        
        Console.WriteLine("Не найдено совпадений!");
        
    }
}
