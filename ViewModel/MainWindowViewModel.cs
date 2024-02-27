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
            CanCalculate = true;
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
        List<List<Point>> bestMixedChildren = await geneticAlgorithm.RunAlgorithm(points, StepByStep);

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
        return shortestPath;
    }

    public void SetDistance(List<Point> path)
    {
        Distance = geneticAlgorithm.TotalDistance(path);
    }
}
