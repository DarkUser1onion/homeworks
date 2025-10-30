using System;

namespace MyApp;

internal class Program
{

    public static void ParseList(string n, ref List<bool> l)
    {
        for (int i = 0; i < n.Length; i++)
        {
            if(n[i] == '0')
                l.Add(false);
            
            else if (n[i] == '1')
                l.Add(true);

            else
                throw new Exception("Invalid number!");
            
        }
    }

    public static List<bool> XOR(List<bool> l, List<bool> polynomial)
    {
        List<bool> result = new List<bool>();
        
        for (int i = 0; i < l.Count; i++)
        {
            result.Add(l[i] ^ polynomial[i]);
        }
        
        return result;
    }

    public static List<bool> Shift(List<bool> l, int polynomialSize)
    {
        List<bool> result = new List<bool>(l);

        while (l.Count != polynomialSize)
        {
            
        }
    }
    
    static void Main(string[] args)
    {
        List<bool> list = new List<bool>();
        List<bool> polynomial = new List<bool>();
        
        Console.WriteLine("Enter the numbers: ");
        string n = Console.ReadLine();
        ParseList(n, ref list);
        
        Console.WriteLine("Enter the polynomial: ");
        n = Console.ReadLine();
        ParseList(n, ref polynomial);

        List<bool> tempList = new List<bool>();

        while (true)
        {
            if (tempList.Count == polynomial.Count)
            {
                XOR(tempList, polynomial);
            }
            tempList.AddRange(list);
        }
        
    }
}
