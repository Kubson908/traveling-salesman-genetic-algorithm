using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TravelingSalesman.Services;

public class GeneticAlgorithm
{
    public List<List<Point>> population;

    public GeneticAlgorithm()
    {
        population = new();
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


    private int Factorial(int number)
    {
        if (number == 1) return 1;
        return number * Factorial(number - 1);
    }
}
