using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace TravelingSalesman.Services;

public class GeneticAlgorithm
{
    public List<List<Point>> population;
    public event EventHandler? NewBestIndividual;
    readonly Random random;
    private readonly int populationCount = 250;
    private readonly float crossoverRate = 0.8f;
    private readonly float mutationRate = 0.2f;
    private readonly int generations = 200;

    public GeneticAlgorithm()
    {
        population = new();
        random = new();
    }

    public async Task<List<List<Point>>> RunAlgorithm(List<Point> points, bool stepByStep)
    {
        population.Clear();
        population = GeneratePopulation(points);
        population[0] = points;

        // Add first generation based on population
        List<List<Point>> bestOfMixedList = AddNewGeneration(population);
        List<Point> shortest = bestOfMixedList.MinBy(p => TotalDistance(p))!;
        double shortestTotalDistance = TotalDistance(shortest);

        // Add more generations and choose the best individuals
        for (int i = 0; i < generations; i++)
        {
            bestOfMixedList = AddNewGeneration(bestOfMixedList);

            List<int> oldPopulationIndices = Enumerable.Range(0, (int)(mutationRate * population.Count))
                .Select(_ => random.Next(0, population.Count)).ToList();

            foreach (int index in oldPopulationIndices)
            {
                bestOfMixedList.Add(population[index]);
            }

            bestOfMixedList = bestOfMixedList.OrderBy(_ => random.Next()).ToList();

            if (stepByStep)
            {

                var newShortest = bestOfMixedList.MinBy(p => TotalDistance(p))!;
                double newShortestTotalDistance = TotalDistance(newShortest);
                if (shortestTotalDistance > newShortestTotalDistance)
                {
                    shortest = newShortest;
                    shortestTotalDistance = newShortestTotalDistance;
                    NewBestIndividualEventArgs args = new(shortest, shortestTotalDistance);
                    NewBestIndividual?.Invoke(this, args);
                    await Task.Delay(400);
                }
            }
        }
        return bestOfMixedList;
    }

    private List<List<Point>> AddNewGeneration(List<List<Point>> selection)
    {
        List<double> fitnessProbs = FitnessProbability(selection);

        List<List<Point>> parentsList = new();

        for (int i = 0; i < population.Count * crossoverRate; i++)
        {
            parentsList.Add(Selection(fitnessProbs, selection));
        }

        List<List<Point>> childrenList = new();

        for (int i = 0; i < parentsList.Count; i += 2)
        {
            (List<Point> child1, List<Point> child2) = Crossover(parentsList[i], parentsList[i + 1]);
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

        return sortedFitnessIndices.Take(population.Count)
            .Select(i => mixedList[i]).ToList();
    }

    public List<List<Point>> GeneratePopulation(List<Point> points)
    {
        population.Clear();
        long factorial = Factorial(points.Count);
        long limit = factorial < populationCount ? factorial : populationCount;
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

    public double TotalDistance(List<Point> path)
    {
        double totalDistance = 0;
        for (int i = 0; i < path.Count; i++)
        {
            if (i == path.Count - 1)
                totalDistance += Distance(path[i], path[0]);
            else
                totalDistance += Distance(path[i], path[i + 1]);
        }
        return totalDistance;
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
        for (int i = 0; i < cumsum.Length; i++)
        {
            boolProbArray[i] = cumsum[i] < randomValue;
        }

        int selectedIndex = Array.FindLastIndex(boolProbArray, b => b == true);
        return selectedIndex != -1 ? selection[selectedIndex] : selection[Array.IndexOf(cumsum, cumsum.Min())];
    }

    public (List<Point>, List<Point>) Crossover(List<Point> parent1, List<Point> parent2)
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

    private long Factorial(int number)
    {
        if (number == 1) return 1;
        return number * Factorial(number - 1);
    }
}

public class NewBestIndividualEventArgs : EventArgs
{
    private readonly List<Point> _path;
    private readonly double _distance;

    public NewBestIndividualEventArgs(List<Point> path, double distance)
    {
        _path = path;
        _distance = distance;
    }
    public List<Point> Path { get { return _path; } }
    public double Distance { get { return _distance; } }
}
