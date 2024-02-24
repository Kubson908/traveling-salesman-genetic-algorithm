using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TravelingSalesman.Services;

public class GeneticAlgorithm
{
    public List<List<Point>> population;
    readonly Random random;

    public GeneticAlgorithm()
    {
        population = new();
        random = new();
    }

    public List<List<Point>> GeneratePopulation(List<Point> points)
    {
        population.Clear();
        int factorial = Factorial(points.Count);
        int limit = factorial < 10 ? factorial : 10;
        while (population.Count < limit)
        {
            var route = points.OrderBy(p => Random.Shared.Next()).ToList();
            if (!population.Any(r => r.SequenceEqual(route)))
                population.Add(route);
        }
        return population;
    }

    private static double Distance(Point point1, Point point2)
    {
        return Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
    }

    public double TotalDistance(List<Point> points)
    {
        double total = 0;
        for (int i = 0; i < population.Count; i++)
        {
            if (i == points.Count - 1)
                total += Distance(points[i], points[0]);
            else
                total += Distance(points[i], points[i+1]);
        }
        return total;
    }

    public List<double> FitnessProbability()
    {
        List<double> totalDistances = new();
        for (int i = 0; i < population.Count; i++)
        {
            totalDistances.Add(TotalDistance(population[i]));
        }
        double maxDistance = totalDistances.Max();
        List<double> populationFitness = totalDistances.Select(e => maxDistance - e).ToList();
        double populationFitnessSum = populationFitness.Sum();
        List<double> populationFitnessProbs = populationFitness.Select(s => s / populationFitnessSum).ToList();
        return populationFitnessProbs;
    }

    public List<Point> Selection(List<double> fitnessProbs)
    {
        double[] cumsum = CumulativeSum(fitnessProbs);
        double randomValue = random.NextDouble();

        bool[] boolProbArray = new bool[cumsum.Length];
        for (int i = 0; i< cumsum.Length; i++)
        {
            boolProbArray[i] = cumsum[i] < randomValue;
        }

        int selectedIndex = Array.FindLastIndex(boolProbArray, b => b == true);
        return population[selectedIndex];
    }

    public (List<Point>, List<Point>) Crossover(List<Point> parent1, List<Point> parent2)
    {
        int maxCutIndex = population.Count - 1;
        int cutIndex = random.Next(1, maxCutIndex);

        List<Point> child1 = new();
        List<Point> child2 = new();

        child1.AddRange(parent1.Take(cutIndex));
        child1.AddRange(parent2.Except(parent1));

        child2.AddRange(parent2.Take(cutIndex));
        child2.AddRange(parent2.Except(parent1));

        return (child1, child2);
    }

    public List<Point> Mutation(List<Point> child)
    {
        int maxIndex = child.Count - 1;
        int index1 = random.Next(1, maxIndex);
        int index2 = random.Next(1, maxIndex);

        Point temp = child[index1];
        child[index1] = child[index2];
        child[index2] = temp;
        return child;
    }


    private static double[] CumulativeSum(List<double> sequence)
    {
        double sum = 0;
        double[] cumulativeSumArray = new double[sequence.Count];
        for (int i = 0; i < sequence.Count; i++)
        {
            sum += sequence[i];
            cumulativeSumArray[i] = sum;
        }
        return cumulativeSumArray;
    }

    private int Factorial(int number)
    {
        if (number == 1) return 1;
        return number * Factorial(number - 1);
    }
}
