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

    private static int shiftCount;
    
    public static List<bool> Shift(List<bool> tempList, int polynomialSize, List<bool> original)
    {
        List<bool> result = new List<bool>(tempList);
    
        while (true)
        {
            if (!result[0])
            {
                result.RemoveAt(0);
                result.Add(original[shiftCount + 1]);
                shiftCount++;
            }
            else if (result.Count != polynomialSize)
            {
                result.Add(original[shiftCount + 1]);
                shiftCount++;
            }
            else
                break;
        }
        return result;
    }
    
    static void Main(string[] args)
    {
        List<bool> list = new List<bool>();
        List<bool> polynomial = new List<bool>();
        
        Console.Write("Enter the numbers: ");
        string n = Console.ReadLine();
        ParseList(n, ref list);
        
        Console.Write("Enter the polynomial: ");
        n = Console.ReadLine();
        ParseList(n, ref polynomial);

        List<bool> tempList = new List<bool>();
        tempList.AddRange(list[..polynomial.Count]);
        shiftCount = polynomial.Count - 1;
        
        while (true)
        {
            if (tempList.Count == polynomial.Count && tempList[0])
                tempList = XOR(tempList, polynomial);
            
            else
            {
                try
                {
                    tempList = Shift(tempList, polynomial.Count, list);
                }
                catch (Exception)
                {
                    break;
                }
            }
            
         
            foreach (bool b in tempList)
            {
                Console.Write(b ? 1 : 0);
            }
            Console.WriteLine();

            Console.ReadKey();
        }
        
        Console.WriteLine("Результат");
        foreach (bool b in tempList)
        {
            Console.Write(b ? 1 : 0);
        }
        Console.WriteLine();
        
    }
}