using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TravelingSalesman.Services;

public class GeneticAlgorithm
{
    public List<List<Point>> population;
    readonly Random random;
    private readonly float crossoverRate = 0.8f;
    private readonly float mutationRate = 0.2f;
    private readonly int generations = 100;

    public GeneticAlgorithm()
    {
        population = new();
        random = new();
    }

    public List<List<Point>> RunAlgorithm(List<Point> points)
    {
        population.Clear();
        population = GeneratePopulation(points);
        List<double> fitnessProbs = FitnessProbability(population);

        List<List<Point>> parentsList = new();

        for (int i = 0; i < population.Count * crossoverRate; i++)
        {
            parentsList.Add(Selection(fitnessProbs, population));
        }

        List<List<Point>> childrenList = new();

        for (int i = 0; i < parentsList.Count; i += 2)
        {
            (List<Point> child1, List<Point> child2) = Crossover(parentsList[i], parentsList[i + 1], population);
            double mutate_threshold = random.NextDouble();
            if (mutate_threshold > (1 - mutationRate))
                child1 = Mutation(child1);

            mutate_threshold = random.NextDouble();
            if (mutate_threshold > (1 - mutationRate))
                child2 = Mutation(child2);

            childrenList.Add(child1);
            childrenList.Add(child2);
        }

        List<List<Point>> mixedList = parentsList.Concat(childrenList).ToList();

        fitnessProbs.Clear();
        fitnessProbs = FitnessProbability(mixedList);
        List<int> sortedFitnessIndices = Enumerable.Range(0, mixedList.Count)
            .OrderByDescending(i => fitnessProbs[i]).ToList();

        List<List<Point>> bestOfMixedList = sortedFitnessIndices.Take(population.Count)
            .Select(i => mixedList[i]).ToList();

        for (int i = 0; i < generations; i++)
        {
            fitnessProbs = FitnessProbability(bestOfMixedList);
            parentsList.Clear();
            for (int j = 0; j < crossoverRate * population.Count; j++)
            {
                parentsList.Add(Selection(fitnessProbs, bestOfMixedList));
            }

            childrenList.Clear();
            for (int j = 0; j < parentsList.Count; j += 2)
            {
                (List<Point> child1, List<Point> child2) = Crossover(parentsList[j], parentsList[j + 1], bestOfMixedList);
                double mutate_threshold = random.NextDouble();
                if (mutate_threshold > (1 - mutationRate))
                    child1 = Mutation(child1);

                mutate_threshold = random.NextDouble();
                if (mutate_threshold > (1 - mutationRate))
                    child2 = Mutation(child2);

                childrenList.Add(child1);
                childrenList.Add(child2);
            }

            mixedList = parentsList.Concat(childrenList).ToList();
            fitnessProbs.Clear();
            fitnessProbs = FitnessProbability(mixedList);
            sortedFitnessIndices = Enumerable.Range(0, mixedList.Count)
                .OrderByDescending(i => fitnessProbs[i]).ToList();
            bestOfMixedList = sortedFitnessIndices.Take((int)(crossoverRate * population.Count))
                .Select(i => mixedList[i]).ToList();
            List<int> oldPopulationIndices = Enumerable.Range(0, (int)(mutationRate * population.Count))
                .Select(_ => random.Next(0, population.Count)).ToList();

            foreach (int index in oldPopulationIndices)
            {
                bestOfMixedList.Add(population[index]);
            }

            bestOfMixedList = bestOfMixedList.OrderBy(_ => random.Next()).ToList();
        }
        return bestOfMixedList;
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
        for (int i = 0; i < points.Count; i++)
        {
            if (i == points.Count - 1)
                total += Distance(points[i], points[0]);
            else
                total += Distance(points[i], points[i+1]);
        }
        return total;
    }

    public List<double> FitnessProbability(List<List<Point>> selection)
    {
        List<double> totalDistances = new();
        for (int i = 0; i < selection.Count; i++)
        {
            totalDistances.Add(TotalDistance(selection[i]));
        }
        double maxDistance = totalDistances.Max();
        List<double> populationFitness = totalDistances.Select(e => maxDistance - e).ToList();
        double populationFitnessSum = populationFitness.Sum();
        List<double> populationFitnessProbs = populationFitness.Select(s => s / populationFitnessSum).ToList();
        return populationFitnessProbs;
    }

    public List<Point> Selection(List<double> fitnessProbs, List<List<Point>> selection)
    {
        double[] cumsum = CumulativeSum(fitnessProbs);
        double randomValue = random.NextDouble();

        bool[] boolProbArray = new bool[cumsum.Length];
        for (int i = 0; i< cumsum.Length; i++)
        {
            boolProbArray[i] = cumsum[i] < randomValue;
        }

        int selectedIndex = Array.FindLastIndex(boolProbArray, b => b == true);
        return selectedIndex != -1 ? selection[selectedIndex] : selection[Array.IndexOf(cumsum, cumsum.Min())];
    }

    public (List<Point>, List<Point>) Crossover(List<Point> parent1, List<Point> parent2, List<List<Point>> selection)
    {
        int maxCutIndex = parent1.Count - 1;
        int cutIndex = random.Next(1, maxCutIndex);

        List<Point> child1 = new();
        List<Point> child2 = new();

        child1.AddRange(parent1.Take(cutIndex));
        child1.AddRange(parent2.Except(child1));

        child2.AddRange(parent2.Take(cutIndex));
        child2.AddRange(parent1.Except(child2));

        return (child1, child2);
    }

    public List<Point> Mutation(List<Point> child)
    {
        int maxIndex = child.Count - 1;
        int index1 = random.Next(1, maxIndex);
        int index2 = random.Next(1, maxIndex);

        (child[index2], child[index1]) = (child[index1], child[index2]);
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
