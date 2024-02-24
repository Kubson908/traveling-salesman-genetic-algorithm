using System;
using System.Collections.Generic;
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

    private int nodesCount = 3;
    public int NodesCount
    {
        get { return nodesCount; }
        set 
        { 
            nodesCount = value; 
            OnPropertyChanged();
        }
    }

    public List<Point> GeneratePointArray()
    {
        var random = new Random();
        points.Clear();
        for (int i = 0; i < NodesCount; i++)
        {
            int x = random.Next(1, 800);
            int y = random.Next(1, 800);
            points.Add(new Point(x, y));
        }
        return points;
    }

    public void Populate()
    {
        var population = geneticAlgorithm.GeneratePopulation(points);
        return;
    }
}
