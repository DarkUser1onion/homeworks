// В чем же смысл?
// Дан рандомный пример, например это: result = a - c / b + e * d + f
// Нужно найти такие переменные, которые дадут самый маленький result
// Вроде так можно сделать, хз даже, правильно это или нет

using System;

namespace MyApp;

internal class Program
{
    const int countGeneration = 12;
    
    public class Generation
    {
        public int a, b, c, d, e, f;

        public Generation(bool create)
        {
            if (create)
            {
                a = Random.Shared.Next(-100, 100);
                b = Random.Shared.Next(-100, 100);
                c = Random.Shared.Next(-100, 100);
                d = Random.Shared.Next(-100, 100);
                e = Random.Shared.Next(-100, 100);
                f = Random.Shared.Next(-100, 100);
            }
        }

        public void SetNumber(int numberId, int number)
        {
            switch (numberId)
            {
                case 0:
                    a = number;
                    break;
                case 1:
                    b = number;
                    break;
                case 2:
                    c = number;
                    break;
                case 3:
                    d = number;
                    break;
                case 4:
                    e = number;
                    break;
                case 5:
                    f = number;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(numberId));
            }
        }

        public int GetNumber(int numberId)
        {
            switch (numberId)
            {
                case 0:
                    return a;
                case 1:
                    return b;
                case 2:
                    return c;
                case 3:
                    return d;
                case 4:
                    return e;
                case 5:
                    return f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(numberId));
            }
        }
        public double GetResult()
            => a * c + b - e * d - f;

        public override string ToString()
            => $"{GetResult()} : {a}, {b}, {c}, {d}, {e}, {f}";        
    }
    
    public static List<Generation> Tournament(List<Generation> oldGenerations)
    {
        List<double> listGenerations = new List<double>();
        List<Generation> resultTournament = new List<Generation>();
        List<int> useIndex = new List<int>();
        
        
        foreach (var generation in oldGenerations)
        {
            listGenerations.Add(generation.GetResult());
        }

        for (int i = 0; i < countGeneration / 2 / 2; i++)
        {
            double minNumber = listGenerations[0];
            int minIndex = 0;
            for (int j = 0; j < countGeneration - 1; j++)
            {
                if (minNumber > listGenerations[j + 1])
                {
                    minNumber = listGenerations[j + 1];
                    minIndex = j + 1;
                }
            }
            resultTournament.Add(oldGenerations[minIndex]);
            useIndex.Add(minIndex);
            listGenerations[minIndex] = double.MaxValue;
        }

        for (int i = 0; i < countGeneration / 2 / 2; i++)
        {
            while (true)
            {
                int index = Random.Shared.Next(countGeneration);

                if (!useIndex.Contains(index))
                {
                    resultTournament.Add(oldGenerations[index]);
                    break;
                }
            }
        }

        return resultTournament;
    }

    public static List<Generation> Crossover(List<Generation> oldGenerations)
    {
        List<Generation> resultCrossover = new List<Generation>();

        for (int i = 0; i < countGeneration / 2; i += 2)
        {
            int start, end;
            while (true)
            {
                start = Random.Shared.Next(6);
                end = Random.Shared.Next(6);
                
                if(start == 0 && end == 5) 
                    continue;
                
                if(start < end)
                    break;
            }

            Console.WriteLine("Start: " + start);
            Console.WriteLine("End: " + end);
            
            var temp1 = new Generation(false);
            var temp2 = new Generation(false);

            for (int j = 0; j <= start; j++)
            {
                temp1.SetNumber(j, oldGenerations[i].GetNumber(j));
                temp2.SetNumber(j, oldGenerations[i + 1].GetNumber(j));
            }

            for (int j = start; j <= end; j++)
            {
                temp1.SetNumber(j, oldGenerations[i + 1].GetNumber(j));
                temp2.SetNumber(j, oldGenerations[i].GetNumber(j));
            }

            for (int j = end + 1; j <= 5; j++)
            {
                temp1.SetNumber(j, oldGenerations[i].GetNumber(j));
                temp2.SetNumber(j, oldGenerations[i + 1].GetNumber(j));
            }
            
            resultCrossover.Add(temp1);
            resultCrossover.Add(temp2);
        }

        resultCrossover.AddRange(oldGenerations);
        
        return resultCrossover;
    }

    public static List<Generation> Mutation(List<Generation> oldGenerations)
    {
        List<Generation> resultMutation = new List<Generation>();
        resultMutation.AddRange(oldGenerations);
        
        for (int i = 0; i <= Random.Shared.Next(countGeneration); i++)
        {
            int rndIndex = Random.Shared.Next(countGeneration);
            resultMutation[rndIndex]
                .SetNumber(Random.Shared.Next(6),
                    Random.Shared.Next(-100, 100));
            
            Console.WriteLine(resultMutation[rndIndex]);
        }

        return resultMutation;
    }
    
    static void Main(string[] args)
    {
        if (countGeneration % 4 != 0)
        {
            Console.WriteLine("Неверно указано количество поколений!");
            return;
        }

        List<Generation> generations = new List<Generation>();

        for (int i = 0; i < countGeneration; i++)
        {
            generations.Add(new Generation(true));
        }

        
        Console.Write("Сколько поколений делать? : ");
        int size = int.Parse(Console.ReadLine());
        if (size < 1)
        {
            Console.WriteLine("Неверно указано количество поколений!");
            return;
        }
        
        while (size-- != 0)
        {
            Console.WriteLine("Популяция");
            foreach (Generation generation in generations)
            {
                Console.WriteLine(generation);       
            }

            if (Random.Shared.Next(0, 5) == 0)
            {
                Console.WriteLine("Новая мутация!");
                generations = Mutation(generations);
            }

            generations = Tournament(generations);
            Console.WriteLine("Турнир");
            foreach (Generation generation in generations)
            {
                Console.WriteLine(generation);
            }
            generations = Crossover(generations);
            Console.WriteLine("Появление новых особей");
            foreach (Generation generation in generations)
            {
                Console.WriteLine(generation);
            }
        }
    }
}
