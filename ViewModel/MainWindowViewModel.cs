using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TravelingSalesman.MVVM;
using TravelingSalesman.Services;

namespace TravelingSalesman.ViewModel;

internal class MainWindowViewModel : BaseViewModel
{
    private readonly GeneticAlgorithm geneticAlgorithm;
    private List<Point> points;
    public MainWindowViewModel()
    {
        geneticAlgorithm = new();
        points = new();
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

    public List<Point> ShortestPath()
    {
        List<List<Point>> bestMixedChildren = geneticAlgorithm.RunAlgorithm(points);

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
        return shortestPath;
    }

    public void SetDistance(List<Point> path)
    {
        Distance = geneticAlgorithm.TotalDistance(path);
    }
}
