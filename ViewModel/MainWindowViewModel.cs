using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TravelingSalesman.MVVM;
using TravelingSalesman.Services;

namespace TravelingSalesman.ViewModel;

internal class MainWindowViewModel : BaseViewModel
{
    private readonly GeneticAlgorithm geneticAlgorithm;
    public event EventHandler? DrawPath;
    private List<Point> points;
    public MainWindowViewModel()
    {
        geneticAlgorithm = new();
        points = new();
        geneticAlgorithm.NewBestIndividual += GeneticAlgorithm_NewBestIndividual;
    }

    private void GeneticAlgorithm_NewBestIndividual(object? sender, EventArgs e)
    {
        DrawPath?.Invoke(sender, e);
        Distance = ((NewBestIndividualEventArgs)e).Distance;
        CurrentGeneration = ((NewBestIndividualEventArgs)e).Generation;
    }

    private int nodesCount = 4;
    public int NodesCount
    {
        get { return nodesCount; }
        set 
        { 
            nodesCount = value; 
            OnPropertyChanged();
        }
    }

    private double? distance = null;
    public double? Distance
    {
        get { return distance; }
        set
        {
            distance = value;
            CanCalculate = true && !IsBusy;
            OnPropertyChanged();
        }
    }

    private bool canCalculate = false;
    public bool CanCalculate
    {
        get { return canCalculate; }
        set
        {
            canCalculate = value;
            OnPropertyChanged();
        }
    }

    private bool stepByStep = false;
    public bool StepByStep
    {
        get { return stepByStep; }
        set
        {
            stepByStep = value;
            OnPropertyChanged();
        }
    }

    private int generations = 200;
    public int Generations
    {
        get { return generations; }
        set
        {
            generations = value;
            OnPropertyChanged();
        }
    }

    private int? currentGeneration = null;
    public int? CurrentGeneration
    {
        get { return currentGeneration; }
        set
        {
            currentGeneration = value;
            OnPropertyChanged();
        }
    }

    public List<Point> GeneratePointArray()
    {
        var random = new Random();
        Distance = null;
        points.Clear();
        for (int i = 0; i < NodesCount; i++)
        {
            int x = random.Next(1, 800);
            int y = random.Next(1, 800);
            points.Add(new Point(x, y));
        }
        return points;
    }

    public async Task<List<Point>> ShortestPath()
    {
        IsBusy = true;
        CurrentGeneration = null;
        List<List<Point>> bestMixedChildren = await geneticAlgorithm.RunAlgorithm(points, StepByStep, Generations);

        List<double> totalDistances = new();
        for (int i = 0; i < bestMixedChildren.Count; i++)
        {
            totalDistances.Add(geneticAlgorithm.TotalDistance(bestMixedChildren[i]));
        }
        double minDistance = totalDistances.Min();
        if (Distance != null && minDistance < Distance)
        {
            Distance = minDistance;
        }
        int indexMin = totalDistances.IndexOf(minDistance);
        List<Point> shortestPath = bestMixedChildren[indexMin];
        points = shortestPath;
        IsBusy = false;
        CanCalculate = true;
        CurrentGeneration = Generations;
        return shortestPath;
    }

    public void SetDistance(List<Point> path)
    {
        Distance = geneticAlgorithm.TotalDistance(path);
    }
}
